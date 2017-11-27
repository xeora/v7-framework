Namespace Xeora.Extension.VisualStudio.Tools
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class DownloadProgress
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
            Me.pbStatus = New System.Windows.Forms.ProgressBar()
            Me.lPullingFile = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label1.Location = New System.Drawing.Point(12, 9)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(328, 29)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Please wait while preparing..."
            '
            'pbStatus
            '
            Me.pbStatus.Location = New System.Drawing.Point(17, 48)
            Me.pbStatus.Name = "pbStatus"
            Me.pbStatus.Size = New System.Drawing.Size(684, 37)
            Me.pbStatus.TabIndex = 1
            '
            'lPullingFile
            '
            Me.lPullingFile.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.5!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.lPullingFile.Location = New System.Drawing.Point(17, 97)
            Me.lPullingFile.Name = "lPullingFile"
            Me.lPullingFile.Size = New System.Drawing.Size(679, 48)
            Me.lPullingFile.TabIndex = 2
            Me.lPullingFile.Text = "Xeora.Web.dll is pulling"
            Me.lPullingFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'DownloadProgress
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(716, 154)
            Me.ControlBox = False
            Me.Controls.Add(Me.lPullingFile)
            Me.Controls.Add(Me.pbStatus)
            Me.Controls.Add(Me.Label1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "DownloadProgress"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Xeora Latest Release is pulling..."
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents Label1 As Label
        Friend WithEvents pbStatus As ProgressBar
        Friend WithEvents lPullingFile As Label
    End Class
End Namespace