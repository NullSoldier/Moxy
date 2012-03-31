﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy
{
	public class DynamicCamera
	{
		public Vector2 Position = Vector2.Zero;
		public float Zoom = 1;
		public float Rotation = 0;
		public List<Player> ViewTargets = new List<Player> ();

		public Matrix GetTransformation(GraphicsDevice graphicsdevice)
		{
			return Matrix.CreateTranslation (new Vector3 (-Position.X, -Position.Y, 0)) *
				Matrix.CreateRotationZ (Rotation) *
				Matrix.CreateScale (new Vector3 (Zoom, Zoom, 0)) *
				Matrix.CreateTranslation (new Vector3 (
				graphicsdevice.Viewport.Width * 0.5f,
				graphicsdevice.Viewport.Height * 0.5f, 0));
		}

		public void Update(GraphicsDevice graphicsdevice)
		{
			if (ViewTargets.Count > 0)
			{
				Vector2 min = ViewTargets[0].Location;
				Vector2 max = ViewTargets[0].Location;

				for (int i = 1; i < ViewTargets.Count; i++)
				{
					if (ViewTargets[i].Location.X < min.X) min.X = ViewTargets[i].Location.X;
					else if (ViewTargets[i].Location.X > max.X) max.X = ViewTargets[i].Location.X;
					if (ViewTargets[i].Location.Y < min.Y) min.Y = ViewTargets[i].Location.Y;
					else if (ViewTargets[i].Location.Y > max.Y) max.Y = ViewTargets[i].Location.Y;
				}

				Rectangle rect = new Rectangle ((int)min.X, (int)min.Y,
					(int)(max.X - min.X), (int)(max.Y - min.Y));

				rect.Inflate (100, 100);

				desiredPosition = new Vector2 (rect.Center.X, rect.Center.Y);

				float widthdiff = ((float)graphicsdevice.Viewport.Width) / ((float)rect.Width);
				float heightdiff = ((float)graphicsdevice.Viewport.Height) / ((float)rect.Height);
				desiredZoom = Math.Min (widthdiff, heightdiff);
			}

			Position = Vector2.Lerp (Position, desiredPosition, 0.05f);
			Zoom = MathHelper.Lerp (Zoom, desiredZoom, 0.05f);
		}

		private float desiredZoom;
		private Vector2 desiredPosition;

	}
}