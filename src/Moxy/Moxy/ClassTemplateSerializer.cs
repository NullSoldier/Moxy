using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Moxy.Interface;

namespace Moxy
{
	public static class ClassTemplateSerializer
	{
		public static CharacterInformation Read (XmlTextReader reader)
		{
			reader.ReadStartElement ("Class");
			string name = reader.ReadElementString ("Name");
			reader.ReadEndElement();

			return new CharacterInformation
			{
				Class = name
			};
		}
	}
}
