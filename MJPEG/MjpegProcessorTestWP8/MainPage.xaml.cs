using System;
using System.Windows;
using Microsoft.Phone.Controls;
using MjpegProcessor;

namespace MjpegProcessorTestWP8
{
	public partial class MainPage : PhoneApplicationPage
	{
		private MjpegDecoder _mjpeg;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			_mjpeg = new MjpegDecoder();
			_mjpeg.FrameReady += mjpeg_FrameReady;
			_mjpeg.Error += _mjpeg_Error;
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			_mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
		}

		private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
		{
			image.Source = e.BitmapImage;
		}

		void _mjpeg_Error(object sender, ErrorEventArgs e)
		{
			MessageBox.Show(e.Message);
		}
	}
}