Imports System.Windows.Forms
Imports DirectShowLib
Imports System.Runtime.InteropServices

Public Class Rendering
    Implements IProcAmp
    Implements IDisposable

#Region " Structures "

    <System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>
    Private Structure RECT

        Private _Left As Integer, _Top As Integer, _Right As Integer, _Bottom As Integer

        Public Sub New(ByVal Rectangle As Drawing.Rectangle)
            Me.New(Rectangle.Top, Rectangle.Left, Rectangle.Right, Rectangle.Bottom)
        End Sub

        Public Sub New(ByVal Left As Integer, ByVal Top As Integer, ByVal Right As Integer, ByVal Bottom As Integer)

            _Left = Left
            _Top = Top
            _Right = Right
            _Bottom = Bottom

        End Sub

        Public Property X() As Integer
            Get
                Return _Left
            End Get
            Set(ByVal value As Integer)
                _Left = value
            End Set
        End Property

        Public Property Y() As Integer
            Get
                Return _Top
            End Get
            Set(ByVal value As Integer)
                _Top = value
            End Set
        End Property

        Public Property Left() As Integer
            Get
                Return _Left
            End Get
            Set(ByVal value As Integer)
                _Left = value
            End Set
        End Property

        Public Property Top() As Integer
            Get
                Return _Top
            End Get
            Set(ByVal value As Integer)
                _Top = value
            End Set
        End Property

        Public Property Right() As Integer
            Get
                Return _Right
            End Get
            Set(ByVal value As Integer)
                _Right = value
            End Set
        End Property

        Public Property Bottom() As Integer
            Get
                Return _Bottom
            End Get
            Set(ByVal value As Integer)
                _Bottom = value
            End Set
        End Property

        Public Property Height() As Integer
            Get
                Return _Bottom - _Top
            End Get
            Set(ByVal value As Integer)
                _Bottom = value - _Top
            End Set
        End Property

        Public Property Width() As Integer
            Get
                Return _Right - _Left
            End Get
            Set(ByVal value As Integer)
                _Right = value + _Left
            End Set
        End Property

        Public Property Location() As Drawing.Point
            Get
                Return New Drawing.Point(Left, Top)
            End Get
            Set(ByVal value As Drawing.Point)

                _Left = value.X
                _Top = value.Y

            End Set
        End Property

        Public Property Size() As Drawing.Size
            Get
                Return New Drawing.Size(Width, Height)
            End Get
            Set(ByVal value As Drawing.Size)

                _Right = value.Width + _Left
                _Bottom = value.Height + _Top

            End Set
        End Property

        Public Function ToRectangle() As Drawing.Rectangle
            Return New Drawing.Rectangle(Me.Left, Me.Top, Me.Width, Me.Height)
        End Function

        Public Shared Function ToRectangle(ByVal Rectangle As RECT) As Drawing.Rectangle
            Return Rectangle.ToRectangle
        End Function

        Public Shared Function FromRectangle(ByVal Rectangle As Drawing.Rectangle) As RECT
            Return New RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
        End Function

        Public Shared Widening Operator CType(ByVal Rectangle As RECT) As Drawing.Rectangle
            Return Rectangle.ToRectangle
        End Operator

        Public Shared Widening Operator CType(ByVal Rectangle As Drawing.Rectangle) As RECT
            Return New RECT(Rectangle)
        End Operator

        Public Shared Operator =(ByVal Rectangle1 As RECT, ByVal Rectangle2 As RECT) As Boolean
            Return Rectangle1.Equals(Rectangle2)
        End Operator

        Public Shared Operator <>(ByVal Rectangle1 As RECT, ByVal Rectangle2 As RECT) As Boolean
            Return Not Rectangle1.Equals(Rectangle2)
        End Operator

        Public Overrides Function ToString() As String
            Return "{Left: " & _Left & "; " & "Top: " & _Top & "; Right: " & _Right & "; Bottom: " & _Bottom & "}"
        End Function

        Public Overloads Function Equals(ByVal Rectangle As RECT) As Boolean
            Return Rectangle.Left = _Left AndAlso Rectangle.Top = _Top AndAlso Rectangle.Right = _Right AndAlso Rectangle.Bottom = _Bottom
        End Function

        Public Overloads Overrides Function Equals(ByVal [Object] As Object) As Boolean

            If TypeOf [Object] Is RECT Then
                Return Equals(DirectCast([Object], RECT))
            ElseIf TypeOf [Object] Is Drawing.Rectangle Then
                Return Equals(New RECT(DirectCast([Object], Drawing.Rectangle)))
            End If

            Return False

        End Function

    End Structure

#End Region

