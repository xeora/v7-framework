Imports Microsoft.VisualStudio.Language
Imports My.Resources
Imports Xeora.Extension.VisualStudio.IDE.Editor.Completion.TemplateCommandHandler

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion.SourceBuilder
    Public Class MiddleOperator
        Inherits BuilderBase

        Public Property WorkingDirectives As Directives

        Public Sub New(ByVal Directive As [Enum])
            MyBase.New(Directive)
        End Sub

        Public Overrides Function Build() As Intellisense.Completion()
            Dim CompList As New Generic.List(Of Intellisense.Completion)()

            Select Case Me.WorkingDirectives
                Case Directives.Control, Directives.ControlWithLeveling, Directives.ControlWithLevelingAndBound, Directives.ControlWithBound
                    CompList.Add(New Intellisense.Completion(": - Define ID", ":", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))

                    If Me.WorkingDirectives <> Directives.ControlWithLevelingAndBound AndAlso Me.WorkingDirectives <> Directives.ControlWithBound Then _
                        CompList.Add(New Intellisense.Completion("[ - Bind to Control", "[", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))

                    If Me.WorkingDirectives = Directives.Control Then _
                        CompList.Add(New Intellisense.Completion("# - Apply Leveling", "#0", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))

                Case Directives.ServerExecutable, Directives.ServerExecutableWithLeveling, Directives.ServerExecutableWithLevelingAndBound, Directives.ServerExecutableWithBound
                    CompList.Add(New Intellisense.Completion(": - Bind Library", ":", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))

                    If Me.WorkingDirectives <> Directives.ServerExecutableWithLevelingAndBound AndAlso Me.WorkingDirectives <> Directives.ServerExecutableWithBound Then _
                        CompList.Add(New Intellisense.Completion("[ - Bind to Control", "[", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))

                    If Me.WorkingDirectives = Directives.ServerExecutable Then _
                        CompList.Add(New Intellisense.Completion("# - Apply Leveling", "#0", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))

                Case Directives.InLineStatement, Directives.InLineStatementWithBound
                    CompList.Add(New Intellisense.Completion(": - Define ID", ":", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))

                    If Me.WorkingDirectives <> Directives.InLineStatementWithBound Then _
                        CompList.Add(New Intellisense.Completion("[ - Bind to Control", "[", String.Empty, Me.ProvideImageSource(IconResource.middleoperator), Nothing))
            End Select

            Return CompList.ToArray()
        End Function

        Public Overrides Function Builders() As Intellisense.Completion()
            Return Nothing
        End Function
    End Class
End Namespace