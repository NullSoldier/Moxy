using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Moxy.Interface
{
	public class CharacterHolder
	{
		public CharacterHolder()
		{
			Characters = new List<CharacterInformation>();
			Classes = new List<CharacterInformation>();
		}

		public int Count
		{
			get { return Characters.Count; }
		}

		public int ClassCount
		{
			get { return Classes.Count; }
		}

		public CharacterInformation GetCharacterSlot (int slot)
		{
			return GetSlotSafely (Characters, slot);
		}

		public CharacterInformation GetClassSlot (int slot)
		{
			return GetSlotSafely (Classes, slot);
		}

		public void Load()
		{
			// Load all characters
			Characters.Add (new CharacterInformation
			{
				Name = "NullSoldier",
				Level = 2,
				Class = "Mage",
				Texture = Moxy.ContentManager.Load<Texture2D> ("Interface//cf_mage")
			});

			Characters.Add (new CharacterInformation
			{
				Name = "Harbing",
				Level = 4,
				Class = "Mage",
				Texture = Moxy.ContentManager.Load<Texture2D> ("Interface//cf_apprentice")
			});

			// Add all the classes
			Classes.Add (new CharacterInformation
			{
				Class = "Mage",
				Texture = Moxy.ContentManager.Load<Texture2D> ("Interface//cf_mage")
			});

			Classes.Add (new CharacterInformation
			{
				Class = "Apprentice",
				Texture = Moxy.ContentManager.Load<Texture2D> ("Interface//cf_apprentice")
			});
		}

		public List<CharacterInformation> Characters;
		public List<CharacterInformation> Classes;

		private CharacterInformation GetSlotSafely (List<CharacterInformation> list, int slot)
		{
			if (slot < 0)
				return list[0];
			if (slot >= Characters.Count)
				return list[Characters.Count - 1];

			return list[slot];
		}
	}

	public class CharacterInformation
	{
		public string Name;
		public int Level;
		public string Class;
		public Texture2D Texture;
	}
}
