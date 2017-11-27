Option Strict On

Imports EnvDTE
Imports Xeora.Extension.Tools

Namespace Xeora.Extension.VisualStudio.Tools
    Public Class CompilerForm
        Private _Project As Project

        Private Function FindAllDomains(ByVal ProjectItems As ProjectItems, ByVal ParentName As String) As String()
            Dim rList As New List(Of String)

            If ProjectItems Is Nothing Then Return rList.ToArray()

            For Each ProjectItem As ProjectItem In ProjectItems
                If String.Compare(ProjectItem.Name, "Domains", True) = 0 OrElse
                    String.Compare(ProjectItem.Name, "Addons", True) = 0 Then

                    For Each SubProjectItem As ProjectItem In ProjectItem.ProjectItems
                        Dim DomainPath As String =
                            String.Format("{0}\{1}", ParentName, SubProjectItem.Name)

                        rList.Add(DomainPath)
                        rList.AddRange(Me.FindAllDomains(SubProjectItem.ProjectItems, DomainPath))
                    Next

                    Exit For
                End If

                rList.AddRange(Me.FindAllDomains(ProjectItem.ProjectItems, ParentName))
            Next

            Return rList.ToArray()
        End Function

        Private Function GetDomainPathLocation(ByVal ProjectItems As ProjectItems, ByVal DomainPath As String()) As String
            If ProjectItems Is Nothing OrElse DomainPath.Length = 0 Then Return String.Empty

            For Each ProjectItem As ProjectItem In ProjectItems
                If String.IsNullOrEmpty(DomainPath(0)) AndAlso
                    String.Compare(ProjectItem.Name, "Domains", True) = 0 Then

                    For Each SubProjectItem As ProjectItem In ProjectItem.ProjectItems
                        If String.Compare(DomainPath(1), SubProjectItem.Name) = 0 Then
                            If DomainPath.Length - 2 < 1 Then _
                                Return CType(SubProjectItem.Properties.Item("FullPath").Value, String)

                            Dim NewDomainPath As String() = CType(Array.CreateInstance(GetType(String), DomainPath.Length - 2), String())
                            Array.Copy(DomainPath, 2, NewDomainPath, 0, NewDomainPath.Length)

                            Return Me.GetDomainPathLocation(SubProjectItem.ProjectItems, NewDomainPath)
                        End If
                    Next

                    Exit For
                End If

                If String.Compare(ProjectItem.Name, "Addons", True) = 0 Then
                    For Each SubProjectItem As ProjectItem In ProjectItem.ProjectItems
                        If String.Compare(DomainPath(0), SubProjectItem.Name) = 0 Then
                            If DomainPath.Length - 1 < 1 Then _
                                Return CType(SubProjectItem.Properties.Item("FullPath").Value, String)

                            Dim NewDomainPath As String() = CType(Array.CreateInstance(GetType(String), DomainPath.Length - 1), String())
                            Array.Copy(DomainPath, 1, NewDomainPath, 0, NewDomainPath.Length)

                            Return Me.GetDomainPathLocation(SubProjectItem.ProjectItems, NewDomainPath)
                        End If
                    Next

                    Exit For
                End If

                Dim DomainPathLocation As String =
                    Me.GetDomainPathLocation(ProjectItem.ProjectItems, DomainPath)

                If Not String.IsNullOrEmpty(DomainPathLocation) Then Return DomainPathLocation
            Next

            Return String.Empty
        End Function

        Public Shadows Function ShowDialog(ByVal owner As IWin32Window, ByVal Project As Project) As DialogResult
            Me._Project = Project

            Dim Domains As String() =
                Me.FindAllDomains(Project.ProjectItems, String.Empty)

            If Domains.Length = 0 Then Return DialogResult.OK

            For Each Domain As String In Domains
                Me.dgvDomains.Rows.Add(0, Domain, 0, String.Empty, String.Empty)
            Next

            MyBase.ShowDialog(owner)
        End Function

        Private Sub dgvDomains_CellContentClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs) Handles dgvDomains.CellContentClick
            Dim dgvDomains As DataGridView = CType(sender, DataGridView)

            Select Case e.ColumnIndex
                Case 0, 2
                    dgvDomains.CommitEdit(DataGridViewDataErrorContexts.Commit)
            End Select
        End Sub

        Private Sub dgvDomains_CellValueChanged(ByVal sender As Object, e As DataGridViewCellEventArgs) Handles dgvDomains.CellValueChanged
            If e.RowIndex = -1 Then Return

            Dim dgvDomains As DataGridView = CType(sender, DataGridView)

            Select Case e.ColumnIndex
                Case 0
                    Dim CheckTotal As Integer = 0
                    For rC As Integer = 0 To Me.dgvDomains.Rows.Count - 1
                        If CType(dgvDomains.Item(e.ColumnIndex, rC).Value, Integer) = 1 Then CheckTotal += 1
                    Next

                    RemoveHandler Me.cbCheckAll.CheckedChanged, AddressOf Me.cbCheckAll_CheckedChanged
                    If CheckTotal = Me.dgvDomains.Rows.Count Then
                        Me.cbCheckAll.CheckState = CheckState.Checked
                    Else
                        Me.cbCheckAll.CheckState = CheckState.Unchecked
                    End If
                    AddHandler Me.cbCheckAll.CheckedChanged, AddressOf Me.cbCheckAll_CheckedChanged
                Case 3
                    dgvDomains.Item(4, e.RowIndex).Value =
                        CType(dgvDomains.Item(e.ColumnIndex, e.RowIndex).Value, String)
            End Select
        End Sub

        Private Sub cbCheckAll_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbCheckAll.CheckedChanged
            For rC As Integer = 0 To Me.dgvDomains.Rows.Count - 1
                RemoveHandler Me.dgvDomains.CellValueChanged, AddressOf Me.dgvDomains_CellValueChanged
                Me.dgvDomains.Item(0, rC).Value = IIf(Me.cbCheckAll.Checked, 1, 0)
                AddHandler Me.dgvDomains.CellValueChanged, AddressOf Me.dgvDomains_CellValueChanged
            Next
        End Sub

        Private _DomainCompilerInfos As New Concurrent.ConcurrentDictionary(Of String, DomainCompilerInfo)
        Private Sub butCompile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butCompile.Click
            Me._DomainCompilerInfos.Clear()

            For rC As Integer = 0 To Me.dgvDomains.Rows.Count - 1
                If CType(Me.dgvDomains.Item(0, rC).Value, Integer) = 1 Then
                    Dim DomainPath As String =
                        Me.GetDomainPathLocation(Me._Project.ProjectItems, CType(Me.dgvDomains.Item(1, rC).Value, String).Split("\"c))

                    If Not String.IsNullOrEmpty(DomainPath) Then
                        Dim Password As String = CType(Me.dgvDomains.Item(3, rC).Value, String)
                        If CType(Me.dgvDomains.Item(2, rC).Value, Integer) = 0 Then Password = Nothing

                        Dim DomainCompilerInfo As New DomainCompilerInfo(DomainPath, Password)
                        Me._DomainCompilerInfos.Item(DomainCompilerInfo.ID) = DomainCompilerInfo
                    End If
                End If
            Next

            If Me._DomainCompilerInfos.Count = 0 Then
                MessageBox.Show(Me, "You should select at least one domain to compile", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Return
            End If

            Dim ProcessThread As New Threading.Thread(AddressOf Me.StartProcess)

            Me.dgvDomains.Enabled = False
            Me.cbShowPassword.Enabled = False
            Me.butCompile.Enabled = False

            ProcessThread.Start()
        End Sub

        Private Sub StartProcess()
            Dim DomainNumber As Integer = 1
            For Each DomainCompilerInfoID As String In Me._DomainCompilerInfos.Keys
                Dim DomainCompilerInfo As DomainCompilerInfo =
                    Me._DomainCompilerInfos.Item(DomainCompilerInfoID)
                Dim DI As New IO.DirectoryInfo(DomainCompilerInfo.DomainPath)

                Me.lCurrentProcess.Text = DomainNumber.ToString()

                Dim XeoraCompiler As New Compiler(DomainCompilerInfo.DomainPath)
                AddHandler XeoraCompiler.Progress, New Compiler.ProgressEventHandler(AddressOf UpdateProgress)

                DomainCompilerInfo.RemoveTarget()

                Me.AddFiles(DomainCompilerInfo.DomainPath, XeoraCompiler)

                Dim ContentFS As IO.Stream = Nothing
                Try
                    ContentFS = New IO.FileStream(DomainCompilerInfo.OutputFile, IO.FileMode.Create, IO.FileAccess.ReadWrite)

                    XeoraCompiler.CreateDomainFile(DomainCompilerInfo.Password, ContentFS)
                Catch ex As Exception
                    Me.Invoke(Sub() MessageBox.Show(Me, ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error))
                Finally
                    If Not ContentFS Is Nothing Then _
                        ContentFS.Close() : GC.SuppressFinalize(ContentFS)
                End Try

                If Not XeoraCompiler.PasswordHash Is Nothing Then
                    Dim SecuredFS As IO.Stream = Nothing
                    Try
                        SecuredFS = New IO.FileStream(DomainCompilerInfo.KeyFile, IO.FileMode.Create, IO.FileAccess.ReadWrite)

                        SecuredFS.Write(XeoraCompiler.PasswordHash, 0, XeoraCompiler.PasswordHash.Length)
                    Catch ex As Exception
                        Me.Invoke(Sub() MessageBox.Show(Me, ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    Finally
                        If Not SecuredFS Is Nothing Then _
                            SecuredFS.Close() : GC.SuppressFinalize(SecuredFS)
                    End Try
                End If

                DomainNumber += 1
            Next

            Me.dgvDomains.Enabled = True
            Me.cbShowPassword.Enabled = True
            Me.butCompile.Enabled = True

            Me.Close()
        End Sub

        Private Sub AddFiles(ByVal WorkingPath As String, ByRef XeoraCompilerObj As Compiler)
            For Each path As String In IO.Directory.GetDirectories(WorkingPath)
                Dim DI As New IO.DirectoryInfo(path)

                If String.Compare(DI.Name, "addons", True) <> 0 AndAlso
                    String.Compare(DI.Name, "executables", True) <> 0 Then

                    Me.AddFiles(path, XeoraCompilerObj)
                End If
            Next

            For Each file As String In IO.Directory.GetFiles(WorkingPath)
                XeoraCompilerObj.AddFile(file)
            Next
        End Sub

        Private Sub UpdateProgress(ByVal Current As Integer, ByVal Total As Integer)
            Me.ProgressBar.Minimum = 0
            Me.ProgressBar.Maximum = Total

            Me.ProgressBar.Value = Current
        End Sub

        Private Class DomainCompilerInfo
            Public Sub New(ByVal DomainPath As String, Password As String)
                Me.ID = Guid.NewGuid().ToString()

                If DomainPath.Chars(DomainPath.Length - 1) = "\"c Then _
                    DomainPath = DomainPath.Remove(DomainPath.Length - 1, 1)
                Me.DomainPath = DomainPath
                Me.Password = Password

                Me.OutputFile = IO.Path.Combine(Me.DomainPath, "Content.xeora")
                Me.KeyFile = IO.Path.Combine(Me.DomainPath, "Content.secure")
            End Sub

            Public ReadOnly Property ID As String
            Public ReadOnly Property DomainPath As String
            Public ReadOnly Property Password As String
            Public ReadOnly Property OutputFile As String
            Public ReadOnly Property KeyFile As String

            Public Sub RemoveTarget()
                Try
                    IO.File.Delete(Me.OutputFile)
                Catch ex As Exception
                    ' Just Handle Exceptions
                End Try

                Try
                    IO.File.Delete(Me.KeyFile)
                Catch ex As Exception
                    ' Just Handle Exceptions
                End Try
            End Sub
        End Class
    End Class
End Namespace