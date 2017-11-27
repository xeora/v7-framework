Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.TextManager.Interop

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion
    Public Class TemplateCommandHandler
        Implements IOleCommandTarget

        Private _NextCommandHandler As IOleCommandTarget
        Private _TextView As IWpfTextView
        Private _Provider As TemplateCommandHandlerProvider
        Private _CurrentSession As ICompletionSession

        Public Enum Directives
            None
            Control
            ControlWithLeveling
            ControlWithBound
            ControlWithLevelingAndBound
            Template
            Translation
            TemplateWithVariablePool
            ServerExecutable
            ServerExecutableWithLeveling
            ServerExecutableWithBound
            ServerExecutableWithLevelingAndBound
            ClientExecutable
            InLineStatement
            InLineStatementWithBound
            UpdateBlock
            MessageBlock
            PartialCache
            Special
            [Operator]
            MiddleOperator
        End Enum

        Public Sub New(ByVal textViewAdapter As IVsTextView, ByVal textView As IWpfTextView, ByVal provider As TemplateCommandHandlerProvider)
            Me._CurrentSession = Nothing

            Me._TextView = textView
            Me._Provider = provider

            textViewAdapter.AddCommandFilter(Me, Me._NextCommandHandler)

            Me.ResetTrackingChars()
        End Sub

        Private _HandleFollowingAction As Action
        Private _CurrentTrackingChars As Char()
        Private _OperatorChars As Char() = New Char() {"^"c, "~"c, "-"c, "+"c, "="c, "#"c, "*"c}

        Private Sub ResetTrackingChars()
            Me._CurrentTrackingChars = New Char() {"$"c, ":"c, "?"c, "."c, "#"c, "["c}
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
            AddHandler Me._CurrentSession.Committed, AddressOf Me.OnSessionCommitted
            AddHandler Me._CurrentSession.Dismissed, AddressOf Me.OnSessionDismissed

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

            Select Case CType(nCmdID, VSConstants.VSStd2KCmdID)
                Case VSConstants.VSStd2KCmdID.AUTOCOMPLETE, VSConstants.VSStd2KCmdID.COMPLETEWORD
                    Dim SearchChar As Char
                    Dim StatementText As String =
                        Me.ExtractXeoraStatement(SearchChar)

                    If Not String.IsNullOrEmpty(StatementText) Then
                        Dim LastIndex As Integer

                        LastIndex = StatementText.LastIndexOf(".")
                        If LastIndex = -1 Then LastIndex = StatementText.LastIndexOf("?")
                        If LastIndex = -1 Then LastIndex = StatementText.LastIndexOf("{")
                        If LastIndex = -1 Then LastIndex = StatementText.LastIndexOf("]")
                        If LastIndex = -1 Then LastIndex = StatementText.LastIndexOf(":")

                        If LastIndex <> -1 Then
                            Dim CursorMoveCount As Integer = StatementText.Length - (LastIndex + 1)

                            Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - CursorMoveCount)
                            Me._TextView.Selection.Select(New SnapshotSpan(Me._TextView.Caret.Position.BufferPosition, CursorMoveCount), False)

                            Me.HandleFollowingCompletion()

                            Return VSConstants.S_OK
                        End If
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
                            Select Case TypedChar
                                Case "$"c
                                    Me.Print(TypedChar)

                                    Me.StartSession(Directives.Special)

                                    Return VSConstants.S_OK

                                Case Else
                                    Dim StatementText As String =
                                        Me.ExtractXeoraStatement(Char.MinValue)

                                    If Not String.IsNullOrEmpty(StatementText) Then
                                        Me.Print(TypedChar)

                                        Me.HandleFollowingCompletion()

                                        Return VSConstants.S_OK
                                    End If

                            End Select

                        End If
                    End If
            End Select

            Return Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            '            If VsShellUtilities.IsInAutomationFunction(Me._Provider.ServiceProvider) Then _
            '                Return Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)


            '            If pguidCmdGroup <> VSConstants.VSStd2K Then
            '                If pguidCmdGroup = Guid.Parse("5efc7975-14bc-11cf-9b2b-00aa00573819") Then
            '                    If CType(nCmdID, VSConstants.VSStd97CmdID) = VSConstants.VSStd97CmdID.Delete Then
            '                        GoTo QuickJumpForDelete
            '                    Else
            '                        Return VSConstants.S_OK 'Me.NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                    End If
            '                Else
            '                    Return VSConstants.S_OK 'Me.NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                End If
            '            End If

            '            Dim Handled As Boolean = False
            '            Dim rExecResult As Integer = VSConstants.S_OK

            '            Dim OperatorChars As Char() = New Char() {"^"c, "~"c, "-"c, "+"c, "="c, "#"c, "*"c}
            '            Dim typedChar1 As Char = Me.GetTypedChar(pvaIn)

            '            Select Case CType(nCmdID, VSConstants.VSStd2KCmdID)
            '                Case VSConstants.VSStd2KCmdID.AUTOCOMPLETE, VSConstants.VSStd2KCmdID.COMPLETEWORD
            '                    Handled = Me.StartSession(Directives.Special)

            '                Case VSConstants.VSStd2KCmdID.RETURN, VSConstants.VSStd2KCmdID.TAB
            '                    Me._HandleFollowingAction = New Action(Sub() Me.HandleFollowingCompletion(Char.MinValue, Char.MinValue, False))

            '                    Handled = Me.Complete(False)

            '                    If Not Handled Then Me._HandleFollowingAction = Nothing

            '                Case CType(103, VSConstants.VSStd2KCmdID) 'VSConstants.VSStd2KCmdID.CANCEL
            '                    Handled = Me.Cancel()

            '                Case Else
            '                    If typedChar1 <> Char.MinValue Then
            '                        If Not Me._CurrentSession Is Nothing AndAlso
            '                            Not Me._CurrentSession.IsDismissed Then

            '                            If Array.IndexOf(Me._CurrentTrackingChars, typedChar1) > -1 Then
            '                                Me._HandleFollowingAction = New Action(Sub() Me.HandleFollowingCompletion(typedChar1, Char.MinValue, False))

            '                                Dim HandleChar As Boolean
            '                                Handled = Me.Complete(HandleChar)
            '                                If HandleChar Then Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            '                                If Not Handled Then Me._HandleFollowingAction = Nothing

            '                            ElseIf Char.IsWhiteSpace(typedChar1) Then
            '                                Me._HandleFollowingAction = New Action(Sub() Me.HandleFollowingCompletion(Char.MinValue, Char.MinValue, False))

            '                                Dim HandleChar As Boolean
            '                                Handled = Me.Complete(HandleChar)
            '                                If HandleChar Then Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            '                                If Not Handled Then Me._HandleFollowingAction = Nothing

            '                            Else
            '                                Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            '                                If Array.IndexOf(OperatorChars, typedChar1) > -1 Then Handled = Me.Cancel() Else Handled = True

            '                            End If
            '                        Else
            '                            If Array.IndexOf(Me._CurrentTrackingChars, typedChar1) = -1 Then
            '                                Dim CursorIndex As Integer
            '                                Dim StatementText As String = Me.ExtractXeoraStatement(Char.MinValue, CursorIndex)

            '                                If Not String.IsNullOrEmpty(StatementText) Then
            '                                    Dim PageContent As String = Me._TextView.TextSnapshot.GetText()
            '                                    Dim RegEx As New System.Text.RegularExpressions.Regex("\$C\#\d*(\+)?")
            '                                    Dim LevelingMatch As System.Text.RegularExpressions.Match =
            '                                        RegEx.Match(PageContent, (Me._TextView.Caret.Position.BufferPosition - CursorIndex))

            '                                    If LevelingMatch.Success AndAlso
            '                                        LevelingMatch.Index = (Me._TextView.Caret.Position.BufferPosition.Position - CursorIndex) AndAlso
            '                                        LevelingMatch.Length >= CursorIndex Then

            '                                        If Char.IsDigit(typedChar1) OrElse typedChar1 = "+"c OrElse typedChar1 = ":"c OrElse typedChar1 = "["c Then
            '                                            If Not Char.IsDigit(typedChar1) AndAlso LevelingMatch.Length > 3 AndAlso Me._TextView.Selection.IsEmpty Then
            '                                                If typedChar1 = "+" AndAlso CursorIndex = LevelingMatch.Length Then
            '                                                    If LevelingMatch.Value.IndexOf("+"c) = -1 Then _
            '                                                        Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                                End If

            '                                                If typedChar1 = ":"c OrElse typedChar1 = "["c Then
            '                                                    If LevelingMatch.Value.Chars(LevelingMatch.Length - 1) = "+"c Then
            '                                                        If LevelingMatch.Length > 4 Then Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                                    Else
            '                                                        If CursorIndex = LevelingMatch.Length Then _
            '                                                            Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                                    End If
            '                                                End If
            '                                            Else
            '                                                If Char.IsDigit(typedChar1) Then _
            '                                                    Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                            End If
            '                                        End If

            '                                        Handled = True
            '                                    End If
            '                                Else
            '                                    Handled = True

            '                                    rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                End If
            '                            Else
            '                                If Not Me._TextView.Selection.IsEmpty Then _
            '                                    Handled = IsNumeric(Me._TextView.Selection.SelectedSpans.Item(0).GetText())
            '                            End If
            '                        End If
            '                    End If

            '            End Select

            '            If Not Handled Then
            '                Select Case CType(nCmdID, VSConstants.VSStd2KCmdID)
            '                    Case VSConstants.VSStd2KCmdID.TYPECHAR
            '                        If typedChar1 <> Char.MinValue Then
            '                            Select Case typedChar1
            '                                Case "$"c
            '                                    Dim StatementText As String =
            '                                        Me.ExtractXeoraStatement(Char.MinValue, 0)

            '                                    If String.IsNullOrEmpty(StatementText) Then
            '                                        rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            '                                        Me.StartSession(Directives.Special)
            '                                    Else
            '                                        rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                    End If

            '                                Case "#"c, "["c, ":"c, "?"c, "."c
            '                                    Me.HandleFollowingCompletion(typedChar1, typedChar1, True)
            '                                Case Else
            '                                    rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            '                                    Me.Filter()
            '                            End Select
            '                        End If
            '                    Case VSConstants.VSStd2KCmdID.BACKSPACE
            'QuickJumpForDelete:
            '                        Dim CursorIndex As Integer
            '                        Dim StatementText As String = Me.ExtractXeoraStatement(Char.MinValue, CursorIndex)

            '                        If Not String.IsNullOrEmpty(StatementText) Then
            '                            Dim PageContent As String = Me._TextView.TextSnapshot.GetText()
            '                            Dim RegEx As New System.Text.RegularExpressions.Regex("\$C\#\d*(\+)?")
            '                            Dim LevelingMatch As System.Text.RegularExpressions.Match =
            '                                RegEx.Match(PageContent, (Me._TextView.Caret.Position.BufferPosition - CursorIndex))

            '                            If LevelingMatch.Success AndAlso LevelingMatch.Index = (Me._TextView.Caret.Position.BufferPosition.Position - CursorIndex) Then
            '                                If pguidCmdGroup = Guid.Parse("5efc7975-14bc-11cf-9b2b-00aa00573819") Then
            '                                    ' It cames from DELETE key
            '                                    If CursorIndex = 2 OrElse (CursorIndex > 2 AndAlso CursorIndex + 1 >= LevelingMatch.Length) Then
            '                                        ' Delete Whole
            '                                        Dim Difference As Integer, Length As Integer

            '                                        If CursorIndex = 2 Then
            '                                            Difference = 0
            '                                        Else
            '                                            Difference = CursorIndex - 2
            '                                        End If
            '                                        Length = LevelingMatch.Length - 2

            '                                        Dim edit As ITextEdit =
            '                                            Me._TextView.TextBuffer.CreateEdit()

            '                                        edit.Replace(Me._TextView.Caret.Position.BufferPosition - Difference, Length, String.Empty)
            '                                        edit.Apply()
            '                                    Else
            '                                        ' Delete Char
            '                                        rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                    End If
            '                                Else
            '                                    ' BackSpace
            '                                    If CursorIndex = 3 OrElse CursorIndex = 4 Then
            '                                        ' Delete Whole
            '                                        Dim Difference As Integer, Length As Integer

            '                                        If CursorIndex = 2 Then
            '                                            Difference = 1
            '                                        Else
            '                                            Difference = CursorIndex - 2
            '                                        End If
            '                                        Length = LevelingMatch.Length - 2

            '                                        Dim edit As ITextEdit =
            '                                            Me._TextView.TextBuffer.CreateEdit()

            '                                        edit.Replace(Me._TextView.Caret.Position.BufferPosition - Difference, Length, String.Empty)
            '                                        edit.Apply()
            '                                    Else
            '                                        ' Delete Char
            '                                        rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                                    End If
            '                                End If
            '                            Else
            '                                rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                            End If
            '                        Else
            '                            rExecResult = Me._NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)

            '                            Me.Filter()
            '                        End If
            '                    Case Else
            '                        rExecResult = VSConstants.S_OK 'Me.NextCommandHandler.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut)
            '                End Select
            '            End If

            '            Return rExecResult
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

        Private Sub [Erase](ByVal start As Integer, ByVal length As Integer)
            Dim edit As ITextEdit =
                Me._TextView.TextBuffer.CreateEdit()

            edit.Delete(start, length)
            edit.Apply()
        End Sub

        Private Function ExtractXeoraStatement(ByRef SearchChar As Char) As String
            Dim PageContent As String =
                Me._TextView.TextSnapshot.GetText()

            Dim CurrentPosition As SnapshotPoint =
                Me._TextView.Caret.Position.BufferPosition

            Dim StatementText As String = String.Empty
            Dim TagIndex As Integer = PageContent.LastIndexOf("$", CurrentPosition)

            If TagIndex > -1 Then
                StatementText = PageContent.Substring(TagIndex, CurrentPosition - TagIndex)

                If StatementText.IndexOf("$"c) <> 0 OrElse
                    (StatementText.IndexOf("$"c) = 0 AndAlso StatementText.Length = 1) Then StatementText = String.Empty

                If Not String.IsNullOrEmpty(StatementText) Then
                    If StatementText.Contains(Environment.NewLine) Then Return String.Empty

                    For cC As Integer = 0 To StatementText.Length - 1
                        If Char.IsWhiteSpace(StatementText.Chars(cC)) Then Return String.Empty
                    Next

                    If SearchChar = Char.MinValue Then _
                        SearchChar = StatementText.Chars(StatementText.Length - 1)

                    Dim MainPattern As New Xeora.Web.RegularExpression.MainCapturePattern()

                    Dim StatementMatch As System.Text.RegularExpressions.Match =
                        MainPattern.Match(PageContent, TagIndex)

                    If StatementMatch.Success AndAlso StatementMatch.Index = 0 Then StatementText = StatementMatch.Value
                End If
            End If

            Return StatementText
        End Function

        Private Function GetDirective(ByRef SearchChar As Char) As Directives
            Dim rDirective As Directives = Directives.Special

            Dim StatementText As String = Me.ExtractXeoraStatement(SearchChar)

            If Not String.IsNullOrEmpty(StatementText) Then
                Dim ColonChar As Char = SearchChar
                If ColonChar = "?"c OrElse ColonChar = "."c Then ColonChar = ":"c

                If ColonChar <> ":"c AndAlso
                    Array.IndexOf(Me._OperatorChars, ColonChar) = -1 Then ColonChar = ":"c

                Dim ColonIndex As Integer =
                    StatementText.IndexOf(ColonChar)

                If ColonIndex > 1 Then
                    Select Case StatementText.Substring(1, ColonIndex - 1)
                        Case "C"
                            rDirective = Directives.Control
                        Case "T"
                            Return Directives.Template
                        Case "L"
                            Return Directives.Translation
                        Case "P"
                            Return Directives.TemplateWithVariablePool
                        Case "F"
                            rDirective = Directives.ServerExecutable
                        Case "XF"
                            Return Directives.ClientExecutable
                        Case "S"
                            rDirective = Directives.InLineStatement
                        Case "H"
                            Return Directives.UpdateBlock
                        Case "MB"
                            Return Directives.MessageBlock
                        Case "PC"
                            Return Directives.PartialCache
                    End Select
                Else
                    ' "^", "~", "-", "+", "=", "#", "*"
                    If StatementText.IndexOf("_VariableName_") > -1 Then _
                        Return Directives.Operator
                End If

                If StatementText.Length > 1 Then
                    ' Check for Leveled and/or Bound Control
                    If StatementText.Length >= 2 AndAlso String.Compare(StatementText.Substring(0, 2), "$C") = 0 Then
                        rDirective = Directives.Control

                        If StatementText.IndexOf("$C[") = 0 Then
                            ' This is Bound Control
                            rDirective = Directives.ControlWithBound
                        Else
                            If StatementText.Length > 3 Then
                                ' Control Leveled and Bound
                                Dim ItemMatch As System.Text.RegularExpressions.Match =
                                    System.Text.RegularExpressions.Regex.Match(StatementText, "\$C\#\d+(\+)?(\[)?")

                                If ItemMatch.Success Then
                                    If ItemMatch.Value.IndexOf("[") > -1 Then
                                        ' Control With Leveling and Bound
                                        rDirective = Directives.ControlWithLevelingAndBound
                                    Else
                                        ' Control With Leveling
                                        rDirective = Directives.ControlWithLeveling
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If StatementText.Length >= 2 AndAlso String.Compare(StatementText.Substring(0, 2), "$F") = 0 Then
                        rDirective = Directives.ServerExecutable

                        If StatementText.IndexOf("$F[") = 0 Then
                            ' This is Bound Execution
                            rDirective = Directives.ServerExecutableWithBound
                        Else
                            If StatementText.Length > 3 Then
                                ' Execution Leveled and Bound
                                Dim ItemMatch As System.Text.RegularExpressions.Match =
                                    System.Text.RegularExpressions.Regex.Match(StatementText, "\$F\#\d+(\+)?(\[)?")

                                If ItemMatch.Success Then
                                    If ItemMatch.Value.IndexOf("[") > -1 Then
                                        ' Execution With Leveling and Bound
                                        rDirective = Directives.ServerExecutableWithLevelingAndBound
                                    Else
                                        ' Execution With Leveling
                                        rDirective = Directives.ServerExecutableWithLeveling
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If StatementText.Length >= 2 AndAlso String.Compare(StatementText.Substring(0, 2), "$S") = 0 Then
                        rDirective = Directives.InLineStatement

                        If StatementText.IndexOf("$S[") = 0 Then
                            ' This is Bound InLineStatement
                            rDirective = Directives.InLineStatementWithBound
                        End If
                    End If
                End If
            End If

            Return rDirective
        End Function

        Private Function GetControlID() As String
            Dim StatementText As String = Me.ExtractXeoraStatement(Char.MinValue)

            If Not String.IsNullOrEmpty(StatementText) Then
                Dim ColonIndex As Integer =
                    StatementText.IndexOf(":"c)

                If ColonIndex > -1 Then
                    StatementText = StatementText.Remove(0, ColonIndex + 1)

                    ColonIndex = StatementText.IndexOf(":"c)

                    If ColonIndex > -1 Then
                        StatementText = StatementText.Substring(0, ColonIndex)
                    Else
                        If String.IsNullOrWhiteSpace(StatementText) Then StatementText = String.Empty
                    End If
                Else
                    StatementText = String.Empty
                End If
            End If

            Return StatementText
        End Function

        Private Sub HandleFollowingCompletion()
            Me._HandleFollowingAction = Nothing

            Dim SearchChar As Char
            Dim Directive As Directives =
                Me.GetDirective(SearchChar)

            Select Case Directive
                Case Directives.Operator
                    Me.Complete_Operator()

                Case Directives.Template, Directives.TemplateWithVariablePool
                    Me.Complete_Template(Directive)

                Case Directives.MessageBlock
                    Me.Complete_MessageBlock()

                Case Directives.PartialCache
                    Me.Complete_PartialCache()

                Case Directives.UpdateBlock
                    Me.Complete_UpdateBlock(SearchChar)

                Case Directives.InLineStatement, Directives.InLineStatementWithBound
                    Me.Complete_InLineStatement(Directive, SearchChar)

                Case Directives.Control,
                     Directives.ControlWithLeveling,
                     Directives.ControlWithBound,
                     Directives.ControlWithLevelingAndBound

                    Me.Complete_Control(Directive, SearchChar)

                Case Directives.ServerExecutable,
                     Directives.ServerExecutableWithLeveling,
                     Directives.ServerExecutableWithBound,
                     Directives.ServerExecutableWithLevelingAndBound

                    Me.Complete_ServerExecutable(Directive, SearchChar)

                Case Directives.ClientExecutable
                    Me.Complete_ClientExecutable()

                Case Directives.Translation
                    Me.Complete_Translation()

                Case Else
                    If Directive <> Directives.Special Then
                        Me._CurrentTrackingChars = New Char() {"$"c, ":"c, "#"c, "["c}
                        Me.StartSession(Directive)
                    End If

            End Select
        End Sub

        Private Sub Complete_Operator()
            Me.Print("$")

            ' _VariableName_ = 14
            Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 15)
            Me._TextView.Selection.Select(New SnapshotSpan(Me._TextView.Caret.Position.BufferPosition, 14), False)
        End Sub

        Private Sub Complete_Template(ByVal Directive As Directives)
            Me._CurrentTrackingChars = New Char() {"$"c}

            Dim TextDocument As ITextDocument = Nothing
            If Me._TextView.TextBuffer.Properties.TryGetProperty(Of ITextDocument)(GetType(ITextDocument), TextDocument) Then
                Me.StartSession(Directive, New Generic.KeyValuePair(Of Object, Object)("Template_FilePath", IO.Path.GetFileNameWithoutExtension(TextDocument.FilePath)))
            Else
                Me.StartSession(Directive)
            End If
        End Sub

        Private Sub Complete_MessageBlock()
            Me.Print("{}:MB$")

            Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 5)
        End Sub

        Private Sub Complete_PartialCache()
            Me.Print("{}:PC$")

            Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 5)
        End Sub

        Private Sub Complete_UpdateBlock(ByVal SearchChar As Char)
            Dim ControlID As String =
                Me.GetControlID()

            If Not String.IsNullOrEmpty(ControlID) Then
                If String.Compare(ControlID, "_DefineID_") <> 0 Then
                    If SearchChar = ":"c Then Me.Print(String.Format("{{}}:{0}$", ControlID))

                    Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - (ControlID.Length + 3))
                Else
                    Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - ControlID.Length)
                    Me._TextView.Selection.Select(New SnapshotSpan(Me._TextView.Caret.Position.BufferPosition, ControlID.Length), False)
                End If
            Else
                Me.Print("_DefineID_")

                ' _DefineID_ = 10
                Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 10)
                Me._TextView.Selection.Select(New SnapshotSpan(Me._TextView.Caret.Position.BufferPosition, 10), False)
            End If
        End Sub

        Private Sub Complete_InLineStatement(ByVal Directive As Directives, ByVal SearchChar As Char)
            If SearchChar = "["c Then
                Me._CurrentTrackingChars = New Char() {"]"c}
                Me.StartSession(Directives.ControlWithBound)
            ElseIf SearchChar = ":"c Then
                Dim ControlID As String =
                    Me.GetControlID()

                If Not String.IsNullOrEmpty(ControlID) Then
                    If String.Compare(ControlID, "_DefineID_") <> 0 Then
                        Me.Print(String.Format("{{}}:{0}$", ControlID))

                        Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - (ControlID.Length + 3))
                    Else
                        Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - (ControlID.Length + 1))
                        Me._TextView.Selection.Select(New SnapshotSpan(Me._TextView.Caret.Position.BufferPosition, ControlID.Length), False)
                    End If
                Else
                    Me.Print("_DefineID_")

                    ' _DefineID_ = 10
                    Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 10)
                    Me._TextView.Selection.Select(New SnapshotSpan(Me._TextView.Caret.Position.BufferPosition, 10), False)
                End If
            Else
                Me._CurrentTrackingChars = New Char() {"["c, ":"c}
                Me.StartSession(Directives.MiddleOperator, New Generic.KeyValuePair(Of Object, Object)("MiddleOperator_WorkingDirective", Directive))
            End If
        End Sub

        Private Sub Complete_Control(ByVal Directive As Directives, ByVal SearchChar As Char)
            If SearchChar = ":"c Then
                Dim ControlID As String =
                    Me.GetControlID()

                If Not String.IsNullOrEmpty(ControlID) Then
                    Me.Print(String.Format("{{}}:{0}$", ControlID))

                    Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - (ControlID.Length + 3))
                Else
                    Me._CurrentTrackingChars = New Char() {"$"c, ":"c}
                    Me.StartSession(Directives.Control)
                End If
            ElseIf SearchChar = "["c Then
                Me._CurrentTrackingChars = New Char() {"]"c}
                Me.StartSession(Directives.ControlWithBound)
            ElseIf SearchChar = "]"c Then
                Me.Print(":")

                Me._CurrentTrackingChars = New Char() {"$"c, ":"c}
                Me.StartSession(Directive)
            Else
                Dim ControlID As String =
                    Me.GetControlID()

                If String.Compare(ControlID, "__CREATE.CONTROL__") = 0 Then
                    Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - ControlID.Length)
                    Me.Erase(Me._TextView.Caret.Position.BufferPosition.Position, ControlID.Length)

                    Dim FormControlCreator As New Tools.Creators.Control()
                    If FormControlCreator.ShowDialog(PackageControl.IDEControl) = DialogResult.OK Then
                        ControlID = FormControlCreator.tbControlID.Text
                        Dim ControlType As String =
                            CType(FormControlCreator.cbTypes.SelectedItem, String)

                        Select Case ControlType
                            Case "DataList", "ConditionalStatement", "VariableBlock"
                                Me.Print(String.Format("{0}:{{}}:{0}$", ControlID))
                                Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - (ControlID.Length + 3))
                            Case Else
                                Me.Print(String.Format("{0}$", ControlID))
                        End Select
                    End If
                Else
                    If Directive = Directives.Control Then
                        Me._CurrentTrackingChars = New Char() {":"c, "["c, "#"c}
                        Me.StartSession(Directives.MiddleOperator, New Generic.KeyValuePair(Of Object, Object)("MiddleOperator_WorkingDirective", Directive))
                    End If
                End If
            End If
        End Sub

        Private Sub Complete_ServerExecutable(ByVal Directive As Directives, ByVal SearchChar As Char)
            'If Array.IndexOf(New Char() {":"c, "?"c, "."c}, SearchChar) > -1 Then
            Dim StatementText As String =
                Me.ExtractXeoraStatement(Char.MinValue)

            If Not String.IsNullOrEmpty(StatementText) Then
                If StatementText.IndexOf("__CREATE.EXECUTABLE__") = 3 Then
                    Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - (StatementText.Length - 3))
                    Me.Erase(Me._TextView.Caret.Position.BufferPosition.Position, (StatementText.Length - 3))

                    Dim ExecuterCreator As New Tools.Creators.Executable()
                    If ExecuterCreator.ShowDialog(PackageControl.IDEControl) = DialogResult.OK Then
                        Dim ExecutableName As String = String.Format("{0}Lib?", ExecuterCreator.tbExecutableID.Text)

                        StatementText = StatementText.Remove(3)
                        StatementText = StatementText.Insert(3, ExecutableName)

                        Me.Print(ExecutableName)
                    End If
                Else
                    StatementText = StatementText.Substring(StatementText.IndexOf(":"c) + 1)

                    Me._CurrentTrackingChars = New Char() {"$"c, "?"c, "."c}
                End If
            Else
                Me._CurrentTrackingChars = New Char() {"?"c}
            End If
            Me.StartSession(Directives.ServerExecutable, New Generic.KeyValuePair(Of Object, Object)("Executable_CurrentStatement", StatementText))
            'ElseIf SearchChar = "["c Then
            '    Me._CurrentTrackingChars = New Char() {"]"c}
            '    Me.StartSession(Directives.ControlWithBound)
            'ElseIf SearchChar = "]"c Then
            '    Me.Print(":")

            '    Me._CurrentTrackingChars = New Char() {"$"c, ":"c}
            '    Me.StartSession(Directive)
            'Else
            '    If Directive = Directives.ServerExecutable Then
            '        Me._CurrentTrackingChars = New Char() {":"c, "["c, "#"c}
            '        Me.StartSession(Directives.MiddleOperator, New Generic.KeyValuePair(Of Object, Object)("MiddleOperator_WorkingDirective", Directive))
            '    End If
            'End If
        End Sub

        Private Sub Complete_ClientExecutable()
            Dim StatementText As String =
                Me.ExtractXeoraStatement(Char.MinValue)

            If StatementText.Length = 4 Then
                Me.Print("{}:XF$")

                Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 5)

                Me._CurrentTrackingChars = New Char() {"?"c}
                Me.StartSession(Directives.ClientExecutable)
            Else
                Me._CurrentTrackingChars = New Char() {"?"c, "."c}
                Me.StartSession(Directives.ClientExecutable, New Generic.KeyValuePair(Of Object, Object)("Executable_CurrentStatement", StatementText.Substring(5)))
            End If
        End Sub

        Private Sub Complete_Translation()
            Dim StatementText As String =
                Me.ExtractXeoraStatement(Char.MinValue)

            If Not String.IsNullOrEmpty(StatementText) AndAlso
                StatementText.IndexOf("__CREATE.TRANSLATE__") = 3 Then

                Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - (StatementText.Length - 3))
                Me.Erase(Me._TextView.Caret.Position.BufferPosition.Position, (StatementText.Length - 3))

                Dim FormTranslationCreator As New Tools.Creators.Translation()
                If FormTranslationCreator.ShowDialog(PackageControl.IDEControl) = DialogResult.OK Then
                    If Not String.IsNullOrEmpty(FormTranslationCreator.SelectedTranslationID) Then
                        Me.Print(String.Format("{0}$", FormTranslationCreator.SelectedTranslationID))
                    Else
                        Me._TextView.Caret.MoveTo(Me._TextView.Caret.Position.BufferPosition - 3)
                        Me.Erase(Me._TextView.Caret.Position.BufferPosition.Position, 3)
                    End If
                End If
            Else
                Me._CurrentTrackingChars = New Char() {"$"c}
                Me.StartSession(Directives.Translation)
            End If
        End Sub

        Private Sub OnSessionCommitted(ByVal sender As Object, ByVal e As EventArgs)
            Me._HandleFollowingAction = New Action(Sub() Me.HandleFollowingCompletion())
        End Sub

        Private Sub OnSessionDismissed(ByVal sender As Object, ByVal e As EventArgs)
            RemoveHandler Me._CurrentSession.Dismissed, AddressOf OnSessionDismissed

            Me._CurrentSession = Nothing

            Me.ResetTrackingChars()

            If Not Me._HandleFollowingAction Is Nothing Then Me._HandleFollowingAction.Invoke()
        End Sub
    End Class
End Namespace