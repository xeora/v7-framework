Imports Xeora.Extension.Executable

Namespace Xeora.Extension.VisualStudio
    Public Class ExecutableLoaderHelper
        Private Shared _AppDomain As AppDomain = Nothing
        Private Shared _ExecutableLoader As ILoader = Nothing

        Public Shared Sub CreateAppDomain()
            If ExecutableLoaderHelper._AppDomain Is Nothing Then
                Dim AppDomainID As String = Guid.NewGuid().ToString()
                Dim ExecutingAssembly As System.Reflection.Assembly =
                    System.Reflection.Assembly.GetExecutingAssembly()
                Dim ExecutingPath As String =
                    IO.Path.GetDirectoryName(ExecutingAssembly.Location)

                AddHandler System.AppDomain.CurrentDomain.AssemblyResolve, AddressOf ExecutableLoaderHelper.AssemblyResolve

                Dim AppDomainInfo As New AppDomainSetup()

                With AppDomainInfo
                    .ApplicationName = String.Format("XeoraDomainExecutableLoaderDomain_{0}", AppDomainID)
                    .ApplicationBase = ExecutingPath
                    .PrivateBinPath = String.Format("{0};{1}", ExecutingPath, System.AppDomain.CurrentDomain.RelativeSearchPath)
                    .ShadowCopyFiles = Boolean.TrueString
                End With

                ExecutableLoaderHelper._AppDomain =
                    System.AppDomain.CreateDomain(
                        String.Format("XeoraDomainExecutableLoaderDomain_{0}", AppDomainID),
                        System.AppDomain.CurrentDomain.Evidence,
                        AppDomainInfo
                    )
                AddHandler ExecutableLoaderHelper._AppDomain.AssemblyResolve, AddressOf ExecutableLoaderHelper.AssemblyResolve
            End If
        End Sub

        Private Shared Function AssemblyResolve(ByVal sender As Object, ByVal e As System.ResolveEventArgs) As System.Reflection.Assembly
            Dim rAssembly As System.Reflection.Assembly = Nothing

            For Each assembly As System.Reflection.Assembly In System.AppDomain.CurrentDomain.GetAssemblies()
                If assembly.FullName = e.Name Then _
                    rAssembly = assembly : Exit For
            Next

            Return rAssembly
        End Function

        Public Shared Sub DestroyAppDomain()
            If Not ExecutableLoaderHelper._AppDomain Is Nothing Then
                RemoveHandler System.AppDomain.CurrentDomain.AssemblyResolve, AddressOf ExecutableLoaderHelper.AssemblyResolve

                System.AppDomain.Unload(ExecutableLoaderHelper._AppDomain)

                ExecutableLoaderHelper._AppDomain = Nothing

                Dim TempLocation As String =
                    IO.Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "XeoraCubeAddInTemp")

                Try
                    If IO.Directory.Exists(TempLocation) Then IO.Directory.Delete(TempLocation, True)
                Catch ex As Exception
                    ' Just Handle Exceptions
                End Try

                ExecutableLoaderHelper._ExecutableLoader = Nothing
            End If
        End Sub

        Public Shared ReadOnly Property ExecutableLoader As ILoader
            Get
                If Not ExecutableLoaderHelper._AppDomain Is Nothing AndAlso
                    ExecutableLoaderHelper._ExecutableLoader Is Nothing Then

                    ExecutableLoaderHelper._ExecutableLoader =
                        CType(
                            ExecutableLoaderHelper._AppDomain.CreateInstanceAndUnwrap(
                                "Xeora.Extension.VisualStudio.Executable",
                                "Xeora.Extension.VisualStudio.Executable.Loader"),
                            ILoader
                        )
                End If

                Return ExecutableLoaderHelper._ExecutableLoader
            End Get
        End Property
    End Class
End Namespace