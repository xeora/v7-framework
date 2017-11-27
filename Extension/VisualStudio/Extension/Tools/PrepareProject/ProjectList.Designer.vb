Namespace Xeora.Extension.VisualStudio.Tools.PrepareProject
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ProjectList
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
            Me.butApply = New System.Windows.Forms.Button()
            Me.lProjectName = New System.Windows.Forms.Label()
            Me.pChoice = New System.Windows.Forms.Panel()
            Me.pChoice.SuspendLayout()
            Me.SuspendLayout()
            '
            'butApply
            '
            Me.butApply.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butApply.Location = New System.Drawing.Point(598, 10)
            Me.butApply.Name = "butApply"
            Me.butApply.Size = New System.Drawing.Size(60, 60)
            Me.butApply.TabIndex = 0
            Me.butApply.Text = "»"
            Me.butApply.UseVisualStyleBackColor = True
            '
            'lProjectName
            '
            Me.lProjectName.BackColor = System.Drawing.SystemColors.ButtonHighlight
            Me.lProjectName.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.lProjectName.Location = New System.Drawing.Point(10, 10)
            Me.lProjectName.Name = "lProjectName"
            Me.lProjectName.Size = New System.Drawing.Size(582, 60)
            Me.lProjectName.TabIndex = 1
            Me.lProjectName.Text = "ProjectName"
            Me.lProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'pChoice
            '
            Me.pChoice.Controls.Add(Me.lProjectName)
            Me.pChoice.Controls.Add(Me.butApply)
            Me.pChoice.Location = New System.Drawing.Point(12, 12)
            Me.pChoice.Name = "pChoice"
            Me.pChoice.Size = New System.Drawing.Size(668, 80)
            Me.pChoice.TabIndex = 0
            '
            'ProjectList
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(696, 107)
            Me.Controls.Add(Me.pChoice)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ProjectList"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Choose which project you want to apply..."
            Me.pChoice.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents butApply As Button
        Friend WithEvents lProjectName As Label
        Friend WithEvents pChoice As Panel
    End Class
End Namespace