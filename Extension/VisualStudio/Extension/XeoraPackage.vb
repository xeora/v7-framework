Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Xeora.Extension.VisualStudio
    <PackageRegistration(UseManagedResourcesOnly:=True)>
    <InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400)>
    <ProvideMenuResource("Menus.ctmenu", 1)>
    <Guid(XeoraPackage.PackageGuidString)>
    <ProvideAutoLoad(UIContextGuids80.SolutionExists)>
    Public NotInheritable Class XeoraPackage
        Inherits Package

        Public Const PackageGuidString As String = "45fe53f7-dca7-49f1-8c9e-bdf5eba3d7a5"

#Region "Package Members"

        ''' <summary>
        ''' Initialization of the package; this method is called right after the package is sited, so this is the place
        ''' where you can put all the initialization code that rely on services provided by VisualStudio.
        ''' </summary>
        Protected Overrides Sub Initialize()
            PackageControl.Initialize(Me)
            MyBase.Initialize()
        End Sub

#End Region

    End Class
End Namespace
