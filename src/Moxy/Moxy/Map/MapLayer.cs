﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy;

namespace Moxy
{
	public class MapLayer
	{
		public MapLayer(MapRoot Parent, MapLayerType Type)
		{
			LayerType = Type;
			this.Parent = Parent;
			Tiles = new uint[(int)Parent.Dimensions.Width, (int)Parent.Dimensions.Height];

		}

		public readonly MapRoot Parent;
		public readonly MapLayerType LayerType;
		public uint[,] Tiles;

		public void Draw(SpriteBatch batch, Rectangle bounds)
		{
			var drawLocation = new Rectangle(bounds.X * (int)Parent.TileDimensions.Width, bounds.Y * (int)Parent.TileDimensions.Height, (int)Parent.TileDimensions.Width, (int)Parent.TileDimensions.Height);
			for (var x = 0; x < bounds.Width; x++)
			{
				drawLocation.Y = bounds.Y * (int)Parent.TileDimensions.Height ;
				for (var y = 0; y < bounds.Height; y++)
				{
					batch.Draw(Parent.Texture, drawLocation, Parent.Boundings[Tiles[bounds.X + x, bounds.Y + y]], Color.White);
					drawLocation.Y += (int)Parent.TileDimensions.Height;
					//= (int)Math.Ceiling(y * Parent.TileDimensions.Height);
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