#Region " APIs "

    ''' <summary>
    ''' Get the RECT of the client window
    ''' </summary>
    ''' <param name="hWnd">Handle of the window</param>
    ''' <param name="lpRECT">Output of the RECT</param>
    ''' <returns>HRESULT</returns>
    ''' <remarks></remarks>
    <System.Runtime.InteropServices.DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)>
    Private Shared Function GetClientRect(ByVal hWnd As System.IntPtr, ByRef lpRECT As RECT) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetDC(ByVal hwnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindowEx", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowEx(ByVal parentHandle As IntPtr,
                      ByVal childAfter As IntPtr,
                      ByVal lclassName As String,
                      ByVal windowTitle As String) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindowEx", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowExByClass(ByVal parentHandle As IntPtr,
                      ByVal childAfter As IntPtr,
                      ByVal lclassName As String,
                      ByVal windowTitle As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", EntryPoint:="FindWindowEx", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowExByTitle(ByVal parentHandle As IntPtr,
                      ByVal childAfter As IntPtr,
                      ByVal lclassName As IntPtr,
                      ByVal windowTitle As String) As IntPtr
    End Function

#End Region

#Region " Variables "

    ''' <summary>
    ''' Renderer object to perform operations on
    ''' </summary>
    ''' <remarks></remarks>
    Private mvmrVideo As IBaseFilter = Nothing 'VideoMixingRenderer9 = Nothing
    ''' <summary>
    ''' Live video device to perform operations on
    ''' </summary>
    ''' <remarks></remarks>
    Private mProcAmpVideo As IAMVideoProcAmp = Nothing

    Private WithEvents mobjContainer As Windows.Forms.Control = Nothing
    Private mwndFlickerFree As FlickerFreeWindow = Nothing

#If Not WINDOWLESS Then
    Private mbolMouseDown As Boolean = False
#End If

    Private WithEvents mfrm As MessagedWindow = Nothing
    Private mbolKeepNativeSize As Boolean = False
    Private mbolKeepAspectRatio As Boolean = True
    Private mintPreviewHandle As Integer = 0
    Private msngBrightness As Single = 0
    Private msngContrast As Single = 0
    Private msngSaturation As Single = 0
    Private msngHue As Single = 0
    Private msngLastFrameRate As Single = 0
    Private mintBackColor As Integer = 0

#End Region

#Region " Friend Scope "

    Friend Sub New()
        MyBase.New()
    End Sub

    Friend Overloads Function BeginInvoke(ByVal method As System.Delegate, ByVal args() As Object) As System.IAsyncResult

        If mobjContainer Is Nothing Then
            Return Nothing
        End If

        Return mobjContainer.BeginInvoke(method, args)

    End Function

    Friend Overloads Function BeginInvoke(ByVal method As System.Delegate) As System.IAsyncResult

        If mobjContainer Is Nothing Then
            Return Nothing
        End If

        Return mobjContainer.BeginInvoke(method)

    End Function

    Friend ReadOnly Property InvokeRequired As Boolean
        Get

            If mobjContainer Is Nothing Then
                Return False
            End If
            Return mobjContainer.InvokeRequired

        End Get
    End Property

    ''' <summary>
    ''' The local Video Mixing Renderer 7/9 that is used to refresh previews
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Property VMRFilter As IBaseFilter
        Get
            Return mvmrVideo
        End Get
        Set(ByVal value As IBaseFilter)

            mvmrVideo = value
            msngLastFrameRate = 0

            If mvmrVideo IsNot Nothing Then

                KeepAspectRatio = mbolKeepAspectRatio
                KeepNativeSize = mbolKeepNativeSize

                Brightness = BrightnessInfo.Default
                Contrast = ContrastInfo.Default
                Saturation = SaturationInfo.Default
                Hue = HueInfo.Default

            End If

            Call Refresh()

        End Set
    End Property

    ''' <summary>
    ''' The local Video Proc Amp from the video capture filter for setting proc amp
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>If this object is Nothing, the VMR will be used for proc amp</remarks>
    Friend Property ProcAmpFilter As IAMVideoProcAmp
        Get
            Return mProcAmpVideo
        End Get
        Set(ByVal value As IAMVideoProcAmp)

            mProcAmpVideo = value

            Brightness = BrightnessInfo.Default
            Contrast = ContrastInfo.Default
            Saturation = SaturationInfo.Default
            Hue = HueInfo.Default

        End Set
    End Property

#End Region

#Region " Public Scope "

#Region " Subroutines "

    ''' <summary>
    ''' Resizes and refreshes the rendering
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Refresh()

        Dim filterinfo As FilterInfo = Nothing
        Dim ibv As IBasicVideo2 = Nothing
        Dim intWidth As Integer = 0
        Dim intHeight As Integer = 0

        Try

            ' Make sure we have a renderer to resize
            If mvmrVideo Is Nothing Then

                If Handle = 0 Then

#If Not WINDOWLESS Then

                    ' Release the last one
                    If mfrm IsNot Nothing Then
                        mfrm.Dispose()
                    End If
                    mfrm = Nothing

#End If

                    mobjContainer = Nothing
                    If mwndFlickerFree IsNot Nothing Then
                        mwndFlickerFree.Dispose()
                        mwndFlickerFree = Nothing
                    End If

                Else

                    ' Get the container control, if it's a .NET control
                    Try

                        If (mobjContainer Is Nothing) OrElse (mobjContainer.Handle <> Handle) Then

                            ' Release the last one
                            If mfrm IsNot Nothing Then
                                mfrm.Dispose()
                            End If
                            mfrm = Nothing

                            If mwndFlickerFree IsNot Nothing Then
                                mwndFlickerFree.Dispose()
                                mwndFlickerFree = Nothing
                            End If
#If WINDOWLESS Then

                            ' Need to get some none-exposed windows messages
                            mfrm = New MessagedWindow(Handle)

#End If

                            mobjContainer = Windows.Forms.Control.FromHandle(Handle)
                            mwndFlickerFree = New FlickerFreeWindow(mobjContainer.Handle)

                        End If

                        If mwndFlickerFree IsNot Nothing Then
                            mwndFlickerFree.EraseBackground = True
                        End If

                    Catch ex As Exception
                    End Try

                End If

                Return

            End If

#If WINDOWLESS Then
            Dim obj As Object = Nothing
            If TypeOf mvmrVideo Is VideoMixingRenderer9 Then
                obj = DirectCast(mvmrVideo, IVMRWindowlessControl9)
            Else
                obj = DirectCast(mvmrVideo, IVMRWindowlessControl)
            End If
            With obj
#Else
            With DirectCast(mvmrVideo, IVideoWindow)
#End If

#If WINDOWLESS Then

                ' Set the border color to black
                .SetBorderColor(0) ' Black

#Else
                .put_BorderColor(mintBackColor)
#End If

                If Handle = 0 Then

                    ' Release info if the handle is empty
#If WINDOWLESS Then

                    ' Windowless requires a handle

#Else

                    ' Release the last one
                    If mfrm IsNot Nothing Then
                        mfrm.Dispose()
                    End If
                    mfrm = Nothing

                    If mwndFlickerFree IsNot Nothing Then
                        mwndFlickerFree.Dispose()
                        mwndFlickerFree = Nothing
                    End If

                    .put_Visible(OABool.False)
                    .put_Owner(IntPtr.Zero)
                    .put_MessageDrain(IntPtr.Zero)
                    .put_WindowStyle(WindowStyle.Child)
#End If

                    mobjContainer = Nothing

                Else

                    ' Get the container control, if it's a .NET control
                    Try

                        If (mobjContainer Is Nothing) OrElse (mobjContainer.Handle <> Handle) Then

                            ' Release the last one
                            If mfrm IsNot Nothing Then
                                mfrm.Dispose()
                            End If
                            mfrm = Nothing

                            If mwndFlickerFree IsNot Nothing Then
                                mwndFlickerFree.Dispose()
                                mwndFlickerFree = Nothing
                            End If

#If WINDOWLESS Then

                            ' Need to get some none-exposed windows messages
                            mfrm = New MessagedWindow(Handle)

#End If

                            mobjContainer = Windows.Forms.Control.FromHandle(Handle)
                            mwndFlickerFree = New FlickerFreeWindow(mobjContainer.Handle)

                        End If

                        If mwndFlickerFree IsNot Nothing Then
                            mwndFlickerFree.EraseBackground = False
                        End If

                    Catch ex As Exception
                        Debug.WriteLine(ex.ToString)
                    End Try

                    ' Get the client rect
                    Dim rect As RECT = New RECT(0, 0, 0, 0)
                    If mobjContainer IsNot Nothing Then
                        rect = New RECT(mobjContainer.ClientRectangle)
                    Else
                        GetClientRect(Handle, rect)
                    End If

                    ' Set info
#If WINDOWLESS Then
                    .SetVideoClippingWindow(Handle)

                    Dim srcRECT As DsRect = New DsRect
                    Dim destRect As DsRect = New DsRect
                    .GetNativeVideoSize(intWidth, intHeight, 0, 0)
                    .GetVideoPosition(srcRECT, destRect)
                    srcRECT = New DsRect(0, 0, intWidth, intHeight)

#Else
                    .put_Owner(Handle)
                    .put_WindowStyle(WindowStyle.Child)
                    .put_MessageDrain(Handle)

                    If mfrm Is Nothing Then

                        Dim win As IntPtr = FindWindowEx(Handle, IntPtr.Zero, "VideoRenderer", "ActiveMovie Window")
                        If win = IntPtr.Zero Then
                            win = FindWindowExByClass(Handle, IntPtr.Zero, "VideoRenderer", IntPtr.Zero)
                        End If
                        If win = IntPtr.Zero Then
                            win = FindWindowExByTitle(Handle, IntPtr.Zero, IntPtr.Zero, "ActiveMovie Window")
                        End If
                        If win <> IntPtr.Zero Then
                            mfrm = New MessagedWindow(win)
                        End If

                    End If

#End If

                    If mbolKeepNativeSize Then

                        ' Set the rendering to the native size of the video

#If WINDOWLESS Then

                        ' Make sure it fits
                        If (rect.Height >= intHeight) And (rect.Width >= intWidth) Then

                            ' Calcualte the new rect
                            destRect = New DsRect((rect.Right - rect.Left - intWidth) / 2, (rect.Bottom - rect.Top - intHeight) / 2, 0, 0)
                            destRect.right = destRect.left + intWidth
                            destRect.bottom = destRect.top + intHeight

                        Else
                            destRect = New DsRect(rect.ToRectangle)
                        End If

                        .SetVideoPosition(srcRECT, destRect)

#Else

                        ' Get the native size
                        filterinfo = New FilterInfo
                        DirectCast(mvmrVideo, IBaseFilter).QueryFilterInfo(filterinfo)
                        ibv = DirectCast(filterinfo.pGraph, IBasicVideo2)
                        ibv.GetVideoSize(intWidth, intHeight)

                        ' Release objects
                        ibv = Nothing
                        If filterinfo.pGraph IsNot Nothing Then
                            Marshal.ReleaseComObject(filterinfo.pGraph)
                        End If
                        filterinfo.pGraph = Nothing
                        filterinfo = Nothing

                        ' Make sure it fits
                        If (rect.Height >= intHeight) And (rect.Width >= intWidth) Then

                            ' Calcualte the new rect
                            Dim newRect As RECT = New RECT((rect.Right - rect.Left - intWidth) / 2, (rect.Bottom - rect.Top - intHeight) / 2, 0, 0)
                            newRect.Right = newRect.Left + intWidth
                            newRect.Bottom = newRect.Top + intHeight
                            .SetWindowPosition(newRect.Left, newRect.Top, newRect.Width, newRect.Height)

                        Else

                            ' Stretch the image to fit window
                            .SetWindowPosition(rect.Left, rect.Top, rect.Width, rect.Height)

                        End If

#End If
                    Else

                        ' Stretch the image to fit window
#If WINDOWLESS Then
                        destRect = New DsRect(rect.ToRectangle)
                        .SetVideoPosition(srcRECT, destRect)
#Else
                        .SetWindowPosition(rect.Left, rect.Top, rect.Width, rect.Height)
#End If

                    End If

#If WINDOWLESS Then

                    If mobjContainer IsNot Nothing Then

                        Dim g As Drawing.Graphics = mobjContainer.CreateGraphics
                        Dim hdc As IntPtr = g.GetHdc
                        .RepaintVideo(Handle, hdc)
                        g.ReleaseHdc(hdc)
                        hdc = IntPtr.Zero
                        g.Dispose()
                        g = Nothing

                    Else

                        Dim hdc As IntPtr = GetDC(Handle)
                        .RepaintVideo(Handle, hdc)
                        Call ReleaseDC(Handle, hdc)
                        hdc = IntPtr.Zero

                    End If
#Else
                    .put_Visible(OABool.True)
#End If

                End If

            End With

        Catch ex As Exception
        Finally

            ibv = Nothing
            If filterinfo.pGraph IsNot Nothing Then
                Marshal.ReleaseComObject(filterinfo.pGraph)
            End If
            filterinfo.pGraph = Nothing
            filterinfo = Nothing

        End Try

    End Sub

#End Region

#Region " Properties "

    Public Property BackColor As Integer
        Get
            Return mintBackColor
        End Get
        Set(value As Integer)
            mintBackColor = value
            Call Refresh()
        End Set
    End Property

    Public ReadOnly Property GetNativeSize As Drawing.Size
        Get

            Dim filterinfo As FilterInfo = Nothing
            Dim ibv As IBasicVideo2 = Nothing
            Dim intWidth As Integer = 0
            Dim intHeight As Integer = 0

            Try

                If mvmrVideo Is Nothing Then
                    Return New Drawing.Size(0, 0)
                End If

#If WINDOWLESS Then
                Dim obj As Object = Nothing
                If TypeOf mvmrVideo Is VideoMixingRenderer9 Then
                    obj = DirectCast(mvmrVideo, IVMRWindowlessControl9)
                Else
                    obj = DirectCast(mvmrVideo, IVMRWindowlessControl)
                End If
                With obj
#Else
                With DirectCast(mvmrVideo, IVideoWindow)
#End If

#If WINDOWLESS Then
                    
                    .GetNativeVideoSize(intWidth, intHeight, 0, 0)

#Else


                    ' Get the native size
                    filterinfo = New FilterInfo
                    DirectCast(mvmrVideo, IBaseFilter).QueryFilterInfo(filterinfo)
                    ibv = DirectCast(filterinfo.pGraph, IBasicVideo2)
                    ibv.GetVideoSize(intWidth, intHeight)

                    ' Release objects
                    ibv = Nothing
                    If filterinfo.pGraph IsNot Nothing Then
                        Marshal.ReleaseComObject(filterinfo.pGraph)
                    End If
                    filterinfo.pGraph = Nothing
                    filterinfo = Nothing

#End If

                    Return New Drawing.Size(intWidth, intHeight)

                End With

            Catch ex As Exception
                Return New Drawing.Size(0, 0)
            Finally

                ibv = Nothing

                If filterinfo.pGraph IsNot Nothing Then
                    Marshal.ReleaseComObject(filterinfo.pGraph)
                End If
                filterinfo.pGraph = Nothing
                filterinfo = Nothing

            End Try

        End Get
    End Property

    ''' <summary>
    ''' Sets the rendering to the native size of the incoming video
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>NOTE: Setting this value to True will set KeepAspectRatio to True</remarks>
    Public Property KeepNativeSize As Boolean
        Get
            Return mbolKeepNativeSize
        End Get
        Set(ByVal value As Boolean)
            If value Then
                KeepAspectRatio = True
            End If

            mbolKeepNativeSize = value

            Call Refresh()

        End Set
    End Property

    ''' <summary>
    ''' Returns the average frame rate of the rendering
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is different from AverageFrameRate in that this is the average frame rate of the rendering as it renders.</remarks>
    Public ReadOnly Property AverageActualFrameRate As Single
        Get

            Dim intFrameRate As Integer = 0

            Try

                If mvmrVideo Is Nothing Then
                    Return 0
                End If

                If DirectCast(mvmrVideo, IQualProp).get_AvgFrameRate(intFrameRate) >= 0 Then
                    Return intFrameRate
                End If

                Return 0

            Catch ex As Exception
                Return 0
            End Try

        End Get
    End Property

    ''' <summary>
    ''' Gets the average frame rate from the video source
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is different from AverageActualFrameRate in that it's the frame rate of the video source</remarks>
    Public ReadOnly Property AverageFrameRate As Single
        Get

            Dim dblFrameRate As Double = 0
            Dim ibf As IBaseFilter = Nothing
            Dim filterinfo As FilterInfo = Nothing
            Dim ibv As IBasicVideo2 = Nothing
            Dim pin As IPin = Nothing
            Dim media As AMMediaType = Nothing

            Try

                If msngLastFrameRate > 0 Then
                    Return msngLastFrameRate
                End If

                If mvmrVideo Is Nothing Then
                    Return 29.97
                End If

                If True Then

                    pin = DsFindPin.ByConnectionStatus(mvmrVideo, PinConnectedStatus.Connected, 0)
                    If pin Is Nothing Then
                        Return 29.97
                    End If
                    media = New AMMediaType
                    If pin.ConnectionMediaType(media) < 0 Then
                        Return 29.97
                    End If
                    Dim vhdr As VideoInfoHeader = New VideoInfoHeader
                    Marshal.PtrToStructure(media.formatPtr, vhdr)
                    dblFrameRate = vhdr.AvgTimePerFrame / 10000000

                Else

                    ibf = DirectCast(mvmrVideo, IBaseFilter)
                    If ibf Is Nothing Then
                        Return 29.97
                    End If

                    filterinfo = New FilterInfo
                    If ibf.QueryFilterInfo(filterinfo) < 0 Then
                        Return 29.97
                    End If
                    ibv = DirectCast(filterinfo.pGraph, IBasicVideo2)
                    If ibv Is Nothing Then
                        Return 29.97
                    End If
                    If ibv.get_AvgTimePerFrame(dblFrameRate) < 0 Then
                        Return 29.97
                    End If

                    ibv = Nothing
                    If filterinfo.pGraph IsNot Nothing Then
                        Marshal.ReleaseComObject(filterinfo.pGraph)
                    End If
                    filterinfo = Nothing
                    ibf = Nothing

                    If dblFrameRate = 0 Then
                        msngLastFrameRate = 29.97
                        Return 29.97
                    End If

                End If

                msngLastFrameRate = Single.Parse((1.0 / dblFrameRate).ToString)
                Return msngLastFrameRate

            Catch ex As Exception

                msngLastFrameRate = 0
                Return 0

            Finally

                If media IsNot Nothing Then
                    DsUtils.FreeAMMediaType(media)
                End If
                media = Nothing

                If pin IsNot Nothing Then
                    Marshal.ReleaseComObject(pin)
                End If
                pin = Nothing

                ibv = Nothing
                If filterinfo.pGraph IsNot Nothing Then
                    Marshal.ReleaseComObject(filterinfo.pGraph)
                End If
                filterinfo.pGraph = Nothing
                filterinfo = Nothing
                ibf = Nothing

            End Try

        End Get
    End Property

    ''' <summary>
    ''' Gets/Sets whether to keep the aspect ratio of the video preview.
    ''' NOTE: If the video is unable to be set, this property will always return FALSE
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property KeepAspectRatio() As Boolean
        Get

            Try

                If mvmrVideo IsNot Nothing Then

