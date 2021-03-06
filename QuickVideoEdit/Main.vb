﻿Public Class Main

    Private mdtCombineVideos As DataTable = Nothing
    Private mbolSaveState As Boolean = False

#Region " Form Events "

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            If My.Settings.NeedsUpgrade Then
                My.Settings.Upgrade()
                My.Settings.NeedsUpgrade = False
                My.Settings.Save()
            End If

            lblAbout.Text = My.Application.Info.Title & vbCrLf & My.Application.Info.Description & vbCrLf &
                "by " & My.Application.Info.CompanyName & vbCrLf & My.Application.Info.Copyright & vbCrLf &
                "v." & My.Application.Info.Version.ToString & vbCrLf & vbCrLf &
                "Uses technology in FFMPEG.  Visit http://ffmpeg.org/"

            Select Case My.Settings.LastState
                Case FormWindowState.Maximized
                    Me.WindowState = FormWindowState.Maximized
                Case FormWindowState.Minimized, FormWindowState.Normal
                    Me.WindowState = FormWindowState.Normal
                    If Not My.Settings.LastSize.IsEmpty Then
                        Me.Size = My.Settings.LastSize
                    End If
                    If Not My.Settings.LastLocation.IsEmpty Then
                        Me.Location = My.Settings.LastLocation
                    End If
            End Select

            Call WriteFFMPEG(True)

            txtCombineOutputPath.Text = My.Settings.LastCombineOutputPath
            txtRemoveAudioSource.Text = My.Settings.LastRemoveAudioSource
            txtRemoveAudioDest.Text = My.Settings.LastRemoveAudioDest
            txtRotateVideoDest.Text = My.Settings.LastRotateVideoDest
            txtRotateVideoSource.Text = My.Settings.LastRotateVideoSource

            Select Case My.Settings.LastRotate
                Case 1
                    optClock.Checked = True
                Case 2
                    optCounterClock.Checked = True
                Case 3
                    optClockFlip.Checked = True
                Case 0
                    optCounterClockFlip.Checked = True
                Case Else
                    optCounterClock.Checked = True
            End Select

            mdtCombineVideos = New DataTable("VideoCombineData")
            Try

                '   My.Settings.LastCombineData = ""
                Dim str As IO.StringReader = New IO.StringReader(My.Settings.LastCombineData)
                mdtCombineVideos.ReadXml(str)

            Catch ex As Exception

                mdtCombineVideos.Clear()
                mdtCombineVideos.Columns.Add("Add", GetType(Boolean))
                mdtCombineVideos.Columns.Add("Name", GetType(String))
                mdtCombineVideos.Columns.Add("Path", GetType(String))

            End Try

            dgvCombineVideos.DataSource = mdtCombineVideos

        Catch ex As Exception
            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        Finally
            mbolSaveState = True
        End Try

    End Sub

    Private Sub Main_Resize(sender As Object, e As EventArgs) Handles Me.Resize, Me.Move

        If mbolSaveState Then

            Select Case Me.WindowState
                Case FormWindowState.Maximized
                    My.Settings.LastState = FormWindowState.Maximized
                Case FormWindowState.Minimized
                    My.Settings.LastState = FormWindowState.Normal
                Case FormWindowState.Normal
                    My.Settings.LastState = FormWindowState.Normal
                    My.Settings.LastLocation = Me.Location
                    My.Settings.LastSize = Me.Size
            End Select

            My.Settings.Save()

        End If

    End Sub

#End Region

