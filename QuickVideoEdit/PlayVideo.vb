Imports DirectShowLib
Imports System.Runtime.InteropServices

Public Class PlayVideo
    Implements IDisposable

#Region " Events "

    Public Event TimeChanged As EventHandler
    Public Event PlaybackFinished(ByVal sender As Object, ByVal LastTime As Decimal, ByVal e As EventArgs)

#End Region

#Region " Variables "

    Private fg As IGraphBuilder = Nothing
    Private rot As DsROTEntry = Nothing
    Private render As Rendering = Nothing
    Private vmr As IBaseFilter = Nothing
    Private mibaAudio As IBasicAudio = Nothing

    Private mbolThreadContinue As Boolean = False
    Private mbolDisposing As Boolean = False
    Private mthdTime As Threading.Thread = Nothing

    Private mintVolume As Integer = 0

#End Region

#Region " Public Scope "

#Region " Properties "

    Public Property MediaFilePath As String = ""

    Public ReadOnly Property isPlaying As Boolean
        Get

            If fg Is Nothing Then
                Return False
            End If

            Dim fs As FilterState = FilterState.Stopped
            DirectCast(fg, IMediaControl).GetState(500, fs)
            Select Case fs
                Case FilterState.Stopped
                    Return False
                Case FilterState.Running
                    Return True
                Case FilterState.Paused
                    Return False
                Case Else
                    Return False
            End Select

        End Get
    End Property

    Public ReadOnly Property Renderer As Rendering
        Get
            Return render
        End Get
    End Property

    Public ReadOnly Property TotalTime As Decimal
        Get

            If fg Is Nothing Then
                Return -1D
            End If

            Dim lng As Long = 0
            DirectCast(fg, IMediaSeeking).GetDuration(lng)

            Return lng

        End Get
    End Property

    Public Property CurrentTime As Decimal
        Get

            If fg Is Nothing Then
                Return -1D
            End If

            Dim lng As Long = 0
            DirectCast(fg, IMediaSeeking).GetCurrentPosition(lng)

            Return lng

        End Get
        Set(value As Decimal)

            If fg Is Nothing Then
                Return
            End If

            Dim lng As Long = 0
            DirectCast(fg, IMediaSeeking).SetPositions(value, AMSeekingSeekingFlags.AbsolutePositioning, 0, AMSeekingSeekingFlags.NoPositioning)

            Return

        End Set
    End Property

    Public Property Volume As Integer
        Get
            Return mintVolume
        End Get
        Set(value As Integer)

            If value > 0 Then
                value = 0
            End If

            If value < -10000 Then
                value = -10000
            End If

            If mintVolume <> value Then

                mintVolume = value

                If mibaAudio IsNot Nothing Then
                    mibaAudio.put_Volume(mintVolume)
                End If

            End If

        End Set
    End Property

#End Region

#Region " Functions "

    Public Function Play() As Boolean

        If fg Is Nothing Then
            Return False
        End If

        Return DirectCast(fg, IMediaControl).Run >= 0

    End Function

    Public Function Pause() As Boolean

        If fg Is Nothing Then
            Return False
        End If

        Return DirectCast(fg, IMediaControl).Pause >= 0

    End Function

    Public Function LoadMedia() As Boolean

        If String.IsNullOrWhiteSpace(Me.MediaFilePath) Then
            Throw New ArgumentNullException("MediaFilePath")
        End If

        If Not IO.File.Exists(Me.MediaFilePath) Then
            Throw New IO.FileNotFoundException("Unable to file media file", Me.MediaFilePath)
        End If

        Dim hr As Integer = 0

        Call Reset()

        fg = New FilterGraph
        rot = New DsROTEntry(fg)
        vmr = New VideoMixingRenderer9

        hr = fg.AddFilter(vmr, "VMR9")

        hr = fg.RenderFile(Me.MediaFilePath, "")

        render.VMRFilter = vmr

        mibaAudio = FilterUtils.FindInterface(fg, FilterUtils.Interfaces.AMDirectSound)
        If mibaAudio IsNot Nothing Then
            mibaAudio.put_Volume(mintVolume)
        End If

        mbolThreadContinue = True
        mthdTime = New Threading.Thread(AddressOf TimeThread)
        mthdTime.SetApartmentState(Threading.ApartmentState.STA)
        mthdTime.Start()

        Return Pause()

    End Function


#End Region

#Region " Subroutnines "

    Public Sub New()

        render = New Rendering

    End Sub

    Public Sub Reset()

        If render IsNot Nothing Then
            render.VMRFilter = Nothing
            render.ProcAmpFilter = Nothing
        Else
            render = New Rendering
        End If

        Call FreeResources(False)

    End Sub

