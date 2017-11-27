Imports System.ComponentModel.Composition
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Highlighter
    Public NotInheritable Class SyntaxClassificationDefinition
        Public Const TagAndDirective As String = "TagAndDirective"
        Public Const DirectiveID As String = "DirectiveID"
        Public Const InternalDirective As String = "InternalDirective"
        Public Const Leveling As String = "Leveling"
        Public Const BlackBracket As String = "BlackBracket"

        <Export(GetType(ClassificationTypeDefinition))>
        <Name(TagAndDirective)>
        Private TagAndDirectiveType As ClassificationTypeDefinition

        <Export(GetType(ClassificationTypeDefinition))>
        <Name(DirectiveID)>
        Private DirectiveIDType As ClassificationTypeDefinition

        <Export(GetType(ClassificationTypeDefinition))>
        <Name(InternalDirective)>
        Private InternalDirectiveType As ClassificationTypeDefinition

        <Export(GetType(ClassificationTypeDefinition))>
        <Name(Leveling)>
        Private LevelingType As ClassificationTypeDefinition

        <Export(GetType(ClassificationTypeDefinition))>
        <Name(BlackBracket)>
        Private BlackBracketType As ClassificationTypeDefinition
    End Class
End Namespace