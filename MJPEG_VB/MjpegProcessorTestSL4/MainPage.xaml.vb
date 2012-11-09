Imports System.Windows.Controls
Imports MjpegProcessor

Partial Public Class MainPage
    Inherits UserControl
    Private Delegate Sub AssignImageDelegate(ByVal buff() As Byte)

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub UserControl_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' need to be out of browser to get past crossdomain.xml not existing on the cam
        ' NOTE: this will only work on cams which properly send MJPEG data to an IE user agent header
        ' I've found several cams (Cisco being one) that tries to be "smart" and sends a single JPEG frame
        ' instead of an MJPEG stream since destkop IE doesn't properly support the MJPEG codec
        If Application.Current.IsRunningOutOfBrowser Then
            Dim mjpeg As New MjpegDecoder
            AddHandler mjpeg.FrameReady, AddressOf mjpeg_FrameReady
            mjpeg.ParseStream(New Uri("http://192.168.2.200/img/video.mjpeg"))
        End If
    End Sub

    Private Sub mjpeg_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
        image.Source = e.BitmapImage
    End Sub
End Class