Imports Microsoft.VisualStudio.Language

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public MustInherit Class BuilderBase
        Public MustOverride Function Build() As Intellisense.Completion()
        Public MustOverride Function Builders() As Intellisense.Completion()

        Public ReadOnly Property RequestingDirective As [Enum]

        Protected Sub New(ByVal Directive As [Enum])
            RequestingDirective = Directive
        End Sub

        Protected Function LocateTemplatesPath(ByVal ActivePath As String) As String
            Dim CheckDI As New IO.DirectoryInfo(ActivePath)

            Do Until CheckDI Is Nothing OrElse String.Compare(CheckDI.Name, "Templates") = 0
                CheckDI = CheckDI.Parent
            Loop

            If CheckDI Is Nothing Then Return ActivePath

            Return CheckDI.FullName
        End Function

        Protected Function IsExists(ByRef Container As Generic.List(Of Intellisense.Completion), ByVal SearchKey As String) As Boolean
            Return Container.FindIndex(New Predicate(Of Intellisense.Completion)(Function(item As Intellisense.Completion)
                                                                                     String.Compare(item.DisplayText, SearchKey)
                                                                                 End Function)) > -1
        End Function

        Protected Function ProvideImageSource(ByVal Image As Drawing.Image) As System.Windows.Media.ImageSource
            Dim rImageSource As System.Windows.Media.Imaging.BitmapImage = Nothing

            Dim ImageBitmap As Drawing.Bitmap = Nothing
            Dim MS As IO.MemoryStream = Nothing

            Try
                ImageBitmap = New Drawing.Bitmap(Image)

                MS = New IO.MemoryStream()
                ImageBitmap.Save(MS, System.Drawing.Imaging.ImageFormat.Png)
                MS.Seek(0, IO.SeekOrigin.Begin)

                rImageSource = New System.Windows.Media.Imaging.BitmapImage()
                rImageSource.BeginInit()
                rImageSource.StreamSource = New IO.MemoryStream(MS.ToArray())
                rImageSource.EndInit()
            Catch ex As Exception
                ' Just Handle Exceptions
            Finally
                If Not ImageBitmap Is Nothing Then ImageBitmap.Dispose()
                If Not MS Is Nothing Then MS.Close() : GC.SuppressFinalize(MS)
            End Try

            Return rImageSource
        End Function

        Protected Function LocateAssembly(ByVal AssemblyID As String, Optional ByVal SearchingDomainRootPath As String = Nothing) As String
            Dim rString As String = String.Empty

            If String.IsNullOrEmpty(SearchingDomainRootPath) Then
                ' If SearchLocation is empty, start from the top domain and search all executables and also child addons
                ' Let's first start assigning the active document path which locating under the Templates folder
                SearchingDomainRootPath = Me.LocateTemplatesPath(PackageControl.IDEControl.DTE.ActiveDocument.Path)

                Dim DomainID As String = String.Empty
                Dim SearchLocationDI As New IO.DirectoryInfo(SearchingDomainRootPath)

                Do
                    SearchLocationDI = SearchLocationDI.Parent
                    If Not SearchLocationDI Is Nothing Then
                        DomainID = SearchLocationDI.Name
                        SearchLocationDI = SearchLocationDI.Parent
                    End If
                Loop Until SearchLocationDI Is Nothing OrElse String.Compare(SearchLocationDI.Name, "Domains", True) = 0

                If Not SearchLocationDI Is Nothing Then
                    SearchingDomainRootPath = IO.Path.Combine(SearchLocationDI.FullName, DomainID)
                Else
                    Return String.Empty
                End If
            End If

            Dim ExecutablesPath As String = IO.Path.Combine(SearchingDomainRootPath, "Executables")
            If IO.Directory.Exists(ExecutablesPath) Then
                If IO.File.Exists(IO.Path.Combine(ExecutablesPath, String.Format("{0}.dll", AssemblyID))) Then
                    rString = ExecutablesPath
                Else
                    For Each SubDomainPath As String In IO.Directory.GetDirectories(IO.Path.Combine(SearchingDomainRootPath, "Addons"))
                        rString = Me.LocateAssembly(AssemblyID, SubDomainPath)

                        If Not String.IsNullOrEmpty(rString) Then Exit For
                    Next
                End If
            End If

            Return rString
        End Function

        Protected Class CompletionComparer
            Inherits Generic.Comparer(Of Intellisense.Completion)

            Public Overrides Function Compare(x As Intellisense.Completion, y As Intellisense.Completion) As Integer
                Return String.Compare(x.DisplayText, y.DisplayText)
            End Function
        End Class
    End Class
End Namespace