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
    public class ModSpriteData
    {
        public static Dictionary<string, ModSpriteData> ModSpriteDatas = new Dictionary<string, ModSpriteData>();
        public ModSpriteData(string modPath, string filename)
        {
            
            string xmlPath = Path.Combine(modPath, "Content", "Atlases", "SpriteData", filename);
            XmlNode xmlNode = Calc.LoadXML(xmlPath);
            
            this.sprites = new Dictionary<string, XmlElement>();
            
            
            foreach (object obj in xmlNode["SpriteData"])
            {
                if (obj is XmlElement)
                {
                    if ((obj as XmlElement).Name=="Atlas")
                    {
                        ModAtlas atlas = ModAtlas.ModAtlases[(obj as XmlElement).Attr("name")];
                        this.atlas = atlas;
                    } else
                    {
                        sprites.Add((obj as XmlElement).Attr("id"), obj as XmlElement);
                    }
                }
            }
        }

        public bool Contains(string id)
        {
            return this.sprites.ContainsKey(id);
        }

        public XmlElement GetXML(string id)
        {
            return this.sprites[id];
        }

        public Sprite<string> GetSpriteString(string id)
        {
            XmlElement xmlElement = this.sprites[id];
            Sprite<string> sprite = new Sprite<string>(this.atlas[xmlElement.ChildText("Texture")], xmlElement.ChildInt("FrameWidth"), xmlElement.ChildInt("FrameHeight"), 0);
            sprite.Origin = new Vector2(xmlElement.ChildFloat("OriginX", 0f), xmlElement.ChildFloat("OriginY", 0f));
            sprite.Position = new Vector2(xmlElement.ChildFloat("X", 0f), xmlElement.ChildFloat("Y", 0f));
            sprite.Color = xmlElement.ChildHexColor("Color", Color.White);
            XmlElement xmlElement2 = xmlElement["Animations"];
            if (xmlElement2 != null)
            {
                foreach (object obj in xmlElement2.GetElementsByTagName("Anim"))
                {
                    XmlElement xml = (XmlElement)obj;
                    sprite.Add(xml.Attr("id"), xml.AttrFloat("delay", 0f), xml.AttrBool("loop", true), Calc.ReadCSVInt(xml.Attr("frames")));
                }
            }
            return sprite;
        }

        public Sprite<int> GetSpriteInt(string id)
        {
            XmlElement xmlElement = this.sprites[id];
            Sprite<int> sprite = new Sprite<int>(atlas[xmlElement.ChildText("Texture")], xmlElement.ChildInt("FrameWidth"), xmlElement.ChildInt("FrameHeight"), 0);
            sprite.Origin = new Vector2(xmlElement.ChildFloat("OriginX", 0f), xmlElement.ChildFloat("OriginY", 0f));
            sprite.Position = new Vector2(xmlElement.ChildFloat("X", 0f), xmlElement.ChildFloat("Y", 0f));
            sprite.Color = xmlElement.ChildHexColor("Color", Color.White);
            XmlElement xmlElement2 = xmlElement["Animations"];
            if (xmlElement2 != null)
            {
                foreach (object obj in xmlElement2.GetElementsByTagName("Anim"))
                {
                    XmlElement xml = (XmlElement)obj;
                    sprite.Add(xml.AttrInt("id"), xml.AttrFloat("delay", 0f), xml.AttrBool("loop", true), Calc.ReadCSVInt(xml.Attr("frames")));
                }
            }
            return sprite;
        }

        public MotionBlurSprite<int> GetMotionBlurSpriteInt(string id, int blurs)
        {
            XmlElement xmlElement = this.sprites[id];
            MotionBlurSprite<int> motionBlurSprite = new MotionBlurSprite<int>(this.atlas[xmlElement.ChildText("Texture")], xmlElement.ChildInt("FrameWidth"), xmlElement.ChildInt("FrameHeight"), blurs);
            motionBlurSprite.Origin = new Vector2(xmlElement.ChildFloat("OriginX", 0f), xmlElement.ChildFloat("OriginY", 0f));
            motionBlurSprite.Position = new Vector2(xmlElement.ChildFloat("X", 0f), xmlElement.ChildFloat("Y", 0f));
            motionBlurSprite.Color = xmlElement.ChildHexColor("Color", Color.White);
            XmlElement xmlElement2 = xmlElement["Animations"];
            if (xmlElement2 != null)
            {
                foreach (object obj in xmlElement2.GetElementsByTagName("Anim"))
                {
                    XmlElement xml = (XmlElement)obj;
                    motionBlurSprite.Add(xml.AttrInt("id"), xml.AttrFloat("delay", 0f), xml.AttrBool("loop", true), Calc.ReadCSVInt(xml.Attr("frames")));
                }
            }
            return motionBlurSprite;
        }

        public SpritePart<int> GetSpritePartInt(string id)
        {
            XmlElement xmlElement = this.sprites[id];
            SpritePart<int> spritePart = new SpritePart<int>(this.atlas[xmlElement.ChildText("Texture")], xmlElement.ChildInt("FrameWidth"), xmlElement.ChildInt("FrameHeight"), 0);
            spritePart.Origin = new Vector2(xmlElement.ChildFloat("OriginX", 0f), xmlElement.ChildFloat("OriginY", 0f));
            spritePart.Position = new Vector2(xmlElement.ChildFloat("X", 0f), xmlElement.ChildFloat("Y", 0f));
            spritePart.Color = xmlElement.ChildHexColor("Color", Color.White);
            XmlElement xmlElement2 = xmlElement["Animations"];
            if (xmlElement2 != null)
            {
                foreach (object obj in xmlElement2.GetElementsByTagName("Anim"))
                {
                    XmlElement xml = (XmlElement)obj;
                    spritePart.Add(xml.AttrInt("id"), xml.AttrFloat("delay", 0f), xml.AttrBool("loop", true), Calc.ReadCSVInt(xml.Attr("frames")));
                }
            }
            return spritePart;
        }

        public Image GetImage(string id)
        {
            XmlElement xmlElement = this.sprites[id];
            Image image = new Image(this.atlas[xmlElement.ChildText("Texture")], null);
            image.Origin = new Vector2(xmlElement.ChildFloat("OriginX", 0f), xmlElement.ChildFloat("OriginY", 0f));
            image.Position = new Vector2(xmlElement.ChildFloat("X", 0f), xmlElement.ChildFloat("Y", 0f));
            image.Color = xmlElement.ChildHexColor("Color", Color.White);
            if (xmlElement.Name != "image")
            {
                image.ClipRect = new Rectangle(image.ClipRect.X, image.ClipRect.Y, xmlElement.ChildInt("FrameWidth"), xmlElement.ChildInt("FrameHeight"));
            }
            return image;
        }

        public Image GetAutoDetect(string id)
        {
            XmlElement xmlElement = this.sprites[id];
            string name = xmlElement.Name;
            if (name == "image")
            {
                return this.GetImage(id);
            }
            if (name == "sprite_int")
            {
                return this.GetSpriteInt(id);
            }
            if (!(name == "sprite_string"))
            {
                throw new Exception("Sprite type '" + xmlElement.Name + "' not recognized for auto-detect!");
            }
            return this.GetSpriteString(id);
        }

        private ModAtlas atlas;

        private Dictionary<string, XmlElement> sprites;
    }
}
