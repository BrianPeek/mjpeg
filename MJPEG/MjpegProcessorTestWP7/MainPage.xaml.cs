using System;
using System.Windows;
using Microsoft.Phone.Controls;
using MjpegProcessor;

namespace MjpegProcessorTestWP7
{
	public partial class MainPage : PhoneApplicationPage
	{
		private MjpegDecoder _mjpeg;

		public MainPage()
		{
			InitializeComponent();
			_mjpeg = new MjpegDecoder();
			_mjpeg.FrameReady += mjpeg_FrameReady;
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			// this works best as a 320x240 stream at 15fps...anything larger/faster and the phone can't quite keep up
			_mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
		}

		private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
		{
			image.Source = e.BitmapImage;
		}
	}
}
