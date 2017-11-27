Imports Microsoft.VisualStudio.Language
Imports My.Resources

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public Class [Type]
        Inherits BuilderBase

        Public Sub New(ByVal Directive As [Enum])
            MyBase.New(Directive)
        End Sub

        Public Overrides Function Build() As Intellisense.Completion()
            Dim CompList As New Generic.List(Of Intellisense.Completion)()

            Dim ControlTypeNames As String() =
                [Enum].GetNames(GetType(Globals.ControlTypes))

            For Each ControlTypeName As String In ControlTypeNames
                If String.Compare(ControlTypeName, Globals.ControlTypes.Unknown.ToString(), True) <> 0 Then
                    CompList.Add(New Intellisense.Completion(ControlTypeName, ControlTypeName, String.Empty, Me.ProvideImageSource(IconResource.controltype), Nothing))
                End If
            Next

            CompList.Sort(New CompletionComparer())

            Return CompList.ToArray()
        End Function

        Public Overrides Function Builders() As Intellisense.Completion()
            Return Nothing
        End Function
    End Class
End Namespace