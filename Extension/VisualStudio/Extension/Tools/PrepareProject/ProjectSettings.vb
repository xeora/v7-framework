Namespace Xeora.Extension.VisualStudio.Tools.PrepareProject
    Public Class ProjectSettings
        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.comboCaching.SelectedIndex = 2
        End Sub

        Public ReadOnly Property DomainID As String
            Get
                Return Me.tbDomainID.Text
            End Get
        End Property

        Public ReadOnly Property LanguageID As String
            Get
                Return Me.tbLanguageID.Text
            End Get
        End Property

        Public ReadOnly Property LanguageName As String
            Get
                Return Me.tbLanguageName.Text
            End Get
        End Property

        Public ReadOnly Property CachingType As String
            Get
                Return CType(Me.comboCaching.SelectedItem, String)
            End Get
        End Property

        Public ReadOnly Property VirtualPath As String
            Get
                Return Me.tbVirtualPath.Text
            End Get
        End Property

        Public ReadOnly Property VariablePoolServicePort As String
            Get
                Return Me.tbVariablePoolServicePort.Text
            End Get
        End Property

        Public ReadOnly Property ScheduledTasksServicePort As String
            Get
                Return Me.tbScheduledTasksServicePort.Text
            End Get
        End Property

        Public ReadOnly Property Use64bitRelease As Boolean
            Get
                Return Me.cb64BitRelease.Checked
            End Get
        End Property

        Public ReadOnly Property DebuggingActive As Boolean
            Get
                Return Me.cbDebug.Checked
            End Get
        End Property

        Private Sub tbDomainID_TextChanged(sender As Object, e As EventArgs) Handles tbDomainID.TextChanged
            If tbDomainID.Text.Length = 0 Then
                tbDomainID.BackColor = Drawing.Color.IndianRed

                Exit Sub
            End If
            tbDomainID.BackColor = Drawing.Color.White

            Dim Pointer As Integer =
            tbDomainID.SelectionStart
            Dim Character As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

            Dim DomainID As String = tbDomainID.Text
            For cC As Integer = DomainID.Length - 1 To 0 Step -1
                If Character.IndexOf(DomainID.Substring(cC, 1)) = -1 Then _
                DomainID = DomainID.Remove(cC, 1) : Pointer -= 1
            Next

            tbDomainID.Text = DomainID
            tbDomainID.SelectionStart = Pointer
        End Sub

        Private Sub tbLanguageName_TextChanged(sender As Object, e As EventArgs) Handles tbLanguageName.TextChanged
            If tbLanguageName.Text.Length = 0 Then
                tbLanguageName.BackColor = Drawing.Color.IndianRed

                Exit Sub
            End If
            tbLanguageName.BackColor = Drawing.Color.White
        End Sub

        Private Sub tbLanguageID_Leave(sender As Object, e As EventArgs) Handles tbLanguageID.Leave
            Dim LanguageID As String = tbLanguageID.Text

            If LanguageID.Length = 3 Then LanguageID = String.Concat(LanguageID, LanguageID.Substring(0, 2))
            If LanguageID.Length <> 5 Then Me.tbLanguageID.Text = "en-US" : Exit Sub
            LanguageID = String.Format("{0}-{1}", LanguageID.Substring(0, 2).ToLower(), LanguageID.Substring(3, 2).ToUpper())

            Me.tbLanguageID.Text = LanguageID
        End Sub

        Private Sub tbVirtualPath_TextChanged(sender As Object, e As EventArgs) Handles tbVirtualPath.TextChanged
            If tbVirtualPath.Text.Length = 0 Then Exit Sub

            Dim Pointer As Integer =
            tbVirtualPath.SelectionStart
            Dim Character As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/_-"

            Dim VirtualPath As String = tbVirtualPath.Text
            For cC As Integer = VirtualPath.Length - 1 To 0 Step -1
                If Character.IndexOf(VirtualPath.Substring(cC, 1)) = -1 Then _
                VirtualPath = VirtualPath.Remove(cC, 1) : Pointer -= 1
            Next

            If VirtualPath.IndexOf("/"c) <> 0 Then _
            VirtualPath = String.Concat("/", VirtualPath) : Pointer += 1

            If VirtualPath.Length > 1 AndAlso
            VirtualPath.LastIndexOf("/"c) <> (VirtualPath.Length - 1) Then

                VirtualPath = String.Concat(VirtualPath, "/")
            End If

            tbVirtualPath.Text = VirtualPath
            tbVirtualPath.SelectionStart = Pointer
        End Sub

        Private Sub tbVirtualPath_Leave(sender As Object, e As EventArgs) Handles tbVirtualPath.Leave
            If tbVirtualPath.Text.Length = 0 Then tbVirtualPath.Text = "/"
        End Sub

        Private Sub OnlyNumber_TextChanged(sender As Object, e As EventArgs) Handles tbVariablePoolServicePort.TextChanged, tbScheduledTasksServicePort.TextChanged
            If CType(sender, TextBox).Text.Length = 0 Then Exit Sub

            Dim Pointer As Integer =
            CType(sender, TextBox).SelectionStart
            Dim Character As String = "0123456789"

            Dim SenderText As String = CType(sender, TextBox).Text
            For cC As Integer = SenderText.Length - 1 To 0 Step -1
                If Character.IndexOf(SenderText.Substring(cC, 1)) = -1 Then _
                SenderText = SenderText.Remove(cC, 1) : Pointer -= 1
            Next

            If Integer.Parse(SenderText) > 65535 Then _
            SenderText = "65535"

            CType(sender, TextBox).Text = SenderText
            CType(sender, TextBox).SelectionStart = Pointer
        End Sub

        Private Sub OnlyNumber_Leave(sender As Object, e As EventArgs) Handles tbVariablePoolServicePort.Leave, tbScheduledTasksServicePort.Leave
            If CType(sender, TextBox).Text.Length = 0 Then
                Select Case CType(sender, TextBox).Name
                    Case "tbVariablePoolServicePort"
                        CType(sender, TextBox).Text = "12005"
                    Case "tbScheduledTasksServicePort"
                        CType(sender, TextBox).Text = "0"
                End Select
            End If
        End Sub

        Private Sub butAccept_Click(sender As Object, e As EventArgs) Handles butAccept.Click
            Me.DialogResult = DialogResult.OK
        End Sub
    End Class
End Namespace