Imports DirectShowLib
Imports System.Runtime.InteropServices

''' <summary>
''' A collection of utility functions that are performed on filters or entir filter graphs
''' </summary>
''' <remarks></remarks>
Public Class FilterUtils

    Public Enum Merit As UInteger
        Preferred = &H800000
        Normal = &H600000
        Unlikely = &H400000
        DoNotUse = &H200000
    End Enum

    <DllImport("kernel32.dll", SetLastError:=True, EntryPoint:="FreeLibrary")> _
    Private Shared Function FreeLibrary(ByVal hModule As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Shared Function LoadLibrary(ByVal lpFileName As String) As IntPtr
    End Function
    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True)> _
    Private Shared Function GetProcAddress(ByVal hModule As IntPtr, ByVal procName As String) As IntPtr
    End Function

    <ComVisible(True), ComImport, _
    Guid("00000001-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)> _
    Friend Interface IClassFactory

        <PreserveSig> _
        Function CreateInstance(<[In], MarshalAs(UnmanagedType.Interface)> pUnkOuter As Object,
                       ByRef riid As Guid,
                       <Out, MarshalAs(UnmanagedType.Interface)> ByRef obj As Object) As Integer

        <PreserveSig> _
        Function LockServer(<[In]> fLock As Boolean) As Integer
    End Interface

    Delegate Function DllGetClassObjectDelegate(<MarshalAs(UnmanagedType.LPStruct)> clsid As Guid, _
                                                <MarshalAs(UnmanagedType.LPStruct)> riid As Guid, _
                                                <MarshalAs(UnmanagedType.IUnknown), Out> ByRef ppv As Object) As Integer

    <DllImport("olepro32.dll", CharSet:=CharSet.Unicode)> _
    Private Shared Sub OleCreatePropertyFrame(<[In]()> ByVal hwndowner As IntPtr, _
        <[In]()> ByVal x As UShort, <[In]()> ByVal y As UShort, _
        <[In](), MarshalAs(UnmanagedType.LPWStr)> ByVal lpszCaption As String, _
        <[In]()> ByVal cObjects As UInteger, _
        <[In](), MarshalAs(UnmanagedType.IUnknown)> ByRef lplpUnk As Object, _
        <[In]()> ByVal cPages As UInteger, _
        <[In]()> ByVal lpPageClsIS As IntPtr, _
        <[In]()> ByVal lcid As UShort, _
        <[In]()> ByVal dwReserved As UShort, _
        <[In]()> ByVal lpvReserved As IntPtr)
    End Sub

    ''' <summary>
    ''' Enumerator for find interfaces
    ''' </summary>
    ''' <remarks></remarks>
    Friend Enum Interfaces

        FileSourceFilter = 0
        VideoWindow = 1
        AMDirectSound = 2
        VMRBitmap9 = 4

    End Enum

    Friend Shared Function FreeCOMFilter(i As IntPtr) As Boolean
        Try
            Return FreeLibrary(i)
        Catch ex As Exception
            Return False
        End Try
    End Function

    <DllImport("ole32.dll")> _
    Private Shared Function CreateClassMoniker(<[In], MarshalAs(UnmanagedType.LPStruct)> rclsid As Guid, <Out> ppmk As ComTypes.IMoniker) As Integer
    End Function

    <DllImport("ole32.dll")> _
    Private Shared Function CreatePointerMoniker(<MarshalAs(UnmanagedType.IUnknown)> punk As Object, <Out> ppmk As System.Runtime.InteropServices.ComTypes.IMoniker) As Integer
    End Function

    Private Shared mdictFilterNames As Dictionary(Of Guid, String) = Nothing

    ''' <summary>
    ''' Gets the list of filters currently used in filter graph by their guid and name
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function FilterInfoUsed(fg As IGraphBuilder) As Dictionary(Of Guid, String)

        Dim dictInfo As Dictionary(Of Guid, String) = Nothing
        Dim ief As IEnumFilters = Nothing
        Dim aibf(0) As IBaseFilter

        Try

            If fg Is Nothing Then
                Return New Dictionary(Of Guid, String)
            End If

            If fg.EnumFilters(ief) < 0 Then
                Return New Dictionary(Of Guid, String)
            End If

            If mdictFilterNames Is Nothing Then

                Dim dictNames As Dictionary(Of Guid, String) = New Dictionary(Of Guid, String)
                Dim e As ICreateDevEnum = Nothing
                Dim iem As System.Runtime.InteropServices.ComTypes.IEnumMoniker = Nothing
                Dim d As DateTime = Now

                Try

                    e = New CreateDevEnum
                    e.CreateClassEnumerator(FilterCategory.LegacyAmFilterCategory, iem, 0)
                    If iem Is Nothing Then
                        Exit Try
                    End If

                    iem.Reset()

                    Dim mon(0) As Runtime.InteropServices.ComTypes.IMoniker
                    Do While (iem.Next(1, mon, Nothing) >= 0)

                        If mon(0) Is Nothing Then
                            Exit Do
                        End If

                        Dim monguid As Guid = Guid.Empty
                        Dim rrr As IBaseFilter = Nothing
                        Try
                            mon(0).BindToObject(Nothing, Nothing, GetType(IBaseFilter).GUID, rrr)
                            rrr.GetClassID(monguid)
                            Marshal.ReleaseComObject(rrr)
                            rrr = Nothing
                        Catch ex As Exception
                            Debug.WriteLine(ex.ToString)
                        Finally
                            If rrr IsNot Nothing Then
                                Marshal.ReleaseComObject(rrr)
                                rrr = Nothing
                            End If
                        End Try

                        If monguid <> Guid.Empty Then

                            Dim objBag As IPropertyBag = Nothing
                            mon(0).BindToStorage(Nothing, Nothing, GetType(IPropertyBag).GUID, objBag)

                            If objBag IsNot Nothing Then

                                Dim strFriendlyName As String = ""
                                If objBag.Read("FriendlyName", strFriendlyName, Nothing) >= 0 Then

                                    If dictNames.ContainsKey(monguid) Then

                                        Debug.WriteLine(dictNames(monguid))

                                        If Not Array.Exists(dictNames(monguid).Split("|"), Function(element)
                                                                                               Return element = strFriendlyName
                                                                                           End Function) Then

                                            dictNames(monguid) = dictNames(monguid) & "|" & strFriendlyName

                                        End If

                                    Else
                                        dictNames.Add(monguid, strFriendlyName)
                                    End If

                                End If

                                Marshal.ReleaseComObject(objBag)
                                objBag = Nothing

                            End If

                        End If
                        Marshal.ReleaseComObject(mon(0))
                        mon(0) = Nothing

                    Loop

                    e.CreateClassEnumerator(FilterCategory.VideoInputDevice, iem, 0)
                    If iem Is Nothing Then
                        Exit Try
                    End If

                    iem.Reset()

                    Do While (iem.Next(1, mon, Nothing) >= 0)

                        If mon(0) Is Nothing Then
                            Exit Do
                        End If

                        Dim monguid As Guid = Guid.Empty
                        Dim rrr As IBaseFilter = Nothing
                        Try
                            mon(0).BindToObject(Nothing, Nothing, GetType(IBaseFilter).GUID, rrr)
                            rrr.GetClassID(monguid)
                            Marshal.ReleaseComObject(rrr)
                            rrr = Nothing
                        Catch ex As Exception
                            Debug.WriteLine(ex.ToString)
                        Finally
                            If rrr IsNot Nothing Then
                                Marshal.ReleaseComObject(rrr)
                                rrr = Nothing
                            End If
                        End Try

                        If monguid <> Guid.Empty Then

                            Dim objBag As IPropertyBag = Nothing
                            mon(0).BindToStorage(Nothing, Nothing, GetType(IPropertyBag).GUID, objBag)

                            If objBag IsNot Nothing Then

                                Dim strFriendlyName As String = ""
                                If objBag.Read("FriendlyName", strFriendlyName, Nothing) >= 0 Then

                                    If dictNames.ContainsKey(monguid) Then

                                        Debug.WriteLine(dictNames(monguid))

                                        If Not Array.Exists(dictNames(monguid).Split("|"), Function(element)
                                                                                               Return element = strFriendlyName
                                                                                           End Function) Then

                                            dictNames(monguid) = dictNames(monguid) & "|" & strFriendlyName

                                        End If

                                    Else
                                        dictNames.Add(monguid, strFriendlyName)
                                    End If

                                End If

                                Marshal.ReleaseComObject(objBag)
                                objBag = Nothing

                            End If

                        End If
                        Marshal.ReleaseComObject(mon(0))
                        mon(0) = Nothing

                    Loop

                    e.CreateClassEnumerator(FilterCategory.AudioInputDevice, iem, 0)
                    If iem Is Nothing Then
                        Exit Try
                    End If

                    iem.Reset()

                    Do While (iem.Next(1, mon, Nothing) >= 0)

                        If mon(0) Is Nothing Then
                            Exit Do
                        End If

                        Dim monguid As Guid = Guid.Empty
                        Dim rrr As IBaseFilter = Nothing
                        Try
                            mon(0).BindToObject(Nothing, Nothing, GetType(IBaseFilter).GUID, rrr)
                            rrr.GetClassID(monguid)
                            Marshal.ReleaseComObject(rrr)
                            rrr = Nothing
                        Catch ex As Exception
                            Debug.WriteLine(ex.ToString)
                        Finally
                            If rrr IsNot Nothing Then
                                Marshal.ReleaseComObject(rrr)
                                rrr = Nothing
                            End If
                        End Try

                        If monguid <> Guid.Empty Then

                            Dim objBag As IPropertyBag = Nothing
                            mon(0).BindToStorage(Nothing, Nothing, GetType(IPropertyBag).GUID, objBag)

                            If objBag IsNot Nothing Then

                                Dim strFriendlyName As String = ""
                                If objBag.Read("FriendlyName", strFriendlyName, Nothing) >= 0 Then

                                    If dictNames.ContainsKey(monguid) Then

                                        Debug.WriteLine(dictNames(monguid))

                                        If Not Array.Exists(dictNames(monguid).Split("|"), Function(element)
                                                                                               Return element = strFriendlyName
                                                                                           End Function) Then

                                            dictNames(monguid) = dictNames(monguid) & "|" & strFriendlyName

                                        End If

                                    Else
                                        dictNames.Add(monguid, strFriendlyName)
                                    End If

                                End If

                                Marshal.ReleaseComObject(objBag)
                                objBag = Nothing

                            End If

                        End If
                        Marshal.ReleaseComObject(mon(0))
                        mon(0) = Nothing

                    Loop

                    mdictFilterNames = dictNames

                Catch ex As Exception
                    Debug.WriteLine(ex.ToString)
                Finally

                    If iem IsNot Nothing Then
                        Marshal.ReleaseComObject(iem)
                        iem = Nothing
                    End If

                    If e IsNot Nothing Then
                        Marshal.ReleaseComObject(e)
                        e = Nothing
                    End If

                    Debug.WriteLine("Complete in " & (Now - d).TotalSeconds.ToString("0.000") & " secs")

                End Try

            End If

            dictInfo = New Dictionary(Of Guid, String)

            ief.Reset()

            Do While (ief.Next(1, aibf, Nothing) >= 0)

                If (aibf Is Nothing) OrElse (aibf.GetLength(0) <= 0) Then
                    Exit Do
                End If

                If aibf(0) Is Nothing Then
                    Exit Do
                End If

                Dim g As Guid = Guid.Empty
                aibf(0).GetClassID(g)

                If (g <> Guid.Empty) AndAlso (Not dictInfo.ContainsKey(g)) Then

                    If mdictFilterNames IsNot Nothing AndAlso (mdictFilterNames.ContainsKey(g)) Then
                        dictInfo.Add(g, mdictFilterNames(g))
                    Else

                        Dim info As FilterInfo = New FilterInfo
                        If aibf(0).QueryFilterInfo(info) >= 0 Then

                            dictInfo.Add(g, info.achName)

                            If info.pGraph IsNot Nothing Then
                                Marshal.ReleaseComObject(info.pGraph)
                            End If

                        End If
                        info = Nothing

                    End If

                End If

                Marshal.ReleaseComObject(aibf(0))
                aibf(0) = Nothing

            Loop

            Return dictInfo

        Catch ex As Exception
            Return New Dictionary(Of Guid, String)
        Finally

            If aibf IsNot Nothing Then

                If aibf(0) IsNot Nothing Then
                    Marshal.ReleaseComObject(aibf(0))
                End If
                aibf(0) = Nothing

            End If

            If ief IsNot Nothing Then
                Marshal.ReleaseComObject(ief)
            End If
            ief = Nothing

        End Try

    End Function

    Private Overloads Shared Function CreateCOMObjectFromPath(ByVal Path As String, ByVal ObjectGuid As Guid, ByVal CastingType As Type, _
                                                              ByRef ReturnObject As Object, ByRef LibraryPointer As IntPtr) As Boolean

        Try

            ReturnObject = Nothing
            LibraryPointer = IntPtr.Zero

            If String.IsNullOrEmpty(Path) OrElse (Not IO.File.Exists(Path)) Then
                Return False
            End If

            LibraryPointer = LoadLibrary(Path)
            Dim ui As IntPtr = GetProcAddress(LibraryPointer, "DllGetClassObject")

            Dim d As DllGetClassObjectDelegate = Marshal.GetDelegateForFunctionPointer(ui, GetType(DllGetClassObjectDelegate))
            Dim fac As Object = Nothing
            Dim hr As Integer = d.Invoke(ObjectGuid, New Guid("00000000-0000-0000-C000-000000000046"), fac)
            Dim ifac As IClassFactory = DirectCast(fac, IClassFactory)

            Dim o As Object = Nothing
            hr = ifac.CreateInstance(Nothing, CastingType.GUID, o)
            ReturnObject = o

            Return (ReturnObject IsNot Nothing) AndAlso (LibraryPointer <> IntPtr.Zero)

        Catch ex As Exception

            If ReturnObject IsNot Nothing Then
                Marshal.ReleaseComObject(ReturnObject)
                ReturnObject = Nothing
            End If

            If LibraryPointer <> IntPtr.Zero Then
                Call FreeCOMFilter(LibraryPointer)
                LibraryPointer = IntPtr.Zero
            End If

            Return False

        End Try

    End Function

    Friend Overloads Shared Function CreateCOMObjectFromPath(Path As String, ObjectGuid As Guid, _
                                                             ByVal CastingType As Type, ByRef ReturnObject As Object) As IntPtr

        Dim i As IntPtr = IntPtr.Zero

        Try
            ' We are going to try multiple paths
            Dim lstPaths As List(Of String) = New List(Of String)
            lstPaths.Add(Path)
            lstPaths.Add(IO.Path.Combine(IO.Path.GetDirectoryName(Path), "filters", IO.Path.GetFileName(Path)))
            If Environment.Is64BitProcess Then
                lstPaths.Add(IO.Path.Combine(IO.Path.GetDirectoryName(Path), "x64", IO.Path.GetFileName(Path)))
                lstPaths.Add(IO.Path.Combine(IO.Path.GetDirectoryName(Path), "filters\x64", IO.Path.GetFileName(Path)))
            Else
                lstPaths.Add(IO.Path.Combine(IO.Path.GetDirectoryName(Path), "x86", IO.Path.GetFileName(Path)))
                lstPaths.Add(IO.Path.Combine(IO.Path.GetDirectoryName(Path), "filters\x86", IO.Path.GetFileName(Path)))
            End If
            lstPaths.Add(IO.Path.Combine(IO.Path.GetDirectoryName(Path), "buildfiles", IO.Path.GetFileName(Path)))

            For Each strPath As String In lstPaths

                If CreateCOMObjectFromPath(strPath, ObjectGuid, CastingType, ReturnObject, i) Then
                    Return i
                End If

                If ReturnObject IsNot Nothing Then
                    Marshal.ReleaseComObject(ReturnObject)
                    ReturnObject = Nothing
                End If
                If i <> IntPtr.Zero Then
                    Call FreeCOMFilter(i)
                    i = IntPtr.Zero
                End If

            Next

            Return i

        Catch ex As Exception

            If ReturnObject IsNot Nothing Then
                Marshal.ReleaseComObject(ReturnObject)
                ReturnObject = Nothing
            End If
            If i <> IntPtr.Zero Then
                Call FreeCOMFilter(i)
                i = IntPtr.Zero
            End If

            Return IntPtr.Zero

        Finally

        End Try

    End Function

    Friend Shared Function CreateCOMFilterFromPath(Path As String, ObjectGUID As Guid, ByRef filter As IBaseFilter) As IntPtr

        Dim i As IntPtr = IntPtr.Zero

        Try

            Return CreateCOMObjectFromPath(Path, ObjectGUID, GetType(DirectShowLib.IBaseFilter), filter)

        Catch ex As Exception
            If filter IsNot Nothing Then
                Marshal.ReleaseComObject(filter)
                filter = Nothing
            End If
            If i <> IntPtr.Zero Then
                Call FreeCOMFilter(i)
            End If
            Debug.WriteLine(ex)
            Return Nothing
        Finally

        End Try
    End Function

    ''' <summary>
    ''' Show the property pages for a filter
    ''' </summary>
    ''' <param name="ibf">Filter to show the property pages for</param>
    ''' <remarks></remarks>
    Friend Shared Sub ShowPropertyPage(ByVal ibf As IBaseFilter)

        Dim qqq As ISpecifyPropertyPages = Nothing
        Dim cu As DsCAUUID = Nothing

        Dim hr As Integer = 0

        Try

            If ibf Is Nothing Then
                Return
            End If

            qqq = DirectCast(ibf, ISpecifyPropertyPages)

            If qqq Is Nothing Then
                Return
            End If

            cu = New DsCAUUID

            hr = qqq.GetPages(cu)

            If hr >= 0 Then
                OleCreatePropertyFrame(0, 0, 0, "Properties", 1, ibf, cu.cElems, cu.pElems, 0, 0, 0)
            End If

            cu = Nothing
            qqq = Nothing

        Catch ex As Exception
        Finally

            cu = Nothing
            qqq = Nothing

        End Try

    End Sub

    Friend Shared Function GetProperties(ByVal strFile As String, ByRef EndTime As Long, ByRef FrameRate As Double, _
                                   ByRef Width As Integer, ByRef Height As Integer, ByVal Audio As Boolean) As Boolean

        Dim d As DES.IMediaDet = New DES.MediaDet
        Dim hr As Integer = d.put_Filename(strFile)
        Dim dbl As Double = 0

        Dim intStreams As Integer = 0
        hr = hr Or d.get_OutputStreams(intStreams)

        If (hr < 0) Or (intStreams <= 0) Then

            Marshal.ReleaseComObject(d)
            d = Nothing

            'Dim objPlayback As MediaPlayback = New MediaPlayback(strFile, IntPtr.Zero)
            'EndTime = (Double.Parse(objPlayback.TotalFrames) / objPlayback.FrameRate) * Double.Parse(10000000)
            'FrameRate = objPlayback.FrameRate
            'Width = objPlayback.VideoSync.VideoSize.width
            'Height = objPlayback.VideoSync.VideoSize.height
            'objPlayback.Dispose()
            'objPlayback = Nothing

            'hr = 0

        Else

            For intIndex As Integer = 0 To intStreams - 1

                hr = d.put_CurrentStream(intIndex)

                Dim media As AMMediaType = New AMMediaType

                hr = hr Or d.get_StreamMediaType(media)

                If Not Audio Then

                    If media.formatType = DirectShowLib.FormatType.VideoInfo Then

                        hr = hr Or d.get_StreamLength(dbl)
                        hr = hr Or d.get_FrameRate(FrameRate)
                        EndTime = dbl * Double.Parse(10000000)

                        Dim vhdr As VideoInfoHeader = New VideoInfoHeader
                        Marshal.PtrToStructure(media.formatPtr, vhdr)
                        Width = vhdr.BmiHeader.Width
                        Height = vhdr.BmiHeader.Height
                        vhdr = Nothing

                        DsUtils.FreeAMMediaType(media)
                        media = Nothing

                        Exit For

                    End If

                Else

                    If media.formatType = DirectShowLib.FormatType.WaveEx Then

                        hr = hr Or d.get_StreamLength(dbl)
                        EndTime = dbl * Double.Parse(10000000)

                        DsUtils.FreeAMMediaType(media)
                        media = Nothing

                        Exit For

                    End If

                End If

                DsUtils.FreeAMMediaType(media)
                media = Nothing

            Next

            Marshal.ReleaseComObject(d)
            d = Nothing

        End If

        Return hr >= 0

    End Function

    ''' <summary>
    ''' Disconnect all the filters in a filter graph
    ''' </summary>
    ''' <param name="igb">Filter graph to search in</param>
    ''' <param name="bolRemoveFilters">Indicates whether to remove the filters from the fitler graph or not</param>
    ''' <remarks></remarks>
    Friend Shared Sub DisconnectFilters(ByVal igb As IGraphBuilder, Optional ByVal bolRemoveFilters As Boolean = True)

        Dim ief As IEnumFilters = Nothing
        Dim aibf(0) As IBaseFilter

        Try

            If IsNothing(igb) Then
                Return
            End If

            igb.EnumFilters(ief)

            If IsNothing(ief) Then
                Return
            End If

            ief.Reset()

            ' Enumerate through all the filters
            While ief.Next(1, aibf, Nothing) >= 0

                If IsNothing(aibf(0)) Then
                    Exit While
                End If

                ' Disconnect al lthe pins on the filter
                DisconnectPins(aibf(0), igb)

                ' Remove the filter from the filter graph if requested
                If bolRemoveFilters Then

                    igb.RemoveFilter(aibf(0))
                    ief.Reset()

                End If

                Marshal.ReleaseComObject(aibf(0))
                aibf(0) = Nothing

            End While

            Marshal.ReleaseComObject(ief)
            ief = Nothing

        Catch ex As Exception

            If Not IsNothing(aibf) Then

                If Not IsNothing(aibf(0)) Then
                    Marshal.ReleaseComObject(aibf(0))
                End If
                aibf(0) = Nothing

            End If

            If Not IsNothing(ief) Then
                Marshal.ReleaseComObject(ief)
            End If
            ief = Nothing

        End Try

    End Sub

    ''' <summary>
    ''' Disconnect all the pins on a particular filter
    ''' </summary>
    ''' <param name="ibf">Filter to disconnect all the pins on</param>
    ''' <param name="igb">Filter graph that the filter is in</param>
    ''' <remarks></remarks>
    Friend Shared Sub DisconnectPins(ByVal ibf As IBaseFilter, ByVal igb As IGraphBuilder)

        Dim iep As IEnumPins = Nothing
        Dim iConnected As IPin = Nothing
        Dim apin(0) As IPin

        Try

            If IsNothing(igb) Then
                Return
            End If

            If IsNothing(ibf) Then
                Return
            End If

            ibf.EnumPins(iep)

            If IsNothing(iep) Then
                Return
            End If

            iep.Reset()

            ' Enumerate through all the pins
            While iep.Next(1, apin, Nothing) >= 0

                If IsNothing(apin(0)) Then
                    Exit While
                End If

                ' Test connection status 
                apin(0).ConnectedTo(iConnected)

                If Not IsNothing(iConnected) Then

                    ' Disconenct at both ends
                    igb.Disconnect(apin(0))
                    igb.Disconnect(iConnected)

                    Marshal.ReleaseComObject(iConnected)
                    iConnected = Nothing

                End If

                Marshal.ReleaseComObject(apin(0))
                apin(0) = Nothing

            End While

            Marshal.ReleaseComObject(iep)
            iep = Nothing

        Catch ex As Exception

            If Not IsNothing(apin) Then

                If Not IsNothing(apin(0)) Then
                    Marshal.ReleaseComObject(apin(0))
                End If
                apin(0) = Nothing

            End If

            If Not IsNothing(iConnected) Then
                Marshal.ReleaseComObject(iConnected)
            End If
            iConnected = Nothing

            If Not IsNothing(iep) Then
                Marshal.ReleaseComObject(iep)
            End If
            iep = Nothing

        End Try

    End Sub

    ''' <summary>
    ''' Search for a pin in a filter for a particular direction, connection status, and index number
    ''' </summary>
    ''' <param name="ibf">Filter to search in</param>
    ''' <param name="dir">Direction of the requested pin</param>
    ''' <param name="Connected">Connection status of the requested pin</param>
    ''' <param name="Index">Index number of the requested pin</param>
    ''' <returns>IPin interface of the pin requested</returns>
    ''' <remarks></remarks>
    Friend Shared Function FindPinByConnectionAndDirection(ByVal ibf As IBaseFilter, ByVal dir As PinDirection, ByVal Connected As Boolean, ByVal Index As Integer) As IPin

        Dim intCounter As Integer = 0

        Dim pinRetVal As IPin = Nothing
        Dim pinConnected As IPin = Nothing

        Try

            If IsNothing(ibf) Then
                Return Nothing
            End If

            Do

                ' Search by direction
                pinRetVal = DsFindPin.ByDirection(ibf, dir, intCounter)
                If IsNothing(pinRetVal) Then
                    ' No more pins
                    Exit Do
                End If

                ' Check connection status
                pinRetVal.ConnectedTo(pinConnected)
                If ((Not IsNothing(pinConnected)) And Connected) Or (IsNothing(pinConnected) And (Not Connected)) Then

                    ' Check index number
                    If Index = 0 Then
                        Exit Do
                    Else
                        Index -= 1
                    End If

                End If

                If Not IsNothing(pinConnected) Then
                    Marshal.ReleaseComObject(pinConnected)
                End If
                pinConnected = Nothing

                Marshal.ReleaseComObject(pinRetVal)
                pinRetVal = Nothing

                intCounter += 1

            Loop

            If Not IsNothing(pinConnected) Then
                Marshal.ReleaseComObject(pinConnected)
            End If
            pinConnected = Nothing

            Return pinRetVal

        Catch ex As Exception

            If Not IsNothing(pinConnected) Then
                Marshal.ReleaseComObject(pinConnected)
            End If
            pinConnected = Nothing

            If Not IsNothing(pinRetVal) Then
                Marshal.ReleaseComObject(pinRetVal)
            End If
            pinRetVal = Nothing

            Return Nothing

        Finally

            pinConnected = Nothing
            pinRetVal = Nothing

        End Try

    End Function

    ''' <summary>
    ''' This function recursively searches backwards in the filter graph from a particular pin to find 
    ''' another pin with a particular direction, media type,index number, and conenction status
    ''' </summary>
    ''' <param name="Pin">The pin to start the search at</param>
    ''' <param name="dir">Direction of the requested pin</param>
    ''' <param name="MediaType">Media type of the requested pin</param>
    ''' <param name="Index">Index number of the requested pin</param>
    ''' <param name="Connected">Connection status of the requested pin</param>
    ''' <returns>IPin interface of the requested pin</returns>
    ''' <remarks></remarks>
    Friend Shared Function BackwardSearchFiltersForMediaType(ByVal Pin As IPin, ByVal dir As PinDirection, ByVal MediaType As System.Guid, ByVal Index As Integer, ByVal Connected As Boolean) As IPin

        Dim intCounter As Integer = 0

        Dim pinRetVal As IPin = Nothing
        Dim pinWorking As IPin = Nothing
        Dim pinConnected As IPin = Nothing
        Dim info As DirectShowLib.PinInfo = Nothing

        Try

            If IsNothing(Pin) Then
                Return Nothing
            End If

            If IsNothing(MediaType) Then
                Return Nothing
            End If

            ' First look at the current filter that the passed in pin is connected to
            info = New DirectShowLib.PinInfo
            Pin.QueryPinInfo(info)
            If IsNothing(info.filter) Then

                info = Nothing
                Return Nothing

            End If

            '  Look on the current filter
            pinRetVal = FindPinByMediaType(info.filter, dir, MediaType, Index, Connected)

            If IsNothing(pinRetVal) Then

                intCounter = 0

                ' Not on the current filter, so we need to recursively search the connected input pins on this filter
                Do

                    ' Find the next connected input pin
                    pinWorking = FindPinByConnectionAndDirection(info.filter, PinDirection.Input, True, intCounter)

                    If IsNothing(pinWorking) Then
                        ' No more connected input pins, so get out
                        Exit Do
                    End If

                    ' Get that pin its connected to
                    pinWorking.ConnectedTo(pinConnected)

                    ' Recursively call self to search that filter
                    pinRetVal = BackwardSearchFiltersForMediaType(pinConnected, dir, MediaType, Index, Connected)

                    If Not IsNothing(pinRetVal) Then
                        Exit Do
                    End If

                    intCounter += 1

                Loop

                If Not IsNothing(pinConnected) Then
                    Marshal.ReleaseComObject(pinConnected)
                End If
                pinConnected = Nothing

                If Not IsNothing(pinWorking) Then
                    Marshal.ReleaseComObject(pinWorking)
                End If
                pinWorking = Nothing

            End If

            Return pinRetVal

        Catch ex As Exception

            If Not IsNothing(pinConnected) Then
                Marshal.ReleaseComObject(pinConnected)
            End If
            pinConnected = Nothing

            If Not IsNothing(pinWorking) Then
                Marshal.ReleaseComObject(pinWorking)
            End If
            pinWorking = Nothing

            If Not IsNothing(pinRetVal) Then
                Marshal.ReleaseComObject(pinRetVal)
            End If
            pinRetVal = Nothing

            If Not IsNothing(info) Then

                If Not IsNothing(info.filter) Then
                    Marshal.ReleaseComObject(info.filter)
                End If
                info.filter = Nothing

            End If
            info = Nothing

            Return Nothing

        Finally
            info = Nothing
        End Try

    End Function

    ''' <summary>
    ''' Search a filter for a pin with a particular direction, media type, connection status, and index number
    ''' </summary>
    ''' <param name="ibf">The filter to search in</param>
    ''' <param name="dir">The direction of the requested pin</param>
    ''' <param name="MediaType">The media type of the requested pin</param>
    ''' <param name="Index">The index of the requested pin.  Zero based</param>
    ''' <param name="Connected">The connection status of the requested pin</param>
    ''' <returns>IPin interface of the pin matching the criteria passed-in</returns>
    ''' <remarks></remarks>
    Friend Shared Function FindPinByMediaType(ByVal ibf As IBaseFilter, ByVal dir As PinDirection, ByVal MediaType As System.Guid, ByVal Index As Integer, ByVal Connected As Boolean) As IPin

        Dim ipinRetVal As IPin = Nothing
        Dim ipinConnected As IPin = Nothing
        Dim iep As IEnumPins = Nothing
        Dim iem As IEnumMediaTypes = Nothing
        Dim apin(0) As IPin
        Dim amedia(0) As AMMediaType
        Dim direction As PinDirection = PinDirection.Input

        Try

            If IsNothing(ibf) Then
                Return Nothing
            End If

            If IsNothing(MediaType) Then
                Return Nothing
            End If

            ibf.EnumPins(iep)

            If IsNothing(iep) Then
                Return Nothing
            End If

            iep.Reset()

            ' Enumerate through all the pins in the filter
            While iep.Next(1, apin, Nothing) >= 0

                If IsNothing(apin(0)) Then
                    Exit While
                End If

                ' Check direction
                apin(0).QueryDirection(direction)
                If direction = dir Then

                    ' Check conenction status
                    apin(0).ConnectedTo(ipinConnected)

                    If (IsNothing(ipinConnected) And (Not Connected)) Or (Not IsNothing(ipinConnected) And Connected) Then

                        ' Enumerate through the preferred media types
                        apin(0).EnumMediaTypes(iem)

                        If Not IsNothing(iem) Then

                            iem.Reset()
                            While iem.Next(1, amedia, Nothing) >= 0

                                If IsNothing(amedia(0)) Then
                                    Exit While
                                End If

                                ' Match media type
                                If amedia(0).majorType = MediaType Then

                                    ' Check index number
                                    If Index = 0 Then

                                        If Not IsNothing(ipinConnected) Then
                                            Marshal.ReleaseComObject(ipinConnected)
                                        End If
                                        ipinConnected = Nothing

                                        DsUtils.FreeAMMediaType(amedia(0))
                                        amedia(0) = Nothing
                                        Marshal.ReleaseComObject(iem)
                                        iem = Nothing
                                        ipinRetVal = apin(0)
                                        apin(0) = Nothing
                                        Marshal.ReleaseComObject(iep)
                                        iep = Nothing

                                        Return ipinRetVal

                                    Else
                                        Index -= 1
                                    End If

                                End If

                                DsUtils.FreeAMMediaType(amedia(0))
                                amedia(0) = Nothing

                            End While

                            Marshal.ReleaseComObject(iem)
                            iem = Nothing

                        End If

                    End If

                    If Not IsNothing(ipinConnected) Then
                        Marshal.ReleaseComObject(ipinConnected)
                    End If
                    ipinConnected = Nothing

                End If

                Marshal.ReleaseComObject(apin(0))
                apin(0) = Nothing

            End While
            Marshal.ReleaseComObject(iep)
            iep = Nothing

            Return Nothing

        Catch ex As Exception

            If Not IsNothing(ipinConnected) Then
                Marshal.ReleaseComObject(ipinConnected)
            End If
            ipinConnected = Nothing

            If Not IsNothing(amedia) Then

                If Not IsNothing(amedia(0)) Then
                    Marshal.ReleaseComObject(amedia(0))
                End If
                amedia(0) = Nothing

            End If

            If Not IsNothing(apin) Then

                If Not IsNothing(apin(0)) Then
                    Marshal.ReleaseComObject(apin(0))
                End If
                apin(0) = Nothing

            End If

            If Not IsNothing(iem) Then
                Marshal.ReleaseComObject(iem)
            End If
            iem = Nothing

            If Not IsNothing(iep) Then
                Marshal.ReleaseComObject(iep)
            End If
            iep = Nothing

            Return Nothing

        End Try

    End Function

    ''' <summary>
    ''' Find a pin in a filter graph with a particular category, direction, and media type
    ''' </summary>
    ''' <param name="Connected">Returns whether the found pin is connected.  If there is no found pin, Connected will be False</param>
    ''' <param name="igb">The IGraphBuilder interface to the filter graph</param>
    ''' <param name="PinCategory">The category of the pin that we are searching for</param>
    ''' <param name="Direction">The direction of the pin we are searching for</param>
    ''' <param name="Type">The media type of the pin we are looking for</param>
    ''' <returns>IPin interface of the found pin</returns>
    ''' <remarks></remarks>
    Friend Shared Function FindPin(ByRef Connected As Boolean, ByVal igb As IGraphBuilder, ByVal PinCategory As System.Guid, ByVal Direction As PinDirection, _
        ByVal [Type] As System.Guid) As IPin

        Dim intCounter As Integer = 0

        Dim icgb As ICaptureGraphBuilder2 = Nothing
        Dim ief As IEnumFilters = Nothing
        Dim aibf(-1) As IBaseFilter

        Dim pinRetVal As IPin = Nothing
        Dim pinConencted As IPin = Nothing

        Connected = False

        Try

            ' Create a new ICaptureGraphBuilder2 interface to perform the find pin
            icgb = New CaptureGraphBuilder2

            If Not IsNothing(icgb) Then

                icgb.SetFiltergraph(igb)

                ' Enumerate through all the filters in the filter graph
                igb.EnumFilters(ief)
                If Not IsNothing(ief) Then

                    ief.Reset()

                    While ief.Next(1, aibf, Nothing) >= 0

                        If Not IsNothing(aibf(0)) Then
                            Exit While
                        End If

                        ' Perform the Find
                        icgb.FindPin(aibf(0), Direction, DsGuid.FromGuid(PinCategory), DsGuid.FromGuid([Type]), False, 0, pinRetVal)

                        Marshal.ReleaseComObject(aibf(0))
                        aibf(0) = Nothing

                        ' We got one?
                        If Not IsNothing(pinRetVal) Then
                            Exit While
                        End If

                    End While

                    Marshal.ReleaseComObject(ief)
                    ief = Nothing

                End If

                icgb.SetFiltergraph(Nothing)
                Marshal.ReleaseComObject(icgb)
                icgb = Nothing

            End If

            If Not IsNothing(pinRetVal) Then

                ' Get whether the pin is connected
                pinRetVal.ConnectedTo(pinConencted)

                If Not IsNothing(pinConencted) Then

                    Connected = True

                    Marshal.ReleaseComObject(pinConencted)
                    pinConencted = Nothing

                End If

            End If

            Return pinRetVal

        Catch ex As Exception

            If Not IsNothing(pinConencted) Then
                Marshal.ReleaseComObject(pinConencted)
            End If

            If Not IsNothing(aibf) Then

                If aibf.Length > 0 Then

                    For intCounter = 0 To aibf.Length - 1

                        If Not IsNothing(aibf(intCounter)) Then
                            Marshal.ReleaseComObject(aibf(intCounter))
                        End If
                        aibf(intCounter) = Nothing

                    Next

                    Array.Clear(aibf, 0, aibf.Length)

                End If

                aibf = Nothing

            End If

            If Not IsNothing(ief) Then
                Marshal.ReleaseComObject(ief)
            End If

            If Not IsNothing(icgb) Then

                icgb.SetFiltergraph(Nothing)
                Marshal.ReleaseComObject(icgb)

            End If

            Connected = False
            Return Nothing

        Finally

            ief = Nothing
            pinConencted = Nothing
            icgb = Nothing

        End Try

    End Function

    ''' <summary>
    ''' Find a particular interface within the filter graph
    ''' </summary>
    ''' <param name="igb">IGraphBuilder interface of the filter graph</param>
    ''' <param name="SearchInterface">The interface to search for.  This parameter is an enum, NOT the actual interface type</param>
    ''' <returns>IBaseFilter interface of the filter that uses the interface passed in</returns>
    ''' <remarks></remarks>
    Friend Shared Function FindInterface(ByVal igb As IGraphBuilder, ByVal SearchInterface As Interfaces) As IBaseFilter

        Dim ief As IEnumFilters = Nothing
        Dim ibf As IBaseFilter = Nothing
        Dim aibf() As IBaseFilter = Nothing

        Try

            If Not IsNothing(igb) Then

                ' Enumerate through all the filters in the filter graph
                If igb.EnumFilters(ief) >= 0 Then

                    ief.Reset()
                    ReDim aibf(0)

                    While (ief.Next(1, aibf, Nothing) >= 0)

                        If IsNothing(aibf(0)) Then
                            Exit While
                        End If

                        Try

                            ' Try to cast the interface we need
                            Select Case SearchInterface
                                Case Interfaces.FileSourceFilter
                                    Dim iTest As IFileSourceFilter = DirectCast(aibf(0), IFileSourceFilter)
                                Case Interfaces.VideoWindow
                                    Dim itest As IVideoWindow = DirectCast(aibf(0), IVideoWindow)
                                Case Interfaces.AMDirectSound
                                    Dim itest As IAMDirectSound = DirectCast(aibf(0), IAMDirectSound)
                                Case Interfaces.VMRBitmap9
                                    Dim itest As IVMRMixerBitmap9 = DirectCast(aibf(0), IVMRMixerBitmap9)
                            End Select

                            ibf = aibf(0)
                            aibf(0) = Nothing

                            Exit While

                        Catch ex As Exception

                        End Try

                        Marshal.ReleaseComObject(aibf(0))
                        aibf(0) = Nothing

                    End While

                    Marshal.ReleaseComObject(ief)
                    ief = Nothing

                End If

            End If

            Return ibf

        Catch ex As Exception

            If Not IsNothing(aibf) Then

                If Not IsNothing(aibf(0)) Then
                    Marshal.ReleaseComObject(aibf(0))
                    aibf(0) = Nothing
                End If

                aibf = Nothing

            End If

            If Not IsNothing(ief) Then

                Marshal.ReleaseComObject(ief)
                ief = Nothing

            End If

            Return Nothing

        Finally

            aibf = Nothing
            ief = Nothing
            ibf = Nothing

        End Try

    End Function

    ''' <summary>
    ''' Determine if the filter passed in is connected to anything else
    ''' </summary>
    ''' <param name="ibf"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function FilterConnected(ByVal ibf As IBaseFilter) As Boolean

        Dim hr As Integer = 0

        Dim bolRetVal As Boolean = False

        Dim iep As IEnumPins = Nothing
        Dim ip() As IPin = Nothing
        Dim ipConnected As IPin = Nothing

        Try

            If Not IsNothing(ibf) Then

                ' Enumerate through all the pins on the filter
                If ibf.EnumPins(iep) >= 0 Then

                    iep.Reset()
                    ReDim ip(0)

                    While (iep.Next(1, ip, Nothing) >= 0)

                        If IsNothing(ip(0)) Then
                            Exit While
                        End If

                        ip(0).ConnectedTo(ipConnected)
                        If Not IsNothing(ipConnected) Then

                            bolRetVal = True

                            Marshal.ReleaseComObject(ipConnected)
                            ipConnected = Nothing
                            Marshal.ReleaseComObject(ip(0))
                            ip(0) = Nothing
                            ip = Nothing

                            Exit While

                        End If

                        Marshal.ReleaseComObject(ip(0))
                        ip(0) = Nothing

                    End While

                    Marshal.ReleaseComObject(iep)
                    iep = Nothing

                End If

            End If

            Return bolRetVal

        Catch ex As Exception

            If Not IsNothing(ip) Then

                If Not IsNothing(ip(0)) Then

                    Marshal.ReleaseComObject(ip(0))
                    ip(0) = Nothing

                End If

            End If

            If Not IsNothing(ipConnected) Then
                Marshal.ReleaseComObject(ipConnected)
            End If

            If Not IsNothing(iep) Then
                Marshal.ReleaseComObject(iep)
            End If

            Return False

        Finally

            ipConnected = Nothing
            ip = Nothing
            iep = Nothing

        End Try

    End Function

    ''' <summary>
    ''' Insert a filter in the connected pin stream.
    ''' </summary>
    ''' <param name="ip">The input or output pin where the filter will be inserted</param>
    ''' <param name="PinDirection">The direction of ip</param>
    ''' <param name="ibfInsert">The filter to insert</param>
    ''' <param name="InputIndex">The input pin index in which the connect the filter being inserted</param>
    ''' <param name="OutputIndex">The output pin index in which the connect the filter being inserted</param>
    ''' <param name="igb">The IGraphBuilder interface of the filter graph</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function InsertFilter(ByVal ip As IPin, ByVal PinDirection As PinDirection, ByVal ibfInsert As IBaseFilter, _
        ByVal InputIndex As Integer, ByVal OutputIndex As Integer, ByVal igb As IGraphBuilder) As Boolean

        ' Assumption that the filter has already been added

        Dim hr As Integer = -1

        Dim [ipIn] As IPin = Nothing
        Dim ipIn2 As IPin = Nothing
        Dim ipOut As IPin = Nothing
        Dim ipOut2 As IPin = Nothing

        Try

            ' Validate the inputs
            If IsNothing(ip) Or IsNothing(ibfInsert) Or IsNothing(igb) Then
                Return False
            End If

            ' Set the original in and out pins
            If PinDirection = DirectShowLib.PinDirection.Input Then

                [ipIn] = ip
                ip.ConnectedTo(ipOut)

            Else

                ipOut = ip
                ip.ConnectedTo([ipIn])

            End If

            ' Get the in and out pins from the insertion filter
            ipIn2 = DsFindPin.ByDirection(ibfInsert, PinDirection.Input, InputIndex)
            ipOut2 = DsFindPin.ByDirection(ibfInsert, PinDirection.Output, OutputIndex)

            ' Make sure success
            If (Not IsNothing([ipIn])) And (Not IsNothing(ipIn2)) And (Not IsNothing(ipOut)) And (Not IsNothing(ipOut2)) Then

                ' Disconnect at the insertion point
                hr = igb.Disconnect(ipOut)
                If hr >= 0 Then

                    hr = igb.Disconnect([ipIn])

                    If hr < 0 Then
                        ' The second disconnect failed, so try to reconnect
                        igb.Connect(ipOut, [ipIn])
                    End If

                End If

                ' Check if disconnect was ok
                If hr >= 0 Then

                    ' Connect the output to the input
                    If igb.Connect(ipOut, ipIn2) >= 0 Then

                        ' Connect the output of the insertion filter to the input
                        If igb.Connect(ipOut2, [ipIn]) < 0 Then

                            hr = -1

                            ' Second conenction failed
                            ' Disconnect first connection
                            igb.Disconnect(ipOut)
                            igb.Disconnect(ipIn2)

                            ' Reconnect original
                            igb.Connect(ipOut, [ipIn])

                        End If

                    Else

                        hr = -1
                        ' First conenct failed so reconnect
                        igb.Connect(ipOut, [ipIn])

                    End If

                End If

            End If

            Return hr >= 0

        Catch ex As Exception
            Return False
        Finally

            ' We are done with the pins
            If Not IsNothing([ipIn]) Then
                Marshal.ReleaseComObject([ipIn])
            End If
            [ipIn] = Nothing

            If Not IsNothing(ipIn2) Then
                Marshal.ReleaseComObject(ipIn2)
            End If
            ipIn2 = Nothing

            If Not IsNothing(ipOut) Then
                Marshal.ReleaseComObject(ipOut)
            End If
            ipOut = Nothing

            If Not IsNothing(ipOut2) Then
                Marshal.ReleaseComObject(ipOut2)
            End If
            ipOut2 = Nothing

        End Try

    End Function

    Public Shared Function GetFilterMerit(g As Guid) As UInteger?

        Try

            Dim categories As Guid() = New Guid() {FilterCategory.LegacyAmFilterCategory, FilterCategory.ActiveMovieCategories, FilterCategory.AMKSAudio, FilterCategory.AMKSCapture, FilterCategory.AMKSCrossbar, FilterCategory.AMKSDataCompressor, _
                                            FilterCategory.AMKSRender, FilterCategory.AMKSSplitter, FilterCategory.AMKSTVAudio, FilterCategory.AMKSTVTuner, _
                                            FilterCategory.AMKSVBICodec, FilterCategory.AMKSVideo, FilterCategory.AudioCompressorCategory, FilterCategory.AudioEffects1Category, _
                                            FilterCategory.AudioEffects2Category, FilterCategory.AudioInputDevice, FilterCategory.AudioRendererCategory, _
                                            FilterCategory.DeviceControlCategory, FilterCategory.DMOFilterCategory, FilterCategory.KSAudioDevice, _
                                             FilterCategory.LTMMVideoProcessors, FilterCategory.MediaEncoderCategory, _
                                            FilterCategory.MediaMultiplexerCategory, FilterCategory.MidiRendererCategory, FilterCategory.TransmitCategory, _
                                            FilterCategory.VideoCompressorCategory, FilterCategory.VideoEffects1Category, FilterCategory.VideoEffects2Category, _
                                            FilterCategory.VideoInputDevice, FilterCategory.WDMStreamingEncoderDevices, FilterCategory.WDMStreamingMultiplexerDevices}

            For Each cat As Guid In categories

                For Each device As DsDevice In DsDevice.GetDevicesOfCat(cat)

                    Dim bag As IPropertyBag = Nothing
                    device.Mon.BindToStorage(Nothing, Nothing, GetType(IPropertyBag).GUID, bag)
                    If bag Is Nothing Then
                        Continue For
                    End If
                    Dim o As String = ""
                    Dim i As Integer = bag.Read("CLSID", o, Nothing)
                    If i <> 0 Then
                        Continue For
                    End If
                    If o.ToUpper <> "{" & g.ToString.ToUpper & "}" Then
                        Continue For
                    End If

                    Dim b As Byte() = Microsoft.Win32.Registry.GetValue("HKEY_CLASSES_ROOT\CLSID\{" & cat.ToString & "}\Instance\" & o, "FilterData", New Byte() {})
                    If b.Length < 8 Then
                        Continue For
                    End If
                    Dim value As UInteger = BitConverter.ToUInt32(b, 4)
                    Return value

                Next

            Next

            Return Nothing

        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Shared Function SetFilterMerit(g As Guid, merit As UInteger) As Boolean

        Try

            Dim categories As Guid() = New Guid() {FilterCategory.LegacyAmFilterCategory, FilterCategory.ActiveMovieCategories, FilterCategory.AMKSAudio, FilterCategory.AMKSCapture, FilterCategory.AMKSCrossbar, FilterCategory.AMKSDataCompressor, _
                                            FilterCategory.AMKSRender, FilterCategory.AMKSSplitter, FilterCategory.AMKSTVAudio, FilterCategory.AMKSTVTuner, _
                                            FilterCategory.AMKSVBICodec, FilterCategory.AMKSVideo, FilterCategory.AudioCompressorCategory, FilterCategory.AudioEffects1Category, _
                                            FilterCategory.AudioEffects2Category, FilterCategory.AudioInputDevice, FilterCategory.AudioRendererCategory, _
                                            FilterCategory.DeviceControlCategory, FilterCategory.DMOFilterCategory, FilterCategory.KSAudioDevice, _
                                             FilterCategory.LTMMVideoProcessors, FilterCategory.MediaEncoderCategory, _
                                            FilterCategory.MediaMultiplexerCategory, FilterCategory.MidiRendererCategory, FilterCategory.TransmitCategory, _
                                            FilterCategory.VideoCompressorCategory, FilterCategory.VideoEffects1Category, FilterCategory.VideoEffects2Category, _
                                            FilterCategory.VideoInputDevice, FilterCategory.WDMStreamingEncoderDevices, FilterCategory.WDMStreamingMultiplexerDevices}

            For Each cat As Guid In categories

                For Each device As DsDevice In DsDevice.GetDevicesOfCat(cat)

                    Dim bag As IPropertyBag = Nothing
                    device.Mon.BindToStorage(Nothing, Nothing, GetType(IPropertyBag).GUID, bag)
                    If bag Is Nothing Then
                        Continue For
                    End If
                    Dim o As String = ""
                    Dim i As Integer = bag.Read("CLSID", o, Nothing)
                    If i <> 0 Then
                        Continue For
                    End If
                    If o.ToUpper <> "{" & g.ToString.ToUpper & "}" Then
                        Continue For
                    End If

                    Dim b As Byte() = Microsoft.Win32.Registry.GetValue("HKEY_CLASSES_ROOT\CLSID\{" & cat.ToString & "}\Instance\" & o, "FilterData", New Byte() {})
                    If b.Length < 8 Then
                        Continue For
                    End If
                    Dim value As UInteger = BitConverter.ToUInt32(b, 4)
                    Dim bMerit As Byte() = BitConverter.GetBytes(merit)
                    b(4) = bMerit(0)
                    b(5) = bMerit(1)
                    b(6) = bMerit(2)
                    b(7) = bMerit(3)
                    Microsoft.Win32.Registry.SetValue("HKEY_CLASSES_ROOT\CLSID\{" & cat.ToString & "}\Instance\" & o, "FilterData", b)

                    Return True

                Next

            Next

            Return False

        Catch ex As Exception
            Return False
        End Try

    End Function

End Class
