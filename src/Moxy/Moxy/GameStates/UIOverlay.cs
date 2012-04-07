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
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, 
				SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
			playerInfo.Draw(batch);
			batch.End();
		}

		public override void Update(GameTime gameTime)
		{
			playerInfo.Update(gameTime);
		}

		private GameState activeGameState;
		private PlayerInfoFrame playerInfo;
	}
}