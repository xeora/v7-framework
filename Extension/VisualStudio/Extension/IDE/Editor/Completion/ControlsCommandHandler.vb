Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.TextManager.Interop

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion
    Public Class ControlsCommandHandler
        Implements IOleCommandTarget

        Private _NextCommandHandler As IOleCommandTarget
        Private _TextView As IWpfTextView
        Private _Provider As ControlsCommandHandlerProvider
        Private _CurrentSession As ICompletionSession

        Public Enum Directives
            Tag

            Bind
            Control
            Type

            [Operator]
            Special
            Translation
            TemplateWithVariablePool
            ServerExecutable

            None
        End Enum

        Public Sub New(ByVal textViewAdapter As IVsTextView, ByVal textView As IWpfTextView, ByVal provider As ControlsCommandHandlerProvider)
            Me._CurrentSession = Nothing

            Me._TextView = textView
            Me._Provider = provider

            textViewAdapter.AddCommandFilter(Me, Me._NextCommandHandler)

            ResetTrackingChars()
        End Sub

        Private _HandleFollowingAction As Action
        Private _CurrentTrackingChars As Char()

        Private Sub ResetTrackingChars()
            Me._CurrentTrackingChars = New Char() {"<"c, "$"c, ":"c, "?"c, "."c}
        End Sub

        Private Function StartSession(ByVal Directive As Directives, ByVal ParamArray Properties As Generic.KeyValuePair(Of Object, Object)()) As Boolean
            If Not Me._CurrentSession Is Nothing OrElse (Not Me._CurrentSession Is Nothing AndAlso Me._CurrentSession.IsStarted) Then Return False

            Me._Provider.CompletionBroker.DismissAllSessions(Me._TextView)

            Dim snapshot As ITextSnapshot =
                Me._TextView.Caret.Position.BufferPosition.Snapshot

            Me._CurrentSession =
                Me._Provider.CompletionBroker.CreateCompletionSession(
                    Me._TextView,
                    snapshot.CreateTrackingPoint(Me._TextView.Caret.Position.BufferPosition, PointTrackingMode.Positive),
                    True
                )
            AddHandler Me._CurrentSession.Dismissed, AddressOf Me.OnSessionDismissed
            AddHandler Me._CurrentSession.Committed, AddressOf Me.OnSessionCommitted

            Me._CurrentSession.Properties.AddProperty("Directive", Directive)

            If Not Properties Is Nothing Then
                For Each [Property] As Generic.KeyValuePair(Of Object, Object) In Properties
                    Me._CurrentSession.Properties.AddProperty([Property].Key, [Property].Value)
                Next
            End If

            Me._CurrentSession.Start()

            Return True
        End Function

        Private Function Complete() As Boolean
            If Me._CurrentSession Is Nothing OrElse (Not Me._CurrentSession Is Nothing AndAlso Me._CurrentSession.IsDismissed) Then Return False

            If Me._CurrentSession.SelectedCompletionSet.SelectionStatus.IsSelected Then
                Me._CurrentSession.Commit()
            Else
                Return Me.Cancel()
            End If

            Return True
        End Function

        Private Function Cancel() As Boolean
            If Me._CurrentSession Is Nothing OrElse (Not Me._CurrentSession Is Nothing AndAlso Me._CurrentSession.IsDismissed) Then Return False

            Me._CurrentSession.Dismiss()

            Return True
        End Function

        Private Sub Filter()
            If Me._CurrentSession Is Nothing OrElse (Not Me._CurrentSession Is Nothing AndAlso Me._CurrentSession.IsDismissed) Then Exit Sub

            Me._CurrentSession.SelectedCompletionSet.SelectBestMatch()
            Me._CurrentSession.SelectedCompletionSet.Recalculate()
        End Sub

        Private Function GetTypedChar(ByVal pvaIn As IntPtr) As Char
            Dim rChar As Char = Char.MinValue

            If pvaIn <> IntPtr.Zero Then _
                rChar = ChrW(CType(Marshal.GetObjectForNativeVariant(pvaIn), UShort))

            Return rChar
        End Function

        Public Function Exec(ByRef pguidCmdGroup As Guid, ByVal nCmdID As UInteger, ByVal nCmdexecopt As UInteger, ByVal pvaIn As IntPtr, ByVal pvaOut As IntPtr) As Integer Implements IOleCommandTarget.Exec
            If Not PackageControl.IDEControl.CheckIsXeoraCubeProject() Then Return Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            If Not PackageControl.IDEControl.CheckIsXeoraTemplateFile() Then Return Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            Dim TextDocument As ITextDocument = Nothing
            If Me._TextView.TextBuffer.Properties.TryGetProperty(Of ITextDocument)(GetType(ITextDocument), TextDocument) Then
                Dim WorkingFileName As String =
                    IO.Path.GetFileName(TextDocument.FilePath)

                If String.Compare(WorkingFileName, "Controls.xml", True) <> 0 Then _
                    Return Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            Else
                Return Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            End If

            Select Case CType(nCmdID, VSConstants.VSStd2KCmdID)
                Case VSConstants.VSStd2KCmdID.AUTOCOMPLETE, VSConstants.VSStd2KCmdID.COMPLETEWORD
                    Dim Directive As Directives
                    Dim StatementText As String =
                        Me.ExtractXeoraStatement(Char.MinValue, Directive)

                    If Directive <> Directives.None Then
                        Me.HandleFollowingCompletion()

                        Return VSConstants.S_OK
                    End If

                Case VSConstants.VSStd2KCmdID.RETURN, VSConstants.VSStd2KCmdID.TAB
                    If Not Me._CurrentSession Is Nothing AndAlso
                        Not Me._CurrentSession.IsDismissed Then

                        Me._HandleFollowingAction = New Action(Sub() Me.HandleFollowingCompletion())

                        If Not Me.Complete() Then Me._HandleFollowingAction = Nothing

                        Return VSConstants.S_OK
                    End If

                Case CType(103, VSConstants.VSStd2KCmdID) 'VSConstants.VSStd2KCmdID.CANCEL
                    Me.Cancel()

                Case VSConstants.VSStd2KCmdID.TYPECHAR
                    Dim TypedChar As Char = Me.GetTypedChar(pvaIn)

                    If Not Me._CurrentSession Is Nothing AndAlso
                        Not Me._CurrentSession.IsDismissed Then

                        If TypedChar <> Char.MinValue Then
                            If Array.IndexOf(Me._CurrentTrackingChars, TypedChar) > -1 Then
                                Me.Cancel()

                                Me.Print(TypedChar)

                                Me.HandleFollowingCompletion()

                                Return VSConstants.S_OK
                            Else
                                Me.Filter()
                            End If
                        End If
                    Else
                        If Array.IndexOf(Me._CurrentTrackingChars, TypedChar) > -1 Then
                            Me.Print(TypedChar)

                            Select Case TypedChar
                                Case "$"c
                                    Me.StartSession(Directives.Special)

                                    Return VSConstants.S_OK

                                Case "<"c
                                    Me.StartSession(Directives.Tag)

                                    Return VSConstants.S_OK

                                Case Else
                                    HandleFollowingCompletion()

                                    Return VSConstants.S_OK

                                    'Case Else
                                    '    Dim StatementText As String =
                                    '        Me.ExtractXeoraStatement(Char.MinValue)

                                    '    If Not String.IsNullOrEmpty(StatementText) Then
                                    '        Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

                                    '        Me.HandleFollowingCompletion()

                                    '        Return VSConstants.S_OK
                                    '    End If

                            End Select

                        End If
                    End If
            End Select

            Return Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
        End Function

        Public Function QueryStatus(ByRef pguidCmdGroup As Guid, ByVal cCmds As UInteger, ByVal prgCmds() As OLECMD, ByVal pCmdText As IntPtr) As Integer Implements IOleCommandTarget.QueryStatus
            Return Me._NextCommandHandler.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText)
        End Function

        Private Sub Print(ByVal text As String)
            Dim edit As ITextEdit =
                Me._TextView.TextBuffer.CreateEdit()

            edit.Insert(Me._TextView.Caret.Position.BufferPosition, text)
            edit.Apply()
        End Sub

        Private Function ExtractXeoraStatement(ByVal SearchChar As Char, ByRef DirectiveType As Directives) As String
            DirectiveType = Directives.None

            Dim PageContent As String =
                Me._TextView.TextSnapshot.GetText()

            Dim CurrentPosition As SnapshotPoint =
                Me._TextView.Caret.Position.BufferPosition

            If SearchChar = Char.MinValue Then SearchChar = "$"c

            Dim TagIndex As Integer = PageContent.LastIndexOf(SearchChar, CurrentPosition - 1)
            If TagIndex = -1 Then
                SearchChar = "<"c
                TagIndex = PageContent.LastIndexOf(SearchChar, CurrentPosition - 1)
            End If

            If TagIndex > -1 Then
                Dim StatementText As String =
                    PageContent.Substring(TagIndex, CurrentPosition - TagIndex)

                If SearchChar = "$"c Then
                    If StatementText.IndexOf("$"c) <> 0 OrElse
                        (StatementText.IndexOf("$"c) = 0 AndAlso StatementText.Length = 1) Then Return String.Empty
                End If

                If Not String.IsNullOrEmpty(StatementText) Then
                    If StatementText.Contains(Environment.NewLine) Then Return String.Empty

                    For cC As Integer = 0 To StatementText.Length - 1
                        If Char.IsWhiteSpace(StatementText.Chars(cC)) Then Return String.Empty
                    Next

                    If SearchChar = "$"c Then
                        If SearchChar = Char.MinValue Then _
                            SearchChar = StatementText.Chars(StatementText.Length - 1)

                        Dim ColonIndex As Integer = StatementText.IndexOf(":"c)

                        If ColonIndex > -1 Then
                            Select Case StatementText.Substring(1, ColonIndex - 1)
                                Case "P"
                                    DirectiveType = Directives.TemplateWithVariablePool
                                Case "L"
                                    DirectiveType = Directives.Translation
                                Case "F"
                                    DirectiveType = Directives.ServerExecutable
                            End Select
                        Else
                            ' "^", "~", "-", "+", "=", "#", "*"
                            If StatementText.IndexOf("_VariableName_") > -1 Then _
                                DirectiveType = Directives.Operator
                        End If
                    Else
                        Dim Patterns As String() = New String() {"\<Bind\>", "\<DefaultButtonID\>", "\<Type\>"}
                        For p As Integer = 0 To Patterns.Length - 1
                            Dim mI As System.Text.RegularExpressions.Match =
                                System.Text.RegularExpressions.Regex.Match(StatementText, Patterns(p), System.Text.RegularExpressions.RegexOptions.RightToLeft)

                            If mI.Success AndAlso mI.Index = 0 Then
                                StatementText = StatementText.Substring(mI.Length)

                                Select Case p
                                    Case 0
                                        DirectiveType = Directives.Bind
                                    Case 1
                                        DirectiveType = Directives.Control
                                    Case 2
                                        DirectiveType = Directives.Type
                                    Case Else
                                        DirectiveType = Directives.None
                                End Select

                                Exit For
                            End If
                        Next
                    End If

                    Return StatementText
                End If
            End If

            Return String.Empty
        End Function

        Private Sub HandleFollowingCompletion()
            Me._HandleFollowingAction = Nothing

            Dim Directive As Directives
            Dim StatementText As String =
                Me.ExtractXeoraStatement(Char.MinValue, Directive)

            Select Case Directive
                Case Directives.Bind
                    Me._CurrentTrackingChars = New Char() {"?"c, "."c}
                    Me.StartSession(Directives.Bind, New Generic.KeyValuePair(Of Object, Object)("Executable_CurrentStatement", StatementText))

                Case Directives.Control
                    If String.IsNullOrEmpty(StatementText) Then _
                        Me.StartSession(Directives.Control)

                Case Directives.Type
                    If String.IsNullOrEmpty(StatementText) Then _
                        Me.StartSession(Directives.Type)

                Case Directives.Operator
                    Me.Print("$")

                    ' _VariableName_ = 14
                    Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 15)
                    Me._TextView.Selection.Select(New SnapshotSpan(Me._TextView.Caret.Position.BufferPosition, 14), False)

                Case Directives.Special
                    Dim TestStatementText As String =
                        Me.ExtractXeoraStatement("<"c, Directive)

                    If Directive = Directives.None Then _
                        Me.StartSession(Directives.Special)

                Case Directives.TemplateWithVariablePool
                    Dim TestStatementText As String =
                        Me.ExtractXeoraStatement("<"c, Directive)

                    If Directive = Directives.None Then _
                        Me.StartSession(Directives.TemplateWithVariablePool)

                Case Directives.Translation
                    Dim TestStatementText As String =
                        Me.ExtractXeoraStatement("<"c, Directive)

                    If Directive = Directives.None Then _
                        Me.StartSession(Directives.Translation)

                Case Directives.ServerExecutable
                    StatementText = StatementText.Substring(StatementText.IndexOf(":"c) + 1)

                    Me._CurrentTrackingChars = New Char() {"$"c, "?"c, "."c}
                    Me.StartSession(Directives.ServerExecutable, New Generic.KeyValuePair(Of Object, Object)("Executable_CurrentStatement", StatementText))

            End Select
        End Sub

        Private Sub OnSessionCommitted(sender As Object, e As EventArgs)
            RemoveHandler Me._CurrentSession.Committed, AddressOf OnSessionCommitted

            If Me._CurrentSession.Properties.GetProperty(Of Directives)("Directive") = Directives.Tag Then
                Dim PageContent As String =
                    Me._TextView.TextSnapshot.GetText()
                Dim CurrentPosition As SnapshotPoint =
                    Me._TextView.Caret.Position.BufferPosition

                Dim SearchRegex As New System.Text.RegularExpressions.Regex("\<\w+\>\<\/\w+\>")
                Dim mIs As System.Text.RegularExpressions.MatchCollection =
                    SearchRegex.Matches(PageContent.Substring(0, CurrentPosition))

                If mIs.Count > 0 Then
                    Dim m As System.Text.RegularExpressions.Match =
                        mIs(mIs.Count - 1)

                    Me._TextView.Caret.MoveTo(New SnapshotPoint(Me._TextView.TextSnapshot, (m.Index + (m.Length \ 2))))
                End If
            End If
        End Sub

        Private Sub OnSessionDismissed(ByVal sender As Object, ByVal e As EventArgs)
            RemoveHandler Me._CurrentSession.Dismissed, AddressOf OnSessionDismissed

            Me._CurrentSession = Nothing

            ResetTrackingChars()

            If Not Me._HandleFollowingAction Is Nothing Then Me._HandleFollowingAction.Invoke()
        End Sub
    End Class
End Namespace