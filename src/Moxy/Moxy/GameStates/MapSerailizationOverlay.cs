using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.Map;

namespace Moxy.GameStates
{
	public class MapSerailizationOverlay
		: BaseGameState
	{
		public MapSerailizationOverlay()
			: base("MapSerailizor", isOverlay: true, acceptsInput: true)
		{

		}

		public override void Load()
		{
			backgroundTexture = new Texture2D(Moxy.Graphics, 1, 1);
			backgroundTexture.SetData(new[] { new Color(255, 255, 255, 255) });
			font = Moxy.ContentManager.Load<SpriteFont>("Fonts//font");
			var saveSize = font.MeasureString("Save");
			var loadSize = font.MeasureString("Load");
			SaveButton = new Rectangle(100, 200, (int) saveSize.X, (int) saveSize.Y);
			LoadButton = new Rectangle(100, 250, (int) loadSize.X, (int) loadSize.Y);
			textLocation = new Vector2(30, 100);
		}

		public override void PostLoad()
		{
			mapState = (MapState)Moxy.StateManager["MapState"];
		}

		public override void Update(GameTime gameTime)
		{
			handleKeys();
			handleMouse();
		}

		public override void Draw(SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			batch.Draw(backgroundTexture, new Rectangle(0, 0, Moxy.ScreenWidth, Moxy.ScreenHeight), null, Color.Black);
			batch.DrawString(font, filename, textLocation, Color.White);
			var saveLocation = new Vector2(SaveButton.X + 5, SaveButton.Y + 5);
			var loadLocation = new Vector2(LoadButton.X + 5, LoadButton.Y + 5);
			batch.DrawString(font, "Save", saveLocation, saveColor);
			batch.DrawString(font, "Load", loadLocation, loadColor);
			batch.End();
		}

		public override void OnFocus()
		{
			map = mapState.Map;

		}

		private void handleMouse()
		{
			var cMouse = Mouse.GetState();
			var location = 0;

			if (SaveButton.Contains(cMouse.X, cMouse.Y))
			{
				location = 1;
				saveColor = Color.Yellow;
			}
			else
				saveColor = Color.White;

			if (LoadButton.Contains(cMouse.X, cMouse.Y))
			{
				location = 2;
				loadColor = Color.Yellow;
			}
			else
				loadColor = Color.White;

			if(cMouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && location != 0)
			{
				switch (location)
				{
					case 1:
						MapSerializer.SaveMap(filename, map);
						break;
					case 2:
						if(File.Exists(filename))
						{
							mapState.Map = MapSerializer.ReadMap(filename);
							mapState.Map.ViewCamera = mapState.Camera;
						}
						else
						{
							Console.WriteLine("Invalid Filename " + filename);
						}
						break;
				}
			}

			oldMouse = cMouse;
		}

		private void handleKeys()
		{
			var newKeys = Moxy.CurrentKeyboard.GetPressedKeys();

			for (var i = 0; i < newKeys.Length; i++)
			{
				var key = newKeys[i];

				if (Moxy.LastKeyboard.IsKeyUp(key))
				{
					switch (key)
					{
						case Keys.Back:
							if (filename.Length > 1)
							{
								filename = filename.Substring(0, filename.Length - 1);
							}
							else
							{
								filename = "";
							}
							break;

						case Keys.RightControl:
						case Keys.LeftControl:
							Moxy.StateManager.Pop();
							mapState.IsFocused = true;
							break;

						case Keys.OemPeriod:
							filename += '.';
							break;

						default:
							var keyV = (int)key;
							if (keyV >= 32 && keyV <= 126)
							{
								var character = char.ToLower((char)keyV);
								if (Moxy.CurrentKeyboard.IsKeyDown(Keys.LeftShift))
									character = char.ToUpper(character);

								filename += character;
							}
							break;
					}
				}
			}
		}

		private Color saveColor = Color.White;
		private Color loadColor = Color.White;
		private Rectangle SaveButton;
		private Rectangle LoadButton;
		private string filename = "";
		private Vector2 textLocation;
		private MapRoot map;
		private MapState mapState;
		private SpriteFont font;
		private Texture2D backgroundTexture;
		private MouseState oldMouse;
	}
}
