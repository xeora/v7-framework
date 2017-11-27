Imports Microsoft.VisualStudio.Language
Imports Xeora.Extension.Executable
Imports My.Resources

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public Class Executable
        Inherits BuilderBase

        Public Sub New(ByVal Directive As [Enum])
            MyBase.New(Directive)
        End Sub

        Public Overrides Function Build() As Intellisense.Completion()
            Dim CompList As New Generic.List(Of Intellisense.Completion)()
            Dim ExecutablesPath As String =
                IO.Path.GetFullPath(IO.Path.Combine(MyBase.LocateTemplatesPath(PackageControl.IDEControl.DTE.ActiveDocument.Path), "..", "Executables"))

            ' Fill Current Domain Executables
            Me.Fill(CompList, ExecutablesPath)

            ' If it is a child then search for parents
            If PackageControl.IDEControl.GetActiveItemDomainType() = Globals.ActiveDomainTypes.Child Then
                Dim DomainID As String = String.Empty
                Dim SearchDI As New IO.DirectoryInfo(ExecutablesPath)

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

                        Me.Fill(CompList, IO.Path.Combine(SearchDI.FullName, DomainID, "Executables"))

                        If String.Compare(SearchDI.Name, "Domains", True) = 0 Then SearchDI = Nothing
                    End If
                Loop Until SearchDI Is Nothing
            End If

            ' fill all children executables
            Me.FillChildren(CompList, ExecutablesPath)

            CompList.Sort(New CompletionComparer())

            Return CompList.ToArray()
        End Function

        Public Overrides Function Builders() As Intellisense.Completion()
            Return New Intellisense.Completion() {
                    New Intellisense.Completion("Create New Executable", "__CREATE.EXECUTABLE__", String.Empty, Nothing, Nothing)
                }
        End Function

        Private Sub FillChildren(ByRef Container As Generic.List(Of Intellisense.Completion), ByVal ExecutablesPath As String)
            Dim AddonsPath As String =
                IO.Path.GetFullPath(IO.Path.Combine(ExecutablesPath, "..", "Addons"))

            If IO.Directory.Exists(AddonsPath) Then
                For Each AddonPath As String In IO.Directory.GetDirectories(AddonsPath)
                    Dim AddonExecutable As String =
                        IO.Path.Combine(AddonPath, "Executables")

                    Me.Fill(Container, AddonExecutable)
                    Me.FillChildren(Container, AddonExecutable)
                Next
            End If
        End Sub

        Private Sub Fill(ByRef Container As Generic.List(Of Intellisense.Completion), ByVal ExecutablesPath As String)
            Try
                If Cache.Instance.IsLatest(ExecutablesPath, QueryTypes.None) Then
                    For Each AssemblyID As String In Cache.Instance.GetIDs(ExecutablesPath)
                        Container.Add(
                            New Intellisense.Completion(AssemblyID, String.Format("{0}?", AssemblyID), String.Empty, Me.ProvideImageSource(IconResource._assembly), Nothing)
                        )
                    Next
                Else
                    For Each AssemblyID As String In ExecutableLoaderHelper.ExecutableLoader.GetAssemblies(ExecutablesPath)
                        Container.Add(
                            New Intellisense.Completion(AssemblyID, String.Format("{0}?", AssemblyID), String.Empty, Me.ProvideImageSource(IconResource._assembly), Nothing)
                        )

                        ' Cache the dll for quick access
                        Cache.Instance.AddInfo(
                            IO.Path.GetFullPath(ExecutablesPath),
                            AssemblyID
                        )
                    Next
                End If
            Catch ex As Exception
                ' Just Handle Exceptions
            End Try
        End Sub
    End Class
End Namespace