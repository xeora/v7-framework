Imports EnvDTE
Imports EnvDTE80

Namespace Xeora.Extension.VisualStudio.Tools.Creators
    Public Class Executable
        Private Sub butAccept_Click(sender As Object, e As EventArgs) Handles butAccept.Click
            If String.IsNullOrEmpty(Me.tbExecutableID.Text) Then
                Me.tbExecutableID.BackColor = Drawing.Color.LightPink

                Return
            End If

            If String.IsNullOrEmpty(Me.tbProjectLocation.Text) Then
                Me.tbProjectLocation.BackColor = Drawing.Color.LightPink

                Return
            End If

            Dim Solution As Solution2 = CType(PackageControl.IDEControl.DTE.Solution, Solution2)

            Dim ExtensionLocation As String =
                IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)

            Dim WorkingFolder As String =
                IO.Path.GetDirectoryName(PackageControl.IDEControl.DTE.ActiveDocument.FullName)
            Dim WorkingFolderDI As New IO.DirectoryInfo(WorkingFolder)

            Dim ExecutablePath As String = String.Empty
            Do Until WorkingFolderDI Is Nothing OrElse String.Compare(WorkingFolderDI.Name, "Domains") = 0
                WorkingFolderDI = WorkingFolderDI.Parent

                If String.IsNullOrEmpty(ExecutablePath) Then _
                    ExecutablePath = IO.Path.Combine(WorkingFolderDI.FullName, "Executables")
            Loop
            If WorkingFolderDI Is Nothing Then
                MessageBox.Show(Me, "Release Folder has not been found!", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return
            End If
            WorkingFolderDI = WorkingFolderDI.Parent

            Dim PA As Reflection.ProcessorArchitecture =
                ExecutableLoaderHelper.ExecutableLoader.FrameworkArchitecture(
                    IO.Path.Combine(WorkingFolderDI.FullName, "bin"))

            Dim TemplatePath As String
            If Me.rbCSharp.Checked Then
                If PA = Reflection.ProcessorArchitecture.Amd64 Then
                    TemplatePath = IO.Path.Combine(ExtensionLocation, "ProjectTemplates", "CSharpx64", "MyTemplate.vstemplate")
                Else
                    TemplatePath = IO.Path.Combine(ExtensionLocation, "ProjectTemplates", "CSharpx86", "MyTemplate.vstemplate")
                End If
            Else
                If PA = Reflection.ProcessorArchitecture.Amd64 Then
                    TemplatePath = IO.Path.Combine(ExtensionLocation, "ProjectTemplates", "VBx64", "MyTemplate.vstemplate")
                Else
                    TemplatePath = IO.Path.Combine(ExtensionLocation, "ProjectTemplates", "VBx86", "MyTemplate.vstemplate")
                End If
            End If

            Solution.AddFromTemplate(TemplatePath, Me.tbProjectLocation.Text, String.Format("Xeora.PlugIn.{0}", Me.tbExecutableID.Text), False)

            For Each Project As Project In Solution.Projects
                If String.Compare(Project.Name, String.Format("Xeora.PlugIn.{0}", Me.tbExecutableID.Text)) = 0 Then
                    Project.Properties.Item("DefaultNamespace").Value = "Xeora.Domain"
                    If Me.rbCSharp.Checked Then
                        Project.Properties.Item("RootNameSpace").Value = "Xeora.Domain"
                    Else
                        Project.Properties.Item("RootNameSpace").Value = ""
                    End If
                    Project.Properties.Item("AssemblyName").Value = String.Format("{0}Lib", Me.tbExecutableID.Text)

                    Project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value = ExecutablePath

                    CType(Project.Object, VSLangProj.VSProject).References.Add("System.dll")
                    CType(Project.Object, VSLangProj.VSProject).References.Add("System.Web.dll")
                    CType(Project.Object, VSLangProj.VSProject).References.Add(
                        IO.Path.Combine(WorkingFolderDI.FullName, "bin", "Xeora.Web.Shared.dll")).CopyLocal = False

                    Dim ClassItemTemplateLocation As String
                    If Me.rbCSharp.Checked Then
                        ClassItemTemplateLocation = IO.Path.Combine(ExtensionLocation, "ItemTemplates", "ClassCSharp", "MyTemplate.vstemplate")
                    Else
                        ClassItemTemplateLocation = IO.Path.Combine(ExtensionLocation, "ItemTemplates", "ClassVB", "MyTemplate.vstemplate")
                    End If

                    Project.ProjectItems.AddFromTemplate(ClassItemTemplateLocation, String.Format("{0}Lib.{1}", Me.tbExecutableID.Text, IIf(Me.rbCSharp.Checked, "cs", "vb")))

                    Exit For
                End If
            Next

            Me.DialogResult = DialogResult.OK
        End Sub

        Private Sub tbExecutableID_TextChanged(sender As Object, e As EventArgs) Handles tbExecutableID.TextChanged
            Dim Solution As Solution2 = CType(PackageControl.IDEControl.DTE.Solution, Solution2)

            For Each Project As Project In Solution.Projects
                If String.Compare(Project.Name, String.Format("Xeora.PlugIn.{0}", Me.tbExecutableID.Text), True) = 0 Then
                    Me.tbExecutableID.BackColor = Drawing.Color.LightPink

                    Exit Sub
                End If
            Next

            Me.tbExecutableID.BackColor = Drawing.Color.LightGreen
        End Sub

        Private Sub butBrowse_Click(sender As Object, e As EventArgs) Handles butBrowse.Click, tbProjectLocation.Click
            Dim FolderBrowser As New FolderBrowserDialog

            Dim ProjectOutputPath As String =
                IO.Path.Combine(IO.Path.GetDirectoryName(PackageControl.IDEControl.DTE.Solution.FullName), String.Format("Xeora.PlugIn.{0}", Me.tbExecutableID.Text))
            If Not IO.Directory.Exists(ProjectOutputPath) Then IO.Directory.CreateDirectory(ProjectOutputPath)

            FolderBrowser.SelectedPath = ProjectOutputPath
            FolderBrowser.ShowNewFolderButton = True

            If FolderBrowser.ShowDialog(Me) = DialogResult.OK Then
                If String.Compare(ProjectOutputPath, FolderBrowser.SelectedPath, True) <> 0 Then
                    If IO.Directory.GetFiles(ProjectOutputPath).Length = 0 AndAlso
                        IO.Directory.GetDirectories(ProjectOutputPath).Length = 0 Then

                        If IO.Directory.Exists(ProjectOutputPath) Then _
                            IO.Directory.Delete(ProjectOutputPath)
                    End If
                End If

                Me.tbProjectLocation.Text = FolderBrowser.SelectedPath
                Me.tbProjectLocation.BackColor = Drawing.Color.LightGreen
            End If
        End Sub
    End Class
End Namespace