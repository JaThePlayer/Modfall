using System;
using System.IO;

namespace TowerFall.ModLoader.mm
{
    public class Logger
    {
        public static string PathLog = Path.Combine(ModLoader.PathGame, "modlog.txt");

        public static void Log(string text)
        {
            if (ModLoaderSettings.LogToFile)
                File.AppendAllText(PathLog, $"[{DateTime.Now.TimeOfDay}]{text}{Environment.NewLine}");
        }
    }
}