#If WINDOWLESS Then

                    Dim mode As VMR9AspectRatioMode = VMR9AspectRatioMode.None

                    If DirectCast(mvmrVideo, IVMRWindowlessControl9).GetAspectRatioMode(mode) >= 0 Then
                        mbolKeepAspectRatio = (mode = VMR9AspectRatioMode.LetterBox)
                    End If

#Else

                    Dim mode As VMRAspectRatioMode = VMRAspectRatioMode.None

                    If DirectCast(mvmrVideo, IVMRAspectRatioControl9).GetAspectRatioMode(mode) >= 0 Then
                        mbolKeepAspectRatio = (mode = VMRAspectRatioMode.LetterBox)
                    End If

#End If

                End If

            Catch ex As Exception
            End Try

            Return mbolKeepAspectRatio

        End Get
        Set(ByVal value As Boolean)

            mbolKeepAspectRatio = value

            Try

                Dim intHR As Integer = 0

                If mvmrVideo IsNot Nothing Then

#If WINDOWLESS Then

                    If TypeOf (mvmrVideo) Is VideoMixingRenderer9 Then

                        If mbolKeepAspectRatio Then
                            intHR = DirectCast(mvmrVideo, IVMRWindowlessControl9).SetAspectRatioMode(VMR9AspectRatioMode.LetterBox)
                        Else
                            intHR = DirectCast(mvmrVideo, IVMRWindowlessControl9).SetAspectRatioMode(VMR9AspectRatioMode.None)
                        End If

                    Else

                        If mbolKeepAspectRatio Then
                            intHR = DirectCast(mvmrVideo, IVMRWindowlessControl).SetAspectRatioMode(VMRAspectRatioMode.LetterBox)
                        Else
                            intHR = DirectCast(mvmrVideo, IVMRWindowlessControl).SetAspectRatioMode(VMRAspectRatioMode.None)
                        End If

                    End If

