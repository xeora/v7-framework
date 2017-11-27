Imports System.ComponentModel.Design
Imports Microsoft.VisualStudio.Shell

Namespace Xeora.Extension.VisualStudio.IDE.Command
    Public NotInheritable Class Command

        Public Const CID_ReloadExecutableLoader As Integer = 250
        Public Const CID_CompileDomain As Integer = 253
        Public Const CID_GotoControlDefinition As Integer = 256
        Public Const CID_PrepareProject As Integer = 260
        Public Const CID_RePullRelease As Integer = 265

        Public Shared ReadOnly CommandSet As New Guid("4c16f99f-8402-4b02-be4e-88a3cfdbbb55")

        Public Shared Property Instance As Command

        Public Shared Sub Initialize()
            Command.Instance = New Command()
        End Sub

        Private Sub New()
            Dim commandService As OleMenuCommandService =
                CType(PackageControl.ServiceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)

            If Not commandService Is Nothing Then
                commandService.AddCommand(
                    New MenuCommand(
                        AddressOf Me.ReloadExecutableLoader,
                        New CommandID(CommandSet, CID_ReloadExecutableLoader)
                    )
                )
                commandService.AddCommand(
                    New MenuCommand(
                        AddressOf Me.CompileDomain,
                        New CommandID(CommandSet, CID_CompileDomain)
                    )
                )
                commandService.AddCommand(
                    New MenuCommand(
                        AddressOf Me.PrepareProject,
                        New CommandID(CommandSet, CID_PrepareProject)
                    )
                )
                commandService.AddCommand(
                    New MenuCommand(
                        AddressOf Me.RePullRelease,
                        New CommandID(CommandSet, CID_RePullRelease)
                    )
                )
                commandService.AddCommand(
                    New MenuCommand(
                        AddressOf Me.GoToReferanceCallback,
                        New CommandID(CommandSet, CID_GotoControlDefinition)
                    )
                )
            End If
        End Sub

        Private Sub ReloadExecutableLoader(sender As Object, e As EventArgs)
            ExecutableLoaderHelper.DestroyAppDomain()
            ExecutableLoaderHelper.CreateAppDomain()
        End Sub

        Private Sub CompileDomain(sender As Object, e As EventArgs)
            PackageControl.IDEControl.UserCommands.CompileDomain()
        End Sub

        Private Sub GoToReferanceCallback(sender As Object, e As EventArgs)
            PackageControl.IDEControl.UserCommands.GotoControlReferance(Nothing, False, False)
        End Sub

        Private Sub PrepareProject(sender As Object, e As EventArgs)
            PackageControl.IDEControl.UserCommands.PrepareProject()
        End Sub

        Private Sub RePullRelease(sender As Object, e As EventArgs)
            ' First Reload and Release the version already using
            Me.ReloadExecutableLoader(sender, e)

            ' Then pull the Framework Release
            PackageControl.IDEControl.UserCommands.RePullRelease()
        End Sub
    End Class
End Namespace