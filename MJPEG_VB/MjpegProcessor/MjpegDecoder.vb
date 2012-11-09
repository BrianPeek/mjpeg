Imports System.Text
Imports System.Net
Imports System.IO

#If Not XNA Then
Imports System.Windows
Imports System.Windows.Media.Imaging
#End If

#If SILVERLIGHT Then
Imports System.Net.Browser
#ElseIf Not XNA Then
Imports System.Drawing
#End If

#If XNA OrElse WINDOWS_PHONE Then
Imports Microsoft.Xna.Framework.Graphics
#End If

Namespace MjpegProcessor
    Public Class MjpegDecoder
#If (Not SILVERLIGHT) AndAlso (Not XNA) Then
    ' WinForms & WPF
    Public Property Bitmap As Bitmap
#End If

#If Not XNA Then
    ' WPF and Silverlight
    Public Property BitmapImage As BitmapImage
#End If

        ' magic 2 byte header for JPEG images
        Private JpegHeader() As Byte = {&HFF, &HD8}

        ' pull down 1024 bytes at a time
        Private Const ChunkSize As Integer = 1024

        ' used to cancel reading the stream
        Private _streamActive As Boolean

        ' current encoded JPEG image
        Private privateCurrentFrame As Byte()
        Public Property CurrentFrame As Byte()
            Get
                Return privateCurrentFrame
            End Get
            Private Set(ByVal value As Byte())
                privateCurrentFrame = value
            End Set
        End Property

#If Not XNA Then
    ' event to get the buffer above handed to you
    Public Event FrameReady As EventHandler(Of FrameReadyEventArgs)
#End If

        Public Sub New()
#If Not XNA Then
        BitmapImage = New BitmapImage
#End If
        End Sub

        Public Sub ParseStream(ByVal uri As Uri)
#If SILVERLIGHT Then
			HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp)
#End If
            Dim request = CType(HttpWebRequest.Create(uri), HttpWebRequest)

#If SILVERLIGHT Then
			' start the stream immediately
			request.AllowReadStreamBuffering = False
#End If
            ' asynchronously get a response
            request.BeginGetResponse(AddressOf OnGetResponse, request)
        End Sub

        Public Sub StopStream()
            _streamActive = False
        End Sub

#If XNA OrElse WINDOWS_PHONE Then
        Public Function GetMjpegFrame(ByVal graphicsDevice As GraphicsDevice) As Texture2D
            ' create a Texture2D from the current byte buffer
            If CurrentFrame IsNot Nothing Then Return Texture2D.FromStream(graphicsDevice, New MemoryStream(CurrentFrame, 0, CurrentFrame.Length))
            Return Nothing
        End Function
#End If
        Private Sub OnGetResponse(ByVal asyncResult As IAsyncResult)
            Dim resp As HttpWebResponse
            Dim buff() As Byte
            Dim imageBuffer(1024 * 1024 - 1) As Byte
            Dim s As Stream

            ' get the response
            Dim req = CType(asyncResult.AsyncState, HttpWebRequest)
            resp = CType(req.EndGetResponse(asyncResult), HttpWebResponse)

            ' find our magic boundary value
            Dim contentType = resp.Headers("Content-Type")
            If (Not String.IsNullOrEmpty(contentType)) AndAlso
                (Not contentType.Contains("=")) Then Throw New Exception("Invalid content-type header.  The camera is likely not returning a proper MJPEG stream.")
            Dim boundary = resp.Headers("Content-Type").Split("="c)(1)
            Dim boundaryBytes() = Encoding.UTF8.GetBytes(If(boundary.StartsWith("--"), boundary, "--" & boundary))

            s = resp.GetResponseStream()
            Dim br As New BinaryReader(s)

            _streamActive = True

            buff = br.ReadBytes(ChunkSize)

            Do While _streamActive
                Dim size As Integer

                ' find the JPEG header
                Dim imageStart = buff.Find(JpegHeader)

                If imageStart <> -1 Then
                    ' copy the start of the JPEG image to the imageBuffer
                    size = buff.Length - imageStart
                    Array.Copy(buff, imageStart, imageBuffer, 0, size)

                    Do
                        buff = br.ReadBytes(ChunkSize)

                        ' find the boundary text
                        Dim imageEnd = buff.Find(boundaryBytes)
                        If imageEnd <> -1 Then
                            ' copy the remainder of the JPEG to the imageBuffer
                            Array.Copy(buff, 0, imageBuffer, size, imageEnd)
                            size += imageEnd

                            ' create a single JPEG frame
                            CurrentFrame = New Byte(size - 1) {}
                            Array.Copy(imageBuffer, 0, CurrentFrame, 0, size)
