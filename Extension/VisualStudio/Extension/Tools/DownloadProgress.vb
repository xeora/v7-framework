Namespace Xeora.Extension.VisualStudio.Tools
    Public Class DownloadProgress
        Private _DownloadThread As Threading.Thread = Nothing
        Private _DownloadLocation As String = String.Empty
        Private _DownloadedFile As Generic.List(Of String)

        Private _DownloadURL As String

        Private Delegate Sub SetText(ByVal Label As Label, ByVal Text As String)
        Private Sub SetTextFunc(ByVal Label As Label, ByVal Text As String)
            If Label.InvokeRequired Then
                Label.Invoke(New SetText(AddressOf Me.SetTextFunc), New Object() {Label, Text})
            Else
                Label.Text = Text
            End If
        End Sub

        Private Delegate Sub SetProgress(ByVal Progress As ProgressBar, ByVal Total As Integer, ByVal Current As Integer)
        Private Sub SetProgressFunc(ByVal Progress As ProgressBar, ByVal Total As Integer, ByVal Current As Integer)
            If Progress.InvokeRequired Then
                Progress.Invoke(New SetProgress(AddressOf Me.SetProgressFunc), New Object() {Progress, Total, Current})
            Else
                Progress.Maximum = Total
                Progress.Value = Current
            End If
        End Sub

        Public Sub StartDownloading(ByVal DownloadLocation As String, ByVal Use64Bit As Boolean, owner As IWin32Window)
            Me.lPullingFile.Text = String.Empty
            Me._DownloadLocation = DownloadLocation
            Me._DownloadURL = String.Format("http://www.xeora.org/Releases/Latest/{0}", IIf(Use64Bit, "x64", "Any"))
            Me._DownloadedFile = New Generic.List(Of String)()

            Me._DownloadThread = New Threading.Thread(AddressOf Me.DownloadThread)
            Me._DownloadThread.Start()

            MyBase.ShowDialog(owner)
        End Sub

        Public ReadOnly Property DownloadedFiles() As String()
            Get
                Return Me._DownloadedFile.ToArray()
            End Get
        End Property

        Private Sub DownloadThread()
            Dim WebRequest As Net.WebRequest = Nothing
            Dim WebResponse As Net.WebResponse = Nothing

            Dim DownloadList As New Generic.List(Of String)

            Dim FileListSR As IO.StreamReader = Nothing
            Try
                WebRequest = Net.HttpWebRequest.Create(String.Format("{0}/files.info", Me._DownloadURL))
                WebResponse = WebRequest.GetResponse()

                FileListSR = New IO.StreamReader(WebResponse.GetResponseStream(), System.Text.Encoding.UTF8)

                Do While FileListSR.Peek() > -1
                    DownloadList.Add(FileListSR.ReadLine())
                Loop
            Catch ex As Exception
                Me.Invoke(New Action(Sub()
                                         MessageBox.Show(Me, ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                     End Sub))

                Me.DialogResult = DialogResult.Cancel

                Exit Sub
            Finally
                If Not FileListSR Is Nothing Then FileListSR.Close()
                If Not WebResponse Is Nothing Then WebResponse.Close()
            End Try

            WebRequest = Nothing : WebResponse = Nothing

            Dim PullStream As IO.Stream = Nothing, TotalProgressPartLength As Integer
            Try
                Me.SetProgressFunc(Me.pbStatus, 100, 0)
                Dim ProgressPartLength As Integer = 100 \ DownloadList.Count

                For Each File As String In DownloadList
                    Me.SetTextFunc(Me.lPullingFile, File)

                    WebRequest = Net.HttpWebRequest.Create(String.Format("{0}/{1}", Me._DownloadURL, File))
                    WebResponse = WebRequest.GetResponse()

                    PullStream = WebResponse.GetResponseStream()

                    Dim FileStream As IO.FileStream = Nothing, buffer As Byte() = New Byte(8192 - 1) {}, rC As Integer
                    Try
                        Dim PullingFileSize As Long = WebResponse.ContentLength, PulledSize As Integer = 0
                        FileStream = New IO.FileStream(IO.Path.Combine(Me._DownloadLocation, File), IO.FileMode.Create, IO.FileAccess.ReadWrite, IO.FileShare.ReadWrite)

                        Do
                            rC = PullStream.Read(buffer, 0, buffer.Length)

                            If rC > 0 Then
                                FileStream.Write(buffer, 0, rC)

                                PulledSize += rC

                                Me.SetProgressFunc(Me.pbStatus, 100, TotalProgressPartLength + CType((PulledSize * ProgressPartLength) \ PullingFileSize, Integer))
                            End If
                        Loop Until rC = 0
                    Catch ex As Exception
                        Throw
                    Finally
                        If Not FileStream Is Nothing Then FileStream.Close()
                    End Try

                    Me._DownloadedFile.Add(File)

                    WebResponse.Close()

                    TotalProgressPartLength += ProgressPartLength
                Next

                Me.DialogResult = DialogResult.OK
            Catch ex As Exception
                Me.Invoke(New Action(Sub()
                                         MessageBox.Show(Me, ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                     End Sub))

                Me.DialogResult = DialogResult.Cancel

                Exit Sub
            Finally
                If Not PullStream Is Nothing Then PullStream.Close()
                If Not WebResponse Is Nothing Then WebResponse.Close()
            End Try
        End Sub
    End Class
End Namespace