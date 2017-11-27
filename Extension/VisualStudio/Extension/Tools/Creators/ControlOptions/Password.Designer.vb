Namespace Xeora.Extension.VisualStudio.Tools.Creators.ControlOptions
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Password
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
            Me.cbUpdatesLocal = New System.Windows.Forms.CheckBox()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.lwBlockID = New System.Windows.Forms.ListView()
            Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.tbBlockID = New System.Windows.Forms.TextBox()
            Me.llAddBlockID = New System.Windows.Forms.LinkLabel()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.tbDefaultButtonID = New System.Windows.Forms.TextBox()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label1.Location = New System.Drawing.Point(12, 9)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(76, 36)
            Me.Label1.TabIndex = 10
            Me.Label1.Text = "Text"
            '
            'tbValue
            '
            Me.tbValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbValue.Location = New System.Drawing.Point(18, 48)
            Me.tbValue.MaxLength = 1000
            Me.tbValue.Name = "tbValue"
            Me.tbValue.Size = New System.Drawing.Size(680, 41)
            Me.tbValue.TabIndex = 0
            '
            'butAccept
            '
            Me.butAccept.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.butAccept.Location = New System.Drawing.Point(549, 567)
            Me.butAccept.Name = "butAccept"
            Me.butAccept.Size = New System.Drawing.Size(149, 41)
            Me.butAccept.TabIndex = 6
            Me.butAccept.Text = "OK"
            Me.butAccept.UseVisualStyleBackColor = True
            '
            'cbUpdatesLocal
            '
            Me.cbUpdatesLocal.AutoSize = True
            Me.cbUpdatesLocal.Checked = True
            Me.cbUpdatesLocal.CheckState = System.Windows.Forms.CheckState.Checked
            Me.cbUpdatesLocal.Location = New System.Drawing.Point(18, 216)
            Me.cbUpdatesLocal.Name = "cbUpdatesLocal"
            Me.cbUpdatesLocal.Size = New System.Drawing.Size(404, 29)
            Me.cbUpdatesLocal.TabIndex = 2
            Me.cbUpdatesLocal.Text = "Update the Block Where It Is Locating"
            Me.cbUpdatesLocal.UseVisualStyleBackColor = True
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label2.Location = New System.Drawing.Point(12, 257)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(298, 36)
            Me.Label2.TabIndex = 13
            Me.Label2.Text = "BlockIDs To Update"
            '
            'lwBlockID
            '
            Me.lwBlockID.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
            Me.lwBlockID.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.lwBlockID.FullRowSelect = True
            Me.lwBlockID.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.lwBlockID.HideSelection = False
            Me.lwBlockID.Location = New System.Drawing.Point(18, 296)
            Me.lwBlockID.Name = "lwBlockID"
            Me.lwBlockID.Size = New System.Drawing.Size(680, 195)
            Me.lwBlockID.TabIndex = 3
            Me.lwBlockID.UseCompatibleStateImageBehavior = False
            Me.lwBlockID.View = System.Windows.Forms.View.Details
            '
            'ColumnHeader1
            '
            Me.ColumnHeader1.Text = "BlockID"
            Me.ColumnHeader1.Width = 620
            '
            'tbBlockID
            '
            Me.tbBlockID.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbBlockID.Location = New System.Drawing.Point(18, 497)
            Me.tbBlockID.MaxLength = 1000
            Me.tbBlockID.Name = "tbBlockID"
            Me.tbBlockID.Size = New System.Drawing.Size(609, 41)
            Me.tbBlockID.TabIndex = 4
            '
            'llAddBlockID
            '
            Me.llAddBlockID.AutoSize = True
            Me.llAddBlockID.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.llAddBlockID.Location = New System.Drawing.Point(633, 501)
            Me.llAddBlockID.Name = "llAddBlockID"
            Me.llAddBlockID.Size = New System.Drawing.Size(65, 31)
            Me.llAddBlockID.TabIndex = 5
            Me.llAddBlockID.TabStop = True
            Me.llAddBlockID.Text = "Add"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.Label3.Location = New System.Drawing.Point(12, 102)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(248, 36)
            Me.Label3.TabIndex = 15
            Me.Label3.Text = "Default ButtonID"
            '
            'tbDefaultButtonID
            '
            Me.tbDefaultButtonID.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(162, Byte))
            Me.tbDefaultButtonID.Location = New System.Drawing.Point(18, 141)
            Me.tbDefaultButtonID.MaxLength = 1000
            Me.tbDefaultButtonID.Name = "tbDefaultButtonID"
            Me.tbDefaultButtonID.Size = New System.Drawing.Size(680, 41)
            Me.tbDefaultButtonID.TabIndex = 1
            '
            'Password
            '
            Me.AcceptButton = Me.butAccept
            Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(714, 623)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.tbDefaultButtonID)
            Me.Controls.Add(Me.llAddBlockID)
            Me.Controls.Add(Me.tbBlockID)
            Me.Controls.Add(Me.lwBlockID)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.cbUpdatesLocal)
            Me.Controls.Add(Me.butAccept)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.tbValue)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "Password"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Password Specific Values"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents Label1 As Label
        Friend WithEvents tbValue As System.Windows.Forms.TextBox
        Friend WithEvents butAccept As System.Windows.Forms.Button
        Friend WithEvents cbUpdatesLocal As System.Windows.Forms.CheckBox
        Friend WithEvents Label2 As Label
        Friend WithEvents lwBlockID As ListView
        Friend WithEvents ColumnHeader1 As ColumnHeader
        Friend WithEvents tbBlockID As System.Windows.Forms.TextBox
        Friend WithEvents llAddBlockID As LinkLabel
        Friend WithEvents Label3 As Label
        Friend WithEvents tbDefaultButtonID As System.Windows.Forms.TextBox
    End Class
End Namespace