#Else

                    If TypeOf (mvmrVideo) Is VideoMixingRenderer9 Then

                        If mbolKeepAspectRatio Then
                            intHR = DirectCast(mvmrVideo, IVMRAspectRatioControl9).SetAspectRatioMode(VMRAspectRatioMode.LetterBox)
                        Else
                            intHR = DirectCast(mvmrVideo, IVMRAspectRatioControl9).SetAspectRatioMode(VMRAspectRatioMode.None)
                        End If

                    Else

                        If mbolKeepAspectRatio Then
                            intHR = DirectCast(mvmrVideo, IVMRAspectRatioControl).SetAspectRatioMode(VMRAspectRatioMode.LetterBox)
                        Else
                            intHR = DirectCast(mvmrVideo, IVMRAspectRatioControl).SetAspectRatioMode(VMRAspectRatioMode.None)
                        End If

                    End If

#End If

                    If intHR < 0 Then
                        mbolKeepAspectRatio = False
                    End If

                End If

            Catch ex As Exception
                mbolKeepAspectRatio = False
            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the handle of the window in order to render the video/audio
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Handle() As Integer
        Get
            Return mintPreviewHandle
        End Get
        Set(ByVal value As Integer)

            If mintPreviewHandle <> value Then

                mintPreviewHandle = value
                Call Refresh()

            End If

        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the renderer to real full screen
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FullScreen As Boolean
        Get

            Dim bolRetVal As OABool = OABool.False

            Try

                If mvmrVideo IsNot Nothing Then

