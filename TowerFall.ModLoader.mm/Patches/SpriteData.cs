using System;
using System.Collections.Generic;
using System.Xml;
using MonoMod;

namespace Monocle
{
    class patch_SpriteData : SpriteData
    {
        public patch_SpriteData() : base("", null)
        {
            // no-op
        }

        [MonoModIgnore]
        private Dictionary<string, XmlElement> sprites;

        public void Add(string id, XmlElement xmlElement)
        {
            sprites.Add(id, xmlElement);
        }
    }
}
