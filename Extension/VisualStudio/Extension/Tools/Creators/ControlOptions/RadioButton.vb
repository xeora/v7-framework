Namespace Xeora.Extension.VisualStudio.Tools.Creators.ControlOptions
    Public Class RadioButton
        Private Sub llAddBlockID_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles llAddBlockID.LinkClicked
            If Not String.IsNullOrEmpty(Me.tbBlockID.Text.Trim()) Then
                Me.lwBlockID.Items.Add(Me.tbBlockID.Text.Trim())
                Me.tbBlockID.Text = String.Empty
            End If
        End Sub

        Private Sub lwBlockID_KeyDown(sender As Object, e As KeyEventArgs) Handles lwBlockID.KeyDown
            If Me.lwBlockID.SelectedIndices.Count = 0 Then Return

            Select Case e.KeyCode
                Case Keys.Back, Keys.Delete
                    Me.lwBlockID.SuspendLayout()
                    Me.lwBlockID.Items.RemoveAt(Me.lwBlockID.SelectedIndices.Item(0))
                    Me.lwBlockID.ResumeLayout()
            End Select
        End Sub

        Private Sub tbBlockID_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles tbBlockID.PreviewKeyDown
            e.IsInputKey = (e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Return)
        End Sub

        Private Sub tbBlockID_KeyDown(sender As Object, e As KeyEventArgs) Handles tbBlockID.KeyDown
            Select Case e.KeyCode
                Case Keys.Enter, Keys.Return
                    Me.llAddBlockID_LinkClicked(sender, Nothing)
            End Select
        End Sub

        Private Sub butAccept_Click(sender As Object, e As EventArgs) Handles butAccept.Click
            Me.DialogResult = DialogResult.OK
        End Sub
    End Class
End Namespace