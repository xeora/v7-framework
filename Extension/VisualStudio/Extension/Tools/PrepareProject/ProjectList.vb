Namespace Xeora.Extension.VisualStudio.Tools.PrepareProject
    Public Class ProjectList
        Private _ProjectList As Generic.Dictionary(Of String, String)
        Private _SelectedProject As Generic.KeyValuePair(Of String, String)

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me._ProjectList = New Generic.Dictionary(Of String, String)()
        End Sub

        Public ReadOnly Property ProjectList As Generic.Dictionary(Of String, String)
            Get
                Return Me._ProjectList
            End Get
        End Property

        Public Shadows Sub ShowDialog(ByVal owner As IWin32Window)
            If Me._ProjectList.Count = 0 Then Exit Sub

            Dim Enumerator As Generic.IEnumerator(Of Generic.KeyValuePair(Of String, String)) =
            Me._ProjectList.GetEnumerator()

            If Me._ProjectList.Count = 1 Then
                Enumerator.MoveNext()

                Me._SelectedProject = New Generic.KeyValuePair(Of String, String)(Enumerator.Current.Key, Enumerator.Current.Value)

                Me.DialogResult = DialogResult.OK
            Else
                Me.pChoice.Visible = False

                Do While Enumerator.MoveNext()
                    Dim CopyPanel As New Panel()
                    CopyPanel.Location = Me.pChoice.Location
                    CopyPanel.Size = Me.pChoice.Size

                    Dim butApply As New Button()
                    butApply.Parent = CopyPanel
                    butApply.Font = Me.butApply.Font
                    butApply.Text = Me.butApply.Text
                    butApply.Location = Me.butApply.Location
                    butApply.Size = Me.butApply.Size
                    butApply.Tag = Enumerator.Current.Key
                    AddHandler butApply.Click, AddressOf Me.SelectionMade

                    Dim lProject As New Label()
                    lProject.Parent = CopyPanel
                    lProject.BackColor = Me.lProjectName.BackColor
                    lProject.Font = Me.lProjectName.Font
                    lProject.Text = Enumerator.Current.Key
                    lProject.TextAlign = Me.lProjectName.TextAlign
                    lProject.Location = Me.lProjectName.Location
                    lProject.Size = Me.lProjectName.Size

                    Me.Controls.Add(CopyPanel)

                    Me.pChoice.Location = New Drawing.Point(Me.pChoice.Location.X, Me.pChoice.Location.Y + Me.pChoice.Size.Height)
                Loop

                Me.Size = New Drawing.Size(Me.Size.Width, Me.pChoice.Location.Y + 100)

                MyBase.ShowDialog(owner)
            End If
        End Sub

        Private Sub SelectionMade(sender As Object, e As EventArgs)
            If TypeOf sender Is Button AndAlso
            Not CType(sender, Button).Tag Is Nothing Then

                Me._SelectedProject = New Generic.KeyValuePair(Of String, String)(CType(CType(sender, Button).Tag, String), Me._ProjectList.Item(CType(CType(sender, Button).Tag, String)))

                Me.DialogResult = DialogResult.OK
            End If
        End Sub

        Public ReadOnly Property SelectedProject() As Generic.KeyValuePair(Of String, String)
            Get
                Return Me._SelectedProject
            End Get
        End Property
    End Class
End Namespace