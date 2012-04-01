﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Moxy.GameStates
{
	public class MainMenuState
		: BaseGameState
	{
		public MainMenuState()
			: base ("MainMenu", isOverlay:false, acceptsInput:true)
		{
		}

		public override void Update (GameTime gameTime)
		{
			if (PollPadForStart (PlayerIndex.One)
				|| PollPadForStart (PlayerIndex.Two)
				|| PollPadForStart (PlayerIndex.Three)
				|| PollPadForStart (PlayerIndex.Four))
			{
				Moxy.StateManager.Set ("CharacterSelect");
				acceptSound.Play();
			}

			if (Moxy.CurrentPadStates[PlayerIndex.One].Buttons.Back.WasButtonPressed (Moxy.LastPadStates[PlayerIndex.One].Buttons.Back))
				Moxy.Instance.Exit();

			if (MediaPlayer.Volume < 1f)
			{
				fadeInPassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
				MediaPlayer.Volume = MathHelper.Lerp (0f, 8f, fadeInPassed / fadeInTime);
			}
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Begin();
			batch.Draw (titleTexture, Vector2.Zero, Color.White);
			batch.End();
		}

		public override void Load()
		{
			titleTexture = Moxy.ContentManager.Load<Texture2D> ("titlescreen");
			acceptSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds\\accept");
			music = Moxy.ContentManager.Load<Song> ("Sounds\\titlemusic");
		}

		public override void OnFocus()
		{
			MediaPlayer.Volume = 0f;
			MediaPlayer.IsRepeating = true;
			//MediaPlayer.Play (music);
		}

		public override void OnLostFocus()
		{
			MediaPlayer.Volume = 8f;
		}

		private Texture2D titleTexture;
		private SoundEffect acceptSound;
		private Song music;
		private float fadeInTime = 3f;
		private float fadeInPassed;
		private bool fadedMusic;

		private bool PollPadForStart(PlayerIndex playerIndex)
		{
			GamePadState padState = Moxy.CurrentPadStates[playerIndex];

			return padState.Buttons.A == ButtonState.Pressed
				|| padState.Buttons.Start == ButtonState.Pressed;
		}
	}
}