#If Not XNA Then
                        ProcessFrame(CurrentFrame)
#End If
                            ' copy the leftover data to the start
                            Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd)

                            ' fill the remainder of the buffer with new data and start over
                            Dim temp() = br.ReadBytes(imageEnd)

                            Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length)
                            Exit Do
                        End If

                        ' copy all of the data to the imageBuffer
                        Array.Copy(buff, 0, imageBuffer, size, buff.Length)
                        size += buff.Length
                    Loop
                End If
            Loop
            resp.Close()
        End Sub

        Private Sub ProcessFrame(ByVal frameBuffer() As Byte)
#If SILVERLIGHT Then
			' need to get this back on the UI thread
				' resets the BitmapImage to the new frame
				' tell whoever's listening that we have a frame to draw
            Deployment.Current.Dispatcher.BeginInvoke(CType(Sub()
                                                                BitmapImage.SetSource(New MemoryStream(frameBuffer, 0, frameBuffer.Length))
                                                                RaiseEvent FrameReady(Me, New FrameReadyEventArgs With
                                                                                          {.FrameBuffer = CurrentFrame,
                                                                                           .BitmapImage = BitmapImage})
                                                            End Sub, Action))
#End If

#If (Not SILVERLIGHT) AndAlso (Not XNA) Then
        ' I assume if there's an Application.Current then we're in WPF, not WinForms
        If Application.Current IsNot Nothing Then
            ' get it on the UI thread
            ' create a new BitmapImage from the JPEG bytes
            ' tell whoever's listening that we have a frame to draw
                Application.Current.Dispatcher.BeginInvoke(
                    CType(Sub()
                              BitmapImage = New BitmapImage
                              BitmapImage.BeginInit()
                              BitmapImage.StreamSource = New MemoryStream(frameBuffer)
                              BitmapImage.EndInit()
                              RaiseEvent FrameReady(Me, New FrameReadyEventArgs With
                                                        {.FrameBuffer = CurrentFrame,
                                                         .Bitmap = Bitmap,
                                                         .BitmapImage = BitmapImage})
                          End Sub, Action))
        Else
            ' create a simple GDI+ happy Bitmap
            Bitmap = New Bitmap(New MemoryStream(frameBuffer))

            ' tell whoever's listening that we have a frame to draw
                RaiseEvent FrameReady(Me, New FrameReadyEventArgs With
                                          {.FrameBuffer = CurrentFrame,
                                           .Bitmap = Bitmap,
                                           .BitmapImage = BitmapImage})
        End If
#End If
        End Sub
    End Class

    Public Module Extensions
        <System.Runtime.CompilerServices.Extension()>
        Public Function Find(ByVal buff() As Byte, ByVal search() As Byte) As Integer
            ' enumerate the buffer but don't overstep the bounds
            For start = 0 To buff.Length - search.Length - 1
                ' we found the first character
                If buff(start) = search(0) Then
                    Dim [next] As Integer

                    ' traverse the rest of the bytes
                    For [next] = 1 To search.Length - 1
                        ' if we don't match, bail
                        If buff(start + [next]) <> search([next]) Then Exit For
                    Next [next]

                    If [next] = search.Length Then Return start
                End If
            Next start
            ' not found
            Return -1
        End Function
    End Module

    Public Class FrameReadyEventArgs
        Inherits EventArgs
        Public FrameBuffer() As Byte
#If (Not SILVERLIGHT) AndAlso (Not XNA) Then
    Public Bitmap As Bitmap
#End If
#If Not XNA Then
    Public BitmapImage As BitmapImage
#End If
    End Class
End Namespace