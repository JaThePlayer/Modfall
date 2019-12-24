using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerFall.ModLoader.mm
{
    public struct ModData
    {
        /// <summary>
        /// The mod's name
        /// </summary>
        public string name;
        /// <summary>
        /// Path to the dll relative to mod root
        /// </summary>
        public string dll;

        public void SetValue(string name, string value)
        {
            switch (name)
            {
                case "name":
                    this.name = value;
                    break;
                case "dll":
                    dll = value.Replace('/', Path.DirectorySeparatorChar);
                    break;
            }

        }
    }
}
