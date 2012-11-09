using System;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MjpegProcessor;

namespace MjpegProcessorTestXna
{
	class Game1 : Game
	{
		private Texture2D _texture;
		private SpriteBatch _spriteBatch;
		private MjpegDecoder _mjpeg;

		public Game1()
		{
			new GraphicsDeviceManager(this);
		}

		protected override void Initialize()
		{
			base.Initialize();

			_spriteBatch = new SpriteBatch(this.GraphicsDevice);

			_mjpeg = new MjpegDecoder();
			_mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if(_mjpeg.CurrentFrame != null)
			{
				BitmapImage img = new BitmapImage();
				img.BeginInit();
				img.StreamSource = new MemoryStream(_mjpeg.CurrentFrame);
				img.EndInit();

				int stride = (img.PixelWidth * img.Format.BitsPerPixel + 7) / 8;
				byte[] frameBuffer = new byte[img.PixelHeight * stride];
				img.CopyPixels(frameBuffer, stride, 0);

				if(_texture == null)
					_texture = new Texture2D(this.GraphicsDevice, img.PixelWidth, img.PixelHeight);

				_texture.SetData(frameBuffer);
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			if(_texture != null)
			{
				_spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
				    _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
				_spriteBatch.End();
			}

			base.Draw(gameTime);
		}

		static void Main(string[] args)
		{
			using (Game1 game = new Game1())
			{
				game.Run();
			}
		}
	}
}
