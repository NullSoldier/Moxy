using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.Entities;
using Moxy.Map;

namespace Moxy.GameStates
{
	public class MapState
		: BaseGameState
	{
		public MapState()
			: base("MapState", isOverlay:false, acceptsInput:true)
		{

			
		}
	
		public override void Update(GameTime gameTime)
		{
			Camera.Update(Moxy.Graphics);
			Map.Update(gameTime);

			var mouseState = Mouse.GetState ();
			var state = Keyboard.GetState ();

			if (IsFocused)
			{
				if (WasKeyPressed (state, Keys.F1))
					currentTool = EditorTool.PlaceTiles;

				HandleCameraControls (state, gameTime);

				if (currentTool == EditorTool.PlaceTiles)
				{
					int tileID = currentTileID;
					if (WasKeyPressed (state, Keys.Right))
						tileID++;
					else if (WasKeyPressed (state, Keys.Left))
						tileID--;

					if (tileID < 0)
						tileID = Map.Boundings.Length + tileID;

					currentTileID = tileID % Map.Boundings.Length;

					if (mouseState.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
						startPlacing = true;
					if (mouseState.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed)
						startPlacing = false;
					if (WasKeyPressed (state, Keys.NumPad1))
						currentLayer = MapLayerType.Base;
					if (WasKeyPressed (state, Keys.NumPad2))
						currentLayer = MapLayerType.Decal;
					if (WasKeyPressed (state, Keys.NumPad3))
						currentLayer = MapLayerType.Collision;

					if (WasKeyPressed (state, Keys.F5))
						ExportMapData();

					if (startPlacing)
						SetTileAtPoint (new Vector2 (mouseState.X, mouseState.Y), currentTileID);
				}

				if (Moxy.CurrentKeyboard.IsKeyDown (Keys.Tab))
				{
					IsFocused = false;
					Moxy.StateManager.Push ("TilePicker");
				}

				// String debugging
				Vector2 mouseVec = new Vector2 (mouseState.X, mouseState.Y);
				WorldAtCursor = Camera.ScreenToWorld (mouseVec);
				TileAtCursor = new Vector2 ((int) WorldAtCursor.X / 64, (int) WorldAtCursor.Y / 64);
			}

			oldMouse = mouseState;
			old = state;
		}

		public override void Load()
		{
			font = Moxy.ContentManager.Load<SpriteFont> ("Fonts//font");
			highlightTexture = Moxy.ContentManager.Load<Texture2D> ("Interface//highlight");
			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new[] { new Color (100, 100, 100, 100) });

			bgtexture = new Texture2D (Moxy.Graphics, 1, 1);
			bgtexture.SetData (new[] { new Color (255, 255, 255, 255) });
		}

		public override void Draw (SpriteBatch batch)
		{
			Moxy.Graphics.Clear (Color.Black);

			batch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, Camera.GetTransformation(Moxy.Graphics));

			// Show the area where the map can be edited
			batch.Draw (bgtexture, new Rectangle (0, 0, (int)(Map.Dimensions.Width * Map.TileDimensions.Width),
				(int)(Map.Dimensions.Height * Map.TileDimensions.Height)), Color.CornflowerBlue);

			Map.Draw (batch);
			batch.End();

			batch.Begin ();
			if (currentTool == EditorTool.PlaceTiles)
				batch.Draw (Map.Texture, new Vector2 (oldMouse.X, oldMouse.Y), Map.Boundings[currentTileID % Map.Boundings.Length], Color.White);

			if (drawUI)
			{
				// Render tile helper at top
				batch.Draw (texture, new Rectangle (0, 0, 800, 70), Color.DarkGray);
				int startIndex = Helpers.LowerClamp (currentTileID - 5, 0);
				//int endIndex = (int)MathHelper.Min(currentTileID + 10, )

				for (int i = startIndex; i < startIndex + 10; i++)
				{
					int modifier = i - (startIndex + 5);
					batch.Draw (Map.Texture, new Vector2 (368 + (modifier * 64), 2), Map.Boundings[i % Map.Boundings.Length],
					            Color.White);

					if (i == currentTileID)
						batch.Draw (highlightTexture, new Vector2 (368 + (modifier * 64), 2), Color.White);
				}

				batch.DrawString (font, "Tile At Cursor: " + TileAtCursor.ToString(), new Vector2 (10, 80), Color.Red);
				batch.DrawString (font, "Current TileID: " + currentTileID, new Vector2 (10, 100), Color.Red);
				batch.DrawString (font, "Current Layer: " + Enum.GetName (typeof (MapLayerType), currentLayer), new Vector2 (10, 120), Color.Red);
				batch.DrawString (font, "World at Cursor: " + WorldAtCursor.ToString(), new Vector2 (10, 140), Color.Red);
				batch.DrawString (font, "Tiles Drawn: " + Map.TilesDrawn, new Vector2 (10, 180), Color.Red);
				batch.DrawString (font, "FPS: " + Math.Round (1 / Moxy.GameTime.ElapsedGameTime.TotalSeconds, 3).ToString(), new Vector2 (10, 200), Color.Red);
			}

			batch.End();
		}

