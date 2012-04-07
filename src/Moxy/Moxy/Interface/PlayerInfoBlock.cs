using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy.Interface
{
	public class PlayerInfoBlock
	{
		public PlayerInfoBlock(ArcanaPlayer Player, PlayerInfoFrame ParentFrame)
		{
			this.Player = Player;
			this.ParentFrame = ParentFrame;
			
		}




		public Vector2 Location;
		public ArcanaPlayer Player;
		public readonly PlayerInfoFrame ParentFrame;

		public void Update(GameTime gameTime)
		{

		}

		public void Draw(SpriteBatch batch)
		{
			var drawRect = new Rectangle((int) Location.X, (int) Location.Y, BlockSize.Width, BlockSize.Height);
			if (Player == null)
			{
				batch.Draw(BackgroundTexture, drawRect, Color.Black );
				return;
			}
				

			
			batch.Draw(BackgroundTexture, drawRect, Player.Color);
			var manaRect = drawRect;
			manaRect.X += 5;
			manaRect.Width = (int)MathHelper.Lerp(0, ManaSize.Width, Player.Energy / Player.MaxEnergy);
			manaRect.Height = ManaSize.Height;
			
			batch.Draw(ManabarTexture, manaRect, Color.Blue);
			var nameLocation = new Vector2(drawRect.X, drawRect.Y);
			var nameSize = InfoFont.MeasureString(Player.Name);
			nameLocation.X += ((drawRect.Width - nameSize.X)/2f);
			nameLocation.Y += 45;
			batch.DrawString(InfoFont, Player.Name, nameLocation, Color.Goldenrod);
			var scoreLocation = new Vector2(drawRect.X, nameLocation.Y);
			scoreLocation.X += 5;
			scoreLocation.Y += nameSize.Y + 10;
			batch.DrawString(InfoFont, ((int)Player.PlayerScore).ToString(), scoreLocation, Color.Goldenrod);
			var healthLocation = scoreLocation + new Vector2(drawRect.Width/2f, 0f);
			batch.DrawString(InfoFont, ((int)Player.Health).ToString(), healthLocation, Color.Goldenrod);
		}


		public readonly static Rectangle BlockSize = new Rectangle(0, 0, Moxy.ScreenWidth / 4, 150);
		public readonly static Rectangle ManaSize = new Rectangle(0, 0, BlockSize.Width - 10, 40);
		public readonly static Texture2D BackgroundTexture = Helpers.Pixel;
		public readonly static Texture2D ManabarTexture = Helpers.Pixel;
		public readonly static SpriteFont InfoFont = Moxy.ContentManager.Load<SpriteFont>("Fonts//font");
	}
}
