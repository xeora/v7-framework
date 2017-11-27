Imports Microsoft.VisualStudio.Language
Imports Microsoft.VisualStudio.Text
Imports My.Resources

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public Class Control
        Inherits BuilderBase

        Private _Session As Intellisense.ICompletionSession
        Private _TextBuffer As ITextBuffer

        Public Sub New(ByVal Directive As [Enum], ByRef Session As Intellisense.ICompletionSession, ByRef TextBuffer As ITextBuffer)
            MyBase.New(Directive)

            Me._Session = Session
            Me._TextBuffer = TextBuffer
        End Sub

        Public Property WorkingControlID As String
        Public Property OnlyButtons As Boolean

        Public Overrides Function Build() As Intellisense.Completion()
            Dim CompList As New Generic.List(Of Intellisense.Completion)()
            Dim TemplatesPath As String =
                MyBase.LocateTemplatesPath(PackageControl.IDEControl.DTE.ActiveDocument.Path)

            If Me._TextBuffer Is Nothing Then
                Me.Fill(CompList, TemplatesPath)

                If PackageControl.IDEControl.GetActiveItemDomainType() = Globals.ActiveDomainTypes.Child Then
                    Dim DomainID As String = String.Empty
                    Dim SearchDI As New IO.DirectoryInfo(TemplatesPath)

                    Do
                        SearchDI = SearchDI.Parent
                        If Not SearchDI Is Nothing Then
                            DomainID = SearchDI.Name
                            SearchDI = SearchDI.Parent
                        End If

                        If Not SearchDI Is Nothing AndAlso
                        (
                            String.Compare(SearchDI.Name, "Domains", True) = 0 OrElse
                            String.Compare(SearchDI.Name, "Addons", True) = 0
                        ) Then

                            Me.Fill(CompList, IO.Path.Combine(SearchDI.FullName, DomainID, "Templates"))

                            If String.Compare(SearchDI.Name, "Domains", True) = 0 Then SearchDI = Nothing
                        End If
                    Loop Until SearchDI Is Nothing
                End If
            Else
                Me.FillParents(CompList, TemplatesPath)
            End If

            CompList.Sort(New CompletionComparer())

            Return CompList.ToArray()
        End Function

        Public Overrides Function Builders() As Intellisense.Completion()
            Return New Intellisense.Completion() {
                        New Intellisense.Completion("Create New Control", "__CREATE.CONTROL__", String.Empty, Nothing, Nothing)
                    }
        End Function

        Private Sub Fill(ByRef Container As Generic.List(Of Intellisense.Completion), ByVal TemplatesPath As String)
            Dim cFStream As IO.FileStream = Nothing

            Try
                Dim ControlID As String, ControlType As Globals.ControlTypes

                cFStream = New IO.FileStream(
                                IO.Path.Combine(TemplatesPath, "Controls.xml"),
                                IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
                Dim xPathDocument As New Xml.XPath.XPathDocument(cFStream)
                Dim xPathNavigator As Xml.XPath.XPathNavigator =
                    xPathDocument.CreateNavigator()
                Dim xPathIter As Xml.XPath.XPathNodeIterator =
                    xPathNavigator.Select("//Controls/Control")
                Dim xPathIter2 As Xml.XPath.XPathNodeIterator

                Do While xPathIter.MoveNext()
                    xPathIter2 = xPathIter.Clone()

                    ControlID = xPathIter.Current.GetAttribute("id", xPathIter.Current.NamespaceURI)
                    ControlType = Globals.ControlTypes.Unknown

                    If xPathIter2.Current.MoveToFirstChild() Then
                        Do
                            Select Case xPathIter2.Current.Name.ToLower(New System.Globalization.CultureInfo("en-US"))
                                Case "type"
                                    Dim xControlType As String =
                                        xPathIter2.Current.Value

                                    ControlType = Globals.ParseControlType(xControlType)

                                    Exit Do
                            End Select
                        Loop While xPathIter.Current.MoveToNext()
                    End If

                    If ControlType <> Globals.ControlTypes.Unknown AndAlso Not Me.IsExists(Container, ControlID) Then
                        Dim Image As Drawing.Bitmap = Nothing

                        Select Case ControlType
                            Case Globals.ControlTypes.ConditionalStatement, Globals.ControlTypes.DataList, Globals.ControlTypes.VariableBlock
                                Select Case ControlType
                                    Case Globals.ControlTypes.DataList
                                        Image = IconResource.datalist
                                    Case Globals.ControlTypes.ConditionalStatement
                                        Image = IconResource.conditionalstatement
                                    Case Globals.ControlTypes.VariableBlock
                                        Image = IconResource.variableblock
                                End Select

                                If Not OnlyButtons Then
                                    Container.Add(
                                        New Intellisense.Completion(
                                            ControlID, String.Format("{0}:", ControlID), String.Empty,
                                            Me.ProvideImageSource(Image), Nothing
                                        )
                                    )
                                End If

                            Case Else
                                Select Case ControlType
                                    Case Globals.ControlTypes.Textbox
                                        Image = IconResource.textbox
                                    Case Globals.ControlTypes.Password
                                        Image = IconResource.password
                                    Case Globals.ControlTypes.Checkbox
                                        Image = IconResource.checkbox
                                    Case Globals.ControlTypes.Button
                                        Image = IconResource.button
                                    Case Globals.ControlTypes.RadioButton
                                        Image = IconResource.radiobutton
                                    Case Globals.ControlTypes.Textarea
                                        Image = IconResource.textarea
                                    Case Globals.ControlTypes.ImageButton
                                        Image = IconResource.imagebutton
                                    Case Globals.ControlTypes.LinkButton
                                        Image = IconResource.linkbutton
                                End Select

                                If Not OnlyButtons Then
                                    Container.Add(
                                        New Intellisense.Completion(
                                            ControlID, String.Format("{0}$", ControlID), String.Empty,
                                            Me.ProvideImageSource(Image), Nothing
                                        )
                                    )
                                Else
                                    If String.Compare(Me.WorkingControlID, ControlID) = 0 Then Continue Do

                                    Select Case ControlType
                                        Case Globals.ControlTypes.Button, Globals.ControlTypes.ImageButton, Globals.ControlTypes.LinkButton
                                            Container.Add(
                                                New Intellisense.Completion(
                                                    ControlID, ControlID, String.Empty,
                                                    Me.ProvideImageSource(Image), Nothing
                                                )
                                            )
                                    End Select
                                End If

                        End Select
                    End If
                Loop
            Catch ex As Exception
                ' Just Handle Exceptions
            Finally
                If Not cFStream Is Nothing Then cFStream.Close()
            End Try
        End Sub

        Private Sub FillParents(ByRef Container As Generic.List(Of Intellisense.Completion), ByVal TemplatesPath As String)
            Dim PageContentText As String =
                Me._TextBuffer.CurrentSnapshot.GetText()
            Dim CurrentPosition As Integer =
                Me._Session.TextView.Caret.Position.BufferPosition

            PageContentText = PageContentText.Substring(CurrentPosition)

            ' Search Controls in PageContentText
            Dim SearchMatches As System.Text.RegularExpressions.MatchCollection =
                System.Text.RegularExpressions.Regex.Matches(PageContentText, "\$C(\<\d+(\+)?\>)?(\[[\.\w\-]+\])?\:(?<ControlID>[\.\w\-]+)\:")

            For Each regexMatch As System.Text.RegularExpressions.Match In SearchMatches
                If regexMatch.Success Then
                    Dim ControlID As String =
                        regexMatch.Groups.Item("ControlID").Value

                    If String.Compare(Me.WorkingControlID, ControlID) = 0 Then Continue For

                    Dim ControlType As Globals.ControlTypes =
                        Me.GetControlType(TemplatesPath, ControlID)

                    If ControlType <> Globals.ControlTypes.Unknown Then
                        Dim Image As Drawing.Bitmap = Nothing

                        Select Case ControlType
                            Case Globals.ControlTypes.Textbox
                                Image = IconResource.textbox
                            Case Globals.ControlTypes.Password
                                Image = IconResource.password
                            Case Globals.ControlTypes.Checkbox
                                Image = IconResource.checkbox
                            Case Globals.ControlTypes.Button
                                Image = IconResource.button
                            Case Globals.ControlTypes.RadioButton
                                Image = IconResource.radiobutton
                            Case Globals.ControlTypes.Textarea
                                Image = IconResource.textarea
                            Case Globals.ControlTypes.ImageButton
                                Image = IconResource.imagebutton
                            Case Globals.ControlTypes.LinkButton
                                Image = IconResource.linkbutton
                            Case Globals.ControlTypes.DataList
                                Image = IconResource.datalist
                            Case Globals.ControlTypes.ConditionalStatement
                                Image = IconResource.conditionalstatement
                            Case Globals.ControlTypes.VariableBlock
                                Image = IconResource.variableblock
                        End Select

                        Container.Add(
                            New Intellisense.Completion(
                                ControlID, String.Format("{0}]:", ControlID), String.Empty,
                                Me.ProvideImageSource(Image), Nothing
                            )
                        )
                    End If
                End If
            Next
        End Sub

        Private Function GetControlType(ByVal TemplatesPath As String, ByVal ControlID As String) As Globals.ControlTypes
            Dim rControlType As Globals.ControlTypes =
                Globals.ControlTypes.Unknown
            Dim cFStream As IO.FileStream = Nothing

            Try
                cFStream = New IO.FileStream(
                                IO.Path.Combine(TemplatesPath, "Controls.xml"),
                                IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
                Dim xPathDocument As New Xml.XPath.XPathDocument(cFStream)
                Dim xPathNavigator As Xml.XPath.XPathNavigator =
                    xPathDocument.CreateNavigator()
                Dim xPathIter As Xml.XPath.XPathNodeIterator =
                    xPathNavigator.Select(String.Format("//Controls/Control[@id='{0}']", ControlID))

                If xPathIter.MoveNext() Then
                    If xPathIter.Current.MoveToFirstChild() Then
                        Do
                            Select Case xPathIter.Current.Name.ToLower(New System.Globalization.CultureInfo("en-US"))
                                Case "type"
                                    Dim xControlType As String =
                                        xPathIter.Current.Value

                                    rControlType = Globals.ParseControlType(xControlType)

                                    Exit Do
                            End Select
                        Loop While xPathIter.Current.MoveToNext()
                    End If
                End If
            Catch ex As Exception
                ' Just Handle Exceptions
            Finally
                If Not cFStream Is Nothing Then cFStream.Close()
            End Try

            If rControlType = Globals.ControlTypes.Unknown AndAlso
                PackageControl.IDEControl.GetActiveItemDomainType() = Globals.ActiveDomainTypes.Child Then

                Dim DomainID As String = String.Empty
                Dim SearchDI As New IO.DirectoryInfo(TemplatesPath)

                Do
                    SearchDI = SearchDI.Parent
                    If Not SearchDI Is Nothing Then
                        DomainID = SearchDI.Name
                        SearchDI = SearchDI.Parent
                    End If

                    If Not SearchDI Is Nothing AndAlso
                        (
                            String.Compare(SearchDI.Name, "Domains", True) = 0 OrElse
                            String.Compare(SearchDI.Name, "Addons", True) = 0
                        ) Then

                        rControlType = Me.GetControlType(
                                            IO.Path.Combine(SearchDI.FullName, DomainID, "Templates"),
                                            ControlID)

                        If String.Compare(SearchDI.Name, "Domains", True) = 0 Then SearchDI = Nothing
                    End If
                Loop Until SearchDI Is Nothing
            End If

            Return rControlType
        End Function
    End Class
End Namespace