#If Not WINDOWLESS Then

                    If DirectCast(mvmrVideo, IVideoWindow).get_FullScreenMode(bolRetVal) >= 0 Then
                        Return bolRetVal = OABool.True
                    End If

#End If

                End If


            Catch ex As Exception
            End Try

            Return False

        End Get
        Set(ByVal value As Boolean)

            Try

                If mvmrVideo IsNot Nothing Then

#If Not WINDOWLESS Then

                    If value Then
                        DirectCast(mvmrVideo, IVideoWindow).put_FullScreenMode(OABool.True)
                    Else
                        DirectCast(mvmrVideo, IVideoWindow).put_FullScreenMode(OABool.False)
                    End If

#End If

                End If

            Catch ex As Exception

            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets/Sets the Brightness value of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Brightness As Single Implements IProcAmp.Brightness
        Get

            Dim intValue As Integer = 0
            Dim flags As VideoProcAmpFlags = VideoProcAmpFlags.None
            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Get the info
                info = Me.BrightnessInfo
                If Not info.IsSupported Then

                    ' Not supported, so return the default
                    msngBrightness = info.Default
                    Return info.Default

                End If

                intValue = info.Default

                If mProcAmpVideo IsNot Nothing Then

                    ' We are gonna get the value of the device
                    If mProcAmpVideo.Get(VideoProcAmpProperty.Brightness, intValue, flags) >= 0 Then

                        ' Return what was read
                        msngBrightness = intValue
                        Return msngBrightness

                    End If

                    ' Getting value failed
                    msngBrightness = 0
                    Return 0

                ElseIf mvmrVideo IsNot Nothing Then

                    ' We are gonna get the value of the renderer
                    ctrl = New VMR9ProcAmpControl
                    ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                    If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                        ' Return what was read
                        msngBrightness = ctrl.Brightness
                        Return ctrl.Brightness

                    End If

                    ' Getting value failed
                    msngBrightness = 0
                    Return 0

                Else

                    ' Neither object exists
                    Return msngBrightness

                End If

            Catch ex As Exception
                Return msngBrightness
            Finally

                info = Nothing
                ctrl = Nothing

            End Try

        End Get
        Set(ByVal value As Single)

            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Save the new value
                info = Me.BrightnessInfo

                ' Get the range info
                If info.IsSupported Then

                    ' Make sure it's within min, max
                    If value < info.Min Then
                        value = info.Min
                    End If

                    If value > info.Max Then
                        value = info.Max
                    End If

                    ' If the object exists, let's change it
                    If mProcAmpVideo IsNot Nothing Then

                        ' Set the device
                        mProcAmpVideo.Set(VideoProcAmpProperty.Brightness, Convert.ToInt32(value), VideoProcAmpFlags.Manual)

                    ElseIf mvmrVideo IsNot Nothing Then

                        ' Set the renderer
                        ctrl = New VMR9ProcAmpControl
                        ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                        If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                            ctrl.Brightness = value
                            DirectCast(mvmrVideo, IVMRMixerControl9).SetProcAmpControl(0, ctrl)

                        End If

                    End If

                End If

            Catch ex As Exception
            Finally

                ' Get the value that we just set as a check
                msngBrightness = Me.Brightness

                ctrl = Nothing
                info = Nothing

            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets information about the Brightness setting of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property BrightnessInfo As IProcAmp.ProcAmpInfo Implements IProcAmp.BrightnessInfo
        Get
            Return GetInfo(VideoProcAmpProperty.Brightness, VMR9ProcAmpControlFlags.Brightness)
        End Get
    End Property

    ''' <summary>
    ''' Gets/Sets the Contrast value of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Contrast As Single Implements IProcAmp.Contrast
        Get

            Dim intValue As Integer = 0
            Dim flags As VideoProcAmpFlags = VideoProcAmpFlags.None
            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Get the info
                info = Me.ContrastInfo
                If Not info.IsSupported Then

                    ' Not supported, so return the default
                    msngContrast = info.Default
                    Return info.Default

                End If

                intValue = info.Default

                If mProcAmpVideo IsNot Nothing Then

                    ' We are gonna get the value of the device
                    If mProcAmpVideo.Get(VideoProcAmpProperty.Contrast, intValue, flags) >= 0 Then

                        ' Return what was read
                        msngContrast = intValue
                        Return msngContrast

                    End If

                    ' Getting value failed
                    msngContrast = 0
                    Return 0

                ElseIf mvmrVideo IsNot Nothing Then

                    ' We are gonna get the value of the renderer
                    ctrl = New VMR9ProcAmpControl
                    ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                    If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                        ' Return what was read
                        msngContrast = ctrl.Contrast
                        Return ctrl.Contrast

                    End If

                    ' Getting value failed
                    msngContrast = 0
                    Return 0

                Else

                    ' Neither object exists
                    Return msngContrast

                End If

            Catch ex As Exception
                Return msngContrast
            Finally

                info = Nothing
                ctrl = Nothing

            End Try

        End Get
        Set(ByVal value As Single)

            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Save the new value
                info = Me.ContrastInfo

                ' Get the range info
                If info.IsSupported Then

                    ' Make sure it's within min, max
                    If value < info.Min Then
                        value = info.Min
                    End If

                    If value > info.Max Then
                        value = info.Max
                    End If

                    ' If the object exists, let's change it
                    If mProcAmpVideo IsNot Nothing Then

                        ' Set the device
                        mProcAmpVideo.Set(VideoProcAmpProperty.Contrast, Convert.ToInt32(value), VideoProcAmpFlags.Manual)

                    ElseIf mvmrVideo IsNot Nothing Then

                        ' Set the renderer
                        ctrl = New VMR9ProcAmpControl
                        ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                        If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                            ctrl.Contrast = value
                            DirectCast(mvmrVideo, IVMRMixerControl9).SetProcAmpControl(0, ctrl)

                        End If

                    End If

                End If

            Catch ex As Exception
            Finally

                ' Get the value that we just set as a check
                msngContrast = Me.Contrast

                ctrl = Nothing
                info = Nothing

            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets information about the Contrast setting of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ContrastInfo As IProcAmp.ProcAmpInfo Implements IProcAmp.ContrastInfo
        Get
            Return GetInfo(VideoProcAmpProperty.Contrast, VMR9ProcAmpControlFlags.Contrast)
        End Get
    End Property

    ''' <summary>
    ''' Gets/Sets the Hue value of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Hue As Single Implements IProcAmp.Hue
        Get

            Dim intValue As Integer = 0
            Dim flags As VideoProcAmpFlags = VideoProcAmpFlags.None
            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Get the info
                info = Me.HueInfo
                If Not info.IsSupported Then

                    ' Not supported, so return the default
                    msngHue = info.Default
                    Return info.Default

                End If

                intValue = info.Default

                If mProcAmpVideo IsNot Nothing Then

                    ' We are gonna get the value of the device
                    If mProcAmpVideo.Get(VideoProcAmpProperty.Hue, intValue, flags) >= 0 Then

                        ' Return what was read
                        msngHue = intValue
                        Return msngHue

                    End If

                    ' Getting value failed
                    msngHue = 0
                    Return 0

                ElseIf mvmrVideo IsNot Nothing Then

                    ' We are gonna get the value of the renderer
                    ctrl = New VMR9ProcAmpControl
                    ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                    If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                        ' Return what was read
                        msngHue = ctrl.Hue
                        Return ctrl.Hue

                    End If

                    ' Getting value failed
                    msngHue = 0
                    Return 0

                Else

                    ' Neither object exists
                    Return msngHue

                End If

            Catch ex As Exception
                Return msngHue
            Finally

                info = Nothing
                ctrl = Nothing

            End Try

        End Get
        Set(ByVal value As Single)

            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Save the new value
                info = Me.HueInfo

                ' Get the range info
                If info.IsSupported Then

                    ' Make sure it's within min, max
                    If value < info.Min Then
                        value = info.Min
                    End If

                    If value > info.Max Then
                        value = info.Max
                    End If

                    ' If the object exists, let's change it
                    If mProcAmpVideo IsNot Nothing Then

                        ' Set the device
                        mProcAmpVideo.Set(VideoProcAmpProperty.Hue, Convert.ToInt32(value), VideoProcAmpFlags.Manual)

                    ElseIf mvmrVideo IsNot Nothing Then

                        ' Set the renderer
                        ctrl = New VMR9ProcAmpControl
                        ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                        If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                            ctrl.Hue = value
                            DirectCast(mvmrVideo, IVMRMixerControl9).SetProcAmpControl(0, ctrl)

                        End If

                    End If

                End If

            Catch ex As Exception
            Finally

                ' Get the value that we just set as a check
                msngHue = Me.Hue

                ctrl = Nothing
                info = Nothing

            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets information about the Hue setting of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property HueInfo As IProcAmp.ProcAmpInfo Implements IProcAmp.HueInfo
        Get
            Return GetInfo(VideoProcAmpProperty.Hue, VMR9ProcAmpControlFlags.Hue)
        End Get
    End Property

    ''' <summary>
    ''' Gets/Sets the Saturation value of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Saturation As Single Implements IProcAmp.Saturation
        Get

            Dim intValue As Integer = 0
            Dim flags As VideoProcAmpFlags = VideoProcAmpFlags.None
            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Get the info
                info = Me.SaturationInfo
                If Not info.IsSupported Then

                    ' Not supported, so return the default
                    msngSaturation = info.Default
                    Return info.Default

                End If

                intValue = info.Default

                If mProcAmpVideo IsNot Nothing Then

                    ' We are gonna get the value of the device
                    If mProcAmpVideo.Get(VideoProcAmpProperty.Saturation, intValue, flags) >= 0 Then

                        ' Return what was read
                        msngSaturation = intValue
                        Return msngSaturation

                    End If

                    ' Getting value failed
                    msngSaturation = 0
                    Return 0

                ElseIf mvmrVideo IsNot Nothing Then

                    ' We are gonna get the value of the renderer
                    ctrl = New VMR9ProcAmpControl
                    ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                    If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                        ' Return what was read
                        msngSaturation = ctrl.Saturation
                        Return ctrl.Saturation

                    End If

                    ' Getting value failed
                    msngSaturation = 0
                    Return 0

                Else

                    ' Neither object exists
                    Return msngSaturation

                End If

            Catch ex As Exception
                Return msngSaturation
            Finally

                info = Nothing
                ctrl = Nothing

            End Try

        End Get
        Set(ByVal value As Single)

            Dim info As IProcAmp.ProcAmpInfo = Nothing
            Dim ctrl As VMR9ProcAmpControl = Nothing

            Try

                ' Save the new value
                info = Me.SaturationInfo

                ' Get the range info
                If info.IsSupported Then

                    ' Make sure it's within min, max
                    If value < info.Min Then
                        value = info.Min
                    End If

                    If value > info.Max Then
                        value = info.Max
                    End If

                    ' If the object exists, let's change it
                    If mProcAmpVideo IsNot Nothing Then

                        ' Set the device
                        mProcAmpVideo.Set(VideoProcAmpProperty.Saturation, Convert.ToInt32(value), VideoProcAmpFlags.Manual)

                    ElseIf mvmrVideo IsNot Nothing Then

                        ' Set the renderer
                        ctrl = New VMR9ProcAmpControl
                        ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                        If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                            ctrl.Saturation = value
                            DirectCast(mvmrVideo, IVMRMixerControl9).SetProcAmpControl(0, ctrl)

                        End If

                    End If

                End If

            Catch ex As Exception
            Finally

                ' Get the value that we just set as a check
                msngSaturation = Me.Saturation

                ctrl = Nothing
                info = Nothing

            End Try

        End Set
    End Property

    ''' <summary>
    ''' Gets information about the Saturation setting of the video device first.  If that doesn't exist, it uses the renderer
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SaturationInfo As IProcAmp.ProcAmpInfo Implements IProcAmp.SaturationInfo
        Get
            Return GetInfo(VideoProcAmpProperty.Saturation, VMR9ProcAmpControlFlags.Saturation)
        End Get
    End Property

