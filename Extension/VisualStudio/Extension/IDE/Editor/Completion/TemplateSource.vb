Imports System.Collections.Generic
Imports Microsoft.VisualStudio.Language
Imports Microsoft.VisualStudio.Text

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion
    Public Class TemplateSource
        Implements Intellisense.ICompletionSource

        Private _SourceProvider As TemplateSourceProvider
        Private _TextBuffer As ITextBuffer

        Public Sub New(ByVal SourceProvider As TemplateSourceProvider, ByVal textBuffer As ITextBuffer)
            Me._SourceProvider = SourceProvider
            Me._TextBuffer = textBuffer
        End Sub

        Public Sub AugmentCompletionSession(ByVal session As Intellisense.ICompletionSession, ByVal completionSets As IList(Of Intellisense.CompletionSet)) Implements Intellisense.ICompletionSource.AugmentCompletionSession
            Dim CompList As Intellisense.Completion() = Nothing
            Dim Builders As Intellisense.Completion() = Nothing

            Dim Directive As TemplateCommandHandler.Directives = TemplateCommandHandler.Directives.None
            session.Properties.TryGetProperty(Of TemplateCommandHandler.Directives)("Directive", Directive)

            Select Case Directive
                Case TemplateCommandHandler.Directives.Special
                    Dim Special As New SourceBuilder.Special(Directive)

                    CompList = Special.Build()

                Case TemplateCommandHandler.Directives.MiddleOperator
                    Dim MiddleOperator As New SourceBuilder.MiddleOperator(Directive)

                    session.Properties.TryGetProperty(Of TemplateCommandHandler.Directives)("MiddleOperator_WorkingDirective", MiddleOperator.WorkingDirectives)
                    CompList = MiddleOperator.Build()

                Case TemplateCommandHandler.Directives.Template, TemplateCommandHandler.Directives.TemplateWithVariablePool
                    Dim Template As New SourceBuilder.Template(Directive)

                    session.Properties.TryGetProperty(Of String)("Template_FilePath", Template.WorkingTemplateID)
                    CompList = Template.Build()
                    Builders = Template.Builders()

                Case TemplateCommandHandler.Directives.Translation
                    Dim Translation As New SourceBuilder.Translation(Directive)

                    CompList = Translation.Build()
                    Builders = Translation.Builders()

                Case TemplateCommandHandler.Directives.Control, TemplateCommandHandler.Directives.ControlWithLeveling
                    Dim Control As New SourceBuilder.Control(Directive, Nothing, Nothing)

                    CompList = Control.Build()
                    Builders = Control.Builders()

                Case TemplateCommandHandler.Directives.ControlWithBound, TemplateCommandHandler.Directives.ControlWithLevelingAndBound
                    Dim ControlWP As New SourceBuilder.Control(Directive, session, Me._TextBuffer)

                    session.Properties.TryGetProperty(Of String)("Control_ControlID", ControlWP.WorkingControlID)
                    CompList = ControlWP.Build()
                    Builders = Nothing

                    If CompList.Length = 0 Then
                        session.Dismiss()

                        Exit Sub
                    End If

                Case TemplateCommandHandler.Directives.ServerExecutable, TemplateCommandHandler.Directives.ClientExecutable
                    Dim StatementText As String = String.Empty
                    session.Properties.TryGetProperty(Of String)("Executable_CurrentStatement", StatementText)

                    If String.IsNullOrEmpty(StatementText) Then
                        Dim Executable As New SourceBuilder.Executable(Directive)

                        CompList = Executable.Build()
                        Builders = Executable.Builders()
                    Else
                        Dim [Class] As New SourceBuilder.Class(Directive)

                        [Class].WorkingExecutableInfo = StatementText
                        CompList = [Class].Build()
                        Builders = [Class].Builders()
                    End If

            End Select

            If Not CompList Is Nothing Then
                completionSets.Add(
                    New Intellisense.CompletionSet("XeoraTemplate", "Xeora³", Me.FindSpanAtPosition(session), CompList, Builders)
                )
            End If
        End Sub

        Private Function FindSpanAtPosition(ByVal session As Intellisense.ICompletionSession) As ITrackingSpan
            Dim currentPoint As SnapshotPoint =
                session.TextView.Caret.Position.BufferPosition

            'Dim navigator As ITextStructureNavigator =
            '    Me._SourceProvider.NavigatorService.GetTextStructureNavigator(Me._TextBuffer)
            'Dim extent As TextExtent =
            '    navigator.GetExtentOfWord(currentPoint)
            'extent.Span.Start, extent.Span.Length - 1

            Dim virtualPoint As VirtualSnapshotSpan =
                CType(session.TextView.Selection.GetSelectionOnTextViewLine(session.TextView.Caret.ContainingTextViewLine), VirtualSnapshotSpan)

            If virtualPoint <> Nothing Then _
                Return currentPoint.Snapshot.CreateTrackingSpan(virtualPoint.Start.Position, virtualPoint.Length, SpanTrackingMode.EdgeInclusive)

            Return currentPoint.Snapshot.CreateTrackingSpan(currentPoint.Position, 0, SpanTrackingMode.EdgeInclusive)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    GC.SuppressFinalize(Me)
                End If
            End If
            Me.disposedValue = True
        End Sub

        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)

            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace