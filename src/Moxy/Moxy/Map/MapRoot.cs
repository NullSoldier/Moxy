using System;
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
		public MapRoot(int Width, int Height, int TileWidth, int TileHeight, string[] TextureNames, DynamicCamera Camera)
		{
			Layers = new MapLayer[3];
			Dimensions = new Size(Width, Height);
			TileDimensions = new Size(TileWidth, TileHeight);
			CollidableID = new HashSet<int> ();
			EnableCollision = false;
			ViewCamera = Camera;
			this.TextureNames = DefaultTextureNames;

			if(TextureNames.Length == 1)
			{
				this.TextureNames[0] = TextureNames[0];
				this.TextureNames[1] = TextureNames[0];
			}
			else if(TextureNames.Length == 2)
			{
				this.TextureNames[0] = TextureNames[0];
				this.TextureNames[1] = TextureNames[1];
			}
			else
			{
				this.TextureNames = TextureNames;
			}

			CreateLayers();
			CreateSpawns();
			CreateLights();
		}

		public MapRoot(string File)
		{
			throw new NotImplementedException();
		}
	
		public readonly MapLayer[] Layers;
		public readonly Size Dimensions;
		public readonly Size TileDimensions;
		public readonly HashSet<int> CollidableID;
		public readonly string[] TextureNames;
		public readonly static string[] DefaultTextureNames = new[]
		                                                      	{
		                                                      		"UnknownTexture",
																	"UnknownTexture",
																	"Collision_TileSet",
		                                                      	};

		public bool EnableCollision;
		public Texture2D BackgroundTexture;
		public DynamicCamera ViewCamera;
		public Color AmbientColor;

		public Rectangle TileDrawBounds;

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
			if(ViewCamera == null)
			{
				TileDrawBounds = new Rectangle(0, 0, (int)Dimensions.Width, (int)Dimensions.Height);
			}
			else
			{
				TileDrawBounds = BuildCullingRectangle();
			}

			Layers[(int)MapLayerType.Base].Draw(batch, TileDrawBounds);
			Layers[(int)MapLayerType.Decal].Draw(batch, TileDrawBounds);
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

		public bool CheckCollision(int X, int Y, bool PreTrans)
		{
			if (EnableCollision)
			{
				var transX = X;
				var transY = Y;
				if (!PreTrans)
				{
					transX = (int) Math.Floor(X/TileDimensions.Width);
					transY = (int) Math.Floor(Y/TileDimensions.Height);
				}
				if ((transX >= Dimensions.Width || transX < 0 || transY < 0 || transY >= Dimensions.Height))
					return true;
				else
					return ((int)Layers[(int)MapLayerType.Collision].Tiles[transX, transY]) != 0;
			}
				
			else
				return false;
		}

		public bool CheckCollision(Point P)
		{
			return CheckCollision(P.X, P.Y, false);
		}

		public bool CheckCollision(Vector2 V)
		{
			return CheckCollision(new Point((int)V.X, (int)V.Y));
		}

		public bool CheckCollision(Rectangle R)
		{
			var transL = (int) Math.Floor(R.Left / TileDimensions.Width);
			var transR = (int) Math.Floor(R.Right / TileDimensions.Width);
			var transT = (int) Math.Floor(R.Top / TileDimensions.Height);
			var transB = (int) Math.Floor(R.Bottom / TileDimensions.Height);
			var failed = false;

			transL = (int)MathHelper.Clamp(transL, 1, transL);
			transR = (int)MathHelper.Clamp(transR, 1, transR);
			transB = (int)MathHelper.Clamp(transB, 1, transB);
			transT = (int)MathHelper.Clamp(transT, 1, transT);
			

			for(var x = transL; x <= transR && !failed; x++)
				for(var y = transT; y <= transB; y++)
				{
					failed = CheckCollision(x, y, true);
					if (failed)
						break;
				}
			return failed;
		}



		public Rectangle[] CreateBoundings(Texture2D Texture)
		{
			var xTiles = (int)(Texture.Width / TileDimensions.Width);
			var yTiles = (int)(Texture.Height / TileDimensions.Height);

			var Boundings = new Rectangle[xTiles * yTiles];
			for (var y = 0; y < yTiles; y++)
			{
				for (var x = 0; x < xTiles; x++)
				{
					Boundings[(x + (y * xTiles))] = new Rectangle((int)(x * TileDimensions.Width), (int)(y * TileDimensions.Height), (int)TileDimensions.Width, (int)TileDimensions.Height);
				}
			}
			return Boundings;
		}

		private void CreateLayers()
		{
			Layers[(int)MapLayerType.Base] = new MapLayer(this, MapLayerType.Base, TextureNames[(int)MapLayerType.Base]);
			Layers[(int)MapLayerType.Decal] = new MapLayer(this, MapLayerType.Decal, TextureNames[(int)MapLayerType.Decal]);
			Layers[(int)MapLayerType.Collision] = new MapLayer(this, MapLayerType.Collision, TextureNames[(int)MapLayerType.Collision]);
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
