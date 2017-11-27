Option Strict On

Imports Microsoft.VisualStudio.CommandBars
Imports EnvDTE

Namespace Xeora.Extension.VisualStudio
    Public Class IDEControl
        Implements IWin32Window

        Private _DTE As DTE
        Private _Commands As Commands

        Private _eventContainer As Generic.List(Of CommandBarEvents)

        Private _solutionEvents As SolutionEvents
        Private _documentEvents As Generic.Dictionary(Of String, DocumentEvents)
        Private _windowEvents As Generic.Dictionary(Of String, WindowEvents)

        Public Sub New(ByVal DTE As DTE)
            Me._DTE = DTE
            Me._Commands = New Commands(Me)

            Me._eventContainer = New Generic.List(Of CommandBarEvents)

            Me._solutionEvents = CType(Me._DTE.Events.SolutionEvents(), SolutionEvents)
            AddHandler Me._solutionEvents.Opened, AddressOf Me.SolutionOpened
            AddHandler Me._solutionEvents.AfterClosing, AddressOf Me.SolutionClosed

            Me._documentEvents = New Generic.Dictionary(Of String, DocumentEvents)
            Me._windowEvents = New Generic.Dictionary(Of String, WindowEvents)

            ' Prepare For Editor Context Menu Command
            For Each oCommandBar As CommandBar In CType(Me._DTE.CommandBars, CommandBars)
                If String.Compare(oCommandBar.Name, "HTML Context") = 0 OrElse
                   String.Compare(oCommandBar.Name, "Code Window") = 0 Then

                    Dim oPopup As CommandBarPopup =
                        CType(
                            oCommandBar.Controls.Add(
                                MsoControlType.msoControlPopup,
                                System.Reflection.Missing.Value,
                                System.Reflection.Missing.Value, 1, True),
                            CommandBarPopup
                        )
                    oPopup.Caption = "Xeora³"

                    ' Goto Control Referance
                    Dim oControl As CommandBarButton =
                        CType(
                            oPopup.Controls.Add(
                                MsoControlType.msoControlButton,
                                System.Reflection.Missing.Value,
                                System.Reflection.Missing.Value, 1, True),
                            CommandBarButton
                        )
                    oControl.Caption = "Go To Definition"
                    oControl.ShortcutText = "Ctrl+Shift+D, Ctrl+Shift+D"

                    Dim cbEvent As CommandBarEvents =
                        CType(Me._DTE.Events.CommandBarEvents(oControl), CommandBarEvents)

                    AddHandler cbEvent.Click, AddressOf Me._Commands.GotoControlReferance

                    Me._eventContainer.Add(cbEvent)
                End If
            Next

            ExecutableLoaderHelper.CreateAppDomain()
        End Sub

        Public ReadOnly Property DTE As DTE
            Get
                Return Me._DTE
            End Get
        End Property

        Public ReadOnly Property UserCommands As Commands
            Get
                Return Me._Commands
            End Get
        End Property

        Private Sub SolutionOpened()
            Dim mainWindowEvent As WindowEvents =
                CType(Me._DTE.Events.WindowEvents(Me._DTE.MainWindow), WindowEvents)

            AddHandler mainWindowEvent.WindowActivated, AddressOf Me.CheckContextMenu

            Me._windowEvents.Item(String.Format("{0}-{1}", Me._DTE.MainWindow.Caption, Me._DTE.MainWindow.Kind)) = mainWindowEvent

            Me.AssignHandlersToWindows(Me._DTE.Windows, Nothing)

            Dim documentEvent As DocumentEvents =
                CType(Me._DTE.Events.DocumentEvents(), DocumentEvents)

            AddHandler documentEvent.DocumentOpened, AddressOf Me.DocumentOpened

            Me._documentEvents.Item("BASE-FIRST") = documentEvent

            Me.AssignHandlersToDocuments(Me._DTE.Documents)
        End Sub

        Private Sub SolutionClosed()
            Me._documentEvents = New Generic.Dictionary(Of String, DocumentEvents)
            Me._windowEvents = New Generic.Dictionary(Of String, WindowEvents)
        End Sub

        Private Sub AssignHandlersToDocuments(ByRef documents As Documents)
            If Not documents Is Nothing Then
                For Each document As Document In documents
                    Dim documentEvent As DocumentEvents =
                        CType(Me._DTE.Events.DocumentEvents(document), DocumentEvents)

                    AddHandler documentEvent.DocumentClosing, AddressOf Me.DocumentClosing

                    Me._documentEvents.Item(String.Format("{0}-{1}", document.Name, document.Kind)) = documentEvent
                Next
            End If
        End Sub

        Private Sub AssignHandlersToWindows(ByRef windows As EnvDTE.Windows, ByRef linkedWindows As LinkedWindows)
            If Not windows Is Nothing Then
                For Each window As Window In windows
                    Dim windowEvent As WindowEvents =
                        CType(Me._DTE.Events.WindowEvents(window), WindowEvents)

                    AddHandler windowEvent.WindowActivated, AddressOf Me.CheckContextMenu

                    Me._windowEvents.Item(String.Format("{0}-{1}", window.Caption, window.Kind)) = windowEvent

                    If Not window.LinkedWindowFrame Is Nothing Then
                        windowEvent = CType(Me._DTE.Events.WindowEvents(window.LinkedWindowFrame), WindowEvents)

                        AddHandler windowEvent.WindowActivated, AddressOf Me.CheckContextMenu

                        Me._windowEvents.Item(String.Format("{0}-{1}", window.LinkedWindowFrame.Caption, window.LinkedWindowFrame.Kind)) = windowEvent
                    End If

                    If Not window.LinkedWindows Is Nothing AndAlso window.LinkedWindows.Count > 0 Then _
                        Me.AssignHandlersToWindows(Nothing, window.LinkedWindows)
                Next
            End If

            If Not linkedWindows Is Nothing Then
                For Each window As Window In linkedWindows
                    Dim windowEvent As WindowEvents =
                        CType(Me._DTE.Events.WindowEvents(window), WindowEvents)

                    AddHandler windowEvent.WindowActivated, AddressOf Me.CheckContextMenu

                    Me._windowEvents.Item(String.Format("{0}-{1}", window.Caption, window.Kind)) = windowEvent

                    If Not window.LinkedWindowFrame Is Nothing Then
                        windowEvent = CType(Me._DTE.Events.WindowEvents(window.LinkedWindowFrame), WindowEvents)

                        AddHandler windowEvent.WindowActivated, AddressOf Me.CheckContextMenu

                        Me._windowEvents.Item(String.Format("{0}-{1}", window.LinkedWindowFrame.Caption, window.LinkedWindowFrame.Kind)) = windowEvent
                    End If

                    If Not window.LinkedWindows Is Nothing AndAlso window.LinkedWindows.Count > 0 Then _
                        Me.AssignHandlersToWindows(Nothing, window.LinkedWindows)
                Next
            End If
        End Sub

        Private Sub DocumentOpened(ByVal document As Document)
            Dim documentEvent As DocumentEvents =
                CType(Me._DTE.Events.DocumentEvents(document), DocumentEvents)

            AddHandler documentEvent.DocumentClosing, AddressOf Me.DocumentClosing

            Me._documentEvents.Item(String.Format("{0}-{1}", document.Name, document.Kind)) = documentEvent
        End Sub

        Private Sub DocumentClosing(ByVal document As Document)
            Me._windowEvents.Remove(String.Format("{0}-{1}", document.Name, document.Kind))
        End Sub

        Private Sub CheckContextMenu(ByVal GotFocus As Window, ByVal LostFocus As Window)
            If Not Me._DTE.ActiveDocument Is Nothing Then
                Dim ActiveDocFI As IO.FileInfo =
                    New IO.FileInfo(Me._DTE.ActiveDocument.FullName)
                Dim IsControlsXMLFile As Boolean =
                    String.Compare(ActiveDocFI.Name, "Controls.xml", True) = 0

                For Each oCommandBar As CommandBar In CType(Me._DTE.CommandBars, CommandBars)
                    If String.Compare(oCommandBar.Name, "Code Window") = 0 Then
                        For Each oCommandBarControl As CommandBarControl In oCommandBar.Controls
                            If String.Compare(oCommandBarControl.Caption, "Xeora³") = 0 Then
                                oCommandBarControl.Visible = IsControlsXMLFile

                                Exit For
                            End If
                        Next

                        Exit For
                    End If
                Next
            End If
        End Sub

        Public Class Commands
            Private _Parent As IDEControl

            Public Sub New(ByVal Parent As IDEControl)
                Me._Parent = Parent
            End Sub

            ' TODO: Should be Removed and Replace with the new Version of the same function.
            Public Function GetDirectiveType(ByRef editPoint As EditPoint, ByRef DirectiveType As Char, ByRef Offset As Integer, ByVal IsXMLFile As Boolean) As String
                Dim SearchID As String = String.Empty
                Dim LastString As String = String.Empty

                Dim OriginalOffset As Integer =
                    editPoint.LineCharOffset

                Do Until editPoint.LineCharOffset = 1
                    editPoint.CharLeft()

                    LastString = editPoint.GetText(1)
                    SearchID = String.Concat(LastString, SearchID)

                    If String.IsNullOrWhiteSpace(LastString) OrElse
                        (SearchID.Length = 2 AndAlso String.Compare(SearchID, "$$") = 0) Then
                        editPoint.CharRight(SearchID.Length)

                        SearchID = String.Empty

                        Exit Do
                    End If

                    Dim SearchMatch As System.Text.RegularExpressions.Match

                    If IsXMLFile Then
                        SearchMatch = System.Text.RegularExpressions.Regex.Match(SearchID, "\<Bind\>")
                    Else
                        SearchMatch = System.Text.RegularExpressions.Regex.Match(SearchID, "\$\w(\#\d+(\+)?)?(\[[\.\w\-]+\])?\:|\$C(\<\d+(\+)?\>)?\[|\$C")
                    End If

                    If SearchMatch.Success Then
                        If IsXMLFile Then
                            DirectiveType = Char.MinValue
                        Else
                            DirectiveType = SearchMatch.Value.Chars(1)
                        End If
                        Offset = editPoint.LineCharOffset + SearchMatch.Length

                        editPoint.CharRight(SearchMatch.Length)

                        Exit Do
                    Else
                        If editPoint.LineCharOffset = 0 Then SearchID = String.Empty
                    End If
                Loop

                Return SearchID
            End Function

            Private Function GetDefinitionLineNumber(ByVal LanguageID As String, ByVal [Namespace] As String, ByVal JoinedTypeName As String, ByVal MethodName As String, ByVal ParameterLength As Integer, ByVal DocumentContent As String) As Integer
                Dim rInteger As Integer = -1

                Dim parser As ICSharpCode.NRefactory.IParser = Nothing

                Dim codeContentReader As New IO.StringReader(DocumentContent)
                Select Case LanguageID
                    Case "vb"
                        parser = ICSharpCode.NRefactory.ParserFactory.CreateParser(
                                    ICSharpCode.NRefactory.SupportedLanguage.VBNet,
                                    codeContentReader)

                    Case "cs"
                        parser = ICSharpCode.NRefactory.ParserFactory.CreateParser(
                                    ICSharpCode.NRefactory.SupportedLanguage.CSharp,
                                    codeContentReader)
                End Select

                If Not parser Is Nothing Then
                    parser.ParseMethodBodies = False
                    parser.Parse()
                End If

                codeContentReader.Close()

                If Not parser Is Nothing AndAlso Not parser.CompilationUnit Is Nothing Then
                    Dim NodeList As Generic.List(Of ICSharpCode.NRefactory.Ast.INode) =
                        parser.CompilationUnit.Children

                    Dim NSFound As Boolean = False, TypeFound As Boolean = False, FuncFound As Boolean = False, TypeNames As String() = JoinedTypeName.Split("."c), WorkingTypeID As Integer = 0
                    Do While Not NodeList Is Nothing AndAlso NodeList.Count > 0
                        If NSFound AndAlso TypeFound AndAlso Not FuncFound AndAlso
                            TypeOf NodeList.Item(0) Is ICSharpCode.NRefactory.Ast.MethodDeclaration Then

                            ' Try to match with HttpMethod BoundMethod
                            Dim AttributeObject As ICSharpCode.NRefactory.Ast.AttributedNode =
                                CType(NodeList.Item(0), ICSharpCode.NRefactory.Ast.AttributedNode)

                            If Not AttributeObject Is Nothing AndAlso AttributeObject.Attributes.Count > 0 AndAlso
                                String.Compare(AttributeObject.Attributes.Item(0).Attributes.Item(0).Name, "HttpMethod") = 0 Then

                                For Each Expression As ICSharpCode.NRefactory.Ast.Expression In AttributeObject.Attributes.Item(0).Attributes.Item(0).PositionalArguments
                                    If TypeOf Expression Is ICSharpCode.NRefactory.Ast.PrimitiveExpression Then
                                        Dim PrimitiveExp As ICSharpCode.NRefactory.Ast.PrimitiveExpression =
                                            CType(Expression, ICSharpCode.NRefactory.Ast.PrimitiveExpression)

                                        If String.Compare(CType(PrimitiveExp.Value, String), MethodName) = 0 AndAlso
                                            CType(NodeList.Item(0), ICSharpCode.NRefactory.Ast.MethodDeclaration).Parameters.Count = ParameterLength Then

                                            FuncFound = True : rInteger = NodeList.Item(0).StartLocation.Line

                                            Exit Do
                                        End If
                                    End If
                                Next
                            End If
                            ' ---

                            If String.Compare(CType(NodeList.Item(0), ICSharpCode.NRefactory.Ast.MethodDeclaration).Name, MethodName) = 0 AndAlso
                                CType(NodeList.Item(0), ICSharpCode.NRefactory.Ast.MethodDeclaration).Parameters.Count = ParameterLength Then

                                FuncFound = True : rInteger = NodeList.Item(0).StartLocation.Line

                                Exit Do
                            End If
                        End If

                        If NSFound AndAlso Not TypeFound Then
                            If TypeOf NodeList.Item(0) Is ICSharpCode.NRefactory.Ast.TypeDeclaration AndAlso
                                String.Compare(CType(NodeList.Item(0), ICSharpCode.NRefactory.Ast.TypeDeclaration).Name, TypeNames(WorkingTypeID)) = 0 Then

                                WorkingTypeID += 1

                                If WorkingTypeID = TypeNames.Length Then TypeFound = True

                                NodeList = NodeList.Item(0).Children : Continue Do
                            End If
                        End If

                        If TypeOf NodeList.Item(0) Is ICSharpCode.NRefactory.Ast.NamespaceDeclaration AndAlso
                            String.Compare(CType(NodeList.Item(0), ICSharpCode.NRefactory.Ast.NamespaceDeclaration).Name, [Namespace]) = 0 Then

                            NSFound = True

                            NodeList = NodeList.Item(0).Children : Continue Do
                        End If

                        NodeList.RemoveAt(0)
                    Loop
                End If

                Return rInteger
            End Function

            Public Sub GotoControlReferance(ByVal CommandaBarControl As Object, ByRef handled As Boolean, ByRef cancelDefault As Boolean)
                If Me._Parent.DTE.ActiveDocument Is Nothing Then Exit Sub

                Dim selection As TextSelection =
                    CType(Me._Parent.DTE.ActiveDocument.Selection, TextSelection)

                Dim cursorPoint As VirtualPoint =
                    selection.ActivePoint
                Dim editPoint As EditPoint =
                    cursorPoint.CreateEditPoint()

                Dim cursorLastPossion As Integer =
                    cursorPoint.LineCharOffset

                Dim ControlTypeReferance As Char, BeginOffset As Integer = -1, EndOffset As Integer = -1

                Dim ActiveDocFI As IO.FileInfo =
                    New IO.FileInfo(Me._Parent.DTE.ActiveDocument.FullName)
                Dim IsXMLFile As Boolean =
                    String.Compare(ActiveDocFI.Name, "Controls.xml", True) = 0

                Me.GetDirectiveType(editPoint, ControlTypeReferance, BeginOffset, IsXMLFile)

                If BeginOffset = -1 OrElse (ControlTypeReferance <> "C"c AndAlso ControlTypeReferance <> "F"c AndAlso (IsXMLFile AndAlso ControlTypeReferance <> Char.MinValue)) Then Exit Sub

                Dim SearchID As String = String.Empty
                Do Until editPoint.LineCharOffset - 1 = editPoint.LineLength
                    SearchID = String.Concat(SearchID, editPoint.GetText(1))

                    If SearchID.Chars(SearchID.Length - 1) = ":"c OrElse SearchID.Chars(SearchID.Length - 1) = "$"c OrElse (IsXMLFile AndAlso SearchID.Chars(SearchID.Length - 1) = "<"c) Then _
                        EndOffset = editPoint.LineCharOffset : Exit Do

                    editPoint.CharRight()
                Loop

                If EndOffset = -1 Then Exit Sub

                editPoint.MoveToLineAndOffset(cursorPoint.Line, BeginOffset)
                SearchID = editPoint.GetText(EndOffset - BeginOffset)

                ' Fix the cursor position to the begining one
                editPoint.MoveToLineAndOffset(cursorPoint.Line, cursorLastPossion)

                Select Case ControlTypeReferance
                    Case "C"c
                        Dim ActiveDocDI As IO.DirectoryInfo =
                            New IO.DirectoryInfo(Me._Parent.DTE.ActiveDocument.Path)

                        Do Until ActiveDocDI Is Nothing OrElse String.Compare(ActiveDocDI.Name, "Templates") = 0
                            ActiveDocDI = ActiveDocDI.Parent
                        Loop

                        For Each proj As Project In Me._Parent.DTE.Solution.Projects
                            Dim CacheList As New Generic.List(Of String), IsAddon As Boolean = False
                            Dim MainProjItem As ProjectItem
                            Do
                                Try
                                    MainProjItem = proj.ProjectItems.Item(ActiveDocDI.Name)
                                Catch ex As Exception
                                    MainProjItem = Nothing
                                End Try

                                If MainProjItem Is Nothing Then
                                    CacheList.Insert(0, ActiveDocDI.Name)
                                    If Not IsAddon AndAlso String.Compare(ActiveDocDI.Name, "Addons", True) = 0 Then IsAddon = True
                                    ActiveDocDI = ActiveDocDI.Parent
                                Else : Exit Do
                                End If
                            Loop Until ActiveDocDI Is Nothing

                            If Not MainProjItem Is Nothing Then
                                Dim ProjItem As ProjectItem = MainProjItem

                                For Each item As String In CacheList
                                    ProjItem = ProjItem.ProjectItems.Item(item)
                                Next
                                ProjItem = ProjItem.ProjectItems.Item("Controls.xml")

                                Dim IsResearched As Boolean = False
