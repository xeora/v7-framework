Namespace Xeora.Extension.VisualStudio.Tools.Creators
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Executable
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
            Me.components = New System.ComponentModel.Container()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.tbExecutableID = New System.Windows.Forms.TextBox()
            Me.butAccept = New System.Windows.Forms.Button()
            Me.cmsAttributes = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.DuplicateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
            Me.AddNewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.butBrowse = New System.Windows.Forms.Button()
            Me.tbProjectLocation = New System.Windows.Forms.TextBox()
            Me.GroupBox3 = New System.Windows.Forms.GroupBox()
            Me.rbCSharp = New System.Windows.Forms.RadioButton()
            Me.rbVisualBasic = New System.Windows.Forms.RadioButton()
            Me.GroupBox1.SuspendLayout()
            Me.cmsAttributes.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Controls.Add(Me.tbExecutableID)
            Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(862, 140)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Executable ID"
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.875!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label1.Location = New System.Drawing.Point(380, 98)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(459, 25)
            Me.Label1.TabIndex = 3
            Me.Label1.Text = "Executable ID should be unique in the Solution"
            '
            'tbExecutableID
            '
            Me.tbExecutableID.BackColor = System.Drawing.SystemColors.Window
            Me.tbExecutableID.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbExecutableID.Location = New System.Drawing.Point(17, 50)
            Me.tbExecutableID.MaxLength = 64
            Me.tbExecutableID.Name = "tbExecutableID"
            Me.tbExecutableID.Size = New System.Drawing.Size(828, 41)
            Me.tbExecutableID.TabIndex = 0
            '
            'butAccept
            '
            Me.butAccept.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butAccept.Location = New System.Drawing.Point(725, 414)
            Me.butAccept.Name = "butAccept"
            Me.butAccept.Size = New System.Drawing.Size(149, 41)
            Me.butAccept.TabIndex = 4
            Me.butAccept.Text = "OK"
            Me.butAccept.UseVisualStyleBackColor = True
            '
            'cmsAttributes
            '
            Me.cmsAttributes.ImageScalingSize = New System.Drawing.Size(32, 32)
            Me.cmsAttributes.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteToolStripMenuItem, Me.EditToolStripMenuItem, Me.DuplicateToolStripMenuItem, Me.ToolStripMenuItem1, Me.AddNewToolStripMenuItem})
            Me.cmsAttributes.Name = "cmsAttributes"
            Me.cmsAttributes.ShowImageMargin = False
            Me.cmsAttributes.Size = New System.Drawing.Size(167, 162)
            '
            'DeleteToolStripMenuItem
            '
            Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
            Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(166, 38)
            Me.DeleteToolStripMenuItem.Text = "Delete"
            '
            'EditToolStripMenuItem
            '
            Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
            Me.EditToolStripMenuItem.Size = New System.Drawing.Size(166, 38)
            Me.EditToolStripMenuItem.Text = "Edit"
            '
            'DuplicateToolStripMenuItem
            '
            Me.DuplicateToolStripMenuItem.Name = "DuplicateToolStripMenuItem"
            Me.DuplicateToolStripMenuItem.Size = New System.Drawing.Size(166, 38)
            Me.DuplicateToolStripMenuItem.Text = "Duplicate"
            '
            'ToolStripMenuItem1
            '
            Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
            Me.ToolStripMenuItem1.Size = New System.Drawing.Size(163, 6)
            '
            'AddNewToolStripMenuItem
            '
            Me.AddNewToolStripMenuItem.Name = "AddNewToolStripMenuItem"
            Me.AddNewToolStripMenuItem.Size = New System.Drawing.Size(166, 38)
            Me.AddNewToolStripMenuItem.Text = "Add New"
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.butBrowse)
            Me.GroupBox2.Controls.Add(Me.tbProjectLocation)
            Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox2.Location = New System.Drawing.Point(12, 158)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(862, 116)
            Me.GroupBox2.TabIndex = 1
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Project Location"
            '
            'butBrowse
            '
            Me.butBrowse.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butBrowse.Location = New System.Drawing.Point(685, 50)
            Me.butBrowse.Name = "butBrowse"
            Me.butBrowse.Size = New System.Drawing.Size(154, 41)
            Me.butBrowse.TabIndex = 1
            Me.butBrowse.Text = "Browse"
            Me.butBrowse.UseVisualStyleBackColor = True
            '
            'tbProjectLocation
            '
            Me.tbProjectLocation.BackColor = System.Drawing.SystemColors.Window
            Me.tbProjectLocation.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbProjectLocation.Location = New System.Drawing.Point(17, 50)
            Me.tbProjectLocation.MaxLength = 64
            Me.tbProjectLocation.Name = "tbProjectLocation"
            Me.tbProjectLocation.ReadOnly = True
            Me.tbProjectLocation.Size = New System.Drawing.Size(662, 41)
            Me.tbProjectLocation.TabIndex = 0
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.rbVisualBasic)
            Me.GroupBox3.Controls.Add(Me.rbCSharp)
            Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox3.Location = New System.Drawing.Point(12, 280)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(862, 116)
            Me.GroupBox3.TabIndex = 2
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Code Language"
            '
            'rbCSharp
            '
            Me.rbCSharp.AutoSize = True
            Me.rbCSharp.Checked = True
            Me.rbCSharp.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.rbCSharp.Location = New System.Drawing.Point(17, 60)
            Me.rbCSharp.Name = "rbCSharp"
            Me.rbCSharp.Size = New System.Drawing.Size(137, 35)
            Me.rbCSharp.TabIndex = 0
            Me.rbCSharp.TabStop = True
            Me.rbCSharp.Tag = "Language"
            Me.rbCSharp.Text = "CSharp"
            Me.rbCSharp.UseVisualStyleBackColor = True
            '
            'rbVisualBasic
            '
            Me.rbVisualBasic.AutoSize = True
            Me.rbVisualBasic.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.rbVisualBasic.Location = New System.Drawing.Point(171, 60)
            Me.rbVisualBasic.Name = "rbVisualBasic"
            Me.rbVisualBasic.Size = New System.Drawing.Size(193, 35)
            Me.rbVisualBasic.TabIndex = 1
            Me.rbVisualBasic.Tag = "Language"
            Me.rbVisualBasic.Text = "Visual Basic"
            Me.rbVisualBasic.UseVisualStyleBackColor = True
            '
            'Executable
            '
            Me.AcceptButton = Me.butAccept
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(886, 469)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.butAccept)
            Me.Controls.Add(Me.GroupBox1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "Executable"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Create Executable"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.cmsAttributes.ResumeLayout(False)
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox3.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As GroupBox
        Friend WithEvents Label1 As Label
        Friend WithEvents tbExecutableID As TextBox
        Friend WithEvents butAccept As Button
        Friend WithEvents cmsAttributes As ContextMenuStrip
        Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents DuplicateToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
        Friend WithEvents AddNewToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents GroupBox2 As GroupBox
        Friend WithEvents butBrowse As Button
        Friend WithEvents tbProjectLocation As TextBox
        Friend WithEvents GroupBox3 As GroupBox
        Friend WithEvents rbVisualBasic As RadioButton
        Friend WithEvents rbCSharp As RadioButton
    End Class
End Namespace