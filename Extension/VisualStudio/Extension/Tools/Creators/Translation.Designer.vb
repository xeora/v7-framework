Namespace Xeora.Extension.VisualStudio.Tools.Creators
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Translation
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
            Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.llAddLanguage = New System.Windows.Forms.LinkLabel()
            Me.lwLanguages = New System.Windows.Forms.ListView()
            Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.cmsAttributes = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
            Me.AddNewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.butAccept = New System.Windows.Forms.Button()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.llAddTranslation = New System.Windows.Forms.LinkLabel()
            Me.lwTranslations = New System.Windows.Forms.ListView()
            Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.dgvTranslation = New System.Windows.Forms.DataGridView()
            Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.GroupBox1.SuspendLayout()
            Me.cmsAttributes.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            CType(Me.dgvTranslation, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.llAddLanguage)
            Me.GroupBox1.Controls.Add(Me.lwLanguages)
            Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(862, 276)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Languages"
            '
            'llAddLanguage
            '
            Me.llAddLanguage.AutoSize = True
            Me.llAddLanguage.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.llAddLanguage.Location = New System.Drawing.Point(656, 229)
            Me.llAddLanguage.Name = "llAddLanguage"
            Me.llAddLanguage.Size = New System.Drawing.Size(189, 31)
            Me.llAddLanguage.TabIndex = 2
            Me.llAddLanguage.TabStop = True
            Me.llAddLanguage.Text = "Add Language"
            '
            'lwLanguages
            '
            Me.lwLanguages.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
            Me.lwLanguages.ContextMenuStrip = Me.cmsAttributes
            Me.lwLanguages.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.lwLanguages.FullRowSelect = True
            Me.lwLanguages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.lwLanguages.HideSelection = False
            Me.lwLanguages.Location = New System.Drawing.Point(17, 43)
            Me.lwLanguages.MultiSelect = False
            Me.lwLanguages.Name = "lwLanguages"
            Me.lwLanguages.Size = New System.Drawing.Size(828, 183)
            Me.lwLanguages.TabIndex = 1
            Me.lwLanguages.UseCompatibleStateImageBehavior = False
            Me.lwLanguages.View = System.Windows.Forms.View.Details
            '
            'ColumnHeader1
            '
            Me.ColumnHeader1.Text = ""
            Me.ColumnHeader1.Width = 800
            '
            'cmsAttributes
            '
            Me.cmsAttributes.ImageScalingSize = New System.Drawing.Size(32, 32)
            Me.cmsAttributes.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteToolStripMenuItem, Me.EditToolStripMenuItem, Me.ToolStripMenuItem1, Me.AddNewToolStripMenuItem})
            Me.cmsAttributes.Name = "cmsAttributes"
            Me.cmsAttributes.ShowImageMargin = False
            Me.cmsAttributes.Size = New System.Drawing.Size(164, 124)
            '
            'DeleteToolStripMenuItem
            '
            Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
            Me.DeleteToolStripMenuItem.Size = New System.Drawing.Size(163, 38)
            Me.DeleteToolStripMenuItem.Text = "Delete"
            '
            'EditToolStripMenuItem
            '
            Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
            Me.EditToolStripMenuItem.Size = New System.Drawing.Size(163, 38)
            Me.EditToolStripMenuItem.Text = "Edit"
            '
            'ToolStripMenuItem1
            '
            Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
            Me.ToolStripMenuItem1.Size = New System.Drawing.Size(160, 6)
            '
            'AddNewToolStripMenuItem
            '
            Me.AddNewToolStripMenuItem.Name = "AddNewToolStripMenuItem"
            Me.AddNewToolStripMenuItem.Size = New System.Drawing.Size(163, 38)
            Me.AddNewToolStripMenuItem.Text = "Add New"
            '
            'butAccept
            '
            Me.butAccept.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butAccept.Location = New System.Drawing.Point(725, 960)
            Me.butAccept.Name = "butAccept"
            Me.butAccept.Size = New System.Drawing.Size(149, 41)
            Me.butAccept.TabIndex = 4
            Me.butAccept.Text = "Save"
            Me.butAccept.UseVisualStyleBackColor = True
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.dgvTranslation)
            Me.GroupBox2.Controls.Add(Me.llAddTranslation)
            Me.GroupBox2.Controls.Add(Me.lwTranslations)
            Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox2.Location = New System.Drawing.Point(12, 294)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(862, 650)
            Me.GroupBox2.TabIndex = 5
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Translations"
            '
            'llAddTranslation
            '
            Me.llAddTranslation.AutoSize = True
            Me.llAddTranslation.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.llAddTranslation.Location = New System.Drawing.Point(641, 229)
            Me.llAddTranslation.Name = "llAddTranslation"
            Me.llAddTranslation.Size = New System.Drawing.Size(204, 31)
            Me.llAddTranslation.TabIndex = 2
            Me.llAddTranslation.TabStop = True
            Me.llAddTranslation.Text = "Add Translation"
            '
            'lwTranslations
            '
            Me.lwTranslations.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader2})
            Me.lwTranslations.ContextMenuStrip = Me.cmsAttributes
            Me.lwTranslations.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.lwTranslations.FullRowSelect = True
            Me.lwTranslations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
            Me.lwTranslations.HideSelection = False
            Me.lwTranslations.Location = New System.Drawing.Point(17, 43)
            Me.lwTranslations.MultiSelect = False
            Me.lwTranslations.Name = "lwTranslations"
            Me.lwTranslations.Size = New System.Drawing.Size(828, 183)
            Me.lwTranslations.TabIndex = 1
            Me.lwTranslations.UseCompatibleStateImageBehavior = False
            Me.lwTranslations.View = System.Windows.Forms.View.Details
            '
            'ColumnHeader2
            '
            Me.ColumnHeader2.Text = ""
            Me.ColumnHeader2.Width = 800
            '
            'dgvTranslation
            '
            Me.dgvTranslation.AllowUserToAddRows = False
            Me.dgvTranslation.AllowUserToDeleteRows = False
            DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
            DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
            DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
            DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
            DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
            DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
            Me.dgvTranslation.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
            Me.dgvTranslation.ColumnHeadersHeight = 50
            Me.dgvTranslation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            Me.dgvTranslation.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2})
            Me.dgvTranslation.Location = New System.Drawing.Point(17, 277)
            Me.dgvTranslation.MultiSelect = False
            Me.dgvTranslation.Name = "dgvTranslation"
            Me.dgvTranslation.RowTemplate.Height = 50
            Me.dgvTranslation.Size = New System.Drawing.Size(828, 352)
            Me.dgvTranslation.TabIndex = 4
            '
            'Column1
            '
            Me.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            Me.Column1.HeaderText = "Language"
            Me.Column1.Name = "Column1"
            Me.Column1.ReadOnly = True
            Me.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
            Me.Column1.Width = 200
            '
            'Column2
            '
            Me.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            Me.Column2.HeaderText = "Translation"
            Me.Column2.Name = "Column2"
            Me.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
            Me.Column2.Width = 540
            '
            'Translation
            '
            Me.AcceptButton = Me.butAccept
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(886, 1013)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.butAccept)
            Me.Controls.Add(Me.GroupBox1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "Translation"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Create Language / Add Translation"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.cmsAttributes.ResumeLayout(False)
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            CType(Me.dgvTranslation, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As GroupBox
        Friend WithEvents butAccept As Button
        Friend WithEvents cmsAttributes As ContextMenuStrip
        Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
        Friend WithEvents AddNewToolStripMenuItem As ToolStripMenuItem
        Friend WithEvents lwLanguages As ListView
        Friend WithEvents ColumnHeader1 As ColumnHeader
        Friend WithEvents llAddLanguage As LinkLabel
        Friend WithEvents GroupBox2 As GroupBox
        Friend WithEvents llAddTranslation As LinkLabel
        Friend WithEvents lwTranslations As ListView
        Friend WithEvents ColumnHeader2 As ColumnHeader
        Friend WithEvents dgvTranslation As DataGridView
        Friend WithEvents Column1 As DataGridViewTextBoxColumn
        Friend WithEvents Column2 As DataGridViewTextBoxColumn
    End Class
End Namespace