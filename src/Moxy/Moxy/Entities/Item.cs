﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Events;

namespace Moxy.Entities
{
	public abstract class Item
	{
		public Item()
		{
			Texture = Moxy.ContentManager.Load<Texture2D>("Gems");
			Light = new Light(Color.Transparent);
			Light.Scale = 0.1f;
		}

		public event EventHandler<GenericEventArgs<ArcanaPlayer>> OnPickup;

		public Vector2 Location;
		public Texture2D Texture;
		public Rectangle Bounds;
		public Rectangle Collision;
		public Vector2 CollisionCenter = Vector2.Zero;
		public float CollisionRadius = 10f;
		public Color Color = Color.White;
		public ItemID ItemID;
		public Light Light;
		public bool Enabled = true;

		public void Draw(SpriteBatch batch, Rectangle ViewFrustrum)
		{
			if(ViewFrustrum.Contains(Collision))
				Draw(batch);
		}

		public void Draw(SpriteBatch batch)
		{
			batch.Draw(Texture, Collision, Bounds, Color, 0f, Vector2.Zero, SpriteEffects.None, 0);
		}

		public void Update(GameTime gameTime)
		{
			CollisionCenter = Location;
			Collision = new Rectangle((int)CollisionCenter.X, (int)CollisionCenter.Y, 1, 1);
			Collision.Inflate((int)CollisionRadius, (int)CollisionRadius);
			if (Light != null)
			{
				Light.Location = Location;
				Light.Color = Color;
			}
		}

		public void CheckCollide (ArcanaPlayer player)
		{
			if (Vector2.Distance(CollisionCenter, player.CollisionCenter) < (CollisionRadius + player.CollisionRadius))
			{
				if (OnPlayerCollision(player))
				{
					OnPickup (this, new GenericEventArgs<ArcanaPlayer> (player));
				}
			}
		}

		public abstract bool OnPlayerCollision (ArcanaPlayer p);
	}

	public enum ItemID
	{
		None,
		EarthRune,
		FireRune,
		WindRune,
		WaterRune,
		HealthPowerup,
		ManaPowerup

	}

}
