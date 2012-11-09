Imports MjpegProcessor

Partial Public Class Form1
    Inherits Form
    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnStart_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStart.Click
        Dim mjpeg As New MjpegDecoder
        AddHandler mjpeg.FrameReady, AddressOf mjpeg_FrameReady
        mjpeg.ParseStream(New Uri("http://192.168.2.200/img/video.mjpeg"))
    End Sub

    Private Sub mjpeg_FrameReady(ByVal sender As Object, ByVal e As FrameReadyEventArgs)
        image.Image = e.Bitmap
    End Sub
End Class