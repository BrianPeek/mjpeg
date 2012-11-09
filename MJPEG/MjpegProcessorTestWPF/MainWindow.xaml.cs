using System;
using System.Windows;
using MjpegProcessor;

namespace MjpegProcessorTestWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MjpegDecoder _mjpeg;

		public MainWindow()
		{
			InitializeComponent();
			_mjpeg = new MjpegDecoder();
			_mjpeg.FrameReady += mjpeg_FrameReady;
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			_mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
		}

		private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
		{
			image.Source = e.BitmapImage;
		}
	}
}
