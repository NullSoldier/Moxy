#define DEBUG_COLLISION
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy.GameStates;
using Moxy.Interface;
using Moxy.Skills;
using Microsoft.Xna.Framework.Input;

namespace Moxy.GameStates
{
	public class UIOverlay
		: BaseGameState
	{
		public UIOverlay()
			: base("UIOverlay", isOverlay: true, acceptsInput: false)
		{

		}

		public override void Load()
		{
			activeGameState = (GameState) Moxy.StateManager["Game"];
			playerInfo = new PlayerInfoFrame(activeGameState);
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, 
				SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);

			playerInfo.Draw(batch);
#if DEBUG_COLLISION
			var old = activeGameState.players[0].Collision;
			var start = activeGameState.camera.WorldToScreen(new Vector2(old.X, old.Y));
			var toScreen = new Rectangle((int) start.X, (int) start.Y, old.Width, old.Height);
			batch.Draw(pixel, toScreen, Color.White);
#endif
			batch.End();
		}

		public override void Update(GameTime gameTime)
		{
			playerInfo.Update(gameTime);
		}

		private Texture2D pixel = Helpers.Pixel;
		private GameState activeGameState;
		private PlayerInfoFrame playerInfo;
	}
}