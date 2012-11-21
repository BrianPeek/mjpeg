using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MjpegProcessor;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Runtime.InteropServices.WindowsRuntime;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MjpegProcessorTestWinRT
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly MjpegDecoder _mjpeg;
		private readonly WriteableBitmap _bmp = new WriteableBitmap(1,1);

		public MainPage()
		{
			this.InitializeComponent();
			_mjpeg = new MjpegDecoder();
			_mjpeg.FrameReady += mjpeg_FrameReady;
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			_mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
		}

		private async void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
		{
			InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
			await stream.WriteAsync(e.FrameBuffer);
			stream.Seek(0);
			_bmp.SetSource(stream);
			image.Source = _bmp;
		}
	}
}