RESEARCHPOINT:
                                If Not ProjItem Is Nothing Then
                                    Dim PrevState As Boolean = ProjItem.IsOpen
                                    Dim itemWindow As Window =
                                        ProjItem.Open(Constants.vsViewKindTextView)
                                    itemWindow.Activate()

                                    CType(itemWindow.Document.Selection, TextSelection).StartOfDocument()
                                    If Not CType(itemWindow.Document.Selection, TextSelection).FindText(String.Format("Control id=['""]{0}['""]", SearchID), vsFindOptions.vsFindOptionsRegularExpression) Then
                                        If Not PrevState Then itemWindow.Close()

                                        If IsAddon AndAlso Not IsResearched Then
                                            IsResearched = True

                                            ProjItem = MainProjItem
                                            For Each item As String In CacheList
                                                If String.Compare(item, "Addons", True) = 0 Then
                                                    ProjItem = ProjItem.ProjectItems.Item("Templates")

                                                    Exit For
                                                Else
                                                    ProjItem = ProjItem.ProjectItems.Item(item)
                                                End If
                                            Next
                                            ProjItem = ProjItem.ProjectItems.Item("Controls.xml")

                                            GoTo RESEARCHPOINT
                                        End If
                                    End If
                                End If

                                Exit For
                            End If
                        Next
                    Case "F"c, Char.MinValue
                        If Not String.IsNullOrWhiteSpace(SearchID) Then
                            Dim AssemblyName_s As String() = SearchID.Split("?"c)
                            If AssemblyName_s.Length < 2 Then Return

                            Dim ClassNameStructure As String = AssemblyName_s(1).Split(","c)(0)

                            Dim AssemblyName As String = AssemblyName_s(0)
                            Dim ClassName As String = String.Empty
                            If ClassNameStructure.LastIndexOf("."c) > -1 Then _
                                ClassName = ClassNameStructure.Substring(0, ClassNameStructure.LastIndexOf("."c))

                            Dim FunctionName As String =
                                ClassNameStructure.Replace(String.Format("{0}.", ClassName), String.Empty)

                            Dim ParametersLength As Integer = 0
                            If AssemblyName_s(1).IndexOf(","c) > -1 Then
                                Dim Parameters As String = AssemblyName_s(1).Substring(AssemblyName_s(1).IndexOf(","c) + 1)
                                ParametersLength = Parameters.Split("|"c).Length
                            End If

                            Dim SearchList As New Generic.List(Of String)
                            SearchList.Add(String.Format("{0}.vb", AssemblyName))
                            SearchList.Add(String.Format("{0}.cs", AssemblyName))

                            For Each CN As String In ClassName.Split("."c)
                                SearchList.Add(String.Format("{0}.vb", CN))
                                SearchList.Add(String.Format("{0}.cs", CN))
                            Next

                            Dim MainProjectItem As ProjectItem
                            For Each proj As Project In Me._Parent.DTE.Solution.Projects
                                MainProjectItem = Me._Parent.SearchProjectItemRecursive(proj.ProjectItems, SearchList.ToArray())

                                If Not MainProjectItem Is Nothing Then
                                    Dim docType As String =
                                        MainProjectItem.Name.Substring(MainProjectItem.Name.LastIndexOf("."c) + 1)

                                    Dim PrevState As Boolean = MainProjectItem.IsOpen
                                    Dim itemWindow As Window =
                                        MainProjectItem.Open(Constants.vsViewKindCode)

                                    Dim TS As TextSelection =
                                        CType(itemWindow.Document.Selection, TextSelection)

                                    TS.EndOfDocument()
                                    Dim DocEndOffset As Integer =
                                        TS.ActivePoint.AbsoluteCharOffset
                                    TS.StartOfDocument()

                                    Dim EP As EditPoint =
                                        TS.ActivePoint.CreateEditPoint()

                                    Dim CodeContent As String =
                                        EP.GetText(DocEndOffset)

                                    Dim LineNumber As Integer =
                                        Me.GetDefinitionLineNumber(docType, "Xeora.Domain", ClassName, FunctionName, ParametersLength, CodeContent)

                                    If LineNumber > -1 Then
                                        TS.MoveToLineAndOffset(LineNumber, 1)

                                        itemWindow.Activate()

                                        Exit For
                                    Else
                                        If Not PrevState Then itemWindow.Close()
                                    End If
                                End If
                            Next
                        End If
                End Select
            End Sub

            Public Sub PrepareProject()
                Dim Projects As Projects =
                    CType(Me._Parent.DTE.Solution.Projects, Projects)

                If Projects.Count = 0 Then Exit Sub

                Dim ProjectList As New Tools.PrepareProject.ProjectList()

                For Each Project As Project In Projects
                    If String.Compare(Project.Kind, "{E24C65DC-7377-472b-9ABA-BC803B73C61A}") = 0 Then _
                        ProjectList.ProjectList.Add(Project.Name, Project.FullName)
                Next

                ProjectList.ShowDialog(PackageControl.IDEControl)

                If ProjectList.DialogResult = DialogResult.OK Then
                    Dim ProjectWorking As Project = Nothing
                    For Each ProjectWorking In Projects
                        If String.Compare(ProjectWorking.FullName, ProjectList.SelectedProject.Value) = 0 Then _
                            Exit For
                    Next

                    Dim ProjectSettings As New Tools.PrepareProject.ProjectSettings()
                    If ProjectSettings.ShowDialog(PackageControl.IDEControl) = DialogResult.OK Then
                        Dim ProjectAlreadyExists As Boolean = False
                        For Each ProjectItem As ProjectItem In ProjectWorking.ProjectItems
                            If String.Compare(ProjectItem.Name, "Domains", True) = 0 Then
                                For Each SubProjectItem As ProjectItem In ProjectItem.ProjectItems
                                    If String.Compare(SubProjectItem.Name, ProjectSettings.DomainID, True) = 0 Then _
                                        ProjectAlreadyExists = True : Exit For
                                Next

                                Exit For
                            End If
                        Next

                        If ProjectAlreadyExists Then
                            If MessageBox.Show(PackageControl.IDEControl,
                                String.Format("Xeora Domain ({0}) is already exists! Do you want override on it?", ProjectSettings.DomainID),
                                "Question?",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then

                                Exit Sub
                            End If
                        End If

                        If Not ProjectWorking Is Nothing Then
                            If String.IsNullOrEmpty(CType(ProjectWorking.Properties.Item("StartPage").Value, String)) Then _
                                ProjectWorking.Properties.Item("StartPage").Value = "/"
                            If String.Compare(CType(ProjectWorking.Properties.Item("StartAction").Value, String), "0") = 0 Then _
                                ProjectWorking.Properties.Item("StartAction").Value = "1"

                            Dim WorkingFolder As String =
                                ProjectList.SelectedProject.Value
                            Dim SW As IO.StreamWriter

                            Dim BinProjectItem As ProjectItem =
                                Me._Parent.CheckProjectItemExists(ProjectWorking.ProjectItems, "bin")
                            Dim ReleasePulled As Boolean = (Not BinProjectItem Is Nothing) AndAlso (BinProjectItem.ProjectItems.Count >= 7)
                            If BinProjectItem Is Nothing Then _
                                BinProjectItem = ProjectWorking.ProjectItems.AddFolder("bin")
                            Dim ProjectItem As ProjectItem =
                                Me._Parent.CheckProjectItemExists(ProjectWorking.ProjectItems, "Domains")
                            Dim NoOtherDomainExists As Boolean = ProjectItem Is Nothing
                            If ProjectItem Is Nothing Then _
                                ProjectItem = ProjectWorking.ProjectItems.AddFolder("Domains")

                            Dim DomainProjectItem As ProjectItem =
                                Me._Parent.CheckProjectItemExists(ProjectWorking.ProjectItems, String.Format("Domains\{0}", ProjectSettings.DomainID))
                            If Not DomainProjectItem Is Nothing Then
                                Try
                                    DomainProjectItem.Remove()
                                Catch ex As Exception
                                    ' It is for debug exception skip purposes!
                                End Try

                                Dim DomainLocation As String =
                                    IO.Path.Combine(WorkingFolder, "Domains", ProjectSettings.DomainID)

                                Try
                                    If IO.Directory.Exists(DomainLocation) Then _
                                        IO.Directory.Delete(DomainLocation)
                                Catch ex As Exception
                                    ' Just Handle Exceptions
                                End Try
                            End If
                            ProjectItem = ProjectItem.ProjectItems.AddFolder(ProjectSettings.DomainID)

                            ProjectItem.ProjectItems.AddFolder("Executables")

                            Dim SubProjectItem As ProjectItem =
                                ProjectItem.ProjectItems.AddFolder("Contents").ProjectItems.AddFolder(ProjectSettings.LanguageID)
                            Dim StyleFileLocation As String =
                                IO.Path.Combine(WorkingFolder, "Domains", ProjectSettings.DomainID, "Contents", ProjectSettings.LanguageID, "styles.css")
                            SW = IO.File.CreateText(StyleFileLocation)
                            SW.WriteLine("/* Default CSS Stylesheet for a New Xeora Web Application project */")
                            SW.Close()
                            SubProjectItem.ProjectItems.AddFromFile(StyleFileLocation)
                            ' favicon.ico should be located here

                            SubProjectItem = ProjectItem.ProjectItems.AddFolder("Languages")
                            Dim TranslationFileLocation As String =
                                IO.Path.Combine(WorkingFolder, "Domains", ProjectSettings.DomainID, "Languages", String.Format("{0}.xml", ProjectSettings.LanguageID))
                            SW = IO.File.CreateText(TranslationFileLocation)
                            SW.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
                            SW.WriteLine(String.Format("<language name=""{0}"" code=""{1}"">", ProjectSettings.LanguageName, ProjectSettings.LanguageID))
                            SW.WriteLine("  <translation id=""TEMPLATE_IDMUSTBESET"">TemplateID must be set</translation>")
                            SW.WriteLine("  <translation id=""CONTROLSXMLNOTFOUND"">ControlsXML file does not exists</translation>")
                            SW.WriteLine("  <translation id=""CONFIGURATIONNOTFOUND"">ConfigurationXML file does not exists</translation>")
                            SW.WriteLine("  <translation id=""TEMPLATE_NOFOUND"">{0} name Template file does not exists</translation>")
                            SW.WriteLine("  <translation id=""TEMPLATE_AUTH"">This Template requires authentication</translation>")
                            SW.WriteLine("  <translation id=""SITETITLE"">Hello, I'm Xeora!</translation>")
                            SW.WriteLine("")
                            SW.WriteLine("</language>")
                            SW.Close()
                            SubProjectItem.ProjectItems.AddFromFile(TranslationFileLocation)

                            SubProjectItem = ProjectItem.ProjectItems.AddFolder("Templates")
                            Dim ControlsXMLFileLocation As String =
                                IO.Path.Combine(WorkingFolder, "Domains", ProjectSettings.DomainID, "Templates", "Controls.xml")
                            SW = IO.File.CreateText(ControlsXMLFileLocation)
                            SW.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
                            SW.WriteLine("<Controls />")
                            SW.Close()
                            SubProjectItem.ProjectItems.AddFromFile(ControlsXMLFileLocation)

                            Dim ConfigurationFileLocation As String =
                                IO.Path.Combine(WorkingFolder, "Domains", ProjectSettings.DomainID, "Templates", "Configuration.xml")
                            SW = IO.File.CreateText(ConfigurationFileLocation)
                            SW.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
                            SW.WriteLine("<Settings>")
                            SW.WriteLine("  <Configuration>")
                            SW.WriteLine("      <Item key=""authenticationpage"" value=""main"" />")
                            SW.WriteLine("      <Item key=""defaultpage"" value=""main"" />")
                            SW.WriteLine(String.Format("      <Item key=""defaultlanguage"" value=""{0}"" />", ProjectSettings.LanguageID))
                            SW.WriteLine(String.Format("      <Item key=""defaultcaching"" value=""{0}"" />", ProjectSettings.CachingType))
                            SW.WriteLine("  </Configuration>")
                            SW.WriteLine("  <Services>")
                            SW.WriteLine("      <AuthenticationKeys />")
                            SW.WriteLine("      <Item type=""template"" id=""main"" />")
                            SW.WriteLine("  </Services>")
                            SW.WriteLine("</Settings>")
                            SW.Close()
                            SubProjectItem.ProjectItems.AddFromFile(ConfigurationFileLocation)

                            Dim DefaultTemplateFileLocation As String =
                                IO.Path.Combine(WorkingFolder, "Domains", ProjectSettings.DomainID, "Templates", "main.xchtml")
                            SW = IO.File.CreateText(DefaultTemplateFileLocation)
                            SW.WriteLine("$S:HelloXeora:{!NOCACHE")
                            SW.WriteLine("  return ""Hello, Xeora Framework is ready!"";")
                            SW.WriteLine("}:HelloXeora$")
                            SW.Close()
                            SubProjectItem.ProjectItems.AddFromFile(DefaultTemplateFileLocation)

                            ProjectItem = Me._Parent.CheckProjectItemExists(ProjectWorking.ProjectItems, "web.config")
                            Dim WebConfigFileLocation As String =
                                IO.Path.Combine(WorkingFolder, "web.config")

                            If ProjectItem Is Nothing OrElse NoOtherDomainExists Then
                                SW = IO.File.CreateText(WebConfigFileLocation)
                                SW.WriteLine("<?xml version=""1.0""?>")
                                SW.WriteLine("<configuration>")
                                SW.WriteLine("  <connectionStrings />")
                                SW.WriteLine("  <system.web>")
                                SW.WriteLine("      <httpRuntime executionTimeout=""86400"" maxRequestLength=""2048000"" requestValidationMode=""2.0"" />")
                                SW.WriteLine("      <!-- ")
                                SW.WriteLine("          Set compilation debug=""True"" To insert debugging ")
                                SW.WriteLine("          symbols into the compiled page. Because this ")
                                SW.WriteLine("          affects performance, set this value to true only ")
                                SW.WriteLine("          during development.")
                                SW.WriteLine("      -->")
                                SW.WriteLine("      <compilation debug=""true"" />")
                                SW.WriteLine("      <!--")
                                SW.WriteLine("          The <authentication> section enables configuration ")
                                SW.WriteLine("          of the security authentication mode used by ")
                                SW.WriteLine("          ASP.NET to identify an incoming user. ")
                                SW.WriteLine("      -->")
                                SW.WriteLine("      <authentication mode=""Windows"" />")
                                SW.WriteLine("      <sessionState mode=""Off"" stateNetworkTimeout=""10"" timeout=""20"" compressionEnabled=""false"" />")
                                SW.WriteLine("      <!--")
                                SW.WriteLine("          The <customErrors> section enables configuration ")
                                SW.WriteLine("          of what to do if/when an unhandled error occurs ")
                                SW.WriteLine("          during the execution of a request. Specifically, ")
                                SW.WriteLine("          it enables developers To configure html Error pages ")
                                SW.WriteLine("          To be displayed In place Of a Error stack trace.")
                                SW.WriteLine("")
                                SW.WriteLine("          <customErrors mode=""RemoteOnly"" defaultRedirect=""GenericErrorPage.htm"">")
                                SW.WriteLine("              <Error statusCode=""403"" redirect=""NoAccess.htm"" />")
                                SW.WriteLine("              <Error statusCode=""404"" redirect=""FileNotFound.htm"" />")
                                SW.WriteLine("          </customErrors>")
                                SW.WriteLine("      -->")
                                SW.WriteLine("  </system.web>")
                                SW.WriteLine("  <system.webServer>")
                                SW.WriteLine("      <handlers>")
                                SW.WriteLine("          <add name=""XeoraCubeHandler"" path=""*"" verb=""*"" type=""Xeora.Web.Handler.RequestHandlerFactory"" resourceType=""Unspecified"" requireAccess=""Script"" preCondition=""integratedMode"" />")
                                SW.WriteLine("      </handlers>")
                                SW.WriteLine("      <modules>")
                                SW.WriteLine("          <add name=""XeoraCubeModule"" type=""Xeora.Web.Handler.RequestModule"" />")
                                SW.WriteLine("      </modules>")
                                SW.WriteLine("  </system.webServer>")
                                SW.WriteLine("</configuration>")
                                SW.Close()

                                ProjectWorking.ProjectItems.AddFromFile(WebConfigFileLocation)
                                'Else
                                '    Dim ConfigurationFI As New IO.FileInfo(IO.Path.Combine(WorkingFolder, "web.config"))
                                '    Dim VDMForConfig As New Web.Configuration.VirtualDirectoryMapping(ConfigurationFI.DirectoryName, True, ConfigurationFI.Name)
                                '    Dim WCFM As New Web.Configuration.WebConfigurationFileMap()
                                '    WCFM.VirtualDirectories.Add("/", VDMForConfig)

                                '    Dim Configuration As System.Configuration.Configuration =
                                '        Web.Configuration.WebConfigurationManager.OpenMappedWebConfiguration(WCFM, "/")

                                '    Configuration.AppSettings.Settings.Item("DefaultDomain").Value = ProjectSettings.DomainID
                                '    Configuration.AppSettings.Settings.Item("VirtualRoot").Value = ProjectSettings.VirtualPath
                                '    Configuration.AppSettings.Settings.Item("Debugging").Value = ProjectSettings.DebuggingActive.ToString()
                                '    Configuration.AppSettings.Settings.Item("VariablePoolServicePort").Value = ProjectSettings.VariablePoolServicePort
                                '    Configuration.AppSettings.Settings.Item("ScheduledTasksServicePort").Value = ProjectSettings.ScheduledTasksServicePort

                                '    Dim SystemWebServerHandlerSection As Web.Configuration.HttpHandlersSection =
                                '        CType(Configuration.GetSection("system.webServer/handlers"), Web.Configuration.HttpHandlersSection)

                                '    Dim HandlerExists As Boolean = False
                                '    For Each handler As Web.Configuration.HttpHandlerAction In SystemWebServerHandlerSection.Handlers
                                '        If String.Compare(handler.Type, "Xeora.Web.Handler.RequestHandlerFactory") = 0 Then _
                                '            HandlerExists = True : Exit For
                                '    Next
                                '    If Not HandlerExists Then
                                '        SystemWebServerHandlerSection.Handlers.Add(
                                '            New Web.Configuration.HttpHandlerAction("*", "Xeora.Web.Handler.RequestHandlerFactory", "*"))
                                '    End If

                                '    Dim SystemWebServerModuleSection As Web.Configuration.HttpModulesSection =
                                '        CType(Configuration.GetSection("system.webServer/modules"), Web.Configuration.HttpModulesSection)

                                '    Dim ModuleExists As Boolean = False
                                '    For Each [module] As Web.Configuration.HttpModuleAction In SystemWebServerModuleSection.Modules
                                '        If String.Compare([module].Type, "Xeora.Web.Handler.RequestModule") = 0 Then _
                                '            ModuleExists = True Exit For
                                '    Next
                                '    If Not ModuleExists Then
                                '        SystemWebServerModuleSection.Modules.Add(
                                '            New Web.Configuration.HttpModuleAction("XeoraCubeModule", "Xeora.Web.Handler.RequestModule"))
                                '    End If

                                '    Configuration.Save()
                            End If

                            ProjectItem = Me._Parent.CheckProjectItemExists(ProjectWorking.ProjectItems, "xeora.config")
                            Dim XeoraConfigFileLocation As String =
                                IO.Path.Combine(WorkingFolder, "xeora.config")

                            If ProjectItem Is Nothing OrElse NoOtherDomainExists Then
                                SW = IO.File.CreateText(XeoraConfigFileLocation)
                                SW.WriteLine("<?xml version=""1.0"" encoding=""utf-8""?>")
                                SW.WriteLine("<xeora>")
                                SW.WriteLine(String.Format("      <main defaultDomain=""{0}"" physicalRoot=""{1}"" virtualRoot=""{2}"" debugging=""{3}"" loggingPath=""{4}"" />",
                                                           ProjectSettings.DomainID,
                                                           WorkingFolder,
                                                           ProjectSettings.VirtualPath,
                                                           ProjectSettings.DebuggingActive.ToString(),
                                                           IO.Path.Combine(WorkingFolder, "XeoraLogs")))
                                SW.WriteLine("      <!-- You may need to set to the proper virtual directory path according to your IIS settings -->")
                                SW.WriteLine(String.Format("      <servicePort variablePool=""{0}"" scheduledTasks=""{1}"" />",
                                                           ProjectSettings.VariablePoolServicePort,
                                                           ProjectSettings.ScheduledTasksServicePort))
                                SW.WriteLine("      <!-- VariablePoolServicePort should be unique for each Xeora Application in the server -->")
                                SW.WriteLine("</xeora>")
                                SW.Close()

                                ProjectWorking.ProjectItems.AddFromFile(XeoraConfigFileLocation)
                            End If

                            If Not ReleasePulled Then Me.RePullRelease(BinProjectItem, CType(IIf(ProjectSettings.Use64bitRelease, 2, 1), Integer))
                        End If
                    End If
                End If
            End Sub

            Public Sub CompileDomain()
                Dim Projects As Projects =
                    CType(Me._Parent.DTE.Solution.Projects, Projects)

                If Projects.Count = 0 Then Exit Sub

                Dim ProjectList As New Tools.PrepareProject.ProjectList()

                For Each Project As Project In Projects
                    If String.Compare(Project.Kind, "{E24C65DC-7377-472b-9ABA-BC803B73C61A}") = 0 Then _
                        ProjectList.ProjectList.Add(Project.Name, Project.FullName)
                Next

                ProjectList.ShowDialog(PackageControl.IDEControl)

                If ProjectList.DialogResult = DialogResult.OK Then
                    Dim ProjectWorking As Project = Nothing
                    For Each ProjectWorking In Projects
                        If String.Compare(ProjectWorking.FullName, ProjectList.SelectedProject.Value) = 0 Then _
                            Exit For
                    Next

                    Dim CompilerForm As New Tools.CompilerForm()
                    CompilerForm.ShowDialog(PackageControl.IDEControl, ProjectWorking)
                End If
            End Sub

            Public Sub RePullRelease()
                Me.RePullRelease(Nothing, 0)
            End Sub

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="BinProjectItem"></param>
            ''' <param name="ReleaseVersion">0 = Unknown, 1 = x86, 2 = x64</param>
            Private Sub RePullRelease(ByVal BinProjectItem As ProjectItem, ByVal ReleaseVersion As Integer)
                Dim WorkingFolder As String = String.Empty
                Dim DomainProjectItemExists As Boolean =
                    (Not BinProjectItem Is Nothing)

                If BinProjectItem Is Nothing Then
                    Dim Projects As Projects =
                        CType(Me._Parent.DTE.Solution.Projects, Projects)

                    If Projects.Count = 0 Then Exit Sub

                    Dim ProjectList As New Tools.PrepareProject.ProjectList()

                    For Each Project As Project In Projects
                        If String.Compare(Project.Kind, "{E24C65DC-7377-472b-9ABA-BC803B73C61A}") = 0 Then _
                            ProjectList.ProjectList.Add(Project.Name, Project.FullName)
                    Next

                    ProjectList.ShowDialog(PackageControl.IDEControl)

                    If ProjectList.DialogResult = DialogResult.OK Then
                        WorkingFolder = ProjectList.SelectedProject.Value

                        Dim ProjectWorking As Project = Nothing
                        For Each ProjectWorking In Projects
                            If String.Compare(ProjectWorking.FullName, ProjectList.SelectedProject.Value) = 0 Then _
                                Exit For
                        Next

                        Dim ApplicationRootPI As ProjectItem =
                           PackageControl.IDEControl.GetApplicationRootProjectItem(Nothing, ProjectWorking.ProjectItems)
                        Dim SearchingProjectItems As ProjectItems = ProjectWorking.ProjectItems

                        If Not ApplicationRootPI Is Nothing Then
                            SearchingProjectItems = ApplicationRootPI.ProjectItems

                            WorkingFolder = CType(ApplicationRootPI.Properties.Item("FullPath").Value, String)
                        End If

                        For Each ProjectItem As ProjectItem In SearchingProjectItems
                            If String.Compare(ProjectItem.Name, "Bin", True) = 0 Then _
                                BinProjectItem = ProjectItem

                            If String.Compare(ProjectItem.Name, "Domains", True) = 0 Then _
                                DomainProjectItemExists = True
                        Next
                    End If
                Else
                    Dim DI As New IO.DirectoryInfo(CType(BinProjectItem.Properties.Item("FullPath").Value, String))
                    DI = DI.Parent

                    WorkingFolder = DI.FullName
                End If

                If Not BinProjectItem Is Nothing AndAlso DomainProjectItemExists Then
                    ' If ReleaseVersion didn't assign then try find from dll build
                    ' Otherwise use x86 version
                    If ReleaseVersion = 0 Then
                        Dim PA As Reflection.ProcessorArchitecture =
                            ExecutableLoaderHelper.ExecutableLoader.FrameworkArchitecture(IO.Path.Combine(WorkingFolder, "bin"))

                        If (PA = Reflection.ProcessorArchitecture.Amd64) Then ReleaseVersion = 2
                    End If

                    Dim DownloadProgress As New Tools.DownloadProgress()
                    DownloadProgress.StartDownloading(IO.Path.Combine(WorkingFolder, "bin"), (ReleaseVersion = 2), Me._Parent)

                    ' TODO: SHOW RELEASE NOTES!
                    If DownloadProgress.DialogResult = DialogResult.OK Then
                        For Each File As String In DownloadProgress.DownloadedFiles
                            Dim ItemExists As ProjectItem =
                                Me._Parent.CheckProjectItemExists(BinProjectItem.ProjectItems, File)
                            If Not ItemExists Is Nothing Then
                                Try
                                    ItemExists.Remove()
                                Catch ex As Exception
                                    ' This exception handling is for debug limitation
                                End Try
                            End If

                            BinProjectItem.ProjectItems.AddFromFile(IO.Path.Combine(WorkingFolder, "bin", File))
                        Next
                    End If
                End If
            End Sub
        End Class

        Public Function CheckProjectItemExists(ByVal SearchingProjectItems As ProjectItems, ByVal SearchPath As String) As ProjectItem
            Dim rProjectItem As ProjectItem = Nothing
            Dim SearchPaths As String() = SearchPath.Split("\"c)

            Dim WorkingProjectItems As ProjectItems = SearchingProjectItems
            For Each SP As String In SearchPaths
                For Each WPI As ProjectItem In WorkingProjectItems
                    If String.Compare(WPI.Name, SP, True) = 0 Then
                        rProjectItem = WPI
                        WorkingProjectItems = WPI.ProjectItems

                        Exit For
                    End If
                Next
            Next

            Return rProjectItem
        End Function

        Private Function SearchProjectItemRecursive(ByVal ProjectItems As ProjectItems, ByVal SearchingNames As String()) As ProjectItem
            Dim rProjectItem As ProjectItem = Nothing

            For Each projItem As ProjectItem In ProjectItems
                If Array.IndexOf(SearchingNames, projItem.Name) > -1 Then
                    rProjectItem = projItem

                    Exit For
                Else
                    If Not projItem.ProjectItems Is Nothing AndAlso projItem.ProjectItems.Count > 0 Then _
                        rProjectItem = Me.SearchProjectItemRecursive(projItem.ProjectItems, SearchingNames)

                    If Not rProjectItem Is Nothing Then Exit For
                End If
            Next

            Return rProjectItem
        End Function

        Private Function CheckProjectFolders(ByVal pItems As ProjectItems) As Boolean
            Dim rBoolean As Boolean = False

            Dim AddonsPath As String = String.Empty
            Dim ContentsPath As String = String.Empty
            Dim LanguagesPath As String = String.Empty
            Dim TemplatesPath As String = String.Empty
            Dim ExecutablesPath As String = String.Empty

            For iC As Integer = 1 To pItems.Count
                If String.Compare(pItems.Item(iC).Name, "Addons", True) = 0 Then
                    AddonsPath = pItems.Item(iC).FileNames(0)
                ElseIf String.Compare(pItems.Item(iC).Name, "Contents", True) = 0 Then
                    ContentsPath = pItems.Item(iC).FileNames(0)
                ElseIf String.Compare(pItems.Item(iC).Name, "Languages", True) = 0 Then
                    LanguagesPath = pItems.Item(iC).FileNames(0)
                ElseIf String.Compare(pItems.Item(iC).Name, "Templates", True) = 0 Then
                    TemplatesPath = pItems.Item(iC).FileNames(0)
                ElseIf String.Compare(pItems.Item(iC).Name, "Executables", True) = 0 Then
                    ExecutablesPath = pItems.Item(iC).FileNames(0)
                Else
                    If (
                            String.IsNullOrEmpty(ContentsPath) OrElse
                            String.IsNullOrEmpty(LanguagesPath) OrElse
                            String.IsNullOrEmpty(TemplatesPath) OrElse
                            String.IsNullOrEmpty(ExecutablesPath)
                        ) AndAlso
                        Not pItems.Item(iC).ProjectItems Is Nothing AndAlso
                        pItems.Item(iC).ProjectItems.Count > 0 Then

                        rBoolean = Me.CheckProjectFolders(pItems.Item(iC).ProjectItems)
                    End If
                End If

                If rBoolean Then Exit For
            Next

            If Not rBoolean AndAlso Not String.IsNullOrEmpty(ContentsPath) AndAlso
                Not String.IsNullOrEmpty(LanguagesPath) AndAlso
                Not String.IsNullOrEmpty(TemplatesPath) Then

                rBoolean = True

                Dim FileNames As String() = IO.Directory.GetFiles(LanguagesPath, "*.xml")

                For Each fileName As String In FileNames
                    If Not IO.Directory.Exists(
                        IO.Path.Combine(ContentsPath, IO.Path.GetFileNameWithoutExtension(fileName))) Then

                        rBoolean = False

                        Exit For
                    End If
                Next

                If rBoolean Then
                    FileNames = IO.Directory.GetFiles(TemplatesPath, "*.xml")

                    For Each fileName As String In FileNames
                        If String.Compare(IO.Path.GetFileNameWithoutExtension(fileName), "Configuration", True) <> 0 AndAlso
                            String.Compare(IO.Path.GetFileNameWithoutExtension(fileName), "Controls", True) <> 0 Then

                            rBoolean = False

                            Exit For
                        End If
                    Next
                End If
            End If

            Return rBoolean
        End Function

        Public Function CheckIsXeoraCubeProject() As Boolean
            If Me._DTE.ActiveDocument Is Nothing Then Return False

            Dim ActiveProject As Project =
                Me._DTE.ActiveDocument.ProjectItem.ContainingProject

            Return Me.CheckProjectFolders(ActiveProject.ProjectItems)
        End Function

        Public Function CheckIsXeoraTemplateFile() As Boolean
            If Me._DTE.ActiveDocument Is Nothing Then Return False

            Dim ActiveItemPath As String =
                Me._DTE.ActiveDocument.Path
            Dim CheckDI As New IO.DirectoryInfo(ActiveItemPath)

            Dim DeepCounter As Integer = 0
            Do
                Select Case DeepCounter
                    Case 0 ' Templates
                        If String.Compare(CheckDI.Name, "Templates", True) = 0 Then
                            CheckDI = CheckDI.Parent
                        Else
                            If CheckDI.FullName.IndexOf("Templates") > -1 Then
                                CheckDI = CheckDI.Parent
                                DeepCounter -= 1
                            Else
                                Return False
                            End If
                        End If
                    Case 1 ' DomainName or AddonName
                        CheckDI = CheckDI.Parent
                    Case 2 ' Domains or Addons
                        If String.Compare(CheckDI.Name, "Domains", True) = 0 Then
                            Return True
                        Else
                            If String.Compare(CheckDI.Name, "Addons", True) = 0 Then
                                CheckDI = CheckDI.Parent

                                DeepCounter = 0
                            Else
                                Return False
                            End If
                        End If

                End Select

                DeepCounter += 1
            Loop Until DeepCounter > 2 OrElse CheckDI Is Nothing

            Return False
        End Function

        Public Function GetApplicationRootProjectItem(ByVal ParentProjectItem As ProjectItem, ByVal ProjectItems As ProjectItems) As ProjectItem
            For Each ProjectItem As ProjectItem In ProjectItems
                If String.Compare(ProjectItem.Name, "xeora.config", True) = 0 Then _
                    Return ParentProjectItem

                If Not ProjectItem.ProjectItems Is Nothing Then
                    Dim TempPI As ProjectItem =
                        Me.GetApplicationRootProjectItem(ProjectItem, ProjectItem.ProjectItems)

                    If Not TempPI Is Nothing Then _
                        Return TempPI
                End If
            Next

            Return Nothing
        End Function

        Public Function GetDomainType(ByVal LookingPath As String) As Globals.ActiveDomainTypes
            Dim sT As Globals.ActiveDomainTypes =
                Globals.ActiveDomainTypes.Domain

            Dim CheckDI As IO.DirectoryInfo =
                New IO.DirectoryInfo(
                    IO.Path.GetFullPath(
                        IO.Path.Combine(LookingPath, "..", "..")
                    )
                )

            If CheckDI.Exists AndAlso
                String.Compare(CheckDI.Name, "Addons", True) = 0 Then _
                sT = Globals.ActiveDomainTypes.Child

            Return sT
        End Function

        Public Function GetActiveItemDomainType() As Globals.ActiveDomainTypes
            Return Me.GetDomainType(Me._DTE.ActiveDocument.Path)
        End Function

        Public ReadOnly Property Handle As IntPtr Implements IWin32Window.Handle
            Get
                Return New IntPtr(Me._DTE.MainWindow.HWnd)
            End Get
        End Property
    End Class
End Namespace
