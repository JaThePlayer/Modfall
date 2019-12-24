using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TowerFall.ModLoader.mm
{
    public class ModAtlas
    {
        public Dictionary<string, Subtexture> SubTextures;
        public static Dictionary<string, ModAtlas> ModAtlases = new Dictionary<string, ModAtlas>();

        public ModAtlas(string modPath, string fileName)
        {
            string xmlPath = Path.Combine(modPath, "Content", "Atlases", fileName);
            XmlNodeList elementsByTagName = Calc.LoadXML(xmlPath)["TextureAtlas"].GetElementsByTagName("SubTexture");
            SubTextures = new Dictionary<string, Subtexture>(elementsByTagName.Count);
            foreach (object obj in elementsByTagName)
            {
                XmlAttributeCollection attributes = ((XmlElement)obj).Attributes;
                Texture texture = new Texture(Path.Combine(modPath, "Content", "Graphics", attributes["name"].Value + ".png"), true);
                SubTextures.Add(attributes["name"].Value, new Subtexture(texture, 0, 0, texture.Width, texture.Height));
            }
        }

        public Subtexture this[string name]
        {
            get
            {
                return SubTextures[name];
            }
        }

        public Subtexture this[string name, Rectangle subRect]
        {
            get
            {
                return new Subtexture(this[name], subRect.X, subRect.Y, subRect.Width, subRect.Height);
            }
        }
    }
}
