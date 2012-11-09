Imports MjpegProcessor

''' <summary>
''' Interaction logic for MainWindow.xaml
''' </summary>
Partial Public Class MainWindow
    Inherits Window
    Private _mjpeg As MjpegDecoder

    Public Sub New()
        InitializeComponent()
        _mjpeg = New MjpegDecoder
        AddHandler _mjpeg.FrameReady, AddressOf mjpeg_FrameReady
    End Sub

    Private Sub Start_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _mjpeg.ParseStream(New Uri("http://192.168.2.200/img/video.mjpeg"))
    End Sub

    Private Sub mjpeg_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
        image.Source = e.BitmapImage
    End Sub
End Class