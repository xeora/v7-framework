Namespace Xeora.Extension.VisualStudio.Tools.Creators
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Control
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
            Me.tbControlID = New System.Windows.Forms.TextBox()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.cbTypes = New System.Windows.Forms.ComboBox()
            Me.butAccept = New System.Windows.Forms.Button()
            Me.gbBind = New System.Windows.Forms.GroupBox()
            Me.tbBind = New System.Windows.Forms.TextBox()
            Me.GroupBox3 = New System.Windows.Forms.GroupBox()
            Me.lwAttributes = New System.Windows.Forms.ListView()
            Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.cmsAttributes = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.DuplicateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
            Me.AddNewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.llAssignControlSpecificValues = New System.Windows.Forms.LinkLabel()
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.gbBind.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.cmsAttributes.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Controls.Add(Me.tbControlID)
            Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(862, 140)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Control ID"
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.875!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label1.Location = New System.Drawing.Point(372, 98)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(473, 25)
            Me.Label1.TabIndex = 3
            Me.Label1.Text = "Control ID should be unique in the Controls XML"
            '
            'tbControlID
            '
            Me.tbControlID.BackColor = System.Drawing.SystemColors.Window
            Me.tbControlID.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbControlID.Location = New System.Drawing.Point(17, 50)
            Me.tbControlID.MaxLength = 64
            Me.tbControlID.Name = "tbControlID"
            Me.tbControlID.Size = New System.Drawing.Size(828, 41)
            Me.tbControlID.TabIndex = 0
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.llAssignControlSpecificValues)
            Me.GroupBox2.Controls.Add(Me.cbTypes)
            Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox2.Location = New System.Drawing.Point(12, 158)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(862, 157)
            Me.GroupBox2.TabIndex = 1
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Type"
            '
            'cbTypes
            '
            Me.cbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbTypes.FormattingEnabled = True
            Me.cbTypes.Items.AddRange(New Object() {"Textbox", "Password", "Checkbox", "Button", "RadioButton", "Textarea", "ImageButton", "LinkButton", "DataList", "ConditionalStatement", "VariableBlock"})
            Me.cbTypes.Location = New System.Drawing.Point(17, 62)
            Me.cbTypes.Name = "cbTypes"
            Me.cbTypes.Size = New System.Drawing.Size(828, 45)
            Me.cbTypes.TabIndex = 0
            '
            'butAccept
            '
            Me.butAccept.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butAccept.Location = New System.Drawing.Point(725, 732)
            Me.butAccept.Name = "butAccept"
            Me.butAccept.Size = New System.Drawing.Size(149, 41)
            Me.butAccept.TabIndex = 4
            Me.butAccept.Text = "OK"
            Me.butAccept.UseVisualStyleBackColor = True
            '
            'gbBind
            '
            Me.gbBind.Controls.Add(Me.tbBind)
            Me.gbBind.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.gbBind.Location = New System.Drawing.Point(12, 321)
            Me.gbBind.Name = "gbBind"
            Me.gbBind.Size = New System.Drawing.Size(862, 140)
            Me.gbBind.TabIndex = 2
            Me.gbBind.TabStop = False
            Me.gbBind.Text = "Bind"
            '
            'tbBind
            '
            Me.tbBind.BackColor = System.Drawing.SystemColors.Window
            Me.tbBind.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbBind.Location = New System.Drawing.Point(17, 62)
            Me.tbBind.MaxLength = 64
            Me.tbBind.Name = "tbBind"
            Me.tbBind.Size = New System.Drawing.Size(828, 41)
            Me.tbBind.TabIndex = 0
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.lwAttributes)
            Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox3.Location = New System.Drawing.Point(12, 467)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(862, 243)
            Me.GroupBox3.TabIndex = 3
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Attributes"
            '
            'lwAttributes
            '
            Me.lwAttributes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
            Me.lwAttributes.ContextMenuStrip = Me.cmsAttributes
            Me.lwAttributes.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.lwAttributes.FullRowSelect = True
            Me.lwAttributes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.lwAttributes.HideSelection = False
            Me.lwAttributes.Location = New System.Drawing.Point(17, 43)
            Me.lwAttributes.MultiSelect = False
            Me.lwAttributes.Name = "lwAttributes"
            Me.lwAttributes.Size = New System.Drawing.Size(828, 183)
            Me.lwAttributes.TabIndex = 0
            Me.lwAttributes.UseCompatibleStateImageBehavior = False
            Me.lwAttributes.View = System.Windows.Forms.View.Details
            '
            'ColumnHeader1
            '
            Me.ColumnHeader1.Text = "Key"
            Me.ColumnHeader1.Width = 150
            '
            'ColumnHeader2
            '
            Me.ColumnHeader2.Text = "Value"
            Me.ColumnHeader2.Width = 620
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
            'llAssignControlSpecificValues
            '
            Me.llAssignControlSpecificValues.AutoSize = True
            Me.llAssignControlSpecificValues.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.llAssignControlSpecificValues.Location = New System.Drawing.Point(461, 110)
            Me.llAssignControlSpecificValues.Name = "llAssignControlSpecificValues"
            Me.llAssignControlSpecificValues.Size = New System.Drawing.Size(384, 31)
            Me.llAssignControlSpecificValues.TabIndex = 1
            Me.llAssignControlSpecificValues.TabStop = True
            Me.llAssignControlSpecificValues.Text = "Assign Control Specific Values"
            '
            'Control
            '
            Me.AcceptButton = Me.butAccept
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(886, 787)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.gbBind)
            Me.Controls.Add(Me.butAccept)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.GroupBox1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "Control"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Create Control"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.gbBind.ResumeLayout(False)
            Me.gbBind.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.cmsAttributes.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As GroupBox
        Friend WithEvents Label1 As Label
        Friend WithEvents tbControlID As TextBox
        Friend WithEvents GroupBox2 As GroupBox
        Friend WithEvents cbTypes As ComboBox
        Friend WithEvents butAccept As Button
        Friend WithEvents gbBind As GroupBox
        Friend WithEvents tbBind As TextBox
        Friend WithEvents GroupBox3 As GroupBox
        Friend WithEvents lwAttributes As ListView
        Friend WithEvents ColumnHeader1 As ColumnHeader
        Friend WithEvents ColumnHeader2 As ColumnHeader
        Friend WithEvents cmsAttributes As ContextMenuStrip
        Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents DuplicateToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
        Friend WithEvents AddNewToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents llAssignControlSpecificValues As LinkLabel
    End Class
End Namespace