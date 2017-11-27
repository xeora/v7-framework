Imports EnvDTE

Namespace Xeora.Extension.VisualStudio
    Public NotInheritable Class PackageControl
        Private Shared _ServiceProvider As IServiceProvider
        Private Shared _IDEControl As IDEControl

        'Private WithEvents _textDocumentKeyPressEvents As EnvDTE80.TextDocumentKeyPressEvents

        Public Shared ReadOnly Property ServiceProvider As IServiceProvider
            Get
                Return PackageControl._ServiceProvider
            End Get
        End Property

        Public Shared ReadOnly Property IDEControl As IDEControl
            Get
                Return PackageControl._IDEControl
            End Get
        End Property

        Public Shared Sub Initialize(ByVal package As IServiceProvider)
            PackageControl._ServiceProvider = package

            ' Initialize Commands
            IDE.Command.Command.Initialize()

            PackageControl._IDEControl =
                New IDEControl(CType(PackageControl._ServiceProvider.GetService(GetType(DTE)), DTE))

            'Dim appEvents As Events2 = CType(Me._applicationObject.Events, Events2)
            'Me._textDocumentKeyPressEvents = CType(appEvents.TextDocumentKeyPressEvents(Nothing), TextDocumentKeyPressEvents)

            'AddHandler Me._textDocumentKeyPressEvents.AfterKeyPress, New _dispTextDocumentKeyPressEvents_AfterKeyPressEventHandler(AddressOf Me._addInControl.event_TDAfterKeyPressed)
        End Sub
    End Class
End Namespace