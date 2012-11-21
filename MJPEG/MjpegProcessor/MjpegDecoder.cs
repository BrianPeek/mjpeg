using System;
using System.Text;
using System.Net;
using System.IO;

#if !XNA && !WINRT
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
#endif

#if SILVERLIGHT
using System.Net.Browser;
#elif WINRT
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;
using System.Runtime.InteropServices.WindowsRuntime;
#elif !XNA
using System.Drawing;
#endif

#if XNA || WINDOWS_PHONE
using Microsoft.Xna.Framework.Graphics;
#endif

namespace MjpegProcessor
{
#if WINRT
	public sealed class MjpegDecoder
#else
	public class MjpegDecoder
#endif
	{
#if !SILVERLIGHT && !XNA && !WINRT
		// WinForms & WPF
		public Bitmap Bitmap { get; set; }
#endif

		// magic 2 byte header for JPEG images
		private readonly byte[] JpegHeader = new byte[] { 0xff, 0xd8 };

		// pull down 1024 bytes at a time
		private const int ChunkSize = 1024;

		// used to cancel reading the stream
		private bool _streamActive;

#if WINRT
		// current encoded JPEG image
		public IBuffer CurrentFrame { get; private set; }
#else
		// current encoded JPEG image
		public byte[] CurrentFrame { get; private set; }
#endif

#if !XNA && !WINRT
		// WPF, Silverlight
		public BitmapImage BitmapImage { get; set; }
#endif

#if !XNA && !WINRT
		// used to marshal back to UI thread
		private SynchronizationContext _context;
#elif WINRT
		private readonly CoreDispatcher _dispatcher;
#endif

		// event to get the buffer above handed to you
		public event EventHandler<FrameReadyEventArgs> FrameReady;

		public MjpegDecoder()
		{
#if !XNA && !WINRT
			_context = SynchronizationContext.Current;

			BitmapImage = new BitmapImage();
#elif WINRT
			if(CoreWindow.GetForCurrentThread() != null)
				_dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
#endif
		}


		public void ParseStream(Uri uri)
		{
			ParseStream(uri, null, null);
		}

		public void ParseStream(Uri uri, string username, string password)
		{
#if SILVERLIGHT
			HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
#endif
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
			if(!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
				request.Credentials = new NetworkCredential(username, password);

#if SILVERLIGHT
			// start the stream immediately
			request.AllowReadStreamBuffering = false;
#endif
			// asynchronously get a response
			request.BeginGetResponse(OnGetResponse, request);
		}

		public void StopStream()
		{
			_streamActive = false;
		}

#if XNA || WINDOWS_PHONE
		public Texture2D GetMjpegFrame(GraphicsDevice graphicsDevice)
		{
			// create a Texture2D from the current byte buffer
			if(CurrentFrame != null)
				return Texture2D.FromStream(graphicsDevice, new MemoryStream(CurrentFrame, 0, CurrentFrame.Length));
			return null;
		}
#endif
		private void OnGetResponse(IAsyncResult asyncResult)
		{
			byte[] imageBuffer = new byte[1024 * 1024];

			// get the response
			HttpWebRequest req = (HttpWebRequest)asyncResult.AsyncState;
			HttpWebResponse resp = (HttpWebResponse)req.EndGetResponse(asyncResult);

			// find our magic boundary value
			string contentType = resp.Headers["Content-Type"];
			if(!string.IsNullOrEmpty(contentType) && !contentType.Contains("="))
				throw new Exception("Invalid content-type header.  The camera is likely not returning a proper MJPEG stream.");
			string boundary = resp.Headers["Content-Type"].Split('=')[1].Replace("\"", "");
			byte[] boundaryBytes = Encoding.UTF8.GetBytes(boundary.StartsWith("--") ? boundary : "--" + boundary);

			Stream s = resp.GetResponseStream();
			BinaryReader br = new BinaryReader(s);

			_streamActive = true;

			byte[] buff = br.ReadBytes(ChunkSize);

			while (_streamActive)
			{
				// find the JPEG header
				int imageStart = buff.Find(JpegHeader);

				if(imageStart != -1)
				{
					// copy the start of the JPEG image to the imageBuffer
					int size = buff.Length - imageStart;
					Array.Copy(buff, imageStart, imageBuffer, 0, size);

					while(true)
					{
						buff = br.ReadBytes(ChunkSize);

						// find the boundary text
						int imageEnd = buff.Find(boundaryBytes);
						if(imageEnd != -1)
						{
							// copy the remainder of the JPEG to the imageBuffer
							Array.Copy(buff, 0, imageBuffer, size, imageEnd);
							size += imageEnd;

							byte[] frame = new byte[size];
							Array.Copy(imageBuffer, 0, frame, 0, size);

							ProcessFrame(frame);

							// copy the leftover data to the start
							Array.Copy(buff, imageEnd, buff, 0, buff.Length - imageEnd);

							// fill the remainder of the buffer with new data and start over
							byte[] temp = br.ReadBytes(imageEnd);

							Array.Copy(temp, 0, buff, buff.Length - imageEnd, temp.Length);
							break;
						}

						// copy all of the data to the imageBuffer
						Array.Copy(buff, 0, imageBuffer, size, buff.Length);
						size += buff.Length;
					}
				}
			}
#if !WINRT
			resp.Close();
#endif
		}

#if WINRT
		private async void ProcessFrame(byte[] frame)
		{
			CurrentFrame = frame.AsBuffer();
#else
		private void ProcessFrame(byte[] frame)
		{
			CurrentFrame = frame;
#endif

#if SILVERLIGHT
			// need to get this back on the UI thread
			_context.Post(delegate
			{
				// resets the BitmapImage to the new frame
				BitmapImage.SetSource(new MemoryStream(frame, 0, frame.Length));

				// tell whoever's listening that we have a frame to draw
				if(FrameReady != null)
					FrameReady(this, new FrameReadyEventArgs { FrameBuffer = CurrentFrame, BitmapImage = BitmapImage });
			}, null);
#endif

#if !SILVERLIGHT && !XNA && !WINRT
			// no Application.Current == WinForms
			if(Application.Current != null)
			{
				// get it on the UI thread
				_context.Post(delegate
				{
					// create a new BitmapImage from the JPEG bytes
					BitmapImage = new BitmapImage();
					BitmapImage.BeginInit();
					BitmapImage.StreamSource = new MemoryStream(frame);
					BitmapImage.EndInit();

					// tell whoever's listening that we have a frame to draw
					if(FrameReady != null)
						FrameReady(this, new FrameReadyEventArgs { FrameBuffer = CurrentFrame, BitmapImage = BitmapImage });
				}, null);
			}
			else
			{
				_context.Post(delegate
				{
					// create a simple GDI+ happy Bitmap
					Bitmap = new Bitmap(new MemoryStream(frame));

					// tell whoever's listening that we have a frame to draw
					if(FrameReady != null)
						FrameReady(this, new FrameReadyEventArgs { FrameBuffer = CurrentFrame, Bitmap = Bitmap });
				}, null);
			}
#elif WINRT
			if(_dispatcher != null)
			{
				await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					// tell whoever's listening that we have a frame to draw
					if (FrameReady != null)
						FrameReady(this, new FrameReadyEventArgs { FrameBuffer = frame.AsBuffer() });
				});
			}
#endif
		}
	}

	static class Extensions
	{
		public static int Find(this byte[] buff, byte[] search)
		{
			// enumerate the buffer but don't overstep the bounds
			for(int start = 0; start < buff.Length - search.Length; start++)
			{
				// we found the first character
				if(buff[start] == search[0])
				{
					int next;

					// traverse the rest of the bytes
					for(next = 1; next < search.Length; next++)
					{
						// if we don't match, bail
						if(buff[start+next] != search[next])
							break;
					}

					if(next == search.Length)
						return start;
				}
			}
			// not found
			return -1;	
		}
	}

#if WINRT
	public sealed class FrameReadyEventArgs
	{
		public IBuffer FrameBuffer { get; set; }
	}
#else
	public class FrameReadyEventArgs : EventArgs
	{
		public byte[] FrameBuffer;
#if !SILVERLIGHT && !XNA && !WINRT
		public Bitmap Bitmap;
#endif
#if !XNA
		public BitmapImage BitmapImage;
#endif
	}
#endif
}
