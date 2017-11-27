Imports System.Data
Imports System.Runtime.InteropServices
Imports EnvDTE
Imports Microsoft.VisualStudio.Language
Imports Xeora.Extension.VisualStudio.IDE.Editor.Completion

Namespace Xeora.Extension.VisualStudio.Tools.Creators
    Public Class Translation
        Public ReadOnly Property SelectedTranslationID As String

        Private _LanguagesDictionary As New Generic.Dictionary(Of String, Generic.Dictionary(Of String, String))
        Private Sub Translation_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Dim ActiveDocFI As IO.FileInfo =
                New IO.FileInfo(PackageControl.IDEControl.DTE.ActiveDocument.FullName)
            Dim WorkingDI As IO.DirectoryInfo = ActiveDocFI.Directory
            Do Until WorkingDI Is Nothing OrElse String.Compare(WorkingDI.Name, "Templates") = 0
                WorkingDI = WorkingDI.Parent
            Loop
            If WorkingDI Is Nothing Then
                MessageBox.Show("Something Wrong! Reopen the document and try again...", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return
            Else
                WorkingDI = WorkingDI.Parent
                WorkingDI = New IO.DirectoryInfo(IO.Path.Combine(WorkingDI.FullName, "Languages"))
            End If

            For Each FI As IO.FileInfo In WorkingDI.GetFiles("*.xml")
                Dim XmlDocument As New Xml.XmlDocument()
                XmlDocument.Load(FI.FullName)

                Dim LanguageName As String =
                    XmlDocument.DocumentElement.Attributes.GetNamedItem("name").Value
                Dim LanguageCode As String =
                    XmlDocument.DocumentElement.Attributes.GetNamedItem("code").Value

                Me.lwLanguages.Items.Add(String.Format("{0} [{1}]", LanguageName, LanguageCode))
                Me.lwLanguages.Items.Item(Me.lwLanguages.Items.Count - 1).SubItems.Add(LanguageName)
                Me.lwLanguages.Items.Item(Me.lwLanguages.Items.Count - 1).SubItems.Add(LanguageCode)

                Dim TranslationsDictionary As New Generic.Dictionary(Of String, String)

                Dim NodeList As Xml.XmlNodeList =
                    XmlDocument.SelectNodes("/language/translation")

                For Each Node As Xml.XmlNode In NodeList
                    Dim TranslationID As String =
                        Node.Attributes.GetNamedItem("id").Value
                    Dim TranslationValue As String = String.Empty
                    If Node.ChildNodes.Count > 0 Then _
                        TranslationValue = Node.ChildNodes.Item(0).Value

                    TranslationsDictionary.Item(TranslationID) = TranslationValue
                Next

                Me._LanguagesDictionary.Item(LanguageCode) = TranslationsDictionary
            Next

            Dim MaxCount As Integer = 0, KeyName As String = String.Empty
            For Each Key As String In Me._LanguagesDictionary.Keys
                If Me._LanguagesDictionary.Item(Key).Count > MaxCount Then
                    MaxCount = Me._LanguagesDictionary.Item(Key).Count
                    KeyName = Key
                End If
            Next

            If Not String.IsNullOrEmpty(KeyName) Then
                Dim WorkingDictionary As Generic.Dictionary(Of String, String) =
                    Me._LanguagesDictionary.Item(KeyName)

                For Each Key As String In WorkingDictionary.Keys
                    Me.lwTranslations.Items.Add(Key)
                Next
            End If
        End Sub

        Private Sub butAccept_Click(sender As Object, e As EventArgs) Handles butAccept.Click
            Dim ActiveDocFI As IO.FileInfo =
                New IO.FileInfo(PackageControl.IDEControl.DTE.ActiveDocument.FullName)
            Dim WorkingDI As IO.DirectoryInfo = ActiveDocFI.Directory
            Do Until WorkingDI Is Nothing OrElse String.Compare(WorkingDI.Name, "Templates") = 0
                WorkingDI = WorkingDI.Parent
            Loop
            If WorkingDI Is Nothing Then
                MessageBox.Show("Something Wrong! Reopen the document and try again...", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return
            Else
                WorkingDI = WorkingDI.Parent
                WorkingDI = New IO.DirectoryInfo(IO.Path.Combine(WorkingDI.FullName, "Languages"))
            End If

            Dim MaxCount As Integer = 0, KeyName As String = String.Empty
            For Each Key As String In Me._LanguagesDictionary.Keys
                If Me._LanguagesDictionary.Item(Key).Count > MaxCount Then
                    MaxCount = Me._LanguagesDictionary.Item(Key).Count
                    KeyName = Key
                End If
            Next

            Dim TemplateTranslationsDictionary As Generic.Dictionary(Of String, String) =
                Me._LanguagesDictionary.Item(KeyName)

            For Each Key As String In Me._LanguagesDictionary.Keys
                Dim Name As String = String.Empty
                For Each Item As ListViewItem In Me.lwLanguages.Items
                    If String.Compare(Item.SubItems.Item(2).Text, Key) = 0 Then
                        Name = Item.SubItems.Item(1).Text

                        Exit For
                    End If
                Next

                Dim LanguageFI As New IO.FileInfo(IO.Path.Combine(WorkingDI.FullName, String.Format("{0}.xml", Key)))

                If Not LanguageFI.Exists Then
                    Dim SW As IO.StreamWriter =
                        LanguageFI.CreateText()
                    SW.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
                    SW.WriteLine(String.Format("<language name=""{0}"" code=""{1}"" />", Name, Key))
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
                    WorkingProjectItems = CType(WorkingProjectItems.Collection.Parent, ProjectItem)

                    Dim ContentsProjectItem As ProjectItem = WorkingProjectItems.Collection.Item("Contents")
                    WorkingProjectItems = WorkingProjectItems.Collection.Item("Languages")

                    WorkingProjectItems.ProjectItems.AddFromFile(LanguageFI.FullName)

                    Dim ContentsLocation As String =
                        IO.Path.Combine(WorkingDI.Parent.FullName, "Contents", Key)
                    If Not IO.Directory.Exists(ContentsLocation) Then
                        IO.Directory.CreateDirectory(ContentsLocation)

                        ContentsProjectItem.ProjectItems.AddFromDirectory(ContentsLocation)

                        Dim SubProjectItem As ProjectItem =
                            ContentsProjectItem.ProjectItems.Item(Key)
                        Dim StyleFileLocation As String =
                            IO.Path.Combine(ContentsLocation, "styles.css")
                        SW = IO.File.CreateText(StyleFileLocation)
                        SW.WriteLine("/* Default CSS Stylesheet for a New Xeora Web Application project */")
                        SW.Close()
                        SubProjectItem.ProjectItems.AddFromFile(StyleFileLocation)
                    End If
                End If

                Dim TranslationsDictionary As Generic.Dictionary(Of String, String) =
                    Me._LanguagesDictionary.Item(Key)

                Dim XmlDocument As New Xml.XmlDocument()
                XmlDocument.Load(LanguageFI.FullName)

                Dim CurrentName As String =
                    XmlDocument.DocumentElement.Attributes.GetNamedItem("name").Value
                If String.Compare(CurrentName, Name) <> 0 Then _
                    XmlDocument.DocumentElement.Attributes.GetNamedItem("name").Value = Name

                Dim CurrentCode As String =
                    XmlDocument.DocumentElement.Attributes.GetNamedItem("code").Value
                If String.Compare(CurrentCode, Key) <> 0 Then _
                    XmlDocument.DocumentElement.Attributes.GetNamedItem("code").Value = Key

                For Each TranslationKey As String In TemplateTranslationsDictionary.Keys
                    Dim TranslationValue As String = String.Empty
                    TranslationsDictionary.TryGetValue(TranslationKey, TranslationValue)

                    Dim XmlNode As Xml.XmlNode =
                        XmlDocument.SelectSingleNode(String.Format("/language/translation[@id='{0}']", TranslationKey))

                    If XmlNode Is Nothing OrElse
                        XmlNode.ChildNodes.Count = 0 OrElse
                        (XmlNode.ChildNodes.Count > 0 AndAlso String.Compare(XmlNode.ChildNodes.Item(0).Value, TranslationValue) <> 0) Then

                        Dim TranslationNode As Xml.XmlNode =
                            XmlDocument.CreateNode(Xml.XmlNodeType.Element, "translation", XmlDocument.NamespaceURI)

                        Dim TranslationIDAttribute As Xml.XmlAttribute =
                            XmlDocument.CreateAttribute("id")
                        TranslationIDAttribute.Value = TranslationKey

                        TranslationNode.Attributes.Append(TranslationIDAttribute)

                        If Not String.IsNullOrEmpty(TranslationValue) Then _
                            TranslationNode.InnerText = TranslationValue

                        If Not XmlNode Is Nothing Then
                            XmlDocument.DocumentElement.ReplaceChild(TranslationNode, XmlNode)
                        Else
                            XmlDocument.DocumentElement.InsertAfter(TranslationNode, XmlDocument.DocumentElement.LastChild)
                        End If
                    End If
                Next

                XmlDocument.Save(LanguageFI.FullName)
            Next

            Me.DialogResult = DialogResult.OK
        End Sub

        Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
            Dim ListViewObject As ListView

            If TypeOf sender Is ListView Then
                ListViewObject = CType(sender, ListView)
            Else
                ListViewObject = CType(CType(CType(sender, ToolStripItem).Owner, ContextMenuStrip).SourceControl, ListView)
            End If

            If ListViewObject.SelectedIndices.Count = 0 Then Return

            If MessageBox.Show("Are you sure to delete?", "QUESTION?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                ListViewObject.SuspendLayout()
                For Each ItemIndex As Integer In ListViewObject.SelectedIndices
                    If ListViewObject Is Me.lwLanguages Then
                        If Me._LanguagesDictionary.ContainsKey(ListViewObject.Items.Item(ItemIndex).SubItems(2).Text) Then _
                            Me._LanguagesDictionary.Remove(ListViewObject.Items.Item(ItemIndex).SubItems(2).Text)
                    ElseIf ListViewObject Is Me.lwTranslations Then
                        For Each dR As DataGridViewRow In Me.dgvTranslation.Rows
                            Dim TranslationDictionary As Generic.Dictionary(Of String, String) = Nothing
                            Me._LanguagesDictionary.TryGetValue(CType(dR.Cells.Item(0).Value, String), TranslationDictionary)

                            If Not TranslationDictionary Is Nothing AndAlso TranslationDictionary.ContainsKey(ListViewObject.Items.Item(ItemIndex).Text) Then _
                                TranslationDictionary.Remove(ListViewObject.Items.Item(ItemIndex).Text)

                            Me._LanguagesDictionary.Item(CType(dR.Cells.Item(0).Value, String)) = TranslationDictionary
                        Next
                    End If

                    ListViewObject.Items.RemoveAt(ItemIndex)
                Next
                ListViewObject.PerformLayout()
            End If

            If Me._LanguagesDictionary.Count = 0 Then Me.lwTranslations.Items.Clear()
        End Sub

        Private Sub AddNewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewToolStripMenuItem.Click
            Dim ListViewObject As ListView

            If TypeOf sender Is ListView Then
                ListViewObject = CType(sender, ListView)
            Else
                ListViewObject = CType(CType(CType(sender, ToolStripItem).Owner, ContextMenuStrip).SourceControl, ListView)
            End If

            If ListViewObject Is Me.lwLanguages Then
                Dim LanguageSettingsForm As New LanguageSettings()

                If LanguageSettingsForm.ShowDialog(Me) = DialogResult.OK Then
                    If Me._LanguagesDictionary.ContainsKey(LanguageSettingsForm.tbLanguageCode.Text) Then
                        MessageBox.Show("Language is already exists!", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        Return
                    End If

                    Me.lwLanguages.Items.Add(String.Format("{0} [{1}]", LanguageSettingsForm.tbLanguageName.Text, LanguageSettingsForm.tbLanguageCode.Text))
                    Me.lwLanguages.Items.Item(Me.lwLanguages.Items.Count - 1).SubItems.Add(LanguageSettingsForm.tbLanguageName.Text)
                    Me.lwLanguages.Items.Item(Me.lwLanguages.Items.Count - 1).SubItems.Add(LanguageSettingsForm.tbLanguageCode.Text)

                    Me._LanguagesDictionary.Item(LanguageSettingsForm.tbLanguageCode.Text) = New Generic.Dictionary(Of String, String)
                End If
            ElseIf ListViewObject Is Me.lwTranslations Then
                Dim TranslationSettingsForm As New TranslationSettings()

                If TranslationSettingsForm.ShowDialog(Me) = DialogResult.OK Then
                    Me.lwTranslations.Items.Add(TranslationSettingsForm.tbTranslationID.Text)

                    For Each Key As String In Me._LanguagesDictionary.Keys
                        Me._LanguagesDictionary.Item(Key).Item(TranslationSettingsForm.tbTranslationID.Text) = String.Empty
                    Next

                    Me.lwTranslations.SelectedIndices.Add(Me.lwTranslations.Items.Count - 1)
                End If
            End If
        End Sub

        Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click
            Dim ListViewObject As ListView

            If TypeOf sender Is ListView Then
                ListViewObject = CType(sender, ListView)
            Else
                ListViewObject = CType(CType(CType(sender, ToolStripItem).Owner, ContextMenuStrip).SourceControl, ListView)
            End If

            If ListViewObject.SelectedIndices.Count = 0 Then Return

            If ListViewObject Is Me.lwLanguages Then
                Dim ItemIndex As Integer = ListViewObject.SelectedIndices.Item(0)
                Dim LanguageSettingsForm As New LanguageSettings()

                LanguageSettingsForm.tbLanguageCode.Text = ListViewObject.Items.Item(ItemIndex).SubItems.Item(2).Text
                LanguageSettingsForm.tbLanguageName.Text = ListViewObject.Items.Item(ItemIndex).SubItems.Item(1).Text

                If LanguageSettingsForm.ShowDialog(Me) = DialogResult.OK Then
                    If String.Compare(ListViewObject.Items.Item(ItemIndex).SubItems.Item(2).Text, LanguageSettingsForm.tbLanguageCode.Text) <> 0 Then
                        Dim TranslationsDictionary As Generic.Dictionary(Of String, String) = Nothing
                        Me._LanguagesDictionary.TryGetValue(ListViewObject.Items.Item(ItemIndex).SubItems.Item(2).Text, TranslationsDictionary)

                        Me._LanguagesDictionary.Item(LanguageSettingsForm.tbLanguageCode.Text) = TranslationsDictionary
                        Me._LanguagesDictionary.Remove(ListViewObject.Items.Item(ItemIndex).SubItems.Item(2).Text)
                    End If

                    ListViewObject.Items.Item(ItemIndex).SubItems.Item(2).Text = LanguageSettingsForm.tbLanguageCode.Text
                    ListViewObject.Items.Item(ItemIndex).SubItems.Item(1).Text = LanguageSettingsForm.tbLanguageName.Text

                    ListViewObject.Items.Item(ItemIndex).Text = String.Format("{0} [{1}]", LanguageSettingsForm.tbLanguageName.Text, LanguageSettingsForm.tbLanguageCode.Text)
                End If
            ElseIf ListViewObject Is Me.lwTranslations Then
                Dim ItemIndex As Integer = ListViewObject.SelectedIndices.Item(0)
                Dim TranslationSettingsForm As New TranslationSettings()

                TranslationSettingsForm.tbTranslationID.Text = ListViewObject.Items.Item(ItemIndex).Text

                If TranslationSettingsForm.ShowDialog(Me) = DialogResult.OK Then
                    If String.Compare(TranslationSettingsForm.tbTranslationID.Text, ListViewObject.Items.Item(ItemIndex).Text) <> 0 Then
                        For Each Key As String In Me._LanguagesDictionary.Keys
                            If Me._LanguagesDictionary.Item(Key).ContainsKey(ListViewObject.Items.Item(ItemIndex).Text) Then
                                Me._LanguagesDictionary.Item(Key).Item(TranslationSettingsForm.tbTranslationID.Text) =
                                    Me._LanguagesDictionary.Item(Key).Item(ListViewObject.Items.Item(ItemIndex).Text)
                                Me._LanguagesDictionary.Item(Key).Remove(ListViewObject.Items.Item(ItemIndex).Text)
                            End If
                        Next
                    End If

                    ListViewObject.Items.Item(ItemIndex).Text = TranslationSettingsForm.tbTranslationID.Text
                End If
            End If
        End Sub

        Private Sub lwTranslations_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles lwTranslations.ItemSelectionChanged
            Dim ListViewObject As ListView = CType(sender, ListView)

            If ListViewObject.SelectedIndices.Count = 0 Then
                Me.dgvTranslation.Rows.Clear()

                Return
            End If

            Me._SelectedTranslationID = ListViewObject.Items.Item(ListViewObject.SelectedIndices.Item(0)).Text

            Me.dgvTranslation.SuspendLayout()
            Me.dgvTranslation.Rows.Clear()
            For Each Item As ListViewItem In Me.lwLanguages.Items
                Dim TranslationDictionary As Generic.Dictionary(Of String, String) = Nothing
                Me._LanguagesDictionary.TryGetValue(Item.SubItems(2).Text, TranslationDictionary)

                Dim TranslationValue As String = String.Empty
                If Not TranslationDictionary Is Nothing Then _
                    TranslationDictionary.TryGetValue(Me.SelectedTranslationID, TranslationValue)

                Me.dgvTranslation.Rows.Add(New Object() {Item.SubItems(2).Text, TranslationValue})
            Next
            Me.dgvTranslation.PerformLayout()
        End Sub

        Private Sub llAddLanguage_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles llAddLanguage.LinkClicked
            Me.AddNewToolStripMenuItem_Click(Me.lwLanguages, Nothing)
        End Sub

        Private Sub llAddTranslation_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles llAddTranslation.LinkClicked
            Me.AddNewToolStripMenuItem_Click(Me.lwTranslations, Nothing)
        End Sub

        Private Sub dgvTranslation_Leave(sender As Object, e As EventArgs) Handles dgvTranslation.Leave
            For Each dR As DataGridViewRow In Me.dgvTranslation.Rows
                Dim TranslationDictionary As Generic.Dictionary(Of String, String) = Nothing
                Me._LanguagesDictionary.TryGetValue(CType(dR.Cells.Item(0).Value, String), TranslationDictionary)

                If Not TranslationDictionary Is Nothing Then _
                    TranslationDictionary.Item(Me._SelectedTranslationID) = CType(dR.Cells.Item(1).Value, String)

                Me._LanguagesDictionary.Item(CType(dR.Cells.Item(0).Value, String)) = TranslationDictionary
            Next
        End Sub
    End Class
End Namespace