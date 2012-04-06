using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.Entities;
using Moxy.Interface;

namespace Moxy.GameStates
{
	public class CharacterSelectState
		: BaseGameState
	{
		public CharacterSelectState()
			: base ("CharacterSelect")
		{
		}

		public List<ArcanaPlayer> Players;
		public bool CharactersSelected;

		public override void Update (GameTime gameTime)
		{
			if (Moxy.WasPadButtonPressed (PlayerIndex.One, Buttons.Back))
				Moxy.StateManager.Set ("MainMenu");

			bool allReady = true;
			foreach (var frame in frames)
			{
				frame.Update (gameTime);

				if (!frame.IsReady && frame.IsConnected)
					allReady = false;
			}

			if (allReady && Moxy.WasPadButtonPressed (PlayerIndex.One, Buttons.Start))
			{
				Players = frames.Select (f => f.Character).ToList();

				CharactersSelected = true;
				Moxy.StateManager.Set ("Game");
			}

			// Lock certain portraits
			int controllerCount = frames.Count(s => s.IsConnected);
			for (int i = 0; i < 4; i++)
				frames[i].IsLocked = i >= controllerCount;
		}

		public override void Draw (SpriteBatch batch)
		{
			Moxy.Graphics.Clear (Color.Black);

			batch.Begin();
			batch.Draw (panelTexture, new Vector2 (0, 431), Color.White);

			foreach (CharacterFrame frame in frames)
				frame.Draw (batch);

			batch.End();
		}

		public override void Load()
		{
			panelTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//cspanel");

			characters = new CharacterHolder();
			characters.Load();

			frames = new CharacterFrame[4];
			for (int i = 0; i < 4; i++)
			{
				frames[i] = new CharacterFrame
				{
					PlayerIndex = (PlayerIndex)i,
					CharacterHolder = characters,
					Location = new Vector2 (i * 200, 0)
				};
			}
	}

		public override void OnFocus()
		{
			// Unready all players
			foreach (var frame in frames)
				frame.IsReady = false;
		}

		private CharacterFrame[] frames;
		private Texture2D panelTexture;
		private CharacterHolder characters;
	}
}
