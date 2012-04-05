using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moxy.Entities;
using Microsoft.Xna.Framework;

namespace Moxy.EventHandlers
{
	public class PlayerMovementEventArgs
		: EventArgs
	{
		public Vector2 CurrentLocation;
		public Vector2 NewLocation;
		public ArcanaPlayer Player;
		public bool Handled;
	}
}
