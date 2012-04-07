using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.GameStates;

namespace Moxy.Interface
{
	public class PlayerInfoFrame
	{
		public PlayerInfoFrame(GameState PlayingState)
		{
			playingState = PlayingState;
			infoBlocks = new List<PlayerInfoBlock>();
			for (int i = 0; i < 4; i++)
			{
				var block = new PlayerInfoBlock(null, this);
				block.Location = new Vector2((PlayerInfoBlock.BlockSize.Width * i) + 28, FramePosition.Y + 5);
				infoBlocks.Add(block);
			}
		}

		public void Draw(SpriteBatch batch)
		{
			var drawRect = FramePosition;
			batch.Draw(FrameTexture, drawRect, Color.White);
			foreach(var block in infoBlocks)
				block.Draw(batch);
		}

		public void Update(GameTime gameTime)
		{
			for (var i = 0; i < playingState.players.Count; i++)
			{
				infoBlocks[i].Player = playingState.players[i];
				infoBlocks[i].Update(gameTime);
			}

		}

		///TODO
		/// Player joined/left/disconneted/stuff


		private readonly GameState playingState;
		private readonly List<PlayerInfoBlock> infoBlocks; 


		public static readonly Rectangle FramePosition = new Rectangle(0, Moxy.ScreenHeight - PlayerInfoBlock.BlockSize.Height, Moxy.ScreenWidth, PlayerInfoBlock.BlockSize.Height);
		public static readonly Texture2D FrameTexture = Moxy.ContentManager.Load<Texture2D>("Interface//highlight");
	}
}
