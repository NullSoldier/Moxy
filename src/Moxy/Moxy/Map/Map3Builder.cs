using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Moxy.GameStates;

namespace Moxy.Map
{
	public class Map3Builder
		: MapBuilder
	{
		public override MapRoot Build()
		{
			MapRoot map = new MapRoot (128, 128, 64, 64, new []{ "oryxtileset" }, ((GameState)Moxy.StateManager["Game"]).camera);

			return map;
		}

		private void BuildBaseLayer ()
		{
		}
	}
}
