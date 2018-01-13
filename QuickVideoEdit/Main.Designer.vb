<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.tabOperations = New System.Windows.Forms.TabControl()
        Me.tpgCombine = New System.Windows.Forms.TabPage()
        Me.btnCombineOutputPath = New System.Windows.Forms.Button()
        Me.lblOutput = New System.Windows.Forms.Label()
        Me.txtCombineOutputPath = New System.Windows.Forms.TextBox()
        Me.btnCombine = New System.Windows.Forms.Button()
        Me.dgvCombineVideos = New System.Windows.Forms.DataGridView()
        Me.btnAddCombineVideos = New System.Windows.Forms.Button()
        Me.tpgSilence = New System.Windows.Forms.TabPage()
        Me.btnRemoveAudioProcess = New System.Windows.Forms.Button()
        Me.btnRemoveAudioDest = New System.Windows.Forms.Button()
        Me.txtRemoveAudioDest = New System.Windows.Forms.TextBox()
        Me.lblRemoveAudioDest = New System.Windows.Forms.Label()
        Me.btnRemoveAudioSource = New System.Windows.Forms.Button()
        Me.txtRemoveAudioSource = New System.Windows.Forms.TextBox()
        Me.lblRemoveAudioSource = New System.Windows.Forms.Label()
        Me.tpgRotate = New System.Windows.Forms.TabPage()
        Me.optCounterClockFlip = New System.Windows.Forms.RadioButton()
        Me.optClock = New System.Windows.Forms.RadioButton()
        Me.optCounterClock = New System.Windows.Forms.RadioButton()
        Me.optClockFlip = New System.Windows.Forms.RadioButton()
        Me.btnRotateProcess = New System.Windows.Forms.Button()
        Me.btnRotateVideoDest = New System.Windows.Forms.Button()
        Me.txtRotateVideoDest = New System.Windows.Forms.TextBox()
        Me.lblRotateVideoDest = New System.Windows.Forms.Label()
        Me.btnRotateVideoSource = New System.Windows.Forms.Button()
        Me.txtRotateVideoSource = New System.Windows.Forms.TextBox()
        Me.lblRotateVideoSource = New System.Windows.Forms.Label()
        Me.tpgSplice = New System.Windows.Forms.TabPage()
        Me.trkSplice = New System.Windows.Forms.TrackBar()
        Me.pnlVideo = New System.Windows.Forms.Panel()
        Me.btnSpliceLoad = New System.Windows.Forms.Button()
        Me.btnSpliceDest = New System.Windows.Forms.Button()
        Me.txtSpliceDest = New System.Windows.Forms.TextBox()
        Me.lblSpliceDest = New System.Windows.Forms.Label()
        Me.btnSpliceSource = New System.Windows.Forms.Button()
        Me.txtSpliceSource = New System.Windows.Forms.TextBox()
        Me.lblSpliceSource = New System.Windows.Forms.Label()
        Me.tpgConsole = New System.Windows.Forms.TabPage()
        Me.txtConsole = New System.Windows.Forms.TextBox()
        Me.tpgAbout = New System.Windows.Forms.TabPage()
        Me.lblAbout = New System.Windows.Forms.Label()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.slblCombineStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.slblRemoveAudioStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.slblRotateVideoStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblSpliceCurrentTime = New System.Windows.Forms.Label()
        Me.tabOperations.SuspendLayout()
        Me.tpgCombine.SuspendLayout()
        CType(Me.dgvCombineVideos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpgSilence.SuspendLayout()
        Me.tpgRotate.SuspendLayout()
        Me.tpgSplice.SuspendLayout()
        CType(Me.trkSplice, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpgConsole.SuspendLayout()
        Me.tpgAbout.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabOperations
        '
        Me.tabOperations.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tabOperations.Controls.Add(Me.tpgCombine)
        Me.tabOperations.Controls.Add(Me.tpgSilence)
        Me.tabOperations.Controls.Add(Me.tpgRotate)
        Me.tabOperations.Controls.Add(Me.tpgSplice)
        Me.tabOperations.Controls.Add(Me.tpgConsole)
        Me.tabOperations.Controls.Add(Me.tpgAbout)
        Me.tabOperations.Location = New System.Drawing.Point(0, 0)
        Me.tabOperations.Name = "tabOperations"
        Me.tabOperations.SelectedIndex = 0
        Me.tabOperations.Size = New System.Drawing.Size(744, 542)
        Me.tabOperations.TabIndex = 0
        '
        'tpgCombine
        '
        Me.tpgCombine.Controls.Add(Me.btnCombineOutputPath)
        Me.tpgCombine.Controls.Add(Me.lblOutput)
        Me.tpgCombine.Controls.Add(Me.txtCombineOutputPath)
        Me.tpgCombine.Controls.Add(Me.btnCombine)
        Me.tpgCombine.Controls.Add(Me.dgvCombineVideos)
        Me.tpgCombine.Controls.Add(Me.btnAddCombineVideos)
        Me.tpgCombine.Location = New System.Drawing.Point(4, 22)
        Me.tpgCombine.Name = "tpgCombine"
        Me.tpgCombine.Padding = New System.Windows.Forms.Padding(3)
        Me.tpgCombine.Size = New System.Drawing.Size(736, 516)
        Me.tpgCombine.TabIndex = 0
        Me.tpgCombine.Text = "Combine Videos"
        Me.tpgCombine.UseVisualStyleBackColor = True
        '
        'btnCombineOutputPath
        '
        Me.btnCombineOutputPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCombineOutputPath.Location = New System.Drawing.Point(555, 6)
        Me.btnCombineOutputPath.Name = "btnCombineOutputPath"
        Me.btnCombineOutputPath.Size = New System.Drawing.Size(37, 23)
        Me.btnCombineOutputPath.TabIndex = 6
        Me.btnCombineOutputPath.Text = "..."
        Me.btnCombineOutputPath.UseVisualStyleBackColor = True
        '
        'lblOutput
        '
        Me.lblOutput.AutoSize = True
        Me.lblOutput.Location = New System.Drawing.Point(99, 11)
        Me.lblOutput.Name = "lblOutput"
        Me.lblOutput.Size = New System.Drawing.Size(42, 13)
        Me.lblOutput.TabIndex = 5
        Me.lblOutput.Text = "Output:"
        '
        'txtCombineOutputPath
        '
        Me.txtCombineOutputPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCombineOutputPath.Location = New System.Drawing.Point(147, 8)
        Me.txtCombineOutputPath.Name = "txtCombineOutputPath"
        Me.txtCombineOutputPath.Size = New System.Drawing.Size(402, 20)
        Me.txtCombineOutputPath.TabIndex = 4
        '
        'btnCombine
        '
        Me.btnCombine.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCombine.Location = New System.Drawing.Point(653, 6)
        Me.btnCombine.Name = "btnCombine"
        Me.btnCombine.Size = New System.Drawing.Size(75, 23)
        Me.btnCombine.TabIndex = 3
        Me.btnCombine.Text = "Combine"
        Me.btnCombine.UseVisualStyleBackColor = True
        '
        'dgvCombineVideos
        '
        Me.dgvCombineVideos.AllowDrop = True
        Me.dgvCombineVideos.AllowUserToAddRows = False
        Me.dgvCombineVideos.AllowUserToResizeRows = False
        Me.dgvCombineVideos.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvCombineVideos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCombineVideos.Location = New System.Drawing.Point(8, 35)
        Me.dgvCombineVideos.Name = "dgvCombineVideos"
        Me.dgvCombineVideos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvCombineVideos.Size = New System.Drawing.Size(720, 478)
        Me.dgvCombineVideos.TabIndex = 2
        '
        'btnAddCombineVideos
        '
        Me.btnAddCombineVideos.Location = New System.Drawing.Point(8, 6)
        Me.btnAddCombineVideos.Name = "btnAddCombineVideos"
        Me.btnAddCombineVideos.Size = New System.Drawing.Size(75, 23)
        Me.btnAddCombineVideos.TabIndex = 1
        Me.btnAddCombineVideos.Text = "Add Videos"
        Me.btnAddCombineVideos.UseVisualStyleBackColor = True
        '
        'tpgSilence
        '
        Me.tpgSilence.Controls.Add(Me.btnRemoveAudioProcess)
        Me.tpgSilence.Controls.Add(Me.btnRemoveAudioDest)
        Me.tpgSilence.Controls.Add(Me.txtRemoveAudioDest)
        Me.tpgSilence.Controls.Add(Me.lblRemoveAudioDest)
        Me.tpgSilence.Controls.Add(Me.btnRemoveAudioSource)
        Me.tpgSilence.Controls.Add(Me.txtRemoveAudioSource)
        Me.tpgSilence.Controls.Add(Me.lblRemoveAudioSource)
        Me.tpgSilence.Location = New System.Drawing.Point(4, 22)
        Me.tpgSilence.Name = "tpgSilence"
        Me.tpgSilence.Padding = New System.Windows.Forms.Padding(3)
        Me.tpgSilence.Size = New System.Drawing.Size(736, 516)
        Me.tpgSilence.TabIndex = 1
        Me.tpgSilence.Text = "Remove Audio"
        Me.tpgSilence.UseVisualStyleBackColor = True
        '
        'btnRemoveAudioProcess
        '
        Me.btnRemoveAudioProcess.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveAudioProcess.Location = New System.Drawing.Point(612, 70)
        Me.btnRemoveAudioProcess.Name = "btnRemoveAudioProcess"
        Me.btnRemoveAudioProcess.Size = New System.Drawing.Size(75, 23)
        Me.btnRemoveAudioProcess.TabIndex = 11
        Me.btnRemoveAudioProcess.Text = "Process"
        Me.btnRemoveAudioProcess.UseVisualStyleBackColor = True
        '
        'btnRemoveAudioDest
        '
        Me.btnRemoveAudioDest.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveAudioDest.Location = New System.Drawing.Point(693, 30)
        Me.btnRemoveAudioDest.Name = "btnRemoveAudioDest"
        Me.btnRemoveAudioDest.Size = New System.Drawing.Size(37, 23)
        Me.btnRemoveAudioDest.TabIndex = 10
        Me.btnRemoveAudioDest.Text = "..."
        Me.btnRemoveAudioDest.UseVisualStyleBackColor = True
        '
        'txtRemoveAudioDest
        '
        Me.txtRemoveAudioDest.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRemoveAudioDest.Location = New System.Drawing.Point(106, 32)
        Me.txtRemoveAudioDest.Name = "txtRemoveAudioDest"
        Me.txtRemoveAudioDest.Size = New System.Drawing.Size(581, 20)
        Me.txtRemoveAudioDest.TabIndex = 9
        '
        'lblRemoveAudioDest
        '
        Me.lblRemoveAudioDest.AutoSize = True
        Me.lblRemoveAudioDest.Location = New System.Drawing.Point(8, 35)
        Me.lblRemoveAudioDest.Name = "lblRemoveAudioDest"
        Me.lblRemoveAudioDest.Size = New System.Drawing.Size(93, 13)
        Me.lblRemoveAudioDest.TabIndex = 8
        Me.lblRemoveAudioDest.Text = "Video Destination:"
        '
        'btnRemoveAudioSource
        '
        Me.btnRemoveAudioSource.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveAudioSource.Location = New System.Drawing.Point(693, 4)
        Me.btnRemoveAudioSource.Name = "btnRemoveAudioSource"
        Me.btnRemoveAudioSource.Size = New System.Drawing.Size(37, 23)
        Me.btnRemoveAudioSource.TabIndex = 7
        Me.btnRemoveAudioSource.Text = "..."
        Me.btnRemoveAudioSource.UseVisualStyleBackColor = True
        '
        'txtRemoveAudioSource
        '
        Me.txtRemoveAudioSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRemoveAudioSource.Location = New System.Drawing.Point(106, 6)
        Me.txtRemoveAudioSource.Name = "txtRemoveAudioSource"
        Me.txtRemoveAudioSource.Size = New System.Drawing.Size(581, 20)
        Me.txtRemoveAudioSource.TabIndex = 1
        '
        'lblRemoveAudioSource
        '
        Me.lblRemoveAudioSource.AutoSize = True
        Me.lblRemoveAudioSource.Location = New System.Drawing.Point(26, 9)
        Me.lblRemoveAudioSource.Name = "lblRemoveAudioSource"
        Me.lblRemoveAudioSource.Size = New System.Drawing.Size(74, 13)
        Me.lblRemoveAudioSource.TabIndex = 0
        Me.lblRemoveAudioSource.Text = "Video Source:"
        '
        'tpgRotate
        '
        Me.tpgRotate.Controls.Add(Me.optCounterClockFlip)
        Me.tpgRotate.Controls.Add(Me.optClock)
        Me.tpgRotate.Controls.Add(Me.optCounterClock)
        Me.tpgRotate.Controls.Add(Me.optClockFlip)
        Me.tpgRotate.Controls.Add(Me.btnRotateProcess)
        Me.tpgRotate.Controls.Add(Me.btnRotateVideoDest)
        Me.tpgRotate.Controls.Add(Me.txtRotateVideoDest)
        Me.tpgRotate.Controls.Add(Me.lblRotateVideoDest)
        Me.tpgRotate.Controls.Add(Me.btnRotateVideoSource)
        Me.tpgRotate.Controls.Add(Me.txtRotateVideoSource)
        Me.tpgRotate.Controls.Add(Me.lblRotateVideoSource)
        Me.tpgRotate.Location = New System.Drawing.Point(4, 22)
        Me.tpgRotate.Name = "tpgRotate"
        Me.tpgRotate.Size = New System.Drawing.Size(736, 516)
        Me.tpgRotate.TabIndex = 2
        Me.tpgRotate.Text = "Rotate Video"
        Me.tpgRotate.UseVisualStyleBackColor = True
        '
        'optCounterClockFlip
        '
        Me.optCounterClockFlip.AutoSize = True
        Me.optCounterClockFlip.Location = New System.Drawing.Point(106, 90)
        Me.optCounterClockFlip.Name = "optCounterClockFlip"
        Me.optCounterClockFlip.Size = New System.Drawing.Size(132, 17)
        Me.optCounterClockFlip.TabIndex = 22
        Me.optCounterClockFlip.TabStop = True
        Me.optCounterClockFlip.Text = "Counter Clockwise-Flip"
        Me.optCounterClockFlip.UseVisualStyleBackColor = True
        '
        'optClock
        '
        Me.optClock.AutoSize = True
        Me.optClock.Location = New System.Drawing.Point(106, 113)
        Me.optClock.Name = "optClock"
        Me.optClock.Size = New System.Drawing.Size(73, 17)
        Me.optClock.TabIndex = 21
        Me.optClock.TabStop = True
        Me.optClock.Text = "Clockwise"
        Me.optClock.UseVisualStyleBackColor = True
        '
        'optCounterClock
        '
        Me.optCounterClock.AutoSize = True
        Me.optCounterClock.Location = New System.Drawing.Point(106, 136)
        Me.optCounterClock.Name = "optCounterClock"
        Me.optCounterClock.Size = New System.Drawing.Size(113, 17)
        Me.optCounterClock.TabIndex = 20
        Me.optCounterClock.TabStop = True
        Me.optCounterClock.Text = "Counter Clockwise"
        Me.optCounterClock.UseVisualStyleBackColor = True
        '
        'optClockFlip
        '
        Me.optClockFlip.AutoSize = True
        Me.optClockFlip.Location = New System.Drawing.Point(106, 67)
        Me.optClockFlip.Name = "optClockFlip"
        Me.optClockFlip.Size = New System.Drawing.Size(92, 17)
        Me.optClockFlip.TabIndex = 19
        Me.optClockFlip.TabStop = True
        Me.optClockFlip.Text = "Clockwise-Flip"
        Me.optClockFlip.UseVisualStyleBackColor = True
        '
        'btnRotateProcess
        '
        Me.btnRotateProcess.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRotateProcess.Location = New System.Drawing.Point(612, 82)
        Me.btnRotateProcess.Name = "btnRotateProcess"
        Me.btnRotateProcess.Size = New System.Drawing.Size(75, 23)
        Me.btnRotateProcess.TabIndex = 18
        Me.btnRotateProcess.Text = "Process"
        Me.btnRotateProcess.UseVisualStyleBackColor = True
        '
        'btnRotateVideoDest
        '
        Me.btnRotateVideoDest.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRotateVideoDest.Location = New System.Drawing.Point(693, 30)
        Me.btnRotateVideoDest.Name = "btnRotateVideoDest"
        Me.btnRotateVideoDest.Size = New System.Drawing.Size(37, 23)
        Me.btnRotateVideoDest.TabIndex = 17
        Me.btnRotateVideoDest.Text = "..."
        Me.btnRotateVideoDest.UseVisualStyleBackColor = True
        '
        'txtRotateVideoDest
        '
        Me.txtRotateVideoDest.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRotateVideoDest.Location = New System.Drawing.Point(106, 32)
        Me.txtRotateVideoDest.Name = "txtRotateVideoDest"
        Me.txtRotateVideoDest.Size = New System.Drawing.Size(581, 20)
        Me.txtRotateVideoDest.TabIndex = 16
        '
        'lblRotateVideoDest
        '
        Me.lblRotateVideoDest.AutoSize = True
        Me.lblRotateVideoDest.Location = New System.Drawing.Point(7, 35)
        Me.lblRotateVideoDest.Name = "lblRotateVideoDest"
        Me.lblRotateVideoDest.Size = New System.Drawing.Size(93, 13)
        Me.lblRotateVideoDest.TabIndex = 15
        Me.lblRotateVideoDest.Text = "Video Destination:"
        '
        'btnRotateVideoSource
        '
        Me.btnRotateVideoSource.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRotateVideoSource.Location = New System.Drawing.Point(693, 4)
        Me.btnRotateVideoSource.Name = "btnRotateVideoSource"
        Me.btnRotateVideoSource.Size = New System.Drawing.Size(37, 23)
        Me.btnRotateVideoSource.TabIndex = 14
        Me.btnRotateVideoSource.Text = "..."
        Me.btnRotateVideoSource.UseVisualStyleBackColor = True
        '
        'txtRotateVideoSource
        '
        Me.txtRotateVideoSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRotateVideoSource.Location = New System.Drawing.Point(106, 6)
        Me.txtRotateVideoSource.Name = "txtRotateVideoSource"
        Me.txtRotateVideoSource.Size = New System.Drawing.Size(581, 20)
        Me.txtRotateVideoSource.TabIndex = 13
        '
        'lblRotateVideoSource
        '
        Me.lblRotateVideoSource.AutoSize = True
        Me.lblRotateVideoSource.Location = New System.Drawing.Point(26, 9)
        Me.lblRotateVideoSource.Name = "lblRotateVideoSource"
        Me.lblRotateVideoSource.Size = New System.Drawing.Size(74, 13)
        Me.lblRotateVideoSource.TabIndex = 12
        Me.lblRotateVideoSource.Text = "Video Source:"
        '
        'tpgSplice
        '
        Me.tpgSplice.Controls.Add(Me.lblSpliceCurrentTime)
        Me.tpgSplice.Controls.Add(Me.trkSplice)
        Me.tpgSplice.Controls.Add(Me.pnlVideo)
        Me.tpgSplice.Controls.Add(Me.btnSpliceLoad)
        Me.tpgSplice.Controls.Add(Me.btnSpliceDest)
        Me.tpgSplice.Controls.Add(Me.txtSpliceDest)
        Me.tpgSplice.Controls.Add(Me.lblSpliceDest)
        Me.tpgSplice.Controls.Add(Me.btnSpliceSource)
        Me.tpgSplice.Controls.Add(Me.txtSpliceSource)
        Me.tpgSplice.Controls.Add(Me.lblSpliceSource)
        Me.tpgSplice.Location = New System.Drawing.Point(4, 22)
        Me.tpgSplice.Name = "tpgSplice"
        Me.tpgSplice.Size = New System.Drawing.Size(736, 516)
        Me.tpgSplice.TabIndex = 5
        Me.tpgSplice.Text = "Splice"
        Me.tpgSplice.UseVisualStyleBackColor = True
        '
        'trkSplice
        '
        Me.trkSplice.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.trkSplice.Location = New System.Drawing.Point(8, 417)
        Me.trkSplice.Name = "trkSplice"
        Me.trkSplice.Size = New System.Drawing.Size(720, 45)
        Me.trkSplice.TabIndex = 0
        Me.trkSplice.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'pnlVideo
        '
        Me.pnlVideo.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlVideo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlVideo.Location = New System.Drawing.Point(8, 86)
        Me.pnlVideo.Name = "pnlVideo"
        Me.pnlVideo.Size = New System.Drawing.Size(720, 325)
        Me.pnlVideo.TabIndex = 25
        '
        'btnSpliceLoad
        '
        Me.btnSpliceLoad.Location = New System.Drawing.Point(104, 57)
        Me.btnSpliceLoad.Name = "btnSpliceLoad"
        Me.btnSpliceLoad.Size = New System.Drawing.Size(75, 23)
        Me.btnSpliceLoad.TabIndex = 24
        Me.btnSpliceLoad.Text = "Load"
        Me.btnSpliceLoad.UseVisualStyleBackColor = True
        '
        'btnSpliceDest
        '
        Me.btnSpliceDest.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSpliceDest.Location = New System.Drawing.Point(691, 29)
        Me.btnSpliceDest.Name = "btnSpliceDest"
        Me.btnSpliceDest.Size = New System.Drawing.Size(37, 23)
        Me.btnSpliceDest.TabIndex = 23
        Me.btnSpliceDest.Text = "..."
        Me.btnSpliceDest.UseVisualStyleBackColor = True
        '
        'txtSpliceDest
        '
        Me.txtSpliceDest.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSpliceDest.Location = New System.Drawing.Point(104, 31)
        Me.txtSpliceDest.Name = "txtSpliceDest"
        Me.txtSpliceDest.Size = New System.Drawing.Size(581, 20)
        Me.txtSpliceDest.TabIndex = 22
        '
        'lblSpliceDest
        '
        Me.lblSpliceDest.AutoSize = True
        Me.lblSpliceDest.Location = New System.Drawing.Point(5, 34)
        Me.lblSpliceDest.Name = "lblSpliceDest"
        Me.lblSpliceDest.Size = New System.Drawing.Size(93, 13)
        Me.lblSpliceDest.TabIndex = 21
        Me.lblSpliceDest.Text = "Video Destination:"
        '
        'btnSpliceSource
        '
        Me.btnSpliceSource.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSpliceSource.Location = New System.Drawing.Point(691, 3)
        Me.btnSpliceSource.Name = "btnSpliceSource"
        Me.btnSpliceSource.Size = New System.Drawing.Size(37, 23)
        Me.btnSpliceSource.TabIndex = 20
        Me.btnSpliceSource.Text = "..."
        Me.btnSpliceSource.UseVisualStyleBackColor = True
        '
        'txtSpliceSource
        '
        Me.txtSpliceSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSpliceSource.Location = New System.Drawing.Point(104, 5)
        Me.txtSpliceSource.Name = "txtSpliceSource"
        Me.txtSpliceSource.Size = New System.Drawing.Size(581, 20)
        Me.txtSpliceSource.TabIndex = 19
        '
        'lblSpliceSource
        '
        Me.lblSpliceSource.AutoSize = True
        Me.lblSpliceSource.Location = New System.Drawing.Point(24, 8)
        Me.lblSpliceSource.Name = "lblSpliceSource"
        Me.lblSpliceSource.Size = New System.Drawing.Size(74, 13)
        Me.lblSpliceSource.TabIndex = 18
        Me.lblSpliceSource.Text = "Video Source:"
        '
        'tpgConsole
        '
        Me.tpgConsole.Controls.Add(Me.txtConsole)
        Me.tpgConsole.Location = New System.Drawing.Point(4, 22)
        Me.tpgConsole.Name = "tpgConsole"
        Me.tpgConsole.Padding = New System.Windows.Forms.Padding(3)
        Me.tpgConsole.Size = New System.Drawing.Size(736, 516)
        Me.tpgConsole.TabIndex = 3
        Me.tpgConsole.Text = "Console"
        Me.tpgConsole.UseVisualStyleBackColor = True
        '
        'txtConsole
        '
        Me.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtConsole.Location = New System.Drawing.Point(3, 3)
        Me.txtConsole.Multiline = True
        Me.txtConsole.Name = "txtConsole"
        Me.txtConsole.Size = New System.Drawing.Size(730, 510)
        Me.txtConsole.TabIndex = 0
        '
        'tpgAbout
        '
        Me.tpgAbout.Controls.Add(Me.lblAbout)
        Me.tpgAbout.Location = New System.Drawing.Point(4, 22)
        Me.tpgAbout.Name = "tpgAbout"
        Me.tpgAbout.Padding = New System.Windows.Forms.Padding(3)
        Me.tpgAbout.Size = New System.Drawing.Size(736, 516)
        Me.tpgAbout.TabIndex = 4
        Me.tpgAbout.Text = "About"
        Me.tpgAbout.UseVisualStyleBackColor = True
        '
        'lblAbout
        '
        Me.lblAbout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblAbout.Location = New System.Drawing.Point(3, 3)
        Me.lblAbout.Name = "lblAbout"
        Me.lblAbout.Size = New System.Drawing.Size(730, 510)
        Me.lblAbout.TabIndex = 0
        Me.lblAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.slblCombineStatus, Me.slblRemoveAudioStatus, Me.slblRotateVideoStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 545)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(744, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'slblCombineStatus
        '
        Me.slblCombineStatus.Name = "slblCombineStatus"
        Me.slblCombineStatus.Size = New System.Drawing.Size(0, 17)
        '
        'slblRemoveAudioStatus
        '
        Me.slblRemoveAudioStatus.Name = "slblRemoveAudioStatus"
        Me.slblRemoveAudioStatus.Size = New System.Drawing.Size(0, 17)
        '
        'slblRotateVideoStatus
        '
        Me.slblRotateVideoStatus.Name = "slblRotateVideoStatus"
        Me.slblRotateVideoStatus.Size = New System.Drawing.Size(0, 17)
        '
        'lblSpliceCurrentTime
        '
        Me.lblSpliceCurrentTime.AutoSize = True
        Me.lblSpliceCurrentTime.Location = New System.Drawing.Point(185, 62)
        Me.lblSpliceCurrentTime.Name = "lblSpliceCurrentTime"
        Me.lblSpliceCurrentTime.Size = New System.Drawing.Size(0, 13)
        Me.lblSpliceCurrentTime.TabIndex = 26
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(744, 567)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.tabOperations)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Main"
        Me.Text = "Quick Video Editing"
        Me.tabOperations.ResumeLayout(False)
        Me.tpgCombine.ResumeLayout(False)
        Me.tpgCombine.PerformLayout()
        CType(Me.dgvCombineVideos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpgSilence.ResumeLayout(False)
        Me.tpgSilence.PerformLayout()
        Me.tpgRotate.ResumeLayout(False)
        Me.tpgRotate.PerformLayout()
        Me.tpgSplice.ResumeLayout(False)
        Me.tpgSplice.PerformLayout()
        CType(Me.trkSplice, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpgConsole.ResumeLayout(False)
        Me.tpgConsole.PerformLayout()
        Me.tpgAbout.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tabOperations As TabControl
    Friend WithEvents tpgCombine As TabPage
    Friend WithEvents tpgSilence As TabPage
    Friend WithEvents tpgRotate As TabPage
    Friend WithEvents btnAddCombineVideos As Button
    Friend WithEvents dgvCombineVideos As DataGridView
    Friend WithEvents btnCombine As Button
    Friend WithEvents tpgConsole As TabPage
    Friend WithEvents txtConsole As TextBox
    Friend WithEvents lblOutput As Label
    Friend WithEvents txtCombineOutputPath As TextBox
    Friend WithEvents btnCombineOutputPath As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents slblCombineStatus As ToolStripStatusLabel
    Friend WithEvents btnRemoveAudioProcess As Button
    Friend WithEvents btnRemoveAudioDest As Button
    Friend WithEvents txtRemoveAudioDest As TextBox
    Friend WithEvents lblRemoveAudioDest As Label
    Friend WithEvents btnRemoveAudioSource As Button
    Friend WithEvents txtRemoveAudioSource As TextBox
    Friend WithEvents lblRemoveAudioSource As Label
    Friend WithEvents slblRemoveAudioStatus As ToolStripStatusLabel
    Friend WithEvents optCounterClockFlip As RadioButton
    Friend WithEvents optClock As RadioButton
    Friend WithEvents optCounterClock As RadioButton
    Friend WithEvents optClockFlip As RadioButton
    Friend WithEvents btnRotateProcess As Button
    Friend WithEvents btnRotateVideoDest As Button
    Friend WithEvents txtRotateVideoDest As TextBox
    Friend WithEvents lblRotateVideoDest As Label
    Friend WithEvents btnRotateVideoSource As Button
    Friend WithEvents txtRotateVideoSource As TextBox
    Friend WithEvents lblRotateVideoSource As Label
    Friend WithEvents slblRotateVideoStatus As ToolStripStatusLabel
    Friend WithEvents tpgAbout As TabPage
    Friend WithEvents lblAbout As Label
    Friend WithEvents tpgSplice As TabPage
    Friend WithEvents btnSpliceDest As Button
    Friend WithEvents txtSpliceDest As TextBox
    Friend WithEvents lblSpliceDest As Label
    Friend WithEvents btnSpliceSource As Button
    Friend WithEvents txtSpliceSource As TextBox
    Friend WithEvents lblSpliceSource As Label
    Friend WithEvents pnlVideo As Panel
    Friend WithEvents btnSpliceLoad As Button
    Friend WithEvents trkSplice As TrackBar
    Friend WithEvents lblSpliceCurrentTime As Label
End Class
