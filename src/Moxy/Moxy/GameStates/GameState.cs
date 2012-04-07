using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.Entities;
using Moxy.Levels;
using Moxy.ParticleSystems;
using Moxy.Events;
using Microsoft.Xna.Framework.Audio;
using Moxy.EventHandlers;

namespace Moxy.GameStates
{
	public class GameState
		: BaseGameState
	{
		public GameState()
			: base ("Game", isOverlay: false, acceptsInput: true)
		{
			players = new List<ArcanaPlayer> (4);
			lights = new List<Light>();
			monsters = new List<Monster>();
			items = new List<Item> ();
			particleManagers = new List<ParticleManager>();

			// Utility queues
			monsterPurgeQueue = new Queue<Monster> ();
			itemPurgeQueue = new Queue<Item> ();
		}

		public override void Update(GameTime gameTime)
		{
			if (!isLoaded)
				return;

			camera.Update (Moxy.Graphics);
			map.Update (gameTime);

			foreach (var player in players)
				player.Update (gameTime);

			if (!InbetweenRounds)
			{
				foreach (Item item in items)
				{
					item.Update (gameTime);

					foreach (ArcanaPlayer player in players)
						item.CheckCollide (player);
				}

				// Clear out items
				while (itemPurgeQueue.Count > 0)
					items.Remove (itemPurgeQueue.Dequeue());

				foreach (Monster monster in monsters)
				{
					monster.Update (gameTime);
					
					foreach (ProjectileEmitter particleManager in particleManagers)
						particleManager.CheckCollision (monster);

					foreach (ArcanaPlayer player in players)
						monster.CheckCollide (player);
				}

				// Clear out monsters
				while (monsterPurgeQueue.Count > 0)
					monsters.Remove (monsterPurgeQueue.Dequeue());

				// Spawn Monsters
				spawnPassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (spawnPassed > Level.SpawnDelay && monsterCount < Level.MaxMonsters)
				{
					var bounds = camera.PlayerFrustrum;
					bounds.Inflate (200, 200);

					int x = 0;
					int y = 0;

					var verticalOrHorizontal = Moxy.Random.NextDouble2 ();
					if (verticalOrHorizontal == 0)
					{
						var topBottom = Moxy.Random.NextDouble2 ();
						x = Moxy.Random.Next (bounds.X, bounds.X + bounds.Width);
						y = bounds.Y + (bounds.Height * topBottom);
					}
					else
					{
						var leftRight = Moxy.Random.NextDouble2 ();
						x = bounds.X + (bounds.Width * leftRight);
						y = Moxy.Random.Next (bounds.Y, bounds.Y + bounds.Height);
					}

					Monster monster = Level.SpawnMonsterRandom ();
					if (monster != null)
					{
						monster.Location = new Vector2(x, y);
						monsterCount++;
						monster.OnDeath += monster_OnDeath;
						monsters.Add(monster);

						//var chanceToAttackGen = Moxy.Random.NextDouble ();
						//TODO: Add proper targeting (chanceToAttackGen <= 0.30f) ? (Player)powerGenerator1 : (Player)gunner1;
						monster.Target = players[0];

						Level.SpawnDelay = Moxy.Random.Next((int)Level.SpawnIntervalLow, (int)Level.SpawnIntervalHigh);
						spawnPassed = 0;
					}
				}
			}

			//GenerateEnergy (gameTime);
			FindMonsterTargets (gameTime);

			// Should we fade the lights?
			if (fadingLight)
			{
				fadePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
				float lerp = MathHelper.Clamp (fadePassed / fadeTotal, 0, 1f);

				float lerpValue = MathHelper.Lerp (startFadeColor.A, Level.AmbientLight.A, fadePassed / fadeTotal);
				map.AmbientColor = new Color (10, 10, 10, (int)lerpValue);
				texture.SetData (new [] { new Color(0, 0, 0, map.AmbientColor.A)});

				if (lerp >= 1)
					fadingLight = false;
			}
			
			// Check if the level timer has expired
			if (Level != null && !InbetweenRounds && DateTime.Now.Subtract (StartLevelTime).TotalSeconds > Level.WaveLength)
			{
				HealPlayers();
				CheckPlayerLevels();
				waveDoneSound.Play (0.8f, 0.1f, 0f);
				LoadNextLevel();
			}
		}

		public void monster_OnDeath (object sender, EventArgs e)
		{
			monsterCount--;

			var monster = sender as Monster;
			monster.OnDeath -= monster_OnDeath;
			monsterPurgeQueue.Enqueue (monster);

			//TODO: Make it get experience for the right team
			players[0].Experience += monster.ScoreGiven;

			var item = monster.DropItem();
			if (item != null)
			{
				item.OnPickup += item_OnPickup;
				items.Add(item);
				Console.WriteLine("Dropping " + Enum.GetName(typeof(ItemID), item.ItemID));
			}
		}

		public void item_OnPickup(object sender, GenericEventArgs<ArcanaPlayer> e)
		{
			Item item = sender as Item;

			e.Data.Experience += 10;
			item.OnPickup -= item_OnPickup;
			itemPurgeQueue.Enqueue (item);

			Moxy.ContentManager.Load<SoundEffect> ("Sounds\\PowerupPickup").Play ();
		}

		public override void Draw(SpriteBatch batch)
		{
			if (!isLoaded)
				return;
			
			DrawGame (batch);
			DrawLights (batch);

			Moxy.Graphics.SetRenderTarget (null);

			// Draw composite
			batch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend);
			
			lightingEffect.Parameters["lightMask"].SetValue (lightTarget);
			lightingEffect.CurrentTechnique.Passes[0].Apply ();

			batch.Draw (gameTarget, Vector2.Zero, Color.White);
			batch.End();
		}

