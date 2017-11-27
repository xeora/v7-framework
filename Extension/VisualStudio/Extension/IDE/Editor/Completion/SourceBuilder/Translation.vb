Imports Microsoft.VisualStudio.Language
Imports My.Resources

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public Class Translation
        Inherits BuilderBase

        Public Sub New(ByVal Directive As [Enum])
            MyBase.New(Directive)
        End Sub

        Public Overrides Function Build() As Intellisense.Completion()
            Dim CompList As New Generic.List(Of Intellisense.Completion)()
            Dim LanguagesPath As String =
                IO.Path.Combine(MyBase.LocateTemplatesPath(PackageControl.IDEControl.DTE.ActiveDocument.Path), "..", "Languages")

            Me.Fill(CompList, LanguagesPath, False)

            If PackageControl.IDEControl.GetActiveItemDomainType() = Globals.ActiveDomainTypes.Child Then
                Dim DomainID As String = String.Empty
                Dim SearchDI As New IO.DirectoryInfo(LanguagesPath)

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

                        Me.Fill(CompList, IO.Path.Combine(SearchDI.FullName, DomainID, "Languages"), True)

                        If String.Compare(SearchDI.Name, "Domains", True) = 0 Then SearchDI = Nothing
                    End If
                Loop Until SearchDI Is Nothing
            End If

            CompList.Sort(New CompletionComparer())

            Return CompList.ToArray()
        End Function

        Public Overrides Function Builders() As Intellisense.Completion()
            Return New Intellisense.Completion() {
                    New Intellisense.Completion("Create New Translation", "__CREATE.TRANSLATE__", String.Empty, Nothing, Nothing)
                }
        End Function

        Private Sub Fill(ByRef Container As Generic.List(Of Intellisense.Completion), ByVal LanguagesPath As String, ByVal IsParentSearch As Boolean)
            Dim cFStream As IO.FileStream = Nothing

            Try
                Dim TranslationFileNames As String() =
                    IO.Directory.GetFiles(LanguagesPath, "*.xml")

                Dim TranslationCompile As New Generic.Dictionary(Of String, Integer)
                For Each TranslationFileName As String In TranslationFileNames
                    Try
                        cFStream = New IO.FileStream(
                                        TranslationFileName, IO.FileMode.Open,
                                        IO.FileAccess.Read, IO.FileShare.ReadWrite)
                        Dim xPathDocument As New Xml.XPath.XPathDocument(cFStream)
                        Dim xPathNavigator As Xml.XPath.XPathNavigator =
                            xPathDocument.CreateNavigator()
                        Dim xPathIter As Xml.XPath.XPathNodeIterator

                        xPathIter = xPathNavigator.Select("/language/translation")

                        Do While xPathIter.MoveNext()
                            Dim TransID As String =
                                xPathIter.Current.GetAttribute("id", xPathIter.Current.NamespaceURI)

                            If TranslationCompile.ContainsKey(TransID) Then _
                                TranslationCompile.Item(TransID) += 1 Else TranslationCompile.Add(TransID, 1)
                        Loop
                    Catch ex As Exception
                        ' Just Handle Exceptions
                    Finally
                        If Not cFStream Is Nothing Then cFStream.Close()
                    End Try
                Next

                For Each TransIDKey As String In TranslationCompile.Keys
                    If TranslationCompile.Item(TransIDKey) = TranslationFileNames.Length AndAlso
                        Not Me.IsExists(Container, TransIDKey) Then

                        If Not IsParentSearch Then
                            Container.Add(
                                New Intellisense.Completion(
                                    TransIDKey, String.Format("{0}$", TransIDKey), String.Empty,
                                    Me.ProvideImageSource(IconResource.domain_level_translation), Nothing
                                )
                            )
                        Else
                            Container.Add(
                                New Intellisense.Completion(
                                    TransIDKey, String.Format("{0}$", TransIDKey), String.Empty,
                                    Me.ProvideImageSource(IconResource.parent_level_translation), Nothing
                                )
                            )
                        End If
                    End If
                Next
            Catch ex As Exception
                ' Just Handle Exceptions
            End Try
        End Sub
    End Class
End Namespace