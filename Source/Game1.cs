using System;
using System.Collections.Generic;
using System.Linq;
using HadoukInput;
using FontBuddyLib;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace RawControllerWrapperSample
{
	/// <summary>
	/// This dude just verifies that all the buttons are being checked correctly on down, held, up
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		/// <summary>
		/// A font buddy we will use to write out to the screen
		/// </summary>
		private FontBuddy _text = new FontBuddy();

		/// <summary>
		/// THe controller object we gonna use to test
		/// </summary>
		private ControllerWrapper _controller;

		/// <summary>
		/// The timers we are gonna use to time the button down events
		/// </summary>
		private CountdownTimer[] _ButtonDownTimer;

		/// <summary>
		/// The timers we gonna use to time the button up events
		/// </summary>
		private CountdownTimer[] _ButtonUpTimer;

		private InputState m_Input = new InputState();

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			_controller = new ControllerWrapper(PlayerIndex.One);
			_ButtonDownTimer = new CountdownTimer[(int)EKeystroke.RTrigger + 1];
			_ButtonUpTimer = new CountdownTimer[(int)EKeystroke.RTrigger + 1];

			for (int i = 0; i < ((int)EKeystroke.RTrigger + 1); i++)
			{
				_ButtonDownTimer[i] = new CountdownTimer();
				_ButtonUpTimer[i] = new CountdownTimer();
			}
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
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			_text.LoadContent(Content, "TestFont");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
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

			// TODO: Add your update logic here

			//Update the controller
			m_Input.Update();
			_controller.Update(m_Input);

			//update all those timers
			for (int i = 0; i < ((int)EKeystroke.RTrigger + 1); i++)
			{
				//update the timers
				_ButtonDownTimer[i].Update(gameTime);
				_ButtonUpTimer[i].Update(gameTime);

				//if this button state changed, start the timer
				if (_controller.KeystrokePress[i])
				{
					_ButtonDownTimer[i].Start(0.5f);
				}
				if (_controller.KeystrokeRelease[i])
				{
					_ButtonUpTimer[i].Start(0.5f);
				}
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			//draw the current state of each keystroke
			Vector2 position = Vector2.Zero;
			for (int i = 0; i < ((int)EKeystroke.RTrigger + 1); i++)
			{
				//Write the name of the button
				position.X = _text.Write(((EKeystroke)i).ToString() + ": ", position, Justify.Left, 1.0f, Color.White, spriteBatch);

				//was the button pressed recently
				if (0.0 < _ButtonDownTimer[i].RemainingTime())
				{
					position.X = _text.Write("pressed ", position, Justify.Left, 1.0f, Color.White, spriteBatch);
				}

				//is the button currently held
				if (_controller.KeystrokeHeld[i])
				{
					position.X = _text.Write("held ", position, Justify.Left, 1.0f, Color.White, spriteBatch);
				}

				//was the button released recently
				if (0.0 < _ButtonUpTimer[i].RemainingTime())
				{
					position.X = _text.Write("released ", position, Justify.Left, 1.0f, Color.White, spriteBatch);
				}

				//move the position to the next line
				position.Y += _text.Font.MeasureString(((EKeystroke)i).ToString()).Y;
				position.X = 0.0f;
			}

			//write the raw thumbstick direction
			position.X = _text.Write("direction: ", position, Justify.Left, 1.0f, Color.White, spriteBatch);
			position.X = _text.Write(_controller.Thumbsticks.LeftThumbstickDirection.ToString(), position, Justify.Left, 1.0f, Color.White, spriteBatch);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