		public override void Load()
		{
			gameTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);
			lightTarget = new RenderTarget2D (Moxy.Graphics, Moxy.ScreenWidth, Moxy.ScreenHeight);

			lightingEffect = Moxy.ContentManager.Load<Effect> ("lighting");
			lightTexture = Moxy.ContentManager.Load<Texture2D> ("light");
			radiusTexture = Moxy.ContentManager.Load<Texture2D> ("Radius");

			waveDoneSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds\\waveComplete");
			levelUpSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds\\LevelUp");

			uiOverlay = (UIOverlay)Moxy.StateManager["UIOverlay"];
			characterSelectState = (CharacterSelectState)Moxy.StateManager["CharacterSelect"];

			gamePauseTimer = new Timer (timer_StartNextRound, null, Timeout.Infinite, Timeout.Infinite);

			ExperienceTable = new int[]
			{
				0,
				1000,
				2000,
				4000
			};
		}

		public override void OnFocus()
		{
			if (characterSelectState.CharactersSelected)
			{
				Reset ();
				LoadMap();
				LoadPlayers();
				LoadNextLevel ();
				characterSelectState.CharactersSelected = false;
			}

			for (int i = 0; i < players.Count; i++)
				players[i].Location = map.PlayerSpawns[i];

			isLoaded = true;
			Moxy.StateManager.Push(uiOverlay);
		}

		public BigBadBoss boss;
		public BaseLevel Level;
		public DynamicCamera camera;
		public bool InbetweenRounds = true;
		public DateTime StartLevelTime;

		private float MaxPlayerDistance = 1000;
		private List<ArcanaPlayer> players;
		private MapRoot map;
		private Texture2D lightTexture;
		private Texture2D texture;
		private Texture2D radiusTexture;
		private SoundEffect waveDoneSound;
		private SoundEffect levelUpSound;
		private List<Light> lights;
		private List<Item> items;
		private List<Monster> monsters;
		private List<ParticleManager> particleManagers;
		private Queue<Item> itemPurgeQueue;
		private Queue<Monster> monsterPurgeQueue;
		private RenderTarget2D gameTarget;
		private RenderTarget2D lightTarget;
		private Effect lightingEffect;
		private UIOverlay uiOverlay;
		private CharacterSelectState characterSelectState;
		private int monsterCount;
		private float spawnPassed;
		private Timer gamePauseTimer;
		private int timeBetweenRounds = 3;
		public int[] ExperienceTable;
		private int playersDead = 0;
		private bool isLoaded;

		private bool fadingLight;
		private Color startFadeColor;
		private float fadePassed;
		private float fadeTotal = 2f;
		
		private void DrawGame (SpriteBatch batch)
		{
			Moxy.Graphics.SetRenderTarget (gameTarget);
			Moxy.Graphics.Clear (Color.CornflowerBlue);

			batch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			map.Draw(batch);

			foreach (ArcanaPlayer player in players)
				player.Draw(batch, camera.ViewFrustrum);

			foreach (Monster monster in monsters)
				monster.Draw (batch, camera.ViewFrustrum);

			foreach (var item in items)
				item.Draw(batch, camera.ViewFrustrum);

			if (boss != null)
				boss.Draw(batch);
			

			batch.End ();

			batch.Begin (SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			foreach (ParticleManager particleManager in particleManagers)
				particleManager.Draw (batch);

			batch.End();
		}

		private void DrawLights(SpriteBatch batch)
		{
			Moxy.Graphics.SetRenderTarget (lightTarget);
			Moxy.Graphics.Clear (Color.CornflowerBlue);

			batch.Begin();
			batch.Draw (texture, new Rectangle (0, 0, 800, 600), new Color (0, 0, 0, 255));
			batch.End();

			batch.Begin (SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None,
				RasterizerState.CullCounterClockwise, null, camera.GetTransformation (Moxy.Graphics));

			foreach (ParticleManager particleManager in particleManagers)
				foreach (Particle particle in particleManager.particles)
					particle.Light.Draw (batch);

			foreach (var monster in monsters)
				if (monster.Light != null)
					monster.Light.Draw(batch);

			foreach (var item in items)
				if (item.Light != null)
					item.Light.Draw(batch);

			foreach (Light light in lights)
				light.Draw (batch);

			foreach (Light light in map.PointLights)
				light.Draw (batch);

			batch.End ();
		}

		private void Reset()
		{
			lights.Clear();
			players.Clear();
			monsters.Clear();

			uiOverlay.ActivePlayers.Clear();
			uiOverlay.StatusBars.Clear();
			uiOverlay.RedEnergyBar = null;
			uiOverlay.RedSkillBar = null;
			uiOverlay.RedRuneBar = null;
			uiOverlay.BlueEnergyBar = null;

			boss = null;
			Moxy.CurrentLevelIndex = -1;
			//TODO: Add blue removable here
		}

		private void LoadPlayers()
		{
			//FireballEmitter = FireballEmitter,

			players.AddRange (characterSelectState.Players);

			foreach (ArcanaPlayer player in players)
			{
				player.Light = new Light (Color.White, lightTexture) { Scale = 1.5f };
				player.Speed = 0.25f;
				player.Color = Color.White;

				player.OnMovement += Player_OnMovement;
				player.OnDeath += Player_OnDeath;

				lights.Add (player.Light);
				camera.ViewTargets.Add (player);
				particleManagers.AddRange (player.ParticleManagers);
			}

			//uiOverlay.ActivePlayers = players;
		}

		private void Player_OnDeath(object sender, EventArgs e)
		{
			playersDead++;

			if (playersDead >= players.Count)
			{
				gamePauseTimer.Change (Timeout.Infinite, Timeout.Infinite);
				boss = new BigBadBoss (players[0].Location);
				boss.Animations.SetAnimation ("Spawn");
			
				Moxy.Dialog.EnqueueMessageBox ("Boss", "Your deaths were\nin vain.", () => Moxy.StateManager.Set ("MainMenu"));
			}
		}

		private void Player_OnMovement (object sender, PlayerMovementEventArgs e)
		{
			// TODO: Add distance limiting
			// TODO: Add collision here
		}

		private void LoadMap()
		{
			camera = new DynamicCamera ();
			camera.MinimumSize = new Size (800, 600);
			camera.UseBounds = true;

			map = new MapRoot(128, 128, 64, 64, Moxy.ContentManager.Load<Texture2D>("tileset"), camera);
			map = Moxy.Maps[0].Build ();
		

			texture = new Texture2D (Moxy.Graphics, 1, 1);
			texture.SetData (new[] { new Color (0, 0, 0, map.AmbientColor.A) });
		}

		private void FindMonsterTargets (GameTime gameTime)
		{
			// TODO: Do this!
		}

		private void LoadNextLevel()
		{
			Moxy.CurrentLevelIndex++;

			// We beat the game! // TODO: Add win screen
			if (Moxy.CurrentLevelIndex >= Moxy.Levels.Length)
			{
				Moxy.CurrentLevelIndex = -1;
				Moxy.StateManager.Set ("MainMenu");
				return;
			}

			monsters.Clear();
			monsterCount = 0;

			Level = Moxy.Levels[Moxy.CurrentLevelIndex];
			InbetweenRounds = true;
			gamePauseTimer.Change (new TimeSpan (0, 0, 0, timeBetweenRounds), new TimeSpan (0, 0, 0, timeBetweenRounds));

			if (Moxy.CurrentLevelIndex == 0)
			{
				Moxy.Dialog.EnqueueTimed ("Boss", "You think you can \n defeat me? Fools!", 3f);
			}

			// Only fade after the first level
			if (Moxy.CurrentLevelIndex > 0)
			{
				startFadeColor = map.AmbientColor;
				fadePassed = 0;
				fadingLight = true;
			}
		}

		private void timer_StartNextRound (object state)
		{
			gamePauseTimer.Change (Timeout.Infinite, Timeout.Infinite);
			StartLevelTime = DateTime.Now;
			InbetweenRounds = false;
		}

		private void HealPlayers()
		{
			foreach (ArcanaPlayer player in players)
 				player.Health = player.MaxHealth;
		}

		private void CheckPlayerLevels()
		{
			foreach (ArcanaPlayer player in players)
			{
				if (player.Level < ExperienceTable.Length && player.Experience > ExperienceTable[player.Level])
				{
					levelUpSound.Play (1.0f, 0f, 0f);
					player.Level++;
				}
			}
		}
	}
}
