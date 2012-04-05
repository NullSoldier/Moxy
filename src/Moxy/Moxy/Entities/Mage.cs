using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.EventHandlers;
using Moxy.ParticleSystems;

namespace Moxy.Entities
{
	public class Mage
		: ArcanaPlayer
	{
		public Mage()
		{
			LoadResources();

			EntityType = EntityType.Mage;
			Animations.SetAnimation("Walk_1");
			
			Health = 100;
			MaxHealth = 100;
			Mana = 500;
			MaxMana = 500;
		}

		public FireballEmitter FireballEmitter
		{
			get { return fireballEmitter; }
			set
			{
				fireballEmitter.OnParticleMonsterCollision += fireballEmitter_OnParticleMonsterCollision;
				fireballEmitter = value;
			}
		}

		public SoundEffect fireSound;
		public float Mana;
		public float MaxMana;

		public override void Update (GameTime gameTime)
		{
			HandleInput(gameTime);

			base.Update(gameTime);
		}

		private void HandleInput(GameTime gameTime)
		{
			GamePadState currentPadState = GamePad.GetState(PadIndex);

			AttackTimeElapsed += gameTime.ElapsedGameTime;

			if (currentPadState.ThumbSticks.Right != Vector2.Zero && AttackTimeElapsed > AttackCooldown && Mana >= 10)
			{
				var lookVector = Vector2.Normalize(currentPadState.ThumbSticks.Right);
				lookVector.Y = -lookVector.Y;

				UseFireballSkill (gameTime, lookVector);
				AttackTimeElapsed = TimeSpan.Zero;
			}
		}

		private void UseFireballSkill (GameTime gameTime, Vector2 lookVector)
		{
			// Play a randomized sound
			var firePitch = MathHelper.Lerp ((float)Moxy.Random.NextDouble(), -0.5f, 0.6f);
			var fireVolume = MathHelper.Lerp ((float)Moxy.Random.NextDouble (), 0.7f, 0.8f);
			fireSound.Play (fireVolume, firePitch, 0f);

			fireballEmitter.GenerateParticles (gameTime, lookVector);
			Mana -= 10;
		}

		private void fireballEmitter_OnParticleMonsterCollision (object sender, Events.GenericEventArgs<Monster> e)
		{
			e.Data.Health -= FireballDamage;
		}

		private FireballEmitter fireballEmitter;
		private TimeSpan AttackTimeElapsed;
		private TimeSpan AttackCooldown = new TimeSpan(0, 0, 0, 0, 500);
		private float FireballDamage = 10f;

		private void LoadResources()
		{
			Texture = Moxy.ContentManager.Load<Texture2D> ("Characters//Team1SpriteSheet");
			Animations = new AnimationManager (Texture, new Animation[] 
			{
				new Animation("Idle_1", new Rectangle[]
				{
					new Rectangle(0, 0, 64, 64)
				}),
				new Animation("Idle_2", new Rectangle[]
				{
					new Rectangle(0, 64, 64, 64)
				}),
				new Animation("Idle_3", new Rectangle[]
				{
					new Rectangle(0, 128, 64, 64)
				}),
				new Animation("Idle_4", new Rectangle[]
				{
					new Rectangle(0, 192, 64, 64)
				}),
				new Animation("Walk_1", new Rectangle[] 
				{
					new Rectangle(0, 0, 64, 64),
					new Rectangle(64, 0, 64, 64),
					new Rectangle(128, 0, 64, 64),
				}, new TimeSpan(0, 0, 0, 0, 200)),
				new Animation("Walk_2", new Rectangle[] 
				{
					new Rectangle(0, 64, 64, 64),
					new Rectangle(64, 64, 64, 64),
					new Rectangle(128, 64, 64, 64),
				}, new TimeSpan(0, 0, 0, 0, 200)),
				new Animation("Walk_3", new Rectangle[] 
				{
					new Rectangle(0, 128, 64, 64),
					new Rectangle(64, 128, 64, 64),
					new Rectangle(128, 128, 64, 64),
				}, new TimeSpan(0, 0, 0, 0, 200)),
				new Animation("Walk_4", new Rectangle[] 
				{
					new Rectangle(0, 192, 64, 64),
					new Rectangle(64, 192, 64, 64),
					new Rectangle(128, 192, 64, 64),
				}, new TimeSpan(0, 0, 0, 0, 200))

			});
			fireSound = Moxy.ContentManager.Load<SoundEffect> ("Sounds//Fire");
		}
	}
}
