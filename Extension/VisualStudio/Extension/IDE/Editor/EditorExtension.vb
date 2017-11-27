Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace Xeora.Extension.VisualStudio.IDE.Editor
    Public NotInheritable Class EditorExtension
        Public Const TemplateContentType As String = "xeora"
        Public Const ControlContentType As String = "xccontrols"

        <Export(GetType(ContentTypeDefinition))>
        <Name(EditorExtension.TemplateContentType)>
        <BaseDefinition("html")>
        Public Property XeoraTemplateContentTypeDefinition As ContentTypeDefinition

        <Export(GetType(FileExtensionToContentTypeDefinition))>
        <FileExtension(".xchtml")>
        <ContentType(EditorExtension.TemplateContentType)>
        Public Property XeoraTemplateFileExtensionDefinition As FileExtensionToContentTypeDefinition

        <Export(GetType(ContentTypeDefinition))>
        <Name(EditorExtension.ControlContentType)>
        <BaseDefinition("xml")>
        Public Property XeoraControlsContentTypeDefinition As ContentTypeDefinition

        <Export(GetType(FileExtensionToContentTypeDefinition))>
        <FileExtension(".xml")>
        <ContentType(EditorExtension.ControlContentType)>
        Public Property XeoraControlsFileExtensionDefinition As FileExtensionToContentTypeDefinition
    End Class
End Namespace