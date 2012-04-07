using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Moxy.Entities;

namespace Moxy
{
	public static class CharacterSerializer
	{
		static CharacterSerializer()
		{
			classConstructors = new Dictionary<string, Func<ArcanaPlayer>>
			{
				{"Mage", () => new Mage()}
			};
		}

		public static void WriteCharacter (XmlTextWriter writer, ArcanaPlayer player)
		{
			writer.WriteStartElement ("Character");
			
			writer.WriteElementString ("CharacterID", player.CharacterID.ToString ());
			writer.WriteElementString ("Name", player.Name);
			writer.WriteElementString ("Class", player.Class);
			writer.WriteElementString ("Level", player.Level.ToString ());

			writer.WriteEndElement();
		}

		public static ArcanaPlayer ReadCharacter (XmlTextReader reader)
		{
			reader.ReadStartElement ("Character");

			int id = Convert.ToInt32 (reader.ReadElementString ("CharacterID"));
			string name = reader.ReadElementString ("Name");
			string pclass = reader.ReadElementString ("Class");
			int level = Convert.ToInt32 (reader.ReadElementString ("Level"));

			reader.ReadEndElement ();

			ArcanaPlayer p = classConstructors[pclass] ();
			{
				p.CharacterID = id;
				p.Name = name;
				p.Class = pclass;
				p.Level = level;
			}
			return p;
		}

		private static Dictionary<string, Func<ArcanaPlayer>> classConstructors;
	}
}
