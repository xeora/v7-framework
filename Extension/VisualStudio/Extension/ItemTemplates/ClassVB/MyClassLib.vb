Imports System.Reflection
Imports Xeora.Web.Shared

Namespace Xeora.Domain
    Public Class $safeitemname$
        Implements Web.Shared.IDomainExecutable

        Public Sub Initialize() Implements IDomainExecutable.Initialize
        End Sub

        Public Sub PostExecute(ExecutionID As String, ByRef Result As Object) Implements IDomainExecutable.PostExecute
        End Sub

        Public Sub PreExecute(ExecutionID As String, ByRef MI As MethodInfo) Implements IDomainExecutable.PreExecute
        End Sub

        Private Sub IDomainExecutable_Finalize() Implements IDomainExecutable.Finalize
        End Sub

        Public Function URLResolver(RequestFilePath As String) As URLMapping.ResolvedMapped Implements IDomainExecutable.URLResolver
            Return Nothing
        End Function
    End Class
End Namespace
