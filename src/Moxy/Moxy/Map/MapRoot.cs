﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy
{
	public class MapRoot
	{
		public MapRoot(int Width, int Height, int TileWidth, int TileHeight, Texture2D texture, DynamicCamera Camera)
		{
			Layers = new MapLayer[3];
			Dimensions = new Size(Width, Height);
			TileDimensions = new Size(TileWidth, TileHeight);
			CollidableID = new HashSet<int> ();
			EnableCollision = false;
			Texture = texture;
			ViewCamera = Camera;

			CreateBoundings ();
			CreateLayers();
			CreateSpawns();
			CreateLights();
			
		}


		public MapRoot(string File)
		{
			throw new NotImplementedException();
		} 

	
		public readonly Texture2D Texture;
		public readonly MapLayer[] Layers;
		public readonly Size Dimensions;
		public readonly Size TileDimensions;
		public readonly HashSet<int> CollidableID;

		public bool EnableCollision;

		public DynamicCamera ViewCamera;

		public Color AmbientColor;
	
		public Rectangle[] Boundings;

		public Vector2 LocationOffset;
		public Vector2[] PlayerSpawns;

		public List<MonsterSpawner> MonsterSpawners;
		public List<Light> PointLights;

		public int TilesDrawn;
		//Mainly a debug field

		public Size TilesToPixel
		{
			get { return new Size(Dimensions.Width * TileDimensions.Width, Dimensions.Height * TileDimensions.Height); }
		}

		public void Draw(SpriteBatch batch)
		{
			TilesDrawn = 0;
			Rectangle bounds;
			if(ViewCamera == null)
			{
				bounds = new Rectangle(0, 0, (int)Dimensions.Width, (int)Dimensions.Height);
			}
			else
			{
				bounds = BuildCullingRectangle();
			}

			Layers[(int)MapLayerType.Base].Draw (batch, bounds);
			Layers[(int)MapLayerType.Decal].Draw (batch, bounds);
			//Layers[(int)MapLayerType.Collision].Draw(batch);
		}

		public void Update(GameTime gameTime)
		{

		}

		public Rectangle BuildCullingRectangle()
		{
			var x = (int) Math.Floor(ViewCamera.ViewFrustrum.Left/TileDimensions.Width);
			var y = (int) Math.Floor(ViewCamera.ViewFrustrum.Top/TileDimensions.Height);
			var width = (int) Math.Ceiling(ViewCamera.ViewFrustrum.Width/TileDimensions.Width) + 1;
			var height = (int) Math.Ceiling(ViewCamera.ViewFrustrum.Height/TileDimensions.Height) + 1;
			x = (int) Math.Max(x, 0);
			y = (int) Math.Max(y, 0);
			width = (int) Math.Min(width, (Dimensions.Width - x));
			height = (int) Math.Min(height, (Dimensions.Height - y));
			


			var rec = new Rectangle(x, y, width, height);
			return rec;
		}

		public bool CheckCollision(int X, int Y)
		{
			if (EnableCollision)
			{
				var transX = (int) Math.Round(X/TileDimensions.Width) - 1;
				var transY = (int) Math.Round(Y/TileDimensions.Height) - 1;
				if (transX >= Dimensions.Width || transX < 0 || transY < 0 || transY >= Dimensions.Height)
					return false;
				else
					return ((int)Layers[(int)MapLayerType.Collision].Tiles[transX, transY]) != 0;
			}
				
			else
				return false;
		}

		public bool CheckCollision(Point P)
		{
			return CheckCollision(P.X, P.Y);
		}

		public bool CheckCollision(Vector2 V)
		{
			return CheckCollision(new Point((int)V.X, (int)V.Y));
		}

		public bool CheckCollision(Rectangle R)
		{
			var failed = false;
			for(var x = R.Left; x < R.Right && !failed; x += (int)TileDimensions.Width)
				for(var y = R.Top; y < R.Bottom; y += (int)TileDimensions.Height)
				{
					failed = CheckCollision(x, y);
					if (failed)
						break;
				}
			return failed;
		}



		private void CreateBoundings()
		{
			var xTiles = (int)(Texture.Width / TileDimensions.Width);
			var yTiles = (int)(Texture.Height / TileDimensions.Height);

			Boundings = new Rectangle[xTiles * yTiles];
			for (var y = 0; y < yTiles; y++)
			{
				for (var x = 0; x < xTiles; x++)
				{
					Boundings[(int)(x + (y * xTiles))] = new Rectangle((int)(x * TileDimensions.Width), (int)(y * TileDimensions.Height), (int)TileDimensions.Width, (int)TileDimensions.Height);
				}
			}
		}

		private void CreateLayers()
		{
			Layers[(int)MapLayerType.Base] = new MapLayer(this, MapLayerType.Base);
			Layers[(int)MapLayerType.Collision] = new MapLayer(this, MapLayerType.Collision);
			Layers[(int)MapLayerType.Decal] = new MapLayer(this, MapLayerType.Decal);
		}

		private void CreateSpawns()
		{
			PlayerSpawns = new Vector2[4];
			MonsterSpawners = new List<MonsterSpawner>();
		}

		private void CreateLights()
		{
			PointLights = new List<Light>();
		}

	}


}
