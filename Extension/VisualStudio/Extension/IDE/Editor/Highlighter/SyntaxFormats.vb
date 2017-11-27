Imports System.ComponentModel.Composition
Imports System.Windows.Media
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Highlighter
    Public NotInheritable Class SyntaxFormats
        <Export(GetType(EditorFormatDefinition))>
        <ClassificationType(ClassificationTypeNames:=SyntaxClassificationDefinition.TagAndDirective)>
        <Name(SyntaxClassificationDefinition.TagAndDirective)>
        <UserVisible(True)>
        <Order(After:=Priority.Default)>
        Public NotInheritable Class TagAndDirectiveFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.DisplayName = SyntaxClassificationDefinition.TagAndDirective
                Me.ForegroundColor = Colors.CornflowerBlue
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition))>
        <ClassificationType(ClassificationTypeNames:=SyntaxClassificationDefinition.DirectiveID)>
        <Name(SyntaxClassificationDefinition.DirectiveID)>
        <UserVisible(True)>
        <Order(After:=Priority.Default)>
        Public NotInheritable Class DirectiveIDFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.DisplayName = SyntaxClassificationDefinition.DirectiveID
                Me.ForegroundColor = Colors.Sienna
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition))>
        <ClassificationType(ClassificationTypeNames:=SyntaxClassificationDefinition.InternalDirective)>
        <Name(SyntaxClassificationDefinition.InternalDirective)>
        <UserVisible(True)>
        <Order(After:=Priority.Default)>
        Public NotInheritable Class InternalDirectiveFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.DisplayName = SyntaxClassificationDefinition.InternalDirective
                Me.ForegroundColor = Colors.Red
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition))>
        <ClassificationType(ClassificationTypeNames:=SyntaxClassificationDefinition.Leveling)>
        <Name(SyntaxClassificationDefinition.Leveling)>
        <UserVisible(True)>
        <Order(After:=Priority.Default)>
        Public NotInheritable Class LevelingFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.DisplayName = SyntaxClassificationDefinition.Leveling
                Me.ForegroundColor = Colors.Green
            End Sub
        End Class

        <Export(GetType(EditorFormatDefinition))>
        <ClassificationType(ClassificationTypeNames:=SyntaxClassificationDefinition.BlackBracket)>
        <Name(SyntaxClassificationDefinition.BlackBracket)>
        <UserVisible(True)>
        <Order(After:=Priority.Default)>
        Public NotInheritable Class BlackBracketFormat
            Inherits ClassificationFormatDefinition

            Public Sub New()
                Me.DisplayName = SyntaxClassificationDefinition.BlackBracket
                Me.ForegroundColor = Colors.Black
            End Sub
        End Class
    End Class
End Namespace