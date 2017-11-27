Namespace Xeora.Extension.VisualStudio.Tools.PrepareProject
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class ProjectSettings
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
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.tbDomainID = New System.Windows.Forms.TextBox()
            Me.GroupBox2 = New System.Windows.Forms.GroupBox()
            Me.tbLanguageID = New System.Windows.Forms.MaskedTextBox()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.tbLanguageName = New System.Windows.Forms.TextBox()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.GroupBox3 = New System.Windows.Forms.GroupBox()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.tbVirtualPath = New System.Windows.Forms.TextBox()
            Me.GroupBox4 = New System.Windows.Forms.GroupBox()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.tbVariablePoolServicePort = New System.Windows.Forms.TextBox()
            Me.GroupBox5 = New System.Windows.Forms.GroupBox()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.tbScheduledTasksServicePort = New System.Windows.Forms.TextBox()
            Me.cbDebug = New System.Windows.Forms.CheckBox()
            Me.GroupBox6 = New System.Windows.Forms.GroupBox()
            Me.comboCaching = New System.Windows.Forms.ComboBox()
            Me.butAccept = New System.Windows.Forms.Button()
            Me.cb64BitRelease = New System.Windows.Forms.CheckBox()
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.GroupBox4.SuspendLayout()
            Me.GroupBox5.SuspendLayout()
            Me.GroupBox6.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Controls.Add(Me.tbDomainID)
            Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(856, 142)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Domain ID"
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.875!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label1.Location = New System.Drawing.Point(421, 98)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(418, 25)
            Me.Label1.TabIndex = 1
            Me.Label1.Text = "Domain ID should be unique in the project!"
            '
            'tbDomainID
            '
            Me.tbDomainID.BackColor = System.Drawing.SystemColors.Window
            Me.tbDomainID.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbDomainID.Location = New System.Drawing.Point(17, 50)
            Me.tbDomainID.MaxLength = 64
            Me.tbDomainID.Name = "tbDomainID"
            Me.tbDomainID.Size = New System.Drawing.Size(828, 41)
            Me.tbDomainID.TabIndex = 0
            Me.tbDomainID.Text = "Main"
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.tbLanguageID)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Controls.Add(Me.Label3)
            Me.GroupBox2.Controls.Add(Me.tbLanguageName)
            Me.GroupBox2.Controls.Add(Me.Label2)
            Me.GroupBox2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox2.Location = New System.Drawing.Point(12, 160)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(856, 156)
            Me.GroupBox2.TabIndex = 1
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Language"
            '
            'tbLanguageID
            '
            Me.tbLanguageID.Culture = New System.Globalization.CultureInfo("en-US")
            Me.tbLanguageID.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbLanguageID.Location = New System.Drawing.Point(129, 48)
            Me.tbLanguageID.Mask = "&&-&&"
            Me.tbLanguageID.Name = "tbLanguageID"
            Me.tbLanguageID.Size = New System.Drawing.Size(716, 41)
            Me.tbLanguageID.TabIndex = 0
            Me.tbLanguageID.Text = "enUS"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label4.Location = New System.Drawing.Point(11, 100)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(96, 36)
            Me.Label4.TabIndex = 4
            Me.Label4.Text = "Name"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label3.Location = New System.Drawing.Point(11, 53)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(90, 36)
            Me.Label3.TabIndex = 3
            Me.Label3.Text = "Code"
            '
            'tbLanguageName
            '
            Me.tbLanguageName.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbLanguageName.Location = New System.Drawing.Point(129, 97)
            Me.tbLanguageName.MaxLength = 20
            Me.tbLanguageName.Name = "tbLanguageName"
            Me.tbLanguageName.Size = New System.Drawing.Size(716, 41)
            Me.tbLanguageName.TabIndex = 1
            Me.tbLanguageName.Text = "English"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.875!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label2.Location = New System.Drawing.Point(432, 94)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(0, 25)
            Me.Label2.TabIndex = 1
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.Label5)
            Me.GroupBox3.Controls.Add(Me.tbVirtualPath)
            Me.GroupBox3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox3.Location = New System.Drawing.Point(12, 440)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(856, 142)
            Me.GroupBox3.TabIndex = 3
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Virtual Path"
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.875!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label5.Location = New System.Drawing.Point(155, 98)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(684, 25)
            Me.Label5.TabIndex = 1
            Me.Label5.Text = "If you run the project under a virtual directory, it is important to specify!"
            '
            'tbVirtualPath
            '
            Me.tbVirtualPath.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbVirtualPath.Location = New System.Drawing.Point(17, 50)
            Me.tbVirtualPath.MaxLength = 128
            Me.tbVirtualPath.Name = "tbVirtualPath"
            Me.tbVirtualPath.Size = New System.Drawing.Size(828, 41)
            Me.tbVirtualPath.TabIndex = 0
            Me.tbVirtualPath.Text = "/"
            '
            'GroupBox4
            '
            Me.GroupBox4.Controls.Add(Me.Label6)
            Me.GroupBox4.Controls.Add(Me.tbVariablePoolServicePort)
            Me.GroupBox4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox4.Location = New System.Drawing.Point(12, 588)
            Me.GroupBox4.Name = "GroupBox4"
            Me.GroupBox4.Size = New System.Drawing.Size(856, 142)
            Me.GroupBox4.TabIndex = 4
            Me.GroupBox4.TabStop = False
            Me.GroupBox4.Text = "Variable Pool Service Port"
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.875!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label6.Location = New System.Drawing.Point(227, 98)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(612, 25)
            Me.Label6.TabIndex = 1
            Me.Label6.Text = "Port should be unique for each Xeora Application in the server!"
            '
            'tbVariablePoolServicePort
            '
            Me.tbVariablePoolServicePort.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbVariablePoolServicePort.Location = New System.Drawing.Point(17, 50)
            Me.tbVariablePoolServicePort.MaxLength = 5
            Me.tbVariablePoolServicePort.Name = "tbVariablePoolServicePort"
            Me.tbVariablePoolServicePort.Size = New System.Drawing.Size(828, 41)
            Me.tbVariablePoolServicePort.TabIndex = 0
            Me.tbVariablePoolServicePort.Text = "12010"
            '
            'GroupBox5
            '
            Me.GroupBox5.Controls.Add(Me.Label7)
            Me.GroupBox5.Controls.Add(Me.tbScheduledTasksServicePort)
            Me.GroupBox5.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox5.Location = New System.Drawing.Point(12, 736)
            Me.GroupBox5.Name = "GroupBox5"
            Me.GroupBox5.Size = New System.Drawing.Size(856, 142)
            Me.GroupBox5.TabIndex = 5
            Me.GroupBox5.TabStop = False
            Me.GroupBox5.Text = "Scheduled Tasks Service Port"
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.875!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label7.Location = New System.Drawing.Point(32, 98)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(807, 25)
            Me.Label7.TabIndex = 1
            Me.Label7.Text = "Port should be unique for each Xeora Application in the server! ""0"" means disable" &
    "d."
            '
            'tbScheduledTasksServicePort
            '
            Me.tbScheduledTasksServicePort.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbScheduledTasksServicePort.Location = New System.Drawing.Point(17, 50)
            Me.tbScheduledTasksServicePort.MaxLength = 5
            Me.tbScheduledTasksServicePort.Name = "tbScheduledTasksServicePort"
            Me.tbScheduledTasksServicePort.Size = New System.Drawing.Size(828, 41)
            Me.tbScheduledTasksServicePort.TabIndex = 0
            Me.tbScheduledTasksServicePort.Text = "0"
            '
            'cbDebug
            '
            Me.cbDebug.AutoSize = True
            Me.cbDebug.Checked = True
            Me.cbDebug.CheckState = System.Windows.Forms.CheckState.Checked
            Me.cbDebug.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.cbDebug.Location = New System.Drawing.Point(295, 903)
            Me.cbDebug.Name = "cbDebug"
            Me.cbDebug.Size = New System.Drawing.Size(185, 33)
            Me.cbDebug.TabIndex = 7
            Me.cbDebug.Text = "Debug Mode"
            Me.cbDebug.UseVisualStyleBackColor = True
            '
            'GroupBox6
            '
            Me.GroupBox6.Controls.Add(Me.comboCaching)
            Me.GroupBox6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.GroupBox6.Location = New System.Drawing.Point(12, 322)
            Me.GroupBox6.Name = "GroupBox6"
            Me.GroupBox6.Size = New System.Drawing.Size(856, 112)
            Me.GroupBox6.TabIndex = 2
            Me.GroupBox6.TabStop = False
            Me.GroupBox6.Text = "Page Caching Type"
            '
            'comboCaching
            '
            Me.comboCaching.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.comboCaching.FormattingEnabled = True
            Me.comboCaching.Items.AddRange(New Object() {"AllContent", "AllContentCookiless", "TextsOnly", "TextsOnlyCookiless", "NoCache", "NoCacheCookiless"})
            Me.comboCaching.Location = New System.Drawing.Point(17, 51)
            Me.comboCaching.Name = "comboCaching"
            Me.comboCaching.Size = New System.Drawing.Size(828, 45)
            Me.comboCaching.TabIndex = 0
            '
            'butAccept
            '
            Me.butAccept.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butAccept.Location = New System.Drawing.Point(719, 898)
            Me.butAccept.Name = "butAccept"
            Me.butAccept.Size = New System.Drawing.Size(149, 41)
            Me.butAccept.TabIndex = 8
            Me.butAccept.Text = "OK"
            Me.butAccept.UseVisualStyleBackColor = True
            '
            'cb64BitRelease
            '
            Me.cb64BitRelease.AutoSize = True
            Me.cb64BitRelease.Checked = True
            Me.cb64BitRelease.CheckState = System.Windows.Forms.CheckState.Checked
            Me.cb64BitRelease.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.cb64BitRelease.Location = New System.Drawing.Point(29, 903)
            Me.cb64BitRelease.Name = "cb64BitRelease"
            Me.cb64BitRelease.Size = New System.Drawing.Size(250, 33)
            Me.cb64BitRelease.TabIndex = 6
            Me.cb64BitRelease.Text = "Use 64-bit Release"
            Me.cb64BitRelease.UseVisualStyleBackColor = True
            '
            'ProjectSettings
            '
            Me.AcceptButton = Me.butAccept
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(886, 955)
            Me.Controls.Add(Me.cb64BitRelease)
            Me.Controls.Add(Me.butAccept)
            Me.Controls.Add(Me.GroupBox6)
            Me.Controls.Add(Me.cbDebug)
            Me.Controls.Add(Me.GroupBox5)
            Me.Controls.Add(Me.GroupBox4)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.GroupBox1)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ProjectSettings"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Xeora Project Settings"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox3.PerformLayout()
            Me.GroupBox4.ResumeLayout(False)
            Me.GroupBox4.PerformLayout()
            Me.GroupBox5.ResumeLayout(False)
            Me.GroupBox5.PerformLayout()
            Me.GroupBox6.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents GroupBox1 As GroupBox
        Friend WithEvents Label1 As Label
        Friend WithEvents tbDomainID As TextBox
        Friend WithEvents GroupBox2 As GroupBox
        Friend WithEvents Label4 As Label
        Friend WithEvents Label3 As Label
        Friend WithEvents tbLanguageName As TextBox
        Friend WithEvents Label2 As Label
        Friend WithEvents GroupBox3 As GroupBox
        Friend WithEvents Label5 As Label
        Friend WithEvents tbVirtualPath As TextBox
        Friend WithEvents GroupBox4 As GroupBox
        Friend WithEvents Label6 As Label
        Friend WithEvents tbVariablePoolServicePort As TextBox
        Friend WithEvents GroupBox5 As GroupBox
        Friend WithEvents Label7 As Label
        Friend WithEvents tbScheduledTasksServicePort As TextBox
        Friend WithEvents cbDebug As CheckBox
        Friend WithEvents GroupBox6 As GroupBox
        Friend WithEvents comboCaching As ComboBox
        Friend WithEvents butAccept As Button
        Friend WithEvents tbLanguageID As MaskedTextBox
        Friend WithEvents cb64BitRelease As CheckBox
    End Class
End Namespace