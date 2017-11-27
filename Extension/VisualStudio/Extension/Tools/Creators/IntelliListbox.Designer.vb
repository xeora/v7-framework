<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IntelliListbox
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lwIntelliItems = New System.Windows.Forms.ListView()
        Me.CHIcon = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.CHText = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SuspendLayout()
        '
        'lwIntelliItems
        '
        Me.lwIntelliItems.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.CHIcon, Me.CHText})
        Me.lwIntelliItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lwIntelliItems.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.125!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lwIntelliItems.FullRowSelect = True
        Me.lwIntelliItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lwIntelliItems.HideSelection = False
        Me.lwIntelliItems.LabelWrap = False
        Me.lwIntelliItems.Location = New System.Drawing.Point(0, 0)
        Me.lwIntelliItems.MultiSelect = False
        Me.lwIntelliItems.Name = "lwIntelliItems"
        Me.lwIntelliItems.ShowGroups = False
        Me.lwIntelliItems.Size = New System.Drawing.Size(540, 300)
        Me.lwIntelliItems.TabIndex = 0
        Me.lwIntelliItems.UseCompatibleStateImageBehavior = False
        Me.lwIntelliItems.View = System.Windows.Forms.View.Details
        '
        'CHIcon
        '
        Me.CHIcon.Text = ""
        Me.CHIcon.Width = 30
        '
        'CHText
        '
        Me.CHText.Text = ""
        Me.CHText.Width = 440
        '
        'IntelliListbox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lwIntelliItems)
        Me.Name = "IntelliListbox"
        Me.Size = New System.Drawing.Size(540, 300)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lwIntelliItems As ListView
    Friend WithEvents CHIcon As ColumnHeader
    Friend WithEvents CHText As ColumnHeader
End Class
