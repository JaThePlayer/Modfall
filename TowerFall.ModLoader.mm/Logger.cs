using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerFall.ModLoader.mm
{
    class Logger
    {
        public static string PathLog = Path.Combine(ModLoader.PathGame, "modlog.txt");

        public static void Log(string text)
        {
            if (ModLoaderSettings.LogToFile)
                File.AppendAllText(PathLog, $"[{DateTime.Now.TimeOfDay}]{text}{Environment.NewLine}");
        }
    }
}
