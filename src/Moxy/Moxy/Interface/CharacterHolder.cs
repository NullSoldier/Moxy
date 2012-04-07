using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy.Interface
{
	public class CharacterHolder
	{
		public CharacterHolder()
		{
			Characters = new List<CharacterInformation>();
			Classes = new List<CharacterInformation>();
			characters = new Dictionary<int, ArcanaPlayer>();
		}

		public List<CharacterInformation> Characters;
		public List<CharacterInformation> Classes;

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
			ReadWriteTestStructure ();

			// Load class templates
			string classTemplateRoot = Path.Combine (Environment.CurrentDirectory, "Content//classes.xml");

			using (XmlTextReader treader = new XmlTextReader (classTemplateRoot))
			{
				treader.ReadStartElement ("Classes");

				while (treader.Read () && treader.NodeType != XmlNodeType.EndElement)
				{
					CharacterInformation template = ClassTemplateSerializer.Read (treader);
					template.Texture = Moxy.ContentManager.Load<Texture2D> ("Interface//cf_" + template.Class.ToLower());
					Classes.Add (template);
				}

				treader.ReadEndElement();
			}

			// Load all characters
			foreach (string file in Directory.EnumerateFiles (Path.Combine(Environment.CurrentDirectory, "saves")))
			{
				using (XmlTextReader reader = new XmlTextReader (file))
				{
					reader.MoveToElement();

					while (reader.Read())
					{
						ArcanaPlayer player = CharacterSerializer.ReadCharacter (reader);
						Characters.Add (new CharacterInformation (player));
						characters.Add (player.CharacterID, player);
					}
				}
			}
		}

		public ArcanaPlayer LoadCharacter(int id)
		{
			if (!characters.ContainsKey (id))
				throw new ArgumentException("Character not found with ID " + id);

			return characters[id];
		}

		private Dictionary<int, ArcanaPlayer> characters;

		private CharacterInformation GetSlotSafely (List<CharacterInformation> list, int slot)
		{
			if (slot < 0)
				return list[0];
			if (slot >= list.Count)
				return list[list.Count - 1];

			return list[slot];
		}

		private void ReadWriteTestStructure()
		{
			string saveRoot = Path.Combine (Environment.CurrentDirectory, "saves");
			string characterPath = Path.Combine (saveRoot, "NullSoldier");

			if (!Directory.Exists (saveRoot))
				Directory.CreateDirectory (saveRoot);

			if (!File.Exists (characterPath))
			{
				ArcanaPlayer testPlayer = new ArcanaPlayer
				{
					CharacterID = 0,
					Name = "NullSoldier",
					Class = "Mage",
					Level = 4
				};

				File.Create (Path.Combine (characterPath)).Close ();
				XmlTextWriter testWriter = new XmlTextWriter (characterPath, Encoding.UTF8);
				CharacterSerializer.WriteCharacter (testWriter, testPlayer);
				testWriter.Flush();
				testWriter.Close();
			}
		}
	}

	public class CharacterInformation
	{
		public CharacterInformation (ArcanaPlayer player = null)
		{
			if (player != null)
			{
				this.Name = player.Name;
				this.Class = player.Class;
				this.Texture = Moxy.ContentManager.Load<Texture2D> ("Interface//cf_" + player.Class.ToLower());
				this.Player = player;
			}
		}

		public string Name;
		public string Class;
		public Texture2D Texture;
		public ArcanaPlayer Player;
	}
}
