Namespace Xeora.Extension.VisualStudio.Tools
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CompilerForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
            Me.cbShowPassword = New System.Windows.Forms.CheckBox()
            Me.ProgressBar = New System.Windows.Forms.ProgressBar()
            Me.butCompile = New System.Windows.Forms.Button()
            Me.dgvDomains = New System.Windows.Forms.DataGridView()
            Me.Selected = New System.Windows.Forms.DataGridViewCheckBoxColumn()
            Me.Domain = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.cbSecure = New System.Windows.Forms.DataGridViewCheckBoxColumn()
            Me.PasswordText = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.PasswordHidden = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.lCurrentProcess = New System.Windows.Forms.Label()
            Me.cbCheckAll = New System.Windows.Forms.CheckBox()
            CType(Me.dgvDomains, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'cbShowPassword
            '
            Me.cbShowPassword.AutoSize = True
            Me.cbShowPassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.cbShowPassword.Location = New System.Drawing.Point(23, 381)
            Me.cbShowPassword.Margin = New System.Windows.Forms.Padding(6)
            Me.cbShowPassword.Name = "cbShowPassword"
            Me.cbShowPassword.Size = New System.Drawing.Size(219, 33)
            Me.cbShowPassword.TabIndex = 6
            Me.cbShowPassword.Text = "Show Password"
            Me.cbShowPassword.UseVisualStyleBackColor = True
            Me.cbShowPassword.Visible = False
            '
            'ProgressBar
            '
            Me.ProgressBar.Location = New System.Drawing.Point(77, 380)
            Me.ProgressBar.Margin = New System.Windows.Forms.Padding(6)
            Me.ProgressBar.Name = "ProgressBar"
            Me.ProgressBar.Size = New System.Drawing.Size(715, 35)
            Me.ProgressBar.TabIndex = 7
            '
            'butCompile
            '
            Me.butCompile.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butCompile.Location = New System.Drawing.Point(804, 374)
            Me.butCompile.Margin = New System.Windows.Forms.Padding(6)
            Me.butCompile.Name = "butCompile"
            Me.butCompile.Size = New System.Drawing.Size(150, 44)
            Me.butCompile.TabIndex = 3
            Me.butCompile.Text = "Compile"
            Me.butCompile.UseVisualStyleBackColor = True
            '
            'dgvDomains
            '
            Me.dgvDomains.AllowUserToAddRows = False
            Me.dgvDomains.AllowUserToDeleteRows = False
            Me.dgvDomains.AllowUserToResizeRows = False
            Me.dgvDomains.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
            DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
            DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
            DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
            DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
            DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
            DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
            Me.dgvDomains.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle4
            Me.dgvDomains.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvDomains.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Selected, Me.Domain, Me.cbSecure, Me.PasswordText, Me.PasswordHidden})
            Me.dgvDomains.Location = New System.Drawing.Point(23, 22)
            Me.dgvDomains.MultiSelect = False
            Me.dgvDomains.Name = "dgvDomains"
            DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
            DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control
            DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.875!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText
            DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
            DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
            DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
            Me.dgvDomains.RowHeadersDefaultCellStyle = DataGridViewCellStyle5
            DataGridViewCellStyle6.BackColor = System.Drawing.Color.White
            DataGridViewCellStyle6.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.875!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            DataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black
            DataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White
            DataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black
            Me.dgvDomains.RowsDefaultCellStyle = DataGridViewCellStyle6
            Me.dgvDomains.RowTemplate.Height = 50
            Me.dgvDomains.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.dgvDomains.Size = New System.Drawing.Size(931, 344)
            Me.dgvDomains.TabIndex = 0
            '
            'Selected
            '
            Me.Selected.FalseValue = "0"
            Me.Selected.HeaderText = ""
            Me.Selected.Name = "Selected"
            Me.Selected.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
            Me.Selected.TrueValue = "1"
            Me.Selected.Width = 50
            '
            'Domain
            '
            Me.Domain.HeaderText = "Domain ID"
            Me.Domain.Name = "Domain"
            Me.Domain.ReadOnly = True
            Me.Domain.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
            Me.Domain.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
            Me.Domain.Width = 250
            '
            'cbSecure
            '
            Me.cbSecure.FalseValue = "0"
            Me.cbSecure.HeaderText = "Secure"
            Me.cbSecure.Name = "cbSecure"
            Me.cbSecure.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
            Me.cbSecure.TrueValue = "1"
            Me.cbSecure.Width = 120
            '
            'PasswordText
            '
            Me.PasswordText.HeaderText = "Password"
            Me.PasswordText.MaxInputLength = 50
            Me.PasswordText.Name = "PasswordText"
            Me.PasswordText.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
            Me.PasswordText.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
            Me.PasswordText.Width = 400
            '
            'PasswordHidden
            '
            Me.PasswordHidden.HeaderText = ""
            Me.PasswordHidden.Name = "PasswordHidden"
            Me.PasswordHidden.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
            Me.PasswordHidden.Visible = False
            '
            'lCurrentProcess
            '
            Me.lCurrentProcess.AutoSize = True
            Me.lCurrentProcess.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.lCurrentProcess.Location = New System.Drawing.Point(17, 378)
            Me.lCurrentProcess.Name = "lCurrentProcess"
            Me.lCurrentProcess.Size = New System.Drawing.Size(33, 36)
            Me.lCurrentProcess.TabIndex = 11
            Me.lCurrentProcess.Text = "0"
            '
            'cbCheckAll
            '
            Me.cbCheckAll.AutoSize = True
            Me.cbCheckAll.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.cbCheckAll.Location = New System.Drawing.Point(75, 33)
            Me.cbCheckAll.Margin = New System.Windows.Forms.Padding(6)
            Me.cbCheckAll.Name = "cbCheckAll"
            Me.cbCheckAll.Size = New System.Drawing.Size(28, 27)
            Me.cbCheckAll.TabIndex = 1
            Me.cbCheckAll.UseVisualStyleBackColor = True
            '
            'CompilerForm
            '
            Me.AcceptButton = Me.butCompile
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(978, 436)
            Me.Controls.Add(Me.cbCheckAll)
            Me.Controls.Add(Me.lCurrentProcess)
            Me.Controls.Add(Me.dgvDomains)
            Me.Controls.Add(Me.butCompile)
            Me.Controls.Add(Me.ProgressBar)
            Me.Controls.Add(Me.cbShowPassword)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.Margin = New System.Windows.Forms.Padding(6)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "CompilerForm"
            Me.ShowIcon = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "XeoraCube Domain Compiler"
            CType(Me.dgvDomains, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents cbShowPassword As System.Windows.Forms.CheckBox
        Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
        Friend WithEvents butCompile As System.Windows.Forms.Button
        Friend WithEvents dgvDomains As DataGridView
        Friend WithEvents Selected As DataGridViewCheckBoxColumn
        Friend WithEvents Domain As DataGridViewTextBoxColumn
        Friend WithEvents cbSecure As DataGridViewCheckBoxColumn
        Friend WithEvents PasswordText As DataGridViewTextBoxColumn
        Friend WithEvents PasswordHidden As DataGridViewTextBoxColumn
        Friend WithEvents lCurrentProcess As Label
        Friend WithEvents cbCheckAll As CheckBox
    End Class
End Namespace