Namespace Xeora.Extension.VisualStudio.Tools.Creators.ControlOptions
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class TextArea
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.tbValue = New System.Windows.Forms.TextBox()
            Me.butAccept = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label1.Location = New System.Drawing.Point(12, 9)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(126, 36)
            Me.Label1.TabIndex = 10
            Me.Label1.Text = "Content"
            '
            'tbValue
            '
            Me.tbValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbValue.Location = New System.Drawing.Point(18, 48)
            Me.tbValue.MaxLength = 1000
            Me.tbValue.Multiline = True
            Me.tbValue.Name = "tbValue"
            Me.tbValue.Size = New System.Drawing.Size(680, 463)
            Me.tbValue.TabIndex = 0
            '
            'butAccept
            '
            Me.butAccept.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butAccept.Location = New System.Drawing.Point(549, 517)
            Me.butAccept.Name = "butAccept"
            Me.butAccept.Size = New System.Drawing.Size(149, 41)
            Me.butAccept.TabIndex = 1
            Me.butAccept.Text = "OK"
            Me.butAccept.UseVisualStyleBackColor = True
            '
            'TextArea
            '
            Me.AcceptButton = Me.butAccept
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(714, 571)
            Me.Controls.Add(Me.butAccept)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.tbValue)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "TextArea"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "TextArea Specific Values"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents Label1 As Label
        Friend WithEvents tbValue As System.Windows.Forms.TextBox
        Friend WithEvents butAccept As System.Windows.Forms.Button
    End Class
End Namespace