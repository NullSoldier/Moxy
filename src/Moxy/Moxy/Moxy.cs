using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Moxy.GameStates;
using System.Reflection;
using Moxy.Levels;
using Moxy.Map;

namespace Moxy
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Moxy
		: Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public static Moxy Instance;
		public static Random Random;
		public static int ScreenWidth = 800;
		public static int ScreenHeight = 600;
		public static GameStateManager StateManager;
		public static ContentManager ContentManager;
		public static GraphicsDevice Graphics;
		public static GameTime GameTime;
		public static Dictionary<PlayerIndex, GamePadState> CurrentPadStates;
		public static Dictionary<PlayerIndex, GamePadState> LastPadStates;
		public static KeyboardState CurrentKeyboard;
		public static KeyboardState LastKeyboard;
		public static int CurrentLevelIndex = -1;
		public static MapBuilder[] Maps;
		public static BaseLevel[] Levels;
		public static MapRoot CurrentMap;
		public static DialogRunner Dialog;

		public Moxy()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = Moxy.ScreenWidth;
			graphics.PreferredBackBufferHeight = Moxy.ScreenHeight;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();

			Content.RootDirectory = "Content";

			CurrentPadStates = new Dictionary<PlayerIndex, GamePadState>
			{
				{PlayerIndex.One, GamePad.GetState (PlayerIndex.One)},
				{PlayerIndex.Two, GamePad.GetState (PlayerIndex.Two)},
				{PlayerIndex.Three, GamePad.GetState (PlayerIndex.Three)},
				{PlayerIndex.Four, GamePad.GetState (PlayerIndex.Four)}
			};
			LastPadStates = new Dictionary<PlayerIndex, GamePadState>();
		}

		protected override void LoadContent()
		{
			Instance = this;
			IsMouseVisible = true;
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Moxy.Instance = this;
			Moxy.ContentManager = Content;
			Moxy.StateManager = new GameStateManager();
			Moxy.Random = new Random();
			Moxy.Graphics = GraphicsDevice;
			Moxy.Dialog = new DialogRunner();

			Moxy.StateManager.Load (Assembly.GetExecutingAssembly());
			Moxy.StateManager.Set("MapState");

			Moxy.Maps = new MapBuilder[]
			{
				new Map2Builder()
			};

			Moxy.Levels = new BaseLevel[]
			{
				new LevelOne(),
				new LevelTwo(),
				new LevelThree(), 
				new LevelFour(), 
				new LevelFive(),
				new LevelSix()
			};
		}

		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		protected override void Update(GameTime gameTime)
		{
			Moxy.GameTime = gameTime;

			foreach (PlayerIndex padIndex in CurrentPadStates.Keys.ToArray ())
			{
				if ((int)padIndex < 4)
					CurrentPadStates[padIndex] = GamePad.GetState (padIndex);
			}

			CurrentKeyboard = Keyboard.GetState ();

			Moxy.StateManager.Update (gameTime);
			Moxy.Dialog.Update (gameTime);

			LastKeyboard = CurrentKeyboard;

			foreach (PlayerIndex padIndex in CurrentPadStates.Keys.ToArray())
				LastPadStates[padIndex] = CurrentPadStates[padIndex];

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			Moxy.StateManager.Draw (spriteBatch);
			Moxy.Dialog.Draw (spriteBatch);

			base.Draw(gameTime);
		}

		public static bool WasPadButtonPressed (PlayerIndex index, Buttons button)
		{
			return CurrentPadStates[index].IsButtonDown (button) && LastPadStates[index].IsButtonUp (button);
		}
	}
}