#End Region

#End Region

#Region " Event Handlers "

#If WINDOWLESS Then

    Private Sub mfrm_DisplayChanged() Handles mfrm.DisplayChanged

        Try

            If mvmrVideo IsNot Nothing Then

                If TypeOf (mvmrVideo) Is VideoMixingRenderer9 Then
                    DirectCast(mvmrVideo, IVMRWindowlessControl9).DisplayModeChanged()
                Else
                    DirectCast(mvmrVideo, IVMRWindowlessControl).DisplayModeChanged()
                End If

            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub mobjContainer_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles mobjContainer.Paint

        'Dim g As Drawing.Graphics = e.Graphics
        'g.Clear(Drawing.Color.Black)
        'g.FillRectangle(Drawing.Brushes.Black, Screen.FromControl(mobjContainer).Bounds)
        'g.Flush()
        'g = Nothing
        Call Refresh()

    End Sub

#Else

    Private Sub mfrm_DoubleClick() Handles mfrm.DoubleClick

        Try

            If mobjContainer IsNot Nothing Then

                Dim method As System.Reflection.MethodInfo = mobjContainer.GetType().GetMethod("OnDoubleClick", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
                If (method IsNot Nothing) Then
                    method.Invoke(mobjContainer, New Object() {New EventArgs})
                End If

            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub mobjContainer_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles mobjContainer.MouseDown
        mbolMouseDown = True
    End Sub

    Private Sub mobjContainer_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles mobjContainer.MouseUp

        Try

            If mbolMouseDown Then

                Dim method As System.Reflection.MethodInfo = mobjContainer.GetType().GetMethod("OnMouseClick", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
                If (method IsNot Nothing) Then
                    method.Invoke(mobjContainer, New Object() {e})
                End If

            End If
        Catch ex As Exception
        Finally
            mbolMouseDown = False
        End Try

    End Sub

#End If

    Private Sub mobjContainer_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles mobjContainer.Resize, mobjContainer.Move
        Call Refresh()
    End Sub

#End Region

#Region " Private Scope "

    Private Function GetInfo(ByVal [Property] As VideoProcAmpProperty, ByVal [Flag] As VMR9ProcAmpControlFlags) As IProcAmp.ProcAmpInfo

        Dim info As IProcAmp.ProcAmpInfo = Nothing
        Dim ctrl As VMR9ProcAmpControl = Nothing
        Dim range As VMR9ProcAmpControlRange = Nothing

        Try

            ' Create default object
            info = New IProcAmp.ProcAmpInfo
            info.Default = 0
            info.IsSupported = False
            info.Max = 0
            info.Min = 0
            info.Step = 0

            If mProcAmpVideo IsNot Nothing Then

                ' Get info from the device
                Dim flags As VideoProcAmpFlags = VideoProcAmpFlags.None
                Dim intMin As Integer = 0, intMax As Integer = 0, intStep As Integer = 0, intDefault As Integer = 0
                If mProcAmpVideo.GetRange([Property], intMin, intMax, intStep, intDefault, flags) >= 0 Then

                    info.IsSupported = True
                    info.Min = intMin
                    info.Max = intMax
                    info.Step = intStep
                    info.Default = intDefault

                End If

            ElseIf mvmrVideo IsNot Nothing Then

                'Get info from the renderer
                ctrl = New VMR9ProcAmpControl
                ctrl.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControl))

                ' Make sure it's supported
                If Not TypeOf mvmrVideo Is IVMRMixerControl9 Then
                    Return info
                End If

                If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControl(0, ctrl) >= 0 Then

                    If (ctrl.dwFlags And [Flag]) = [Flag] Then

                        ' Get the range
                        range = New VMR9ProcAmpControlRange
                        range.dwSize = Marshal.SizeOf(GetType(VMR9ProcAmpControlRange))
                        range.dwProperty = [Flag]

                        If DirectCast(mvmrVideo, IVMRMixerControl9).GetProcAmpControlRange(0, range) >= 0 Then

                            info.IsSupported = True
                            info.Default = range.DefaultValue
                            info.Max = range.MaxValue
                            info.Min = range.MinValue
                            info.Step = range.StepSize

                        End If

                    End If

                End If

            End If

            Return info

        Catch ex As Exception
            Return New IProcAmp.ProcAmpInfo
        Finally

            ctrl = Nothing
            range = Nothing
            info = Nothing

        End Try

    End Function

    Private Sub FreeResources(ByVal Managed As Boolean)

        Try

            If Managed Then

                If mfrm IsNot Nothing Then
                    mfrm.Dispose()
                    mfrm = Nothing
                End If
                If mwndFlickerFree IsNot Nothing Then
                    mwndFlickerFree.Dispose()
                    mwndFlickerFree = Nothing
                End If
                mobjContainer = Nothing

            Else

                mvmrVideo = Nothing
                mProcAmpVideo = Nothing

            End If

        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region " IDisposable Support "

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Call FreeResources(True)
            End If

            Call FreeResources(False)

        End If
        Me.disposedValue = True
    End Sub

    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

