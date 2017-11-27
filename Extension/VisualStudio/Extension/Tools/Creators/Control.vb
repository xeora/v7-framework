Imports System.Runtime.InteropServices
Imports EnvDTE
Imports Microsoft.VisualStudio.Language
Imports Xeora.Extension.VisualStudio.IDE.Editor.Completion

Namespace Xeora.Extension.VisualStudio.Tools.Creators
    Public Class Control
        Private _ControlSpecificOptions As New Generic.Dictionary(Of String, Object)

        Private Sub Control_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Me.cbTypes.SelectedIndex = 0
        End Sub

        Private Sub butAccept_Click(sender As Object, e As EventArgs) Handles butAccept.Click
            If String.IsNullOrEmpty(Me.tbControlID.Text) Then
                Me.tbControlID.BackColor = Drawing.Color.LightPink

                Return
            End If

            Dim ActiveDocFI As IO.FileInfo =
                New IO.FileInfo(PackageControl.IDEControl.DTE.ActiveDocument.FullName)
            Dim WorkingDI As IO.DirectoryInfo = ActiveDocFI.Directory
            Do Until WorkingDI Is Nothing OrElse String.Compare(WorkingDI.Name, "Templates") = 0
                WorkingDI = WorkingDI.Parent
            Loop
            If WorkingDI Is Nothing Then
                MessageBox.Show("Something Wrong! Reopen the document and try again...", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return
            End If
            Dim ControlsXMLFileLocation As String =
                IO.Path.Combine(WorkingDI.FullName, "Controls.xml")

            If Not IO.File.Exists(ControlsXMLFileLocation) Then
                Dim SW As IO.StreamWriter =
                    IO.File.CreateText(ControlsXMLFileLocation)
                SW.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
                SW.WriteLine("<Controls />")
                SW.Close()

                Dim WorkingProjectItems As ProjectItem =
                    PackageControl.IDEControl.DTE.ActiveDocument.ProjectItem

                Do Until WorkingProjectItems.Collection.Parent Is Nothing OrElse String.Compare(CType(WorkingProjectItems.Collection.Parent, ProjectItem).Name, "Templates") = 0
                    WorkingProjectItems = CType(WorkingProjectItems.Collection.Parent, ProjectItem)
                Loop
                If WorkingProjectItems Is Nothing Then
                    MessageBox.Show("Something Wrong! Reopen the document and try again...", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Return
                End If
                WorkingProjectItems.ProjectItems.AddFromFile(ControlsXMLFileLocation)
            End If

            Dim XmlDocument As New Xml.XmlDocument()
            XmlDocument.Load(ControlsXMLFileLocation)

            Dim ControlsNode As Xml.XmlNode =
                XmlDocument.CreateNode(Xml.XmlNodeType.Element, "Control", XmlDocument.NamespaceURI)

            Dim ControlIDAttribute As Xml.XmlAttribute =
                XmlDocument.CreateAttribute("id")
            ControlIDAttribute.Value = Me.tbControlID.Text

            ControlsNode.Attributes.Append(ControlIDAttribute)

            Dim ControlTypeNode As Xml.XmlNode =
                XmlDocument.CreateNode(Xml.XmlNodeType.Element, "Type", XmlDocument.NamespaceURI)
            ControlTypeNode.InnerText = CType(Me.cbTypes.SelectedItem, String)

            ControlsNode.AppendChild(ControlTypeNode)

            Dim ControlBindNode As Xml.XmlNode =
                XmlDocument.CreateNode(Xml.XmlNodeType.Element, "Bind", XmlDocument.NamespaceURI)
            ControlBindNode.InnerText = Me.tbBind.Text

            ControlsNode.AppendChild(ControlBindNode)

            If Me.lwAttributes.Items.Count > 0 Then
                Dim AttributesNode As Xml.XmlNode =
                    XmlDocument.CreateNode(Xml.XmlNodeType.Element, "Attributes", XmlDocument.NamespaceURI)

                For Each Item As ListViewItem In Me.lwAttributes.Items
                    Dim AttributeNode As Xml.XmlNode =
                        XmlDocument.CreateNode(Xml.XmlNodeType.Element, "Attribute", XmlDocument.NamespaceURI)
                    Dim AttributeAttribute As Xml.XmlAttribute =
                        XmlDocument.CreateAttribute("key")
                    AttributeAttribute.Value = Item.Text

                    AttributeNode.Attributes.Append(AttributeAttribute)

                    AttributeNode.InnerText = Item.SubItems.Item(1).Text

                    AttributesNode.AppendChild(AttributeNode)
                Next

                ControlsNode.AppendChild(AttributesNode)
            End If

            For Each Key As String In Me._ControlSpecificOptions.Keys
                If String.Compare(Key, "BlockLocalUpdate") = 0 Then Continue For

                Dim SpecificOptionNode As Xml.XmlNode =
                    XmlDocument.CreateNode(Xml.XmlNodeType.Element, Key, XmlDocument.NamespaceURI)

                Dim Value As Object = Me._ControlSpecificOptions.Item(Key)

                If TypeOf Value Is String Then
                    SpecificOptionNode.InnerText = CType(Value, String)
                ElseIf TypeOf Value Is String() Then ' BlockIDsToUpdate
                    If CType(Value, String()).Length = 0 Then Continue For

                    If Not CType(Me._ControlSpecificOptions.Item("BlockLocalUpdate"), Boolean) Then
                        Dim SpecificOptionAttribute As Xml.XmlAttribute =
                            XmlDocument.CreateAttribute("localupdate")
                        SpecificOptionAttribute.Value = "false"

                        SpecificOptionNode.Attributes.Append(SpecificOptionAttribute)
                    End If

                    For Each Item As String In CType(Value, String())
                        Dim ItemNode As Xml.XmlNode =
                            XmlDocument.CreateNode(Xml.XmlNodeType.Element, "BlockID", XmlDocument.NamespaceURI)
                        ItemNode.InnerText = Item

                        SpecificOptionNode.AppendChild(ItemNode)
                    Next
                End If

                ControlsNode.AppendChild(SpecificOptionNode)
            Next

            XmlDocument.DocumentElement.InsertAfter(ControlsNode, XmlDocument.DocumentElement.LastChild)
            XmlDocument.Save(ControlsXMLFileLocation)

            Me.DialogResult = DialogResult.OK
        End Sub

        Private Sub cbTypes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbTypes.SelectedIndexChanged
            Select Case CType(Me.cbTypes.SelectedItem, String)
                Case "DataList", "ConditionalStatement", "VariableBlock"
                    Me.llAssignControlSpecificValues.Enabled = False
                    Me.tbBind.Enabled = True
                Case "Textarea"
                    Me.llAssignControlSpecificValues.Enabled = True
                    Me.tbBind.Enabled = False
                Case Else
                    Me.llAssignControlSpecificValues.Enabled = True
                    Me.tbBind.Enabled = True
            End Select
        End Sub

        Private Sub tbControlID_TextChanged(sender As Object, e As EventArgs) Handles tbControlID.TextChanged
            Dim ActiveDocFI As IO.FileInfo =
                New IO.FileInfo(PackageControl.IDEControl.DTE.ActiveDocument.FullName)
            Dim WorkingDI As IO.DirectoryInfo = ActiveDocFI.Directory
            Do Until WorkingDI Is Nothing OrElse String.Compare(WorkingDI.Name, "Templates") = 0
                WorkingDI = WorkingDI.Parent
            Loop
            If WorkingDI Is Nothing Then
                MessageBox.Show("Something Wrong! Reopen the document and try again...", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return
            End If
            Dim ControlsXMLFileLocation As String =
                IO.Path.Combine(WorkingDI.FullName, "Controls.xml")

            Dim cFStream As IO.FileStream = Nothing
            Try
                cFStream = New IO.FileStream(ControlsXMLFileLocation,
                                IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
                Dim xPathDocument As New Xml.XPath.XPathDocument(cFStream)
                Dim xPathNavigator As Xml.XPath.XPathNavigator =
                    xPathDocument.CreateNavigator()
                Dim xPathIter As Xml.XPath.XPathNodeIterator =
                    xPathNavigator.Select(String.Format("//Controls/Control[id='{0}']", Me.tbControlID.Text))

                If xPathIter.MoveNext() Then
                    Me.tbControlID.BackColor = Drawing.Color.LightPink
                Else
                    Me.tbControlID.BackColor = Drawing.Color.LightGreen
                End If
            Catch ex As Exception
                ' Just Handle Exceptions
            Finally
                If Not cFStream Is Nothing Then cFStream.Close()
            End Try
        End Sub

        Private _SelectionStarted As Integer = 0
        Private _IntelliListBox As IntelliListbox = Nothing
        Private Sub tbBind_KeyPress(sender As Object, e As KeyPressEventArgs) Handles tbBind.KeyPress
            If Me._IntelliListBox Is Nothing AndAlso
                Not String.IsNullOrEmpty(Me.tbBind.Text) AndAlso
                e.KeyChar <> "?"c AndAlso e.KeyChar <> "."c Then Return

            Me.ShowIntelli(e.KeyChar)
        End Sub

        Private Sub tbBind_KeyDown(sender As Object, e As KeyEventArgs) Handles tbBind.KeyDown
            If Not Me._IntelliListBox Is Nothing Then
                Select Case e.KeyCode
                    Case Keys.Escape
                        Me.Controls.Remove(Me._IntelliListBox)
                        Me._IntelliListBox = Nothing

                        e.Handled = True

                        Return
                    Case Keys.Up
                        Me._IntelliListBox.Previous()

                        e.Handled = True

                        Return
                    Case Keys.Down
                        Me._IntelliListBox.Next()

                        e.Handled = True

                        Return
                    Case Keys.Tab, Keys.Enter, Keys.Return
                        Dim SelectedValue As String =
                            Me._IntelliListBox.GetSelectedValue()

                        Me.CompleteIntelli(SelectedValue)

                        e.Handled = True

                        Return
                End Select
            Else
                If e.Control AndAlso e.KeyCode = Keys.Space Then
                    Me.ShowIntelli(Char.MinValue)
                End If
            End If
        End Sub

        Private Sub ShowIntelli(ByVal CurrentChar As Char)
            Dim CompList As Intellisense.Completion() = Nothing
            Dim Builders As Intellisense.Completion() = Nothing

            If Me.tbBind.Text.IndexOf("?"c) = -1 Then
                Dim Executable As New SourceBuilder.Executable(TemplateCommandHandler.Directives.ClientExecutable)

                CompList = Executable.Build()
                Builders = Executable.Builders()
            Else
                Dim [Class] As New SourceBuilder.Class(TemplateCommandHandler.Directives.ClientExecutable)

                [Class].WorkingExecutableInfo = Me.tbBind.Text
                CompList = [Class].Build()
                Builders = [Class].Builders()
            End If

            If Me._IntelliListBox Is Nothing Then
                Me._SelectionStarted = Me.tbBind.SelectionStart
                If CurrentChar = "?"c OrElse CurrentChar = "."c Then Me._SelectionStarted += 1

                Me._IntelliListBox = New IntelliListbox()
                AddHandler Me._IntelliListBox.NoItem,
                    New IntelliListbox.NoItemEventHandler(Sub()
                                                              Me.CompleteIntelli(String.Empty)
                                                          End Sub)
                AddHandler Me._IntelliListBox.ItemSelected,
                    New IntelliListbox.ItemSelectedEventHandler(AddressOf Me.CompleteIntelli)
            End If

            Me._IntelliListBox.Builders = Builders
            Me._IntelliListBox.ListItems = CompList

            Dim CurrentPoint As System.Drawing.Point =
                Me.tbBind.GetPositionFromCharIndex(Me.tbBind.SelectionStart - 1)
            CurrentPoint.X += Me.gbBind.Location.X + Me.tbBind.Location.X
            CurrentPoint.Y += Me.gbBind.Location.Y + (Me.gbBind.Size.Height - Me.tbBind.Height)

            Me._IntelliListBox.Location = CurrentPoint
            Me.Controls.Add(Me._IntelliListBox)
            Me._IntelliListBox.Build(Me.tbBind.Text.Substring(Me.tbBind.SelectionStart))
        End Sub

        Private Sub CompleteIntelli(ByVal value As String)
            Me.Controls.Remove(Me._IntelliListBox)
            Me._IntelliListBox = Nothing

            If Not String.IsNullOrEmpty(value) Then
                Dim ModifiedString As String =
                    Me.tbBind.Text.Remove(Me._SelectionStarted, Me.tbBind.SelectionStart - Me._SelectionStarted)
                ModifiedString = ModifiedString.Insert(Me._SelectionStarted, value)
                Me.tbBind.Text = ModifiedString
                Me.tbBind.SelectionStart = Me._SelectionStarted + value.Length
                Me.tbBind.SelectionLength = 0

                Me.ShowIntelli(Char.MinValue)
            End If
        End Sub

        Private Sub tbBind_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles tbBind.PreviewKeyDown
            e.IsInputKey = (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.Tab OrElse e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Return OrElse e.KeyCode = Keys.Escape)
        End Sub

        Private Sub tbBind_Leave(sender As Object, e As EventArgs) Handles tbBind.Leave
            If Not Me._IntelliListBox Is Nothing Then
                Me.Controls.Remove(Me._IntelliListBox)
                Me._IntelliListBox = Nothing
            End If
        End Sub

        Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
            If Me.lwAttributes.SelectedIndices.Count = 0 Then Return

            Me.lwAttributes.SuspendLayout()
            For Each ItemIndex As Integer In Me.lwAttributes.SelectedIndices
                Me.lwAttributes.Items.RemoveAt(ItemIndex)
            Next
            Me.lwAttributes.PerformLayout()
        End Sub

        Private Sub DuplicateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DuplicateToolStripMenuItem.Click
            If Me.lwAttributes.SelectedIndices.Count = 0 Then Return

            Me.lwAttributes.SuspendLayout()
            For Each ItemIndex As Integer In Me.lwAttributes.SelectedIndices
                Me.lwAttributes.Items.Add(
                    CType(Me.lwAttributes.Items.Item(ItemIndex).Clone(), ListViewItem))
            Next
            Me.lwAttributes.PerformLayout()
        End Sub

        Private Sub AddNewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewToolStripMenuItem.Click
            Dim ControlAttributeForm As New ControlAttributes()

            If ControlAttributeForm.ShowDialog(Me) = DialogResult.OK Then
                Me.lwAttributes.Items.Add(ControlAttributeForm.tbKey.Text)
                Me.lwAttributes.Items.Item(Me.lwAttributes.Items.Count - 1).SubItems.Add(ControlAttributeForm.tbValue.Text)
            End If
        End Sub

        Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click
            If Me.lwAttributes.SelectedIndices.Count = 0 Then Return

            Dim ItemIndex As Integer = Me.lwAttributes.SelectedIndices.Item(0)
            Dim ControlAttributeForm As New ControlAttributes()

            ControlAttributeForm.tbKey.Text = Me.lwAttributes.Items.Item(ItemIndex).Text
            ControlAttributeForm.tbValue.Text = Me.lwAttributes.Items.Item(ItemIndex).SubItems.Item(1).Text

            If ControlAttributeForm.ShowDialog(Me) = DialogResult.OK Then
                Me.lwAttributes.Items.Item(ItemIndex).Text = ControlAttributeForm.tbKey.Text
                Me.lwAttributes.Items.Item(ItemIndex).SubItems.Item(1).Text = ControlAttributeForm.tbValue.Text
            End If
        End Sub

        Private Sub lwAttributes_DoubleClick(sender As Object, e As EventArgs) Handles lwAttributes.DoubleClick
            If Me.lwAttributes.SelectedIndices.Count = 0 Then
                Me.AddNewToolStripMenuItem_Click(sender, Nothing)
            Else
                Me.EditToolStripMenuItem_Click(sender, Nothing)
            End If
        End Sub

        Private Sub lwAttributes_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles lwAttributes.MouseDoubleClick
            If Me.lwAttributes.SelectedIndices.Count = 0 Then
                Me.AddNewToolStripMenuItem_Click(sender, Nothing)
            Else
                Me.EditToolStripMenuItem_Click(sender, Nothing)
            End If
        End Sub

        Private Sub llAssignControlSpecificValues_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles llAssignControlSpecificValues.LinkClicked
            Select Case CType(Me.cbTypes.SelectedItem, String)
                Case "Textarea"
                    Me.TextAreaAssignment()

                Case "Button"
                    Me.ButtonAssignment()

                Case "Checkbox"
                    Me.CheckboxAssignment()

                Case "ImageButton"
                    Me.ImageButtonAssignment()

                Case "LinkButton"
                    Me.LinkButtonAssignment()

                Case "Password"
                    Me.PasswordAssignment()

                Case "RadioButton"
                    Me.RadioButtonAssignment()

                Case "Textbox"
                    Me.TextboxAssignment()

            End Select
        End Sub

        Private Sub SaveBlockIDsToUpdate(ByVal BlockLocalUpdate As Boolean, ByVal BlockIDsListItems As ListView.ListViewItemCollection)
            Me._ControlSpecificOptions.Item("BlockLocalUpdate") = BlockLocalUpdate

            Dim BlockIDList As String() =
                CType(Array.CreateInstance(GetType(String), BlockIDsListItems.Count), String())

            Dim ItemIndex As Integer = 0
            For Each Item As ListViewItem In BlockIDsListItems
                BlockIDList(ItemIndex) = Item.Text

                ItemIndex += 1
            Next
            Me._ControlSpecificOptions.Item("BlockIDsToUpdate") = BlockIDList
        End Sub

        Private Sub AssignBlockIDsToUpdate(ByRef BlockLocalUpdateCheckbox As CheckBox, ByRef BlockIDsListView As ListView)
            If Me._ControlSpecificOptions.ContainsKey("BlockLocalUpdate") Then
                BlockLocalUpdateCheckbox.Checked = CType(Me._ControlSpecificOptions.Item("BlockLocalUpdate"), Boolean)
            End If

            If Me._ControlSpecificOptions.ContainsKey("BlockIDsToUpdate") Then
                Dim BlockIDList As String() =
                    CType(Me._ControlSpecificOptions.Item("BlockIDsToUpdate"), String())

                For Each Item As String In BlockIDList
                    BlockIDsListView.Items.Add(Item)
                Next
            End If
        End Sub

        Private Sub TextAreaAssignment()
            Dim TextAreaForm As New ControlOptions.TextArea()

            If Me._ControlSpecificOptions.ContainsKey("Content") Then
                TextAreaForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Content"), String)
            End If

            If TextAreaForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Content") = TextAreaForm.tbValue.Text
            End If
        End Sub

        Private Sub ButtonAssignment()
            Dim ButtonForm As New ControlOptions.Button()

            If Me._ControlSpecificOptions.ContainsKey("Text") Then
                ButtonForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Text"), String)
            End If

            Me.AssignBlockIDsToUpdate(ButtonForm.cbUpdatesLocal, ButtonForm.lwBlockID)

            If ButtonForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Text") = ButtonForm.tbValue.Text

                Me.SaveBlockIDsToUpdate(ButtonForm.cbUpdatesLocal.Checked, ButtonForm.lwBlockID.Items)
            End If
        End Sub

        Private Sub CheckboxAssignment()
            Dim CheckboxForm As New ControlOptions.Checkbox()

            If Me._ControlSpecificOptions.ContainsKey("Text") Then
                CheckboxForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Text"), String)
            End If

            Me.AssignBlockIDsToUpdate(CheckboxForm.cbUpdatesLocal, CheckboxForm.lwBlockID)

            If CheckboxForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Text") = CheckboxForm.tbValue.Text

                Me.SaveBlockIDsToUpdate(CheckboxForm.cbUpdatesLocal.Checked, CheckboxForm.lwBlockID.Items)
            End If
        End Sub

        Private Sub ImageButtonAssignment()
            Dim ImageButtonForm As New ControlOptions.ImageButton()

            If Me._ControlSpecificOptions.ContainsKey("Source") Then
                ImageButtonForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Source"), String)
            End If

            Me.AssignBlockIDsToUpdate(ImageButtonForm.cbUpdatesLocal, ImageButtonForm.lwBlockID)

            If ImageButtonForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Source") = ImageButtonForm.tbValue.Text

                Me.SaveBlockIDsToUpdate(ImageButtonForm.cbUpdatesLocal.Checked, ImageButtonForm.lwBlockID.Items)
            End If
        End Sub

        Private Sub LinkButtonAssignment()
            Dim LinkButtonForm As New ControlOptions.LinkButton()

            If Me._ControlSpecificOptions.ContainsKey("Text") Then
                LinkButtonForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Text"), String)
            End If

            If Me._ControlSpecificOptions.ContainsKey("URL") Then
                LinkButtonForm.tbURL.Text = CType(Me._ControlSpecificOptions.Item("URL"), String)
            End If

            Me.AssignBlockIDsToUpdate(LinkButtonForm.cbUpdatesLocal, LinkButtonForm.lwBlockID)

            If LinkButtonForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Text") = LinkButtonForm.tbValue.Text
                Me._ControlSpecificOptions.Item("URL") = LinkButtonForm.tbURL.Text

                Me.SaveBlockIDsToUpdate(LinkButtonForm.cbUpdatesLocal.Checked, LinkButtonForm.lwBlockID.Items)
            End If
        End Sub

        Private Sub PasswordAssignment()
            Dim PasswordForm As New ControlOptions.Password()

            If Me._ControlSpecificOptions.ContainsKey("Text") Then
                PasswordForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Text"), String)
            End If

            If Me._ControlSpecificOptions.ContainsKey("DefaultButtonID") Then
                PasswordForm.tbDefaultButtonID.Text = CType(Me._ControlSpecificOptions.Item("DefaultButtonID"), String)
            End If

            Me.AssignBlockIDsToUpdate(PasswordForm.cbUpdatesLocal, PasswordForm.lwBlockID)

            If PasswordForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Text") = PasswordForm.tbValue.Text
                Me._ControlSpecificOptions.Item("DefaultButtonID") = PasswordForm.tbDefaultButtonID.Text

                Me.SaveBlockIDsToUpdate(PasswordForm.cbUpdatesLocal.Checked, PasswordForm.lwBlockID.Items)
            End If
        End Sub

        Private Sub RadioButtonAssignment()
            Dim RadioButtonForm As New ControlOptions.RadioButton()

            If Me._ControlSpecificOptions.ContainsKey("Text") Then
                RadioButtonForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Text"), String)
            End If

            Me.AssignBlockIDsToUpdate(RadioButtonForm.cbUpdatesLocal, RadioButtonForm.lwBlockID)

            If RadioButtonForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Text") = RadioButtonForm.tbValue.Text

                Me.SaveBlockIDsToUpdate(RadioButtonForm.cbUpdatesLocal.Checked, RadioButtonForm.lwBlockID.Items)
            End If
        End Sub

        Private Sub TextboxAssignment()
            Dim TextboxForm As New ControlOptions.Textbox()

            If Me._ControlSpecificOptions.ContainsKey("Text") Then
                TextboxForm.tbValue.Text = CType(Me._ControlSpecificOptions.Item("Text"), String)
            End If

            If Me._ControlSpecificOptions.ContainsKey("DefaultButtonID") Then
                TextboxForm.tbDefaultButtonID.Text = CType(Me._ControlSpecificOptions.Item("DefaultButtonID"), String)
            End If

            Me.AssignBlockIDsToUpdate(TextboxForm.cbUpdatesLocal, TextboxForm.lwBlockID)

            If TextboxForm.ShowDialog() = DialogResult.OK Then
                Me._ControlSpecificOptions.Item("Text") = TextboxForm.tbValue.Text
                Me._ControlSpecificOptions.Item("DefaultButtonID") = TextboxForm.tbDefaultButtonID.Text

                Me.SaveBlockIDsToUpdate(TextboxForm.cbUpdatesLocal.Checked, TextboxForm.lwBlockID.Items)
            End If
        End Sub
    End Class
End Namespace