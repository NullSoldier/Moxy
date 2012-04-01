﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Moxy.Map
{
	public class MapLayer
	{
		public MapLayer(Map Parent, MapLayerType Type)
		{
			LayerType = Type;
			this.Parent = Parent;
			Tiles = new uint[(int)Parent.TileDimensions.Width, (int)Parent.TileDimensions.Height];

		}

		public readonly Map Parent;
		public readonly MapLayerType LayerType;
		public readonly uint[,] Tiles;

		public void Draw(SpriteBatch batch)
		{
			var drawLocation = new Rectangle((int)Parent.LocationOffset.X, (int)Parent.LocationOffset.Y, (int)Parent.TileDimensions.Width, (int)Parent.TileDimensions.Height);
			for (var x = 0; x < Parent.Dimensions.Width; x++)
			{
				drawLocation.Y = (int)Parent.LocationOffset.Y;
				for (var y = 0; y < Parent.Dimensions.Height; y++)
				{
					batch.Draw(Parent.Texture, drawLocation, Parent.Boundings[Tiles[x, y]], Color.White);
					drawLocation.Y += (int)Parent.TileDimensions.Height;
				}
				drawLocation.X += (int)Parent.TileDimensions.Width;
			}
		}

		public void Update(GameTime gameTime)
		{


		}
	}

	public enum MapLayerType
	{
		Base,
		Decal,
		Collision
	}
}