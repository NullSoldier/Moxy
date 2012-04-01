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
			MaxMonsters = 20;
			SpawnIntervalLow = 1f;
			SpawnIntervalHigh = 2.4f;
			
			AddMonster (2, "Slime");
			AddMonster(1, "EyeBall");
		}
	}
}