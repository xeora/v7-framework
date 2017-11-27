Namespace Xeora.Extension.VisualStudio.Tools.Creators
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class TranslationSettings
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
            Me.butAccept = New System.Windows.Forms.Button()
            Me.tbTranslationID = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label1.Location = New System.Drawing.Point(12, 9)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(46, 36)
            Me.Label1.TabIndex = 8
            Me.Label1.Text = "ID"
            '
            'butAccept
            '
            Me.butAccept.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butAccept.Location = New System.Drawing.Point(549, 110)
            Me.butAccept.Name = "butAccept"
            Me.butAccept.Size = New System.Drawing.Size(149, 41)
            Me.butAccept.TabIndex = 3
            Me.butAccept.Text = "OK"
            Me.butAccept.UseVisualStyleBackColor = True
            '
            'tbTranslationID
            '
            Me.tbTranslationID.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbTranslationID.Location = New System.Drawing.Point(18, 48)
            Me.tbTranslationID.MaxLength = 255
            Me.tbTranslationID.Name = "tbTranslationID"
            Me.tbTranslationID.Size = New System.Drawing.Size(680, 41)
            Me.tbTranslationID.TabIndex = 2
            '
            'TranslationSettings
            '
            Me.AcceptButton = Me.butAccept
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(710, 165)
            Me.Controls.Add(Me.tbTranslationID)
            Me.Controls.Add(Me.butAccept)
            Me.Controls.Add(Me.Label1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "TranslationSettings"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Translation ID Assignment"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents Label1 As Label
        Friend WithEvents butAccept As Button
        Friend WithEvents tbTranslationID As TextBox
    End Class
End Namespace