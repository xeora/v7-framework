Imports Microsoft.VisualStudio.Language

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public Class Template
        Inherits BuilderBase

        Private _Component As System.ComponentModel.Container
        Private _ImageSet As ImageList

        Public Sub New(ByVal Directive As [Enum])
            MyBase.New(Directive)

            Me._Component = New System.ComponentModel.Container()

            Dim resources As System.ComponentModel.ComponentResourceManager =
                New System.ComponentModel.ComponentResourceManager(Me.GetType())

            Me._ImageSet = New ImageList(Me._Component)
            Me._ImageSet.ImageStream = CType(resources.GetObject("Images"), ImageListStreamer)
            Me._ImageSet.TransparentColor = System.Drawing.Color.Transparent

            ' This comment is for remembering which id belongs to which key.
            'Me._ImageSet.Images.SetKeyName(0, "0standart.png")
            'Me._ImageSet.Images.SetKeyName(1, "1standartwithauth.png")
            'Me._ImageSet.Images.SetKeyName(2, "2standartoverriable.png")
            'Me._ImageSet.Images.SetKeyName(3, "3standartoverriablewithauth.png")
            'Me._ImageSet.Images.SetKeyName(4, "4standartunregistered.png")
            'Me._ImageSet.Images.SetKeyName(5, "5standalone.png")
            'Me._ImageSet.Images.SetKeyName(6, "6standalonewithauth.png")
            'Me._ImageSet.Images.SetKeyName(7, "7standaloneoverriable.png")
            'Me._ImageSet.Images.SetKeyName(8, "8standaloneoverriablewithauth.png")
        End Sub

        Public Property WorkingTemplateID As String

        Public Overrides Function Build() As Intellisense.Completion()
            Dim CompList As New Generic.List(Of Intellisense.Completion)()
            Dim TemplatesPath As String =
                MyBase.LocateTemplatesPath(PackageControl.IDEControl.DTE.ActiveDocument.Path)

            Me.Fill(CompList, TemplatesPath, TemplatesPath)

            If PackageControl.IDEControl.GetActiveItemDomainType() = Globals.ActiveDomainTypes.Child Then
                Dim DomainID As String = String.Empty
                Dim SearchDI As New IO.DirectoryInfo(TemplatesPath)

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

                        Me.Fill(CompList, IO.Path.Combine(SearchDI.FullName, DomainID, "Templates"), IO.Path.Combine(SearchDI.FullName, DomainID, "Templates"))

                        If String.Compare(SearchDI.Name, "Domains", True) = 0 Then SearchDI = Nothing
                    End If
                Loop Until SearchDI Is Nothing
            End If

            CompList.Sort(New CompletionComparer())

            Return CompList.ToArray()
        End Function

        Public Overrides Function Builders() As Intellisense.Completion()
            ' TODO: Template Creation Form
            Return New Intellisense.Completion() {
                    New Intellisense.Completion("Create New Template", "__CREATE.TEMPLATE__", String.Empty, Nothing, Nothing)
                }
        End Function

        Private Sub Fill(ByRef Container As Generic.List(Of Intellisense.Completion), ByVal TemplatesPath As String, ByVal TemplatesRootPath As String)
            Dim cFStream As IO.FileStream = Nothing

            Try
                Dim TemplateFileNames As String() =
                    IO.Directory.GetFiles(TemplatesPath, "*.xchtml")

                cFStream = New IO.FileStream(
                                IO.Path.Combine(TemplatesRootPath, "Configuration.xml"),
                                IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite)
                Dim xPathDocument As New Xml.XPath.XPathDocument(cFStream)
                Dim xPathNavigator As Xml.XPath.XPathNavigator =
                    xPathDocument.CreateNavigator()
                Dim xPathIter As Xml.XPath.XPathNodeIterator

                Dim TemplateID As String, ImageIndex As Integer
                For Each TemplateFileName As String In TemplateFileNames
                    ImageIndex = 4
                    TemplateID = TemplateFileName.Replace(TemplatesRootPath, String.Empty)
                    TemplateID = TemplateID.Replace(".xchtml", String.Empty)
                    If TemplateID.IndexOf("\"c) = 0 Then TemplateID = TemplateID.Substring(1)
                    TemplateID = TemplateID.Replace("\", "/")

                    If String.Compare(Me.WorkingTemplateID, TemplateID) = 0 Then Continue For

                    xPathIter = xPathNavigator.Select(String.Format("/Settings/Services/Item[@type='template' and @id='{0}']", TemplateID))

                    If xPathIter.MoveNext() Then
                        Dim authentication As String, [overridable] As String, standalone As String
                        Dim authentication_b As Boolean, overridable_b As Boolean, standalone_b As Boolean

                        authentication = xPathIter.Current.GetAttribute("authentication", xPathIter.Current.NamespaceURI)
                        [overridable] = xPathIter.Current.GetAttribute("overridable", xPathIter.Current.NamespaceURI)
                        standalone = xPathIter.Current.GetAttribute("standalone", xPathIter.Current.NamespaceURI)

                        If String.IsNullOrEmpty(authentication) Then authentication = "false"
                        If String.IsNullOrEmpty([overridable]) Then [overridable] = "false"
                        If String.IsNullOrEmpty(standalone) Then standalone = "false"

                        Boolean.TryParse(authentication, authentication_b)
                        Boolean.TryParse([overridable], overridable_b)
                        Boolean.TryParse(standalone, standalone_b)

                        If standalone_b Then
                            If authentication_b Then
                                If overridable_b Then
                                    ImageIndex = 8
                                Else
                                    ImageIndex = 6
                                End If
                            Else
                                If overridable_b Then
                                    ImageIndex = 7
                                Else
                                    ImageIndex = 5
                                End If
                            End If
                        Else
                            If authentication_b Then
                                If overridable_b Then
                                    ImageIndex = 3
                                Else
                                    ImageIndex = 1
                                End If
                            Else
                                If overridable_b Then
                                    ImageIndex = 2
                                Else
                                    ImageIndex = 0
                                End If
                            End If
                        End If
                    End If

                    If Not Me.IsExists(Container, TemplateID) Then
                        Container.Add(
                            New Intellisense.Completion(
                                TemplateID, String.Format("{0}$", TemplateID), String.Empty,
                                Me.ProvideImageSource(Me._ImageSet.Images.Item(ImageIndex)),
                                Nothing
                            )
                        )
                    End If
                Next

                For Each SubDirectory As String In IO.Directory.GetDirectories(TemplatesPath)
                    Me.Fill(Container, SubDirectory, TemplatesRootPath)
                Next
            Catch ex As Exception
                ' Just Handle Exceptions
            Finally
                If Not cFStream Is Nothing Then cFStream.Close()
            End Try
        End Sub
    End Class
End Namespace