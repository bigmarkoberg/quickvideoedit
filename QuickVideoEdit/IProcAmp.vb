Public Interface IProcAmp

    Enum ProcAmpType

        Brightness = 0
        Contrast = 1
        Saturation = 2
        Hue = 4

    End Enum

    ''' <summary>
    ''' Information regarding Brightness, Saturation, Contrast, and Hue
    ''' </summary>
    ''' <remarks></remarks>
    Structure ProcAmpInfo

        ''' <summary>
        ''' Indicator whether item is supported
        ''' </summary>
        ''' <remarks></remarks>
        Public IsSupported As Boolean

        ''' <summary>
        ''' Minimum value supported
        ''' </summary>
        ''' <remarks></remarks>
        Public Min As Single

        ''' <summary>
        ''' Maximum value supported
        ''' </summary>
        ''' <remarks></remarks>
        Public Max As Single

        ''' <summary>
        ''' Default value
        ''' </summary>
        ''' <remarks></remarks>
        Public [Default] As Single

        ''' <summary>
        ''' The incremental step size between the Min and Max values
        ''' </summary>
        ''' <remarks></remarks>
        Public [Step] As Single

    End Structure

    ''' <summary>
    ''' Gets the Hue information from the device passed in
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property HueInfo() As ProcAmpInfo
    ''' <summary>
    ''' Gets/Sets the Hue for the video
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Hue() As Single

    ''' <summary>
    ''' Gets the Contrast information from the device passed in
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ContrastInfo() As ProcAmpInfo
    ''' <summary>
    ''' Gets/Sets the Contrast for the video
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Contrast() As Single

    ''' <summary>
    ''' Gets the Brightness information from the device passed in
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property BrightnessInfo() As ProcAmpInfo
    ''' <summary>
    ''' Gets/Sets the Brightness for the video
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Brightness() As Single

    ''' <summary>
    ''' Gets the Saturation information from the device passed in
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property SaturationInfo() As ProcAmpInfo
    ''' <summary>
    ''' Gets/Sets the Saturation for the video
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property Saturation() As Single

End Interface
