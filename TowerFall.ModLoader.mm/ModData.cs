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
        /// <summary>
        /// Link to the github repo for this mod
        /// </summary>
        public string GithubLink;

        public Version MinVersion;

        public void SetValue(string name, string value)
        {
            switch (name.ToLower())
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
                case "github":
                    GithubLink = value;
                    break;
                case "minversion":
                    MinVersion = new Version(value);
                    break;
                default:
                    Logger.Log($"[Modfall] Unknown ModData property: {name}");
                    break;
            }

        }
    }
}