#Region " Protected Scope "

    Protected Class FlickerFreeWindow
        Inherits NativeWindow
        Implements IDisposable

        Public Property EraseBackground As Boolean = True

        Sub New(ByVal Handle As IntPtr)
            MyBase.New()
            MyBase.AssignHandle(Handle)
        End Sub

        Protected Overrides Sub WndProc(ByRef m As Message)
            Select Case m.Msg
                Case &H14 'WM_ERASEBKGND
                    If Not EraseBackground Then
                        Return
                    End If
            End Select
            MyBase.WndProc(m)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    MyBase.ReleaseHandle()
                End If

            End If
            Me.disposedValue = True
        End Sub

        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
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

    Protected Class MessagedWindow
        Inherits NativeWindow
        Implements IDisposable

        Event DisplayChanged()
        Event Click()
        Event DoubleClick()

        Sub New(ByVal Handle As IntPtr)
            MyBase.New()
            MyBase.AssignHandle(Handle)
        End Sub

        Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
            If m.HWnd = Me.Handle Then

                Select Case m.Msg
                    Case &H203 ' WM_LBUTTONDBCLK
                        RaiseEvent DoubleClick()
                    Case &H7E ' WM_DISPLAYCHANGE
                        RaiseEvent DisplayChanged()
                End Select

            End If
            MyBase.WndProc(m)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    MyBase.ReleaseHandle()
                End If

            End If
            Me.disposedValue = True
        End Sub

        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(False)
            MyBase.Finalize()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

#End Region

End Class


