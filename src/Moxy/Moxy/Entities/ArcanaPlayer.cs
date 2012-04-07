using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moxy.EventHandlers;

namespace Moxy.Entities
{
	public class ArcanaPlayer
		: Entity
	{
		public ArcanaPlayer()
		{
			ParticleManagers = new List<ParticleManager>();
			CollisionRadius = 32;
		}

		public event EventHandler OnDeath;
		public event EventHandler<PlayerMovementEventArgs> OnMovement;

		// Character attributes
		public int CharacterID;
		public string Name;
		public string Class;
		public int Level;
		public int Experience;

		// Character state
		public PlayerIndex PadIndex;
		public float PlayerScore;
		public Color Color;
		public Light Light;
		public bool AIControlled;
		public bool MovementDisabled;
		public float Speed;
		public List<ParticleManager> ParticleManagers;

		public void Damage (float amount)
		{
			if (Health > 0 && !AIControlled)
			{
				GamePad.SetVibration (PadIndex, 0, MathHelper.Lerp (amount, 100, 1));
				GamePad.SetVibration (PadIndex, 0, 0);
			}

			Health -= Math.Max (0, (amount - defense));
		}

		public override void Update (GameTime gameTime)
		{
			if (AIControlled)
				ProcessAI (gameTime);

			Health = MathHelper.Clamp (Health, 0, MaxHealth);
			HandleInput (gameTime);
			Animations.Update (gameTime);
			
			if (Health <= 0 && OnDeath != null)
				OnDeath (this, null);

			Light.Location = Location + new Vector2 (32, 32);
			Collision = Helpers.CreateCenteredRectangle (Location, CollisionRadius, CollisionRadius);
			CollisionCenter = Collision.Center.ToVector2();

			if (needUpdateAnimation)
				SetAnimation (animation);
		}

		public override void Draw (SpriteBatch batch)
		{
			batch.Draw (Texture, new Rectangle ((int)Location.X, (int)Location.Y, 64, 64), Animations.Bounding,
				Color, Rotation - MathHelper.PiOver2, new Vector2 (32, 32), SpriteEffects.None, 0);
		}

		protected virtual void ProcessAI(GameTime gameTime)
		{
		}

		private void HandleInput (GameTime gameTime)
		{
			Vector2 moveVector = Moxy.CurrentPadStates[PadIndex].ThumbSticks.Left
				.SafelyNormalize ()
				.NegateY ();

			// Set rotation
			if (moveVector.Length() != 0)
				base.Rotation = (float)Math.Atan2 (moveVector.Y, moveVector.X);

			// Set animation
			if (moveVector == Vector2.Zero)
				SetAnimation ("Idle");
			else if (lastMovement == Vector2.Zero)
				SetAnimation ("Walk");

			HandleMovement (gameTime, moveVector);
			lastMovement = moveVector;
		}

		private void HandleMovement (GameTime gameTime, Vector2 moveVector)
		{
			var playerMoveEventArgs = new PlayerMovementEventArgs
			{
				CurrentLocation = Location,
				NewLocation = Location + moveVector * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds,
				Player = this,
				Handled = false
			};

			if (OnMovement != null)
				OnMovement (this, playerMoveEventArgs);

			base.Location += !playerMoveEventArgs.Handled
				? moveVector * Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds
				: playerMoveEventArgs.NewLocation;

			if (moveVector.Length () != 0)
				base.Rotation = (float)Math.Atan2 (moveVector.Y, moveVector.X);
		}

		protected int level = 1;
		protected float speed;
		protected float defense;
		protected string animation;
		private Vector2 lastMovement;
		protected bool needUpdateAnimation;

		private void SetAnimation (string animation)
		{
			this.animation = animation;
			Animations.SetAnimation (animation + "_" + level);
		}
	}
}
