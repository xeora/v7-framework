Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Language.Intellisense
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Operations
Imports Microsoft.VisualStudio.Utilities

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion
    <Export(GetType(ICompletionSourceProvider))>
    <Name("XeoraControls")>
    <Order(Before:="all")>
    <ContentType(EditorExtension.ControlContentType)>
    Public NotInheritable Class ControlsSourceProvider
        Implements ICompletionSourceProvider

        <Import()>
        Public Property NavigatorService() As ITextStructureNavigatorSelectorService

        Public Function TryCreateCompletionSource(ByVal textBuffer As ITextBuffer) As ICompletionSource Implements ICompletionSourceProvider.TryCreateCompletionSource
            Return New ControlsSource(Me, textBuffer)
        End Function
    End Class
End Namespace