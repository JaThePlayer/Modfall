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
        public string Name;
        public string Version;
        /// <summary>
        /// Path to the dll relative to mod root
        /// </summary>
        public string DLL;

        public void SetValue(string name, string value)
        {
            switch (name)
            {
                case "name":
                    Name = value;
                    break;
                case "dll":
                    DLL = value.Replace('/', Path.DirectorySeparatorChar);
                    break;
                case "version":
                    Version = value;
                    break;
                default:
                    Logger.Log($"[Modfall] Unknown ModData property: {name}");
                    break;
            }

        }
    }
}
