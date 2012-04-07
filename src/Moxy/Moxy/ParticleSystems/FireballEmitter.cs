using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Moxy.Entities;
using Moxy.Events;

namespace Moxy.ParticleSystems
{
	public class FireballEmitter
		: ProjectileEmitter
	{
		public FireballEmitter()
		{
			particleDelay = new TimeSpan(0, 0, 0, 0, 0);
			particleDamage = 20;
			maxParticleRange = 1000;
		}
	}
}
