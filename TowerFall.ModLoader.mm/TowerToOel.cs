using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TowerFall.ModLoader.mm
{
    public static class TowerToOel
    {
        /// <summary>
        /// Converts a .tower file to .oel
        /// </summary>
        /// <param name="towerPath"></param>
        /// <returns></returns>
        public static void Convert(string path)
        {
            
            // Init
            string Title;
            string ThemeName;
            string Darkness;
            string BGstr;
            string Solidstr;
            string BGTileset = "";
            string BGTilestr;
            string Entitystr;

            // Read .tower
            XmlElement tower = Calc.LoadXML(path + ".tower")["tower"];
            Title = tower.ChildText("title");
            ThemeName = tower.ChildText("tileset");
            Darkness = tower.ChildText("darkOpacity");
            BGTileset = tower.ChildText("bgtileset");
            bool Cold = tower.ChildBool("cold");
            string Tileset = tower.ChildText("tileset");
            // Read level data, we'll only worry about the first level
            XmlElement target = (XmlElement)tower.GetElementsByTagName("level")[0];
            BGstr = target.ChildText("BG");
            Solidstr = target.ChildText("Solids");
            BGTilestr = target.ChildText("BGTiles");
            Entitystr = target.GetElementsByTagName("Entities")[0].InnerXml;
            // BGTiles and Solids get compressed in .tower files, undo that
            BGstr = UncompressTiles(BGstr);
            Solidstr = UncompressTiles(Solidstr);
            // Save .oel
            XmlDocument oel = new XmlDocument();
            XmlElement level = oel.CreateElement("level");
            oel.AppendChild(level);
            level.SetAttr("width", "320");
            level.SetAttr("height", "240");
            level.SetAttr("Darkness", Darkness);
            level.SetAttr("WrapMode", "Both");
            level.SetAttr("CanUnlockMoonstone", "False");
            level.SetAttr("CanUnlockPurple", "False");
            level.SetAttr("MiasmaH", "False");
            level.CreateChild("title").InnerText = Title;
            level.CreateChild("theme").InnerText = ThemeName;
            level.CreateChild("tileset").InnerText = Tileset;
            level.CreateChild("cold").InnerText = Cold.ToString();
            XmlElement bg = oel.CreateElement("BG");
            bg.SetAttr("exportMode", "Bitstring");
            bg.InnerText = BGstr;
            level.AppendChild(bg);

            XmlElement solid = oel.CreateElement("Solids");
            solid.SetAttr("exportMode", "Bitstring");
            solid.InnerText = Solidstr;
            level.AppendChild(solid);

            XmlElement bgtiles = oel.CreateElement("BGTiles");
            bgtiles.SetAttr("exportMode", "TrimmedCSV");
            bgtiles.SetAttr("tileset", BGTileset);
            bgtiles.InnerText = BGTilestr;
            level.AppendChild(bgtiles);

            XmlElement entities = oel.CreateElement("Entities");
            entities.InnerXml = Entitystr;
            int playerCount = 0;
            for (int i = entities.ChildNodes.Count - 1; i > -1; i -= 1)
            {
                XmlElement xmlElement = (XmlElement)entities.ChildNodes[i];
                xmlElement.SetAttr("id", i);
                
                if(xmlElement.Name == "Spawner")
                {
                    xmlElement.SetAttr("name", "nameHere" + i);
                }
                if(xmlElement.Name == "PlayerSpawn")
                {
                    playerCount++;
                    if (playerCount > 2)
                    {
                        entities.RemoveChild(xmlElement);
                    }
                }
            }

            level.AppendChild(entities);

            oel.Save(path + ".oel");
        }

        public static string UncompressTiles(string compressed)
        {
            string text = "";
            // w:32
            // h:24
            string[] lines = compressed.Split();
            for (int i = 0; i < lines.Length; i++)
            {
                while (lines[i].Length < 32)
                {
                    // Line got compressed, fill the rest with 0's
                    lines[i] += "0";
                }
                text += lines[i] + Environment.NewLine;
            }
            return text;
        }
    }
}
