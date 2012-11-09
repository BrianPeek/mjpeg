using System;
using System.Windows;
using System.Windows.Controls;
using MjpegProcessor;

namespace MjpegProcessorTestSL4
{
	public partial class MainPage : UserControl
	{
		private delegate void AssignImageDelegate(byte[] buff);

		public MainPage()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			// need to be out of browser to get past crossdomain.xml not existing on the cam
			// NOTE: this will only work on cams which properly send MJPEG data to an IE user agent header
			// I've found several cams (Cisco being one) that tries to be "smart" and sends a single JPEG frame
			// instead of an MJPEG stream since destkop IE doesn't properly support the MJPEG codec
			if(Application.Current.IsRunningOutOfBrowser)
			{
				MjpegDecoder mjpeg = new MjpegDecoder();
				mjpeg.FrameReady += mjpeg_FrameReady;
				mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
			}
		}

		private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
		{
			image.Source = e.BitmapImage;
		}
	}
}
