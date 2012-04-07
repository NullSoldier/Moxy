using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Moxy
{
	public static class Helpers
	{
		public static Texture2D Pixel
		{
			get 
			{ 
				var pixel = new Texture2D(Moxy.Graphics, 1, 1, false, SurfaceFormat.Color); 
				pixel.SetData(new[] { Color.White });
				return pixel;
			}
		}

		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
				return min;
			if (value > max)
				return max;

			return value;
		}

		public static int LowerClamp (int value, int min)
		{
			if (value < min)
				return min;

			return value;
		}

		public static bool WasButtonPressed(this ButtonState currentState, ButtonState lastState)
		{
			return currentState == ButtonState.Pressed && lastState == ButtonState.Released;
		}

		public static Vector2 ToVector2(this Point self)
		{
			return new Vector2 (self.X, self.Y);
		}

		public static Vector2 SafelyNormalize(this Vector2 self)
		{
			if (self.Length () <= 0)
				return self;

			return Vector2.Normalize (self);
		}

		public static Vector2 NegateY(this Vector2 self)
		{
			self.Y *= -1;
			return self;
		}

		public static Rectangle CreateCenteredRectangle(Vector2 location, int width, int height)
		{
			Rectangle rect = new Rectangle((int)location.X , (int)location.Y, 1, 1);
			rect.Inflate (width, height);

			return rect;
		}

		public static int GetLoopableInt (int value, int min, int max)
		{
			if (value < min)
				return max;
			if (value > max)
				return min;

			return value;
		}

		public static int NextDouble2 (this Random self)
		{
			return self.NextDouble () < 0.5 ? 0 : 1;
		}
	}
}
