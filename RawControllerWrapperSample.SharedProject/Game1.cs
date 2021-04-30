using FontBuddyLib;
using GameTimer;
using HadoukInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RawControllerWrapperSample
{
	/// <summary>
	/// This dude just verifies that all the buttons are being checked correctly on down, held, up
	/// </summary>
	public class Game1 : Game
	{
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

		private GameClock _clock;

		private InputState _input = new InputState();

		public Game1()
		{
			var _ = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			_controller = new ControllerWrapper(0);

			_clock = new GameClock();

			var keystrokes = Enum.GetValues(typeof(EKeystroke));
			_ButtonDownTimer = new CountdownTimer[keystrokes.Length];
			_ButtonUpTimer = new CountdownTimer[keystrokes.Length];
			for (int i = 0; i < keystrokes.Length; i++)
			{
				_ButtonDownTimer[i] = new CountdownTimer();
				_ButtonUpTimer[i] = new CountdownTimer();
			}
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
			_text.LoadContent(Content, "ArialBlack10");

			_clock.Start();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			//Update the controller
			_clock.Update(gameTime);
			_input.Update();
			_controller.Update(_input);

			//update all those timers
			var keystrokes = Enum.GetValues(typeof(EKeystroke));
			for (int i = 0; i < keystrokes.Length; i++)
			{
				//update the timers
				_ButtonDownTimer[i].Update(gameTime);
				_ButtonUpTimer[i].Update(gameTime);

				//if this button state changed, start the timer
				if (_controller.CheckKeystroke((EKeystroke)i))
				{
					_ButtonDownTimer[i].Start(0.5f);
				}
				//if (_controller.KeystrokeRelease[i])
				//{
				//	_ButtonUpTimer[i].Start(0.5f);
				//}
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
			for (int i = 0; i <= (int)EKeystroke.RTriggerRelease; i++)
			{
				//Write the name of the button
				position.X = _text.Write(((EKeystroke)i).ToString() + ": ", position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);

				//was the button pressed recently
				if (_ButtonDownTimer[i].HasTimeRemaining)
				{
					position.X = _text.Write("pressed ", position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);
				}

				//is the button currently held
				if (_controller.CheckKeystrokeHeld((EKeystroke)i))
				{
					position.X = _text.Write("held ", position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);
				}

				//was the button released recently
				if (_ButtonUpTimer[i].HasTimeRemaining)
				{
					position.X = _text.Write("released ", position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);
				}

				//move the position to the next line
				if (0 == (i % (((int)EKeystroke.RTrigger))))
				{
					position.Y = 0;
				}
				position.Y += _text.MeasureString(((EKeystroke)i).ToString()).Y;
				position.X = (i / ((int)EKeystroke.RTrigger)) * 180;
			}

			//write the raw thumbstick direction
			position = new Vector2(0, 380);
			position.X = _text.Write("L direction: ", position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);
			position.X = _text.Write(_controller.Thumbsticks.LeftThumbstick.Direction.ToString(), position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);

			position = new Vector2(0, 400);
			position.X = _text.Write("R direction: ", position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);
			position.X = _text.Write(_controller.Thumbsticks.RightThumbstick.Direction.ToString(), position, Justify.Left, 1.0f, Color.White, spriteBatch, _clock);

			position = new Vector2(0, 420);
			position.X = _text.Write($"buttons: {_input._currentGamePadStates[0].Buttons}", position, Justify.Left, 0.7f, Color.White, spriteBatch, _clock);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
