Imports Microsoft.VisualStudio.Language
Imports Microsoft.VisualStudio.Text
Imports My.Resources

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public Class ControlTag
        Inherits BuilderBase

        Private _Session As Intellisense.ICompletionSession
        Private _TextBuffer As ITextBuffer

        Public Property RequestFromControl As Boolean

        Public Sub New(ByVal Directive As [Enum], ByRef Session As Intellisense.ICompletionSession, ByRef TextBuffer As ITextBuffer)
            MyBase.New(Directive)

            Me._Session = Session
            Me._TextBuffer = TextBuffer
        End Sub

        Public Overrides Function Build() As Intellisense.Completion()
            Dim CompList As New Generic.List(Of Intellisense.Completion)()

            Try
                For Each AvailableTag As String In Me.PossibleTags()
                    CompList.Add(New Intellisense.Completion(AvailableTag, String.Format("{0}></{0}>", AvailableTag), String.Empty, Me.ProvideImageSource(IconResource.xmltag), Nothing))
                Next
            Catch ex As Exception
                Return Nothing
            End Try

            ' It is available for all tags
            ' Bind, Type

            ' It is available for Textbox, Password
            ' DefaultButtonID

            ' It is available for ImageButton
            ' Source

            ' It is avalable for Textarea
            ' Content

            ' It is avalable for LinkButton
            ' Url

            ' It is avalable for Button, LinkButton, Textbox, Password, Checkbox, RadioButton,  
            ' Text

            ' It is available for all except Block Ones
            ' BlockIDsToUpdate, Attributes

            Return CompList.ToArray()
        End Function

        Public Overrides Function Builders() As Intellisense.Completion()
            Return Nothing
        End Function

        Private Function PossibleTags() As String()
            Dim rAvailableTags As New Generic.List(Of String)

            Dim PageContentText As String =
                Me._TextBuffer.CurrentSnapshot.GetText()
            Dim CurrentPosition As Integer =
                Me._Session.TextView.Caret.Position.BufferPosition

            ' Search Active Control in PageContentText
            Dim StartIndex As Integer = PageContentText.LastIndexOf("<Control", CurrentPosition)
            Dim EndIndex As Integer = PageContentText.IndexOf("</Control>", CurrentPosition)

            If StartIndex > -1 AndAlso EndIndex > -1 AndAlso StartIndex < EndIndex Then
                Dim ControlContent As String =
                    PageContentText.Substring(StartIndex, (EndIndex + "</Control>".Length) - StartIndex)

                Dim Start As Integer = CurrentPosition - StartIndex - 1
                Dim CustomID As String = String.Format("_{0}", Guid.NewGuid().ToString().Replace("-"c, String.Empty))
                ControlContent = ControlContent.Remove(Start, 1).Insert(Start, String.Format("<{0} />", CustomID))

                Dim XmlReader As Xml.XmlReader =
                    Xml.XmlReader.Create(New IO.StringReader(ControlContent))

                Dim ControlType As Globals.ControlTypes =
                    Globals.ControlTypes.Unknown
                Dim UsedTags As New Generic.List(Of String)

                Dim ParentTagName As String = String.Empty
                Dim TagStack As New Generic.Stack(Of String)

                Dim Depth As Integer = 0
                Do While XmlReader.Read()
                    Select Case XmlReader.NodeType
                        Case Xml.XmlNodeType.Element
                            If String.Compare(XmlReader.Name, CustomID) = 0 Then ParentTagName = TagStack.Peek()
                            TagStack.Push(XmlReader.Name)

                            If Depth = 1 Then
                                If String.Compare(XmlReader.Name, CustomID) <> 0 Then _
                                    UsedTags.Add(XmlReader.Name)

                                Select Case XmlReader.Name
                                    Case "Type"
                                        [Enum].TryParse(Of Globals.ControlTypes)(XmlReader.Value, ControlType)
                                End Select
                            End If

                            Depth += 1

                        Case Xml.XmlNodeType.EndElement
                            TagStack.Pop()

                            Depth -= 1

                    End Select
                Loop

                XmlReader.Close()

                rAvailableTags.Add("Type")

                If ControlType <> Globals.ControlTypes.Unknown Then
                    If String.Compare(ParentTagName, "Control") = 0 Then
                        rAvailableTags.Add("Bind")

                        Select Case ControlType
                            Case Globals.ControlTypes.Textbox,
                                 Globals.ControlTypes.Password

                                rAvailableTags.Add("DefaultButtonID")
                                rAvailableTags.Add("Text")
                                rAvailableTags.Add("BlockIDsToUpdate")

                            Case Globals.ControlTypes.Checkbox,
                                 Globals.ControlTypes.Button,
                                 Globals.ControlTypes.RadioButton

                                rAvailableTags.Add("Text")
                                rAvailableTags.Add("BlockIDsToUpdate")

                            Case Globals.ControlTypes.Textarea
                                rAvailableTags.Add("Content")

                            Case Globals.ControlTypes.ImageButton
                                rAvailableTags.Add("Source")
                                rAvailableTags.Add("BlockIDsToUpdate")

                            Case Globals.ControlTypes.LinkButton
                                rAvailableTags.Add("Text")
                                rAvailableTags.Add("Url")
                                rAvailableTags.Add("BlockIDsToUpdate")

                        End Select

                        rAvailableTags.Add("Attributes")
                    ElseIf String.Compare(ParentTagName, "Attributes") = 0 OrElse
                            String.Compare(ParentTagName, "BlockIDsToUpdate") = 0 Then

                        rAvailableTags.Add("Item")
                    End If

                    For Each UsedTag As String In UsedTags
                        If rAvailableTags.IndexOf(UsedTag) > -1 Then _
                            rAvailableTags.Remove(UsedTag)
                    Next
                End If
            End If

            Return rAvailableTags.ToArray()
        End Function

        Public Shared Function CurrentControlID(ByRef Session As Intellisense.ICompletionSession, ByRef TextBuffer As ITextBuffer) As String
            Dim rControlID As String = String.Empty

            Dim PageContentText As String =
                TextBuffer.CurrentSnapshot.GetText()
            Dim CurrentPosition As Integer =
                Session.TextView.Caret.Position.BufferPosition

            ' Search Active Control in PageContentText
            Dim StartIndex As Integer = PageContentText.LastIndexOf("<Control", CurrentPosition)
            Dim EndIndex As Integer = PageContentText.IndexOf("</Control>", CurrentPosition)

            If StartIndex > -1 AndAlso EndIndex > -1 AndAlso StartIndex < EndIndex Then
                Dim ControlContent As String =
                    PageContentText.Substring(StartIndex, (EndIndex + "</Control>".Length) - StartIndex)
                ControlContent = ControlContent.Remove(CurrentPosition - StartIndex - 1, 1)

                Dim XmlReader As Xml.XmlReader =
                    Xml.XmlReader.Create(New IO.StringReader(ControlContent))

                Dim ControlType As Globals.ControlTypes =
                    Globals.ControlTypes.Unknown
                Dim UsedTags As New Generic.List(Of String)

                Dim Depth As Integer = 0
                Do While XmlReader.Read()
                    Select Case XmlReader.NodeType
                        Case Xml.XmlNodeType.Element
                            If String.Compare(XmlReader.Name, "Control") = 0 Then
                                rControlID = XmlReader.GetAttribute("id")

                                Exit Do
                            End If

                            Depth += 1
                        Case Xml.XmlNodeType.EndElement
                            Depth -= 1
                    End Select
                Loop

                XmlReader.Close()
            End If

            Return rControlID
        End Function
    End Class
End Namespace