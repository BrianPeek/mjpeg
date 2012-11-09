using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MjpegProcessor;

namespace MjpegProcessorTestXna4
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager graphics;

		private Texture2D _texture;
		private SpriteBatch _spriteBatch;
		private MjpegDecoder _mjpeg;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);

#if WINDOWS_PHONE
			// Frame rate is 30 fps by default for Windows Phone.
			TargetElapsedTime = TimeSpan.FromTicks(333333);
			graphics.SupportedOrientations = DisplayOrientation.Portrait;
			graphics.IsFullScreen = true;
#endif
			graphics.PreferredBackBufferWidth = 480;
			graphics.PreferredBackBufferHeight = 800;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();

			_spriteBatch = new SpriteBatch(this.GraphicsDevice);

			_mjpeg = new MjpegDecoder();
			_mjpeg.ParseStream(new Uri("http://192.168.2.200/img/video.mjpeg"));
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			_texture = _mjpeg.GetMjpegFrame(this.GraphicsDevice);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			if(_texture != null)
			{
				_spriteBatch.Begin();
					_spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
				_spriteBatch.End();
			}

			base.Draw(gameTime);
		}
	}

#if WINDOWS || XBOX || WINDOWS_PHONE
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (Game1 game = new Game1())
			{
				game.Run();
			}
		}
	}
#endif
}