#Region " Button Event Handlers "

    Private Sub btnAddCombineVideos_Click(sender As Object, e As EventArgs) Handles btnAddCombineVideos.Click

        Try

            Using open As New OpenFileDialog

                open.Multiselect = True
                Try
                    open.InitialDirectory = My.Settings.LastCombineAddVideoDirectory
                Catch ex As Exception
                End Try

                open.AddExtension = False
                open.CheckFileExists = True
                open.CheckPathExists = True
                open.Filter = "Video Files(*.mpeg;*.mpg;*.mp4;*.avi;*.mov)|*.mpeg;*.mpg;*.mp4;*.avi;*.mov|All Files(*.*)|*.*"
                open.FilterIndex = 0

                Select Case open.ShowDialog
                    Case DialogResult.Cancel
                        Return
                End Select

                If open.FileNames IsNot Nothing AndAlso
                    open.FileNames.Length > 0 Then

                    My.Settings.LastCombineAddVideoDirectory =
                        IO.Path.GetDirectoryName(open.FileNames(0))
                    My.Settings.Save()

                    For Each strFile As String In open.FileNames

                        Dim dr As DataRow = mdtCombineVideos.NewRow
                        dr("Add") = True
                        dr("Name") = IO.Path.GetFileName(strFile)
                        dr("Path") = IO.Path.GetDirectoryName(strFile)
                        mdtCombineVideos.Rows.Add(dr)

                    Next

                    mdtCombineVideos.AcceptChanges()

                    dgvCombineVideos.AutoResizeColumns()

                    Call WriteXML()

                End If

            End Using

        Catch ex As Exception
            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub btnRotateProcess_Click(sender As Object, e As EventArgs) Handles btnRotateProcess.Click

        Try

            btnRotateProcess.Enabled = False

            If String.IsNullOrWhiteSpace(txtRotateVideoSource.Text) OrElse
                Not IO.File.Exists(txtRotateVideoSource.Text) Then

                MessageBox.Show("Please select an input file to rotate.", "No Input",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    txtRemoveAudioSource.Focus()
                Catch ex As Exception
                End Try

                Return

            ElseIf IO.File.Exists(txtRotateVideoDest.Text) Then

                Select Case MessageBox.Show("Do you want to overwrite file at location:" & vbCrLf &
                                            txtRotateVideoDest.Text, "Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    Case DialogResult.No
                        Return
                End Select

                IO.File.Delete(txtRotateVideoDest.Text)

            ElseIf Not IO.Directory.Exists(IO.Path.GetDirectoryName(txtRotateVideoDest.Text)) Then

                MessageBox.Show("Output folder does not exist.  Select a new one", "No Output Folder",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    txtRotateVideoDest.Focus()
                Catch ex As Exception
                End Try

                Return

            End If

            Dim strFFMPEG As String = WriteFFMPEG()

            slblRotateVideoStatus.Text = "Rotating Video ...."

            Using p As Process = New Process()

                AddHandler p.ErrorDataReceived, Sub(s As Object, ex As DataReceivedEventArgs)
                                                    If ex.Data IsNot Nothing Then
                                                        txtConsole.Invoke(Sub()
                                                                              txtConsole.AppendText(ex.Data & vbCrLf)
                                                                          End Sub)
                                                    End If
                                                End Sub
                AddHandler p.OutputDataReceived, Sub(s As Object, ex As DataReceivedEventArgs)
                                                     If ex.Data IsNot Nothing Then
                                                         txtConsole.Invoke(Sub()
                                                                               txtConsole.AppendText(ex.Data & vbCrLf)
                                                                           End Sub)
                                                     End If
                                                 End Sub

                Dim intTransposeValue As Integer = 2
                Select Case True
                    Case optClock.Checked
                        intTransposeValue = 1
                    Case optClockFlip.Checked
                        intTransposeValue = 3
                    Case optCounterClock.Checked
                        intTransposeValue = 2
                    Case optCounterClockFlip.Checked
                        intTransposeValue = 0
                    Case Else
                        intTransposeValue = 2
                End Select

                Dim pInfo As ProcessStartInfo = New ProcessStartInfo
                pInfo.Arguments = "-i """ & txtRotateVideoSource.Text & """ -vf ""transpose=" & intTransposeValue &
                    """ -c:a copy -an """ & txtRotateVideoDest.Text & """"
                pInfo.FileName = strFFMPEG

                pInfo.RedirectStandardError = True
                pInfo.RedirectStandardOutput = True
                pInfo.CreateNoWindow = True
                pInfo.UseShellExecute = False

                p.StartInfo = pInfo
                p.Start()

                p.BeginOutputReadLine()
                p.BeginErrorReadLine()

                Do While Not p.HasExited
                    Application.DoEvents()
                    Threading.Thread.Sleep(10)
                Loop

            End Using

            slblRotateVideoStatus.Text = "Rotating Video Complete"

        Catch ex As Exception

            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            slblRotateVideoStatus.Text = "Error Rotating Video"

        Finally
            btnRotateProcess.Enabled = True
        End Try

    End Sub

    Private Sub btnRemoveAudioProcess_Click(sender As Object, e As EventArgs) Handles btnRemoveAudioProcess.Click

        Try

            btnRemoveAudioProcess.Enabled = False

            If String.IsNullOrWhiteSpace(txtRemoveAudioSource.Text) OrElse
                Not IO.File.Exists(txtRemoveAudioSource.Text) Then

                MessageBox.Show("Please select an input file to remove audio from.", "No Input",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    txtRemoveAudioSource.Focus()
                Catch ex As Exception
                End Try

                Return

            ElseIf IO.File.Exists(txtRemoveAudioDest.Text) Then

                Select Case MessageBox.Show("Do you want to overwrite file at location:" & vbCrLf &
                                            txtRemoveAudioDest.Text, "Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    Case DialogResult.No
                        Return
                End Select

                IO.File.Delete(txtRemoveAudioDest.Text)

            ElseIf Not IO.Directory.Exists(IO.Path.GetDirectoryName(txtRemoveAudioDest.Text)) Then

                MessageBox.Show("Output folder does not exist.  Select a new one", "No Output Folder",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    txtRemoveAudioDest.Focus()
                Catch ex As Exception
                End Try

                Return

            End If

            Dim strFFMPEG As String = WriteFFMPEG()

            slblRemoveAudioStatus.Text = "Removing Audio ...."

            Using p As Process = New Process()

                AddHandler p.ErrorDataReceived, Sub(s As Object, ex As DataReceivedEventArgs)
                                                    If ex.Data IsNot Nothing Then
                                                        txtConsole.Invoke(Sub()
                                                                              txtConsole.AppendText(ex.Data & vbCrLf)
                                                                          End Sub)
                                                    End If
                                                End Sub
                AddHandler p.OutputDataReceived, Sub(s As Object, ex As DataReceivedEventArgs)
                                                     If ex.Data IsNot Nothing Then
                                                         txtConsole.Invoke(Sub()
                                                                               txtConsole.AppendText(ex.Data & vbCrLf)
                                                                           End Sub)
                                                     End If
                                                 End Sub

                Dim pInfo As ProcessStartInfo = New ProcessStartInfo
                pInfo.Arguments = "-i """ & txtRemoveAudioSource.Text & """ -c copy -an """ & txtRemoveAudioDest.Text & """"
                pInfo.FileName = strFFMPEG

                pInfo.RedirectStandardError = True
                pInfo.RedirectStandardOutput = True
                pInfo.CreateNoWindow = True
                pInfo.UseShellExecute = False

                p.StartInfo = pInfo
                p.Start()

                p.BeginOutputReadLine()
                p.BeginErrorReadLine()

                Do While Not p.HasExited
                    Application.DoEvents()
                    Threading.Thread.Sleep(10)
                Loop

            End Using

            slblRemoveAudioStatus.Text = "Removing Audio Complete"

        Catch ex As Exception

            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            slblRemoveAudioStatus.Text = "Error Removing Audio"

        Finally

            btnRemoveAudioProcess.Enabled = True

        End Try

    End Sub

    Private Sub btnCombine_Click(sender As Object, e As EventArgs) Handles btnCombine.Click

        Try

            btnCombine.Enabled = False

            mdtCombineVideos.AcceptChanges()

            If String.IsNullOrWhiteSpace(txtCombineOutputPath.Text) Then

                MessageBox.Show("Please select an output file to combine to.", "No Output",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    txtCombineOutputPath.Focus()
                Catch ex As Exception
                End Try

                Return

            ElseIf IO.File.Exists(txtCombineOutputPath.Text) Then

                Select Case MessageBox.Show("Do you want to overwrite file at location:" & vbCrLf &
                                            txtCombineOutputPath.Text, "Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    Case DialogResult.No
                        Return
                End Select

                IO.File.Delete(txtCombineOutputPath.Text)

            ElseIf Not IO.Directory.Exists(IO.Path.GetDirectoryName(txtCombineOutputPath.Text)) Then

                MessageBox.Show("Output folder does not exist.  Select a new one", "No Output Folder",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Try
                    txtCombineOutputPath.Focus()
                Catch ex As Exception
                End Try

                Return

            End If

            Dim lstRows As List(Of DataRow) = New List(Of DataRow)

            For Each dr As DataRow In mdtCombineVideos.Rows

                If Not dr("Add") Then
                    Continue For
                End If

                lstRows.Add(dr)

            Next

            If lstRows.Count <= 0 Then
                MessageBox.Show("Please select videos to combine.", "No Videos To Add",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Return
            End If

            Dim strFFMPEG As String = WriteFFMPEG()

            Dim strList As String = IO.Path.Combine(IO.Path.GetTempPath, "list.txt")
            Dim strListContents As Text.StringBuilder = New System.Text.StringBuilder
            For Each dr As DataRow In lstRows
                strListContents.AppendLine("file '" & IO.Path.Combine(dr("Path").ToString, dr("Name").ToString) & "'")
            Next

            IO.File.WriteAllText(strList, strListContents.ToString)

            slblCombineStatus.Text = "Combining ...."

            Using p As Process = New Process()

                AddHandler p.ErrorDataReceived, Sub(s As Object, ex As DataReceivedEventArgs)
                                                    If ex.Data IsNot Nothing Then
                                                        txtConsole.Invoke(Sub()
                                                                              txtConsole.AppendText(ex.Data & vbCrLf)
                                                                          End Sub)
                                                    End If
                                                End Sub
                AddHandler p.OutputDataReceived, Sub(s As Object, ex As DataReceivedEventArgs)
                                                     If ex.Data IsNot Nothing Then
                                                         txtConsole.Invoke(Sub()
                                                                               txtConsole.AppendText(ex.Data & vbCrLf)
                                                                           End Sub)
                                                     End If
                                                 End Sub

                Dim pInfo As ProcessStartInfo = New ProcessStartInfo
                pInfo.Arguments = "-f concat -safe 0 -i """ & strList & """ -c copy """ & txtCombineOutputPath.Text & """"
                pInfo.FileName = strFFMPEG

                pInfo.RedirectStandardError = True
                pInfo.RedirectStandardOutput = True
                pInfo.CreateNoWindow = True
                pInfo.UseShellExecute = False

                p.StartInfo = pInfo
                p.Start()

                p.BeginOutputReadLine()
                p.BeginErrorReadLine()

                Do While Not p.HasExited
                    Application.DoEvents
                    Threading.Thread.Sleep(10)
                Loop

            End Using

            slblCombineStatus.Text = "Combining Complete"

        Catch ex As Exception

            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            slblCombineStatus.Text = "Error Combining"

        Finally

            btnCombine.Enabled = True

        End Try

    End Sub

    Private Sub btnRemoveAudioSource_Click(sender As Object, e As EventArgs) Handles btnRemoveAudioSource.Click

        Try

            Using open As New OpenFileDialog

                open.CheckFileExists = True
                open.CheckPathExists = True
                open.Filter = "Video Files(*.mpeg;*.mpg;*.mp4;*.avi;*.mov)|*.mpeg;*.mpg;*.mp4;*.avi;*.mov|All Files(*.*)|*.*"
                open.FilterIndex = 0
                Try
                    open.InitialDirectory = IO.Path.GetDirectoryName(My.Settings.LastRemoveAudioSource)
                Catch ex As Exception
                End Try
                open.Title = "Open Video/Audio"
                Select Case open.ShowDialog()
                    Case DialogResult.Cancel
                        Return
                End Select

                txtRemoveAudioSource.Text = open.FileName
                txtRemoveAudioDest.Text = IO.Path.Combine(IO.Path.GetDirectoryName(open.FileName),
                                                        IO.Path.GetFileNameWithoutExtension(open.FileName) &
                                                        " - No Audio" &
                                                        IO.Path.GetExtension(open.FileName))
                My.Settings.LastRemoveAudioSource = open.FileName
                My.Settings.Save()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub btnRemoveAudioDest_Click(sender As Object, e As EventArgs) Handles btnRemoveAudioDest.Click

        Try

            Using save As New SaveFileDialog

                save.CheckPathExists = True
                save.Filter = "Video Files(*.mpeg;*.mpg;*.mp4;*.avi;*.mov)|*.mpeg;*.mpg;*.mp4;*.avi;*.mov|All Files(*.*)|*.*"
                save.FilterIndex = 0
                Try
                    save.InitialDirectory = IO.Path.GetDirectoryName(My.Settings.LastRemoveAudioDest)
                Catch ex As Exception
                End Try
                save.Title = "Save No Audio"
                save.OverwritePrompt = False
                Select Case save.ShowDialog()
                    Case DialogResult.Cancel
                        Return
                End Select

                txtRemoveAudioDest.Text = save.FileName
                My.Settings.LastRemoveAudioDest = save.FileName
                My.Settings.Save()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub btnRotateVideoSource_Click(sender As Object, e As EventArgs) Handles btnRotateVideoSource.Click

        Try

            Using open As New OpenFileDialog

                open.CheckFileExists = True
                open.CheckPathExists = True
                open.Filter = "Video Files(*.mpeg;*.mpg;*.mp4;*.avi;*.mov)|*.mpeg;*.mpg;*.mp4;*.avi;*.mov|All Files(*.*)|*.*"
                open.FilterIndex = 0
                Try
                    open.InitialDirectory = IO.Path.GetDirectoryName(My.Settings.LastRotateVideoSource)
                Catch ex As Exception
                End Try
                open.Title = "Open Video/Audio"
                Select Case open.ShowDialog()
                    Case DialogResult.Cancel
                        Return
                End Select

                txtRotateVideoSource.Text = open.FileName
                txtRotateVideoDest.Text = IO.Path.Combine(IO.Path.GetDirectoryName(open.FileName),
                                                        IO.Path.GetFileNameWithoutExtension(open.FileName) &
                                                        " - Rotated" &
                                                        IO.Path.GetExtension(open.FileName))
                My.Settings.LastRotateVideoSource = open.FileName
                My.Settings.Save()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub btnRotateVideoDest_Click(sender As Object, e As EventArgs) Handles btnRotateVideoDest.Click

        Try

            Using save As New SaveFileDialog

                save.CheckPathExists = True
                save.Filter = "Video Files(*.mpeg;*.mpg;*.mp4;*.avi;*.mov)|*.mpeg;*.mpg;*.mp4;*.avi;*.mov|All Files(*.*)|*.*"
                save.FilterIndex = 0
                Try
                    save.InitialDirectory = IO.Path.GetDirectoryName(My.Settings.LastRotateVideoDest)
                Catch ex As Exception
                End Try
                save.Title = "Save No Audio"
                save.OverwritePrompt = False
                Select Case save.ShowDialog()
                    Case DialogResult.Cancel
                        Return
                End Select

                txtRotateVideoDest.Text = save.FileName
                My.Settings.LastRotateVideoDest = save.FileName
                My.Settings.Save()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub


    Private Sub btnCombineOutputPath_Click(sender As Object, e As EventArgs) Handles btnCombineOutputPath.Click

        Try

            Using save As New SaveFileDialog

                save.CheckPathExists = True
                save.Filter = "Video Files(*.mpeg;*.mpg;*.mp4;*.avi;*.mov)|*.mpeg;*.mpg;*.mp4;*.avi;*.mov|All Files(*.*)|*.*"
                save.FilterIndex = 0
                Try
                    save.InitialDirectory = IO.Path.GetDirectoryName(My.Settings.LastCombineOutputPath)
                Catch ex As Exception
                End Try
                save.Title = "Save Combined Video"
                save.OverwritePrompt = False
                Select Case save.ShowDialog()
                    Case DialogResult.Cancel
                        Return
                End Select

                txtCombineOutputPath.Text = save.FileName
                My.Settings.LastCombineOutputPath = save.FileName
                My.Settings.Save()

            End Using

        Catch ex As Exception
            MessageBox.Show("Error in [" & Reflection.MethodBase.GetCurrentMethod.Name & "]: " & ex.ToString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

#End Region

#Region " Drag-n-Drop "

    Private dragBoxFromMouseDown As Rectangle
    Private rowIndexFromMouseDown As Integer
    Private rowIndexOfItemUnderMouseToDrop As Integer

    Private Sub dgvCombineVideos_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles dgvCombineVideos.MouseMove
        If (e.Button And MouseButtons.Left) = MouseButtons.Left Then
            If dragBoxFromMouseDown <> Rectangle.Empty AndAlso Not dragBoxFromMouseDown.Contains(e.X, e.Y) Then

                dgvCombineVideos.EndEdit()
                Dim a As DataGridViewRow() = New DataGridViewRow(dgvCombineVideos.SelectedRows.Count - 1) {}
                dgvCombineVideos.SelectedRows.CopyTo(a, 0)
                Dim dropEffect As DragDropEffects =
                    dgvCombineVideos.DoDragDrop(a, DragDropEffects.Move)
                'dgvCombineVideos.DoDragDrop(dgvCombineVideos.Rows(rowIndexFromMouseDown), DragDropEffects.Move)

            End If
        End If
    End Sub

    Private Sub dataGridView1_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles dgvCombineVideos.MouseDown

        If e.Button And MouseButtons.Left = MouseButtons.Left Then

            rowIndexFromMouseDown = dgvCombineVideos.HitTest(e.X, e.Y).RowIndex
            If rowIndexFromMouseDown <> -1 AndAlso dgvCombineVideos.Rows(rowIndexFromMouseDown).Selected Then
                Dim dragSize As Size = SystemInformation.DragSize
                dragBoxFromMouseDown = New Rectangle(New Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize)
            Else
                dragBoxFromMouseDown = Rectangle.Empty
            End If

        End If

    End Sub

    Private Sub dgvCombineVideos_DragOver(ByVal sender As Object, ByVal e As DragEventArgs) Handles dgvCombineVideos.DragOver
        e.Effect = DragDropEffects.Move
    End Sub

    Private Sub dgvCombineVideos_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles dgvCombineVideos.DragDrop
        Dim clientPoint As Point = dgvCombineVideos.PointToClient(New Point(e.X, e.Y))
        rowIndexOfItemUnderMouseToDrop = dgvCombineVideos.HitTest(clientPoint.X, clientPoint.Y).RowIndex
        If e.Effect = DragDropEffects.Move Then

            If rowIndexOfItemUnderMouseToDrop < 0 Then
                rowIndexOfItemUnderMouseToDrop = 0
            End If

            If rowIndexOfItemUnderMouseToDrop > dgvCombineVideos.Rows.Count - 1 Then
                rowIndexOfItemUnderMouseToDrop = dgvCombineVideos.Rows.Count - 1
            End If

            For Each rowToMove As DataGridViewRow In TryCast(e.Data.GetData(GetType(DataGridViewRow())), DataGridViewRow())

                Dim row As DataRow = rowToMove.DataBoundItem.row
                Dim data As Object() = row.ItemArray
                dgvCombineVideos.Rows.Remove(rowToMove)
                row = mdtCombineVideos.NewRow
                row.ItemArray = data

                If rowIndexOfItemUnderMouseToDrop < 0 Then
                    rowIndexOfItemUnderMouseToDrop = 0
                End If

                If rowIndexOfItemUnderMouseToDrop > dgvCombineVideos.Rows.Count - 1 Then
                    rowIndexOfItemUnderMouseToDrop = dgvCombineVideos.Rows.Count - 1
                End If

                mdtCombineVideos.Rows.InsertAt(row, rowIndexOfItemUnderMouseToDrop)
                rowIndexOfItemUnderMouseToDrop += 1
                'dgvCombineVideos.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove)

            Next

        End If
    End Sub

#End Region

#Region " Functions "

    Private Sub WriteXML()

        Try

            Dim str As IO.StringWriter = New IO.StringWriter()
            mdtCombineVideos.WriteXml(str, XmlWriteMode.WriteSchema)
            My.Settings.LastCombineData = str.ToString
            My.Settings.Save()

        Catch ex As Exception
            txtConsole.AppendText(ex.ToString & vbCrLf)
        End Try

    End Sub

    ''' <summary>
    ''' Writes the ffmpeg.exe from project resources to the temp folder
    ''' </summary>
    ''' <param name="Overwrite">Force overwriting of the ffmpeg.exe regardless of it's existance</param>
    ''' <returns>The full path to the ffmpeg.exe in the system's temp folder</returns>
    Private Function WriteFFMPEG(Optional Overwrite As Boolean = False) As String

        Dim strFFMPEG As String = IO.Path.Combine(IO.Path.GetTempPath, "ffmpeg.exe")

        Try

            Dim b As Byte() = Nothing
            If Environment.Is64BitOperatingSystem Then
                b = My.Resources.ffmpegx64
            Else
                b = My.Resources.ffmpegx86
            End If

            If Not IO.File.Exists(strFFMPEG) OrElse Overwrite Then
                IO.File.WriteAllBytes(strFFMPEG, b)
            End If

        Catch ex As Exception
            txtConsole.AppendText(ex.ToString & vbCrLf)
        End Try

        Return strFFMPEG

    End Function

#End Region

#Region " DataGridView Event Handlers "

    Private Sub dgvCombineVideos_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dgvCombineVideos.CellBeginEdit

        If e.ColumnIndex > 0 Then
            e.Cancel = True
        End If

    End Sub

    Private Sub dgvCombineVideos_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvCombineVideos.DataError

    End Sub

    Private Sub dgvCombineVideos_UserDeletedRow(sender As Object, e As DataGridViewRowEventArgs) Handles dgvCombineVideos.UserDeletedRow
        dgvCombineVideos.EndEdit()
        Call WriteXML()
    End Sub

    Private Sub dgvCombineVideos_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCombineVideos.CellEndEdit
        Call WriteXML()
    End Sub

#End Region

#Region " Other Event Handlers "

    Private Sub optClock_CheckedChanged(sender As Object, e As EventArgs) Handles optClock.CheckedChanged,
            optClockFlip.CheckedChanged, optCounterClock.CheckedChanged, optCounterClockFlip.CheckedChanged

        If mbolSaveState Then

            Select Case True
                Case optClock.Checked
                    My.Settings.LastRotate = 1
                Case optClockFlip.Checked
                    My.Settings.LastRotate = 3
                Case optCounterClock.Checked
                    My.Settings.LastRotate = 2
                Case optCounterClockFlip.Checked
                    My.Settings.LastRotate = 0
                Case Else
                    My.Settings.LastRotate = 2
            End Select

            My.Settings.Save()

        End If

    End Sub

#End Region

End Class
