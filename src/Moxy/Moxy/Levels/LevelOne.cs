﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Levels
{
	public class LevelOne
		: BaseLevel
	{
		public LevelOne()
		{
			AmbientLight = new Color(10, 10, 10, 200);
			WaveLength = 5;
			MaxMonsters = 10;
			SpawnIntervalLow = 1f;
			SpawnIntervalHigh = 2f;
			
			AddMonster (1, "Slime");

		}
	}
}
