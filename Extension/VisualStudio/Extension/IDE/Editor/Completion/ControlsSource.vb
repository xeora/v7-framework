Imports System.Collections.Generic
Imports Microsoft.VisualStudio.Language
Imports Microsoft.VisualStudio.Text

Namespace Xeora.Extension.VisualStudio.IDE.Editor.Completion
    Public Class ControlsSource
        Implements Intellisense.ICompletionSource

        Private _SourceProvider As ControlsSourceProvider
        Private _TextBuffer As ITextBuffer

        Public Sub New(ByVal SourceProvider As ControlsSourceProvider, ByVal textBuffer As ITextBuffer)
            Me._SourceProvider = SourceProvider
            Me._TextBuffer = textBuffer
        End Sub

        Public Sub AugmentCompletionSession(ByVal session As Intellisense.ICompletionSession, ByVal completionSets As IList(Of Intellisense.CompletionSet)) Implements Intellisense.ICompletionSource.AugmentCompletionSession
            Dim CompList As Intellisense.Completion() = Nothing
            Dim Builders As Intellisense.Completion() = Nothing

            Dim Directive As ControlsCommandHandler.Directives = ControlsCommandHandler.Directives.None
            session.Properties.TryGetProperty(Of ControlsCommandHandler.Directives)("Directive", Directive)

            Select Case Directive
                Case ControlsCommandHandler.Directives.Tag
                    Dim ControlTag As New SourceBuilder.ControlTag(Directive, session, Me._TextBuffer)

                    CompList = ControlTag.Build()
                    Builders = ControlTag.Builders()

                Case ControlsCommandHandler.Directives.Bind, ControlsCommandHandler.Directives.ServerExecutable
                    Dim WorkingExecutableInfo As String = String.Empty
                    session.Properties.TryGetProperty(Of String)("Executable_CurrentStatement", WorkingExecutableInfo)

                    If String.IsNullOrEmpty(WorkingExecutableInfo) Then
                        Dim Executable As New SourceBuilder.Executable(Directive)

                        CompList = Executable.Build()
                        Builders = Executable.Builders()
                    Else
                        Dim [Class] As New SourceBuilder.Class(Directive)

                        [Class].WorkingExecutableInfo = WorkingExecutableInfo
                        CompList = [Class].Build()
                        Builders = [Class].Builders()
                    End If

                Case ControlsCommandHandler.Directives.Control
                    Dim Control As New SourceBuilder.Control(Directive, Nothing, Nothing)

                    Control.WorkingControlID = SourceBuilder.ControlTag.CurrentControlID(session, Me._TextBuffer)
                    Control.OnlyButtons = True

                    CompList = Control.Build()
                    Builders = Control.Builders()

                Case ControlsCommandHandler.Directives.Type
                    Dim [Type] As New SourceBuilder.Type(Directive)

                    CompList = [Type].Build()
                    Builders = [Type].Builders()

                    ' Xeora Tag Requests
                Case ControlsCommandHandler.Directives.Special
                    Dim Special As New SourceBuilder.Special(Directive)

                    Special.RequestFromControl = True

                    CompList = Special.Build()

                Case ControlsCommandHandler.Directives.TemplateWithVariablePool
                    Dim Template As New SourceBuilder.Template(Directive)

                    CompList = Template.Build()
                    Builders = Template.Builders()

                Case ControlsCommandHandler.Directives.Translation
                    Dim Translation As New SourceBuilder.Translation(Directive)

                    CompList = Translation.Build()
                    Builders = Translation.Builders()

            End Select

            If Not CompList Is Nothing Then
                completionSets.Add(
                    New Intellisense.CompletionSet("XeoraControls", "Xeora³", Me.FindSpanAtPosition(session), CompList, Builders)
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
            Return currentPoint.Snapshot.CreateTrackingSpan(currentPoint, 0, SpanTrackingMode.EdgeInclusive)
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