		public override void OnFocus()
		{
			Camera = new DynamicCamera ();
			Camera.UseBounds = true;
			Camera.MinimumSize = new Size (800, 600);
			Camera.Position = new Vector2 (0, 0);

			MapBuilder builder = new Map2Builder ();
			Map = builder.Build ();
			Map.ViewCamera = Camera;

			//Map = new MapRoot (128, 128, 64, 64, Moxy.ContentManager.Load<Texture2D> ("fantasytileset"), Camera);
			
			//InitializeBaseLayer (0);

			drawUI = true;
			IsFocused = true;
		}

		public override void OnLostFocus()
		{
			drawUI = false;
			startPlacing = false;
			IsFocused = false;
		}

		public MapRoot Map;
		public DynamicCamera Camera;
		public Texture2D highlightTexture;
		public int currentTileID;
		public bool IsFocused;

		private KeyboardState old;
		private MouseState oldMouse;
		private EditorTool currentTool;
		private MapLayerType currentLayer;
		private SpriteFont font;
		private Texture2D texture;
		private Texture2D bgtexture;
		private bool startPlacing;

		private Vector2 TileAtCursor;
		private Vector2 WorldAtCursor;
		private bool drawUI;

		private void HandleCameraControls(KeyboardState state, GameTime gameTime)
		{
			// Moving
			if (state.IsKeyDown(Keys.W))
				Camera.MoveDiff(-new Vector2(0, 600 / Camera.Zoom) * (float)gameTime.ElapsedGameTime.TotalSeconds);
			else if (state.IsKeyDown(Keys.S))
				Camera.MoveDiff (new Vector2 (0, 600 / Camera.Zoom) * (float)gameTime.ElapsedGameTime.TotalSeconds);
			if (state.IsKeyDown(Keys.A))
				Camera.MoveDiff (-new Vector2 (600 / Camera.Zoom, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds);
			else if (state.IsKeyDown(Keys.D))
				Camera.MoveDiff (new Vector2 (600 / Camera.Zoom, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds);

			// Zooming
			if (state.IsKeyDown(Keys.Up))
				Camera.ZoomDiff (0.0075f);
			else if (state.IsKeyDown(Keys.Down))
				Camera.ZoomDiff (-0.0075f);
		}

		public void SetTileAtPoint (Vector2 Location, int Value)
		{
			if (Location.X > 0 && Location.X < Moxy.ScreenWidth && Location.Y > 0 && Location.Y < Moxy.ScreenHeight)
			{
				var worldVec = Camera.ScreenToWorld (Location);
				var tileVec = new Vector2 ((int)worldVec.X / 64, (int)worldVec.Y / 64);

				if (tileVec.X < 0 || tileVec.Y < 0)
					return;

				Map.Layers[(int)currentLayer].Tiles[(int)tileVec.X, (int)tileVec.Y] = (uint)Value;
			}
		}

		private void InitializeBaseLayer (int id)
		{
			for (int x=0; x < Map.Dimensions.Width; x++)
					for (int y=0; y < Map.Dimensions.Height; y++)
						Map.Layers[0].Tiles[x, y] = (uint)id;
		}

		private void ExportMapData()
		{
			StringBuilder b = new StringBuilder();

			for (int i = 0; i < 2; i++)
			{
				b.Append ("map.Layers[0].Tiles = new uint[,] {");

				for (int x=0; x < Map.Dimensions.Width; x++)
				{
					b.Append ("{");
					for (int y=0; y < Map.Dimensions.Height; y++)
					{
						b.Append (Map.Layers[i].Tiles[x, y]);

						if (y+1 < Map.Dimensions.Height)
							b.Append (",");
					}
					b.Append ("},");

				}
				
				b.Append ("};");

				string code = b.ToString();
				if (code != null)
					code = code;
			}
		}

		private bool WasKeyPressed(KeyboardState current, Keys key)
		{
			return current.IsKeyDown (key) && old.IsKeyUp (key);
		}

		private bool WasLeftMousePressed (MouseState current)
		{
			return current.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;
		}

		private enum EditorTool
		{
			PlaceTiles,
			PlaceMonsterSpawners
		}
	}
}
