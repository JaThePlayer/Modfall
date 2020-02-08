using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.

namespace TowerFall
{
    class patch_TFGame : TFGame
    {
        public patch_TFGame(bool noIntro) : base (noIntro)
        {
            // to make the compiler shut up
        }

        public static Atlas[] ModAtlases;

        public extern void orig_ctor(bool noIntro);
        [MonoModConstructor]
        public void ctor(bool noIntro)
        {
            File.WriteAllText(Logger.PathLog, "");
            orig_ctor(true);
        }

        public extern void orig_LoadContent();
        // Load mods
        protected override void LoadContent()
        {
            orig_LoadContent();
            ModLoader.mm.ModLoader.LoadMods();
        }

        [MonoModIgnore]
        private static readonly string FILENAME;

        // Inject Modfall information into the log
        public static void Log(Exception e, bool whileLoading)
        {
            string text = "";
            if (File.Exists(FILENAME))
            {
                StreamReader streamReader = new StreamReader(FILENAME);
                text = streamReader.ReadToEnd();
                streamReader.Close();
                if (!text.Contains(">>>>>"))
                {
                    text = "";
                }
            }
            new StackTrace(e, true);
            string modList = "";
            foreach (Mod mod in ModLoader.mm.ModLoader.Mods)
            {
                modList += $"{mod.Data.Name} v{mod.Data.Version}\r\n";
            }
            string text2 = string.Concat(new string[]
            {
                "Ver ",
                Version.ToString(),
                "\r\n",
                "Modfall Ver ",
                ModLoader.mm.ModLoader.ModfallVersion.ToString(),
                "\r\n",
                "Mods Loaded",
                "\r\n",
                modList,
                "\r\n",
                "Time ",
                DateTime.Now.ToString(),
                "\r\n",
                DebugFlags.GetOutput(),
                "\r\n",
                e.ToString(),
                "\r\n"
            });
            TextWriter textWriter = new StreamWriter(FILENAME, false);
            if (text == "")
            {
                textWriter.WriteLine("TowerFall Error Log");
                textWriter.WriteLine("==========================================");
                textWriter.WriteLine(">>>>>");
                textWriter.WriteLine();
                textWriter.Write(text2);
            }
            else
            {
                int num = text.IndexOf(">>>>>") + ">>>>>".Length;
                string str = text.Substring(0, num);
                string str2 = text.Substring(num);
                string text3 = text2;
                if (whileLoading)
                {
                    text3 = "~LOADING ERROR~\r\n" + text3;
                }
                textWriter.Write(str + "\r\n\r\n" + text3 + str2);
            }
            textWriter.Close();
        }
    }
}
