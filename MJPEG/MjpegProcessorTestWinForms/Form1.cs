using System;
using System.Windows.Forms;
using MjpegProcessor;

namespace MjpegProcessorTestWinForms
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			MjpegDecoder mjpeg = new MjpegDecoder();
			mjpeg.FrameReady += mjpeg_FrameReady;
			mjpeg.Error += mjpeg_Error;
			mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
		}

		private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
		{
			image.Image = e.Bitmap;
		}

		void mjpeg_Error(object sender, ErrorEventArgs e)
		{
			MessageBox.Show(e.Message);
		}
	}
}
