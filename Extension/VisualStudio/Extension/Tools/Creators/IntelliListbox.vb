Imports Microsoft.VisualStudio.Language

Public Class IntelliListbox

    Public Event NoItem()
    Public Event ItemSelected(ByVal value As String)
    Public Property Builders As Intellisense.Completion() = Nothing
    Public Property ListItems As Intellisense.Completion() = Nothing

    Private _InitialHeight As Integer = 300
    'Private _InitialWidth As Integer = 540
    Public Sub Build(ByVal TypedText As String)
        Me.lwIntelliItems.Items.Clear()
        Me.lwIntelliItems.SmallImageList =
            New ImageList()

        If Me.ListItems Is Nothing OrElse Me.ListItems.Length = 0 Then
            Me.Visible = False
            RaiseEvent NoItem()
        End If

        Dim MatchExists As Boolean = False
        For Each ListItem As Intellisense.Completion In Me.ListItems
            Me.lwIntelliItems.SmallImageList.Images.Add(
                Me.GetBitmap(CType(ListItem.IconSource, System.Windows.Media.Imaging.BitmapSource)))

            Me.lwIntelliItems.Items.Add(String.Empty, Me.lwIntelliItems.SmallImageList.Images.Count - 1)
            Me.lwIntelliItems.Items.Item(Me.lwIntelliItems.Items.Count - 1).SubItems.Add(ListItem.DisplayText)
            Me.lwIntelliItems.Items.Item(Me.lwIntelliItems.Items.Count - 1).SubItems.Add(ListItem.InsertionText)

            If Not MatchExists Then
                If ListItem.DisplayText.IndexOf(TypedText) = 0 Then
                    Me.lwIntelliItems.Items.Item(Me.lwIntelliItems.Items.Count - 1).Selected = True
                    Me.lwIntelliItems.EnsureVisible(Me.lwIntelliItems.Items.Count - 1)

                    MatchExists = True
                End If
            End If
        Next

        Me.Visible = True
        Me.BringToFront()

        Dim CalculationGraphic As System.Drawing.Graphics = Me.CreateGraphics()
        Dim CalculatedHeight As Integer = 0, CalculatedWidth As Integer = 0
        For iC As Integer = 0 To Me.lwIntelliItems.Items.Count - 1
            CalculatedHeight += Me.lwIntelliItems.GetItemRect(iC).Height

            Dim TextSize As Drawing.SizeF =
                CalculationGraphic.MeasureString(Me.lwIntelliItems.Items.Item(iC).SubItems(1).Text, Me.lwIntelliItems.Font)
            If CalculatedWidth < TextSize.Width Then _
                CalculatedWidth = CType(TextSize.Width, Integer)
        Next

        Dim ScrollVisible As Boolean = False
        If CalculatedHeight > 0 AndAlso CalculatedHeight < Me._InitialHeight Then
            Me.Size = New Drawing.Size(Me.Size.Width, CalculatedHeight)
        Else
            Me.Size = New Drawing.Size(Me.Size.Width, Me._InitialHeight)
            ScrollVisible = True
        End If

        'If CalculatedWidth > 0 AndAlso CalculatedWidth < Me._InitialWidth Then
        Me.lwIntelliItems.Columns.Item(1).Width = CalculatedWidth
        CalculatedWidth += Me.lwIntelliItems.Columns.Item(0).Width
        If ScrollVisible Then CalculatedWidth += 40
        Me.Size = New Drawing.Size(CalculatedWidth, Me.Size.Height)
        'Else
        '    If ScrollVisible Then
        '        Me.lwIntelliItems.Columns.Item(1).Width = (Me._InitialWidth - 40)
        '    Else
        '        Me.lwIntelliItems.Columns.Item(1).Width = Me._InitialWidth
        '    End If

        '    Me.Size = New Drawing.Size(Me._InitialWidth, Me.Size.Height)
        'End If
    End Sub

    Public Sub Previous()
        Dim CurrentSelected As Integer = 0
        If Me.lwIntelliItems.SelectedIndices.Count > 0 Then _
            CurrentSelected = Me.lwIntelliItems.SelectedIndices.Item(0)

        CurrentSelected -= 1
        If CurrentSelected < 0 Then CurrentSelected = Me.lwIntelliItems.Items.Count - 1

        Me.lwIntelliItems.SelectedIndices.Clear()
        Me.lwIntelliItems.SelectedIndices.Add(CurrentSelected)
        Me.lwIntelliItems.FocusedItem = Me.lwIntelliItems.Items.Item(CurrentSelected)
        Me.lwIntelliItems.EnsureVisible(CurrentSelected)
    End Sub

    Public Sub [Next]()
        Dim CurrentSelected As Integer = 0
        If Me.lwIntelliItems.SelectedIndices.Count > 0 Then _
            CurrentSelected = Me.lwIntelliItems.SelectedIndices.Item(0)

        CurrentSelected += 1
        If CurrentSelected > Me.lwIntelliItems.Items.Count - 1 Then CurrentSelected = 0

        Me.lwIntelliItems.SelectedIndices.Clear()
        Me.lwIntelliItems.SelectedIndices.Add(CurrentSelected)
        Me.lwIntelliItems.FocusedItem = Me.lwIntelliItems.Items.Item(CurrentSelected)
        Me.lwIntelliItems.EnsureVisible(CurrentSelected)
    End Sub

    Public Function GetSelectedValue() As String
        If Me.lwIntelliItems.SelectedItems.Count = 0 Then Return String.Empty

        Return Me.lwIntelliItems.SelectedItems.Item(0).SubItems.Item(2).Text
    End Function

    Public Function GetBitmap(ByVal source As System.Windows.Media.Imaging.BitmapSource) As System.Drawing.Image
        Dim MS As New IO.MemoryStream
        Dim encoder As New System.Windows.Media.Imaging.BmpBitmapEncoder()
        encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(source))
        encoder.Save(MS)
        MS.Flush()

        Return System.Drawing.Image.FromStream(MS)
    End Function

    Private Sub lwIntelliItems_DoubleClick(sender As Object, e As EventArgs) Handles lwIntelliItems.DoubleClick
        If Me.lwIntelliItems.SelectedItems.Count > 0 Then
            RaiseEvent ItemSelected(Me.GetSelectedValue())
        End If
    End Sub
End Class
