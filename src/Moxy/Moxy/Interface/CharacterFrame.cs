using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.GameStates;

namespace Moxy.Interface
{
	public class CharacterFrame
	{
		public CharacterFrame()
		{
			checkTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//checkmark");
			lockTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//lock");
			startTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//startButton");
			frameTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//characterframe");
			newTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//PlayMenu");

			acceptSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//accept");
			declineSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//decline");
			moveSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//move");

			font = Moxy.ContentManager.Load<SpriteFont> ("Fonts//font");
		}

		public PlayerIndex PlayerIndex;
		public CharacterHolder CharacterHolder;
		public Vector2 Location;
		public Rectangle Framebounds;
		public Vector2 checkOrigin;
		public Vector2 lockOrigin;
		public Vector2 startOrigin;
		public Vector2 newOrigin;
		public bool IsActive;
		public bool IsReady;
		public bool IsLocked;
		public bool IsConnected;

		public void Draw(SpriteBatch batch)
		{
			Color color = IsReady || IsLocked ? Color.Gray : Color.White;

			batch.Draw (frameTexture, Framebounds, color);

			if (Framebounds == Rectangle.Empty)
			{
				Framebounds = new Rectangle ((int)Location.X, (int)Location.Y, frameTexture.Width, frameTexture.Height);
				checkOrigin = new Vector2 (checkTexture.Width / 2, checkTexture.Height / 2);
				lockOrigin = new Vector2 (lockTexture.Width / 2, lockTexture.Height / 2);
				startOrigin = new Vector2 (startTexture.Width / 2, startTexture.Height / 2);
				newOrigin = new Vector2 (newTexture.Width / 2, newTexture.Height / 2);
			}

			if (IsLocked)
			{
				batch.Draw (lockTexture, Framebounds.Center.ToVector2(), null, Color.White, 0f, lockOrigin, 1f, SpriteEffects.None, 1f);
				return;
			}

			if (frameState == FrameState.WaitingForPlayer && !IsLocked)
			{
				batch.Draw (startTexture, Framebounds.Center.ToVector2(), null, Color.White, 0f, startOrigin, 0.5f, SpriteEffects.None, 1f);
			}
			else if (frameState == FrameState.Ready)
			{
				batch.Draw (checkTexture, Framebounds.Center.ToVector2(), null, Color.White, 0f, checkOrigin, 1f, SpriteEffects.None, 1f);
			}
			else if (frameState == FrameState.DisplayCreateCharacter)
			{
				batch.Draw (newTexture, Framebounds.Center.ToVector2 (), null, Color.White, 0f, newOrigin, 1f, SpriteEffects.None, 1f);
			}
			else if (frameState == FrameState.SelectCharacter)
			{
				batch.Draw (charInfo.Texture, Framebounds, null, Color.White);
				batch.DrawString (font, charInfo.Name, Framebounds.Center.ToVector2 (), Color.White);
				batch.DrawString (font, charInfo.Class, Framebounds.Center.ToVector2 () + new Vector2(0, 20), Color.White);
			}
			else if (frameState == FrameState.PickClass)
			{
				batch.Draw (classInfo.Texture, Framebounds, null, Color.White);
				batch.DrawString (font, classInfo.Class, Framebounds.Center.ToVector2(), Color.White);
			}
		}

		public void Update (GameTime gameTime)
		{
			GamePadState padState = Moxy.CurrentPadStates[PlayerIndex];
			GamePadState lastState = Moxy.LastPadStates[PlayerIndex];
			IsConnected = padState.IsConnected;

			if (frameState == FrameState.WaitingForPlayer)
			{
				if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.Start) || Moxy.WasPadButtonPressed (PlayerIndex, Buttons.A))
				{
					charInfo = CharacterHolder.GetCharacterSlot (characterSlot = 0);
					frameState = FrameState.SelectCharacter;
					IsActive = true;
				}
			}
			else if (frameState == FrameState.Ready)
			{
				if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.B))
				{
					frameState = FrameState.SelectCharacter;
					IsReady = false;
				}
			}
			else if (frameState == FrameState.SelectCharacter)
			{
				bool selectNew = false;
				if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadRight))
				{
					characterSlot = Helpers.GetLoopableInt (++characterSlot, 0, CharacterHolder.Count - 1);
					selectNew = true;
				}
				else if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadLeft))
				{
					characterSlot = Helpers.GetLoopableInt (--characterSlot, 0, CharacterHolder.Count - 1);
					selectNew = true;
				}
				else if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadDown)
					|| Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadUp))
				{
					frameState = FrameState.DisplayCreateCharacter;
				}
				else if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.A))
				{
					frameState = FrameState.Ready;
				}

				if (selectNew)
					charInfo = CharacterHolder.GetCharacterSlot (characterSlot);
			}
			else if (frameState == FrameState.DisplayCreateCharacter)
			{
				if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadDown)
					|| Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadUp))
				{
					frameState = FrameState.SelectCharacter;
				}
				else if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.A))
				{
					classInfo = CharacterHolder.GetClassSlot (classSlot = 0);
					frameState = FrameState.PickClass;
				}
			}
			else if (frameState == FrameState.PickClass)
			{
				bool selectNew = false;
				if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadRight))
				{
					classSlot = Helpers.GetLoopableInt (++classSlot, 0, CharacterHolder.ClassCount - 1);
					selectNew = true;
				}
				else if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.DPadLeft))
				{
					classSlot = Helpers.GetLoopableInt (--classSlot, 0, CharacterHolder.ClassCount - 1);
					selectNew = true;
				}
				else if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.B))
				{
					// Go back to new screen
					frameState = FrameState.SelectCharacter;
				}
				else if (Moxy.WasPadButtonPressed (PlayerIndex, Buttons.A))
				{
					CharacterHolder.Characters.Add (new CharacterInformation
					{
						Name = "No Name",
						Level = 1,
						Class = classInfo.Class,
						Texture = classInfo.Texture
					});

					// Select our newly created character
					charInfo = CharacterHolder.GetCharacterSlot (characterSlot = CharacterHolder.Count - 1);
					frameState = FrameState.SelectCharacter;
				}

				if (selectNew)
					classInfo = CharacterHolder.GetClassSlot (classSlot);
			}
		}

		private FrameState frameState = FrameState.WaitingForPlayer;
		private int characterSlot = 0;
		private int classSlot = 0;
		private CharacterInformation charInfo;
		private CharacterInformation classInfo;

		private SpriteFont font;
		private Texture2D frameTexture;
		private Texture2D checkTexture;
		private Texture2D lockTexture;
		private Texture2D startTexture;
		private Texture2D newTexture;
		private SoundEffect acceptSound;
		private SoundEffect declineSound;
		private SoundEffect moveSound;

		private enum FrameState
		{
			WaitingForPlayer,
			SelectCharacter,
			DisplayCreateCharacter,
			PickClass,
			PickName,
			Ready
		}
	}
}
