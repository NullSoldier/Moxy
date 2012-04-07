using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Moxy.GameStates
{
	public class TilePickerOverlay
		: BaseGameState
	{
		public TilePickerOverlay()
			: base ("TilePicker", isOverlay:true, acceptsInput:true)
		{

		}

		public override void Load()
		{
			backgroundTexture = new Texture2D (Moxy.Graphics, 1, 1);
			backgroundTexture.SetData (new [] { new Color(255, 255, 255, 255) });
		}

		public override void PostLoad()
		{
			mapState = (MapState)Moxy.StateManager["MapState"];
		}

		public override void Update (GameTime gameTime)
		{
			if (Moxy.CurrentKeyboard.IsKeyUp (Keys.Tab))
			{
				Moxy.StateManager.Pop();
				mapState.IsFocused = true;
			}

			MouseState mouse = Mouse.GetState ();

			if (mouse.LeftButton == ButtonState.Pressed)
			{
				int tilesWide = (int)(map.Texture.Width / map.TileDimensions.Width);
				int x = (int)((mouse.X - tileDisplayOffset.X) / (map.TileDimensions.Width * scale));
				int y = (int)((mouse.Y - tileDisplayOffset.Y) / (map.TileDimensions.Height * scale));

				mapState.currentTileID = (y * tilesWide) + x;
			}
		}

		public override void Draw (SpriteBatch batch)
		{
			int tilesWide = (int)(map.Texture.Width / map.TileDimensions.Width);
			int tilesHigh = (int)(map.Texture.Height / map.TileDimensions.Height);

			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			batch.Draw (backgroundTexture, new Rectangle (0, 0, Moxy.ScreenWidth, Moxy.ScreenHeight), null, Color.Black);
			batch.End();

			batch.Begin (SpriteSortMode.Texture, BlendState.AlphaBlend);

			for (int x = 0; x < tilesWide; x++)
			{
				for (int y = 0; y < tilesHigh; y++)
				{
					int tileID = (y * tilesWide) + x;
					Vector2 tileLocation = new Vector2(x * (map.TileDimensions.Width * scale), y * (map.TileDimensions.Height * scale))
						+ tileDisplayOffset;

					// Draw the tile at it's respective location
					batch.Draw (map.Texture, tileLocation, map.Boundings[tileID], Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

					// Highlight if it's currently selected
					if (tileID == mapState.currentTileID)
						batch.Draw (mapState.highlightTexture, tileLocation, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
				}
			}

			batch.End();
		}

		public override void OnFocus()
		{
			map = mapState.Map;
		}

		private MapRoot map;
		private MapState mapState;
		private float scale = 0.4f;
		private Vector2 tileDisplayOffset = new Vector2(0, 0);
		private Texture2D backgroundTexture;
	}
}
