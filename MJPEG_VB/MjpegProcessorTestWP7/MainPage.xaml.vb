Imports System.Windows
Imports Microsoft.Phone.Controls
Imports MjpegProcessor

Partial Public Class MainPage
    Inherits PhoneApplicationPage
    Private _mjpeg As MjpegDecoder

    Public Sub New()
        InitializeComponent()
        _mjpeg = New MjpegDecoder
        AddHandler _mjpeg.FrameReady, AddressOf mjpeg_FrameReady
    End Sub

    Private Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' this works best as a 320x240 stream at 15fps...anything larger/faster and the phone can't quite keep up
        _mjpeg.ParseStream(New Uri("http://192.168.2.200/img/video.mjpeg"))
    End Sub

    Private Sub mjpeg_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
        image.Source = e.BitmapImage
    End Sub
End Class