#End Region

#End Region

#Region " Private Scope "

    Private Delegate Sub TimeCheckDelegate()

#Region " Subroutines "

    Private Sub ThreadCheck()

        Static slngTime As Decimal = -1
        Static slngNumberofCyclesTimeNotChanged As Long = 0
        Static sbolFinishFire As Boolean = True

        Try

            If mbolDisposing Then
                Return
            End If

            If fg Is Nothing Then
                Return
            End If

            ' See the time changed
            If slngTime <> Me.CurrentTime Then

                sbolFinishFire = True

                ' Check if the state of the graph is running
                If Me.isPlaying Then

                    ' It's running and the time number changed.  Indicate as such
                    slngNumberofCyclesTimeNotChanged = 0
                    ' mbolPlaying = True

                Else

                    ' time may have changed, but media samples cannot be continuously going if the filter graph is paused or stopped
                    ' mbolPlaying = False

                End If

                ' Save the last time number
                slngTime = Me.CurrentTime
                ' Indicate a change
                RaiseEvent TimeChanged(Me, New EventArgs)

            Else

                ' time number hasn't changed, is the filter graph running?
                If Me.isPlaying Then

                    ' We are running, but time has not changed.  check how many cycles we've encountered this in a row
                    If slngNumberofCyclesTimeNotChanged >= 3 Then

                        ' 3 cycles without a time change, indicate as such
                        ' mbolPlaying = False

                    Else

                        ' Less than 3 cycles without a time change.  Increment the number and indicate we are "still" playing
                        ' mbolPlaying = True
                        slngNumberofCyclesTimeNotChanged += 1

                    End If

                Else

                    ' If the filter graph state is not running, we cannot be sending media samples regardless of time number changing.
                    '  mbolPlaying = False

                End If

            End If

            Try

                ' Get the last time number
                Dim lngLastTime As Decimal = Me.TotalTime
                If (slngTime >= lngLastTime) And (lngLastTime > 0) Then

                    If sbolFinishFire Then

                        ' Passed the last time indicator
                        RaiseEvent PlaybackFinished(Me, lngLastTime, New EventArgs)
                        sbolFinishFire = False

                    End If

                End If

            Catch ex As Exception
            End Try

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Thread loop for querying a change in the time
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TimeThread()

        Try

            ' Loop while required
            Do While mbolThreadContinue

                ' Make sure the filter graph exists
                If (fg IsNot Nothing) Then

                    If render.InvokeRequired Then
                        render.BeginInvoke(New TimeCheckDelegate(AddressOf ThreadCheck))
                    Else
                        Call ThreadCheck()
                    End If

                End If

                ' Wait for other threads to process a bit
                Threading.Thread.Sleep(100)

            Loop

        Catch exThread As Threading.ThreadAbortException
            Debug.WriteLine(exThread.ToString)
        Catch ex As Exception
            Debug.WriteLine(ex.ToString)
        End Try

    End Sub

    Private Sub FreeResources(ByVal Managed As Boolean)

        If Managed Then

            If render IsNot Nothing Then
                render.VMRFilter = Nothing
                render.ProcAmpFilter = Nothing
                render.Dispose()
                render = Nothing
            End If

        Else

            mbolThreadContinue = False
            If mthdTime IsNot Nothing Then

                Dim intCounter As Integer = 0

                Do While mthdTime.IsAlive

                    mthdTime.Abort()
                    Threading.Thread.Sleep(10)

                    intCounter += 1

                    If intCounter > 20 Then
                        Exit Do
                    End If

                Loop

            End If
            mthdTime = Nothing

            If fg IsNot Nothing Then
                DirectCast(fg, IMediaControl).Stop()
            End If

            If mibaAudio IsNot Nothing Then
                Marshal.ReleaseComObject(mibaAudio)
                mibaAudio = Nothing
            End If

            If vmr IsNot Nothing Then

                FilterUtils.DisconnectPins(vmr, fg)
                Marshal.ReleaseComObject(vmr)
                vmr = Nothing

            End If

            If rot IsNot Nothing Then
                rot.Dispose()
                rot = Nothing
            End If

            If fg IsNot Nothing Then

                Call FilterUtils.DisconnectFilters(fg, True)
                Marshal.ReleaseComObject(fg)
                fg = Nothing

            End If


        End If

    End Sub

#End Region

#End Region

#Region " IDisposable Support "
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        mbolDisposing = True
        If Not disposedValue Then

            If disposing Then
                Call FreeResources(True)
            End If
            Call FreeResources(False)
        End If
        disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
