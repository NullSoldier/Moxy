﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Moxy.Levels
{
	public class LevelThree
		: BaseLevel
	{
		public LevelThree()
		{
			AmbientLight = new Color (10, 10, 10, 200);
			WaveLength = 30;
			MaxMonsters = 15;
			SpawnIntervalLow = 0.8f;
			SpawnIntervalHigh = 1f;

			AddMonster (5, "Slime");
			AddMonster (1, "EyeBall");

		}
	}
}
