using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Moxy.Entities;

namespace Moxy.Map
{
	public static class  MapSerializer
	{
		public static void SaveMap(string Filename, MapRoot map)
		{
			var doc = WriteMapRoot(map);
			doc.Save(Filename);
		}

		public static MapRoot ReadMap(string Filename)
		{
			if(!File.Exists(Filename))
				return null;
			var data = File.ReadAllText(Filename);

			var doc = XDocument.Parse(data);

			return ReadMapRoot(doc.Root);
		}

		private static XDocument WriteMapRoot(MapRoot root)
		{
			var doc = new XDocument();
			var rootElement = new XElement("MapRoot");
			rootElement.SetAttributeValue("Width", (int)root.Dimensions.Width);
			rootElement.SetAttributeValue("Height", (int)root.Dimensions.Height);
			rootElement.SetAttributeValue("TileWidth", (int)root.TileDimensions.Width);
			rootElement.SetAttributeValue("TileHeight", (int) root.TileDimensions.Height);
			rootElement.SetAttributeValue("Texture", "tileset"); //Texture.Name Will report null :(
			var layersElement = new XElement("Layers");
			for(var i = 0; i < 3; i++)
			{
				layersElement.Add(WriteMapLayer(root.Layers[i]));
			}
			rootElement.Add(layersElement);
			doc.Add(rootElement);
			return doc;
		}

		private static XElement WriteMapLayer(MapLayer layer)
		{
			var element = new XElement("Layer");
			element.SetAttributeValue("Index", (int)layer.LayerType);
			for(var x = 0; x < layer.Parent.Dimensions.Width; x++)
				for(var y = 0; y < layer.Parent.Dimensions.Height; y++)
				{
					if(layer.Tiles[x,y] != 0)
					{
						var tileElement = new XElement("Tile");
						tileElement.SetAttributeValue("X", x);
						tileElement.SetAttributeValue("Y", y);
						tileElement.SetAttributeValue("ID", layer.Tiles[x,y]);
						element.Add(tileElement);
					}				
				}

			//if(layer.LayerType == MapLayerType.Collision)
			//{
			//    var count = layer.Parent.CollidableID.Count;
			//    var ids = new int[count];
			//    layer.Parent.CollidableID.CopyTo(ids);
			//    for(var x = 0; x < count; x++)
			//    {
			//        var cElement = new XElement("Collide");
			//    }
			//}
			return element;
		}

		private static MapRoot ReadMapRoot(XElement element)
		{
			var mapRoot = new MapRoot(
										Convert.ToInt32(element.Attribute("Width").Value)
			                          , Convert.ToInt32(element.Attribute("Height").Value)
			                          , Convert.ToInt32(element.Attribute("TileWidth").Value)
			                          , Convert.ToInt32(element.Attribute("TileHeight").Value)
			                          , Moxy.ContentManager.Load<Texture2D>(element.Attribute("Texture").Value)
			                          , null);

			var layerElements = element.Element("Layers").Elements("Layer");
			foreach(var layer in layerElements)
			{
				ReadMapLayer(layer, ref mapRoot.Layers[Convert.ToInt32(layer.Attribute("Index").Value)]);
			}
			//mapRoot.PointLights = ReadMapLights(element.Element("Lights"));
			//mapRoot.MonsterSpawners = ReadMapSpawners(element.Element("Spawns"));
			return mapRoot;
		}

		private static void ReadMapLayer(XElement element, ref MapLayer layer)
		{
			var tiles = element.Elements("Tile");
			foreach(var tile in tiles)
			{
				var x = Convert.ToInt32(tile.Attribute("X").Value);
				var y = Convert.ToInt32(tile.Attribute("Y").Value);
				layer.Tiles[x, y] = Convert.ToUInt32(tile.Attribute("ID").Value);
			}
			//if(layer.LayerType == MapLayerType.Collision)
			//{
			//    var collides = element.Elements("Collide");
			//    foreach(var collide in collides)
			//    {
			//        layer.Parent.CollidableID.Add(Convert.ToInt32(collide.Value));
			//    }
			//}
		}

		private static List<Light> ReadMapLights(XElement element, ref List<Light> lights)
		{
			return null;
		}

		private static List<MonsterSpawner> ReadMapSpawners(XElement element)
		{
			return null;
		}


	}
}
