using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono;
using TowerFall;

namespace TowerFall.ModLoader.mm
{
    public class ModLoader
    {
        public readonly static Type[] _EmptyTypeArray = new Type[0];
        public readonly static object[] _EmptyObjectArray = new object[0];
        /// <summary>
        /// A list of all loaded mods
        /// </summary>
        public static List<Mod> Mods = new List<Mod>();
        /// <summary>
        /// Mod name -> Mod Path
        /// </summary>
        public static Dictionary<string, string> ModPaths = new Dictionary<string, string>();
        /// <summary>
        /// Mod Path -> Mod name
        /// </summary>
        public static Dictionary<string, string> ModPathsInv = new Dictionary<string, string>();
        public static List<string> Errors = new List<string>();
        public static string PathMods { get; internal set; }
        public static string PathGame = Path.GetDirectoryName(typeof(TFGame).Assembly.Location);

        public static void LoadModAssembly(Assembly asm, string modName)
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            
            catch (Exception e)
            {
                Logger.Log($"Failed reading assembly: {e}");
                Errors.Add($"{asm.FullName} threw an exception when loading: {e}");
                return;
            }
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (typeof(Mod).IsAssignableFrom(type))
                {
                    var mod = (Mod)type.GetConstructor(_EmptyTypeArray).Invoke(_EmptyObjectArray);
                    mod.Name = modName;
                    Mods.Add(mod);
                }
            }
        }

        static List<string> Blacklist = new List<string>();
        public static void LoadMods()
        {
            Directory.CreateDirectory(PathMods = Path.Combine(PathGame, "Mods")); // Make sure it exists before mods get loaded

            string settingsFile = Path.Combine(PathGame, "modLoaderSettings.txt");
            if (File.Exists(settingsFile))
            {
                string text = File.ReadAllText(settingsFile);
                string[] lines = text.Trim().Split(';');
                foreach (string line in lines)
                {
                    string[] vals = line.Split(':');
                    switch(vals[0].Trim())
                    {
                        case "LogToFile":
                            ModLoaderSettings.LogToFile = bool.Parse(vals[1].Trim());
                            break;
                    }
                }
            } else
            {
                Logger.Log("[Modfall] Creating modLoaderSettings.txt");
                string text = $"LogToFile:true;";
                File.WriteAllText(settingsFile, text);
            }

            if (File.Exists(Path.Combine(PathMods, "blacklist.txt")))
            {
                Logger.Log("[Modfall] Reading blacklist.txt");
                string text = File.ReadAllText(Path.Combine(PathMods, "blacklist.txt"));
                string[] lines = text.Trim().Split(';');
                foreach (string line in lines)
                {
                    if (!line.Trim().StartsWith("#"))
                    {
                        Blacklist.Add(Path.Combine(PathMods, line.Trim()));
                    }
                }
            } else
            {
                string text = $"# This is the blacklist. Type in the names of directories you don't want to load mods from here, seperated by ; {Environment.NewLine}# Lines starting with # are ignored, though they still need to end with ;";
                File.WriteAllText(Path.Combine(PathMods, "blacklist.txt"), text);
            }
            Logger.Log("[Modfall] Loading Mods");
            foreach (string modPath in Directory.GetDirectories(PathMods))
            {
                if (!Blacklist.Contains(modPath))
                    LoadDir(modPath);
            }

            foreach (Mod mod in Mods)
            {
                Logger.Log($"[Modfall] Initializing Mod: {mod.Name}");
                mod.Load();
            }

        }

        public static void LoadDir(string path)
        {
            ModData modData = GetModData(path);
            string modName = modData.name;
            Logger.Log($"[Modfall] Loading Mod: {modName}");
            ModPaths.Add(modName, path);
            ModPathsInv.Add(path, modName);
            // Code mods
            if (!string.IsNullOrEmpty(modData.dll))
            {
                try
                {
                    LoadModAssembly(Assembly.LoadFile(Path.Combine(path, modData.dll) + ".dll"), modName);
                }
                catch (Exception ex)
                {
                    Errors.Add($"{modName} threw an exception when loading: {ex.Message}");
                    Logger.Log(ex.Message);
                }
            } else
            {
                // No dll, so make a null module
                Mods.Add(new NullMod(modName));
            }
            // Graphics
            // Atlases
            string atlasPath = Path.Combine(path, "Content", "Atlases");
            if (Directory.Exists(atlasPath))
            {
                foreach(string file in Directory.GetFiles(atlasPath))
                {
                    if (file.EndsWith(".xml"))
                    {
                        Logger.Log("[Modfall] Adding Atlas: " + Path.GetFileNameWithoutExtension(file));
                        ModAtlas.ModAtlases.Add(Path.GetFileNameWithoutExtension(file), new ModAtlas(path, file));
                    }
                        
                }
            }
            // SpriteData
            string spriteDataPath = Path.Combine(atlasPath, "SpriteData");
            if (Directory.Exists(spriteDataPath))
            {
                foreach (string file in Directory.GetFiles(spriteDataPath))
                {
                    Logger.Log("[Modfall] Adding SpriteData: " + Path.GetFileNameWithoutExtension(file));
                    if (file.EndsWith(".xml"))
                        //ModAtlas.ModAtlases.Add(Path.GetFileNameWithoutExtension(file), new ModAtlas(path, file));
                        ModSpriteData.ModSpriteDatas.Add(Path.GetFileNameWithoutExtension(file), new ModSpriteData(path, file));
                    
                }
            }
        }

        /// <summary>
        /// read the modfall.txt file
        /// </summary>
        /// <param name="modFolderPath"></param>
        /// <returns></returns>
        public static ModData GetModData(string modFolderPath)
        {
            Logger.Log($"[Modfall] Loading ModData from: {modFolderPath}");
            string file = File.ReadAllText(Path.Combine(modFolderPath, "modfall.txt"));
            ModData data = new ModData();
            string[] entries = file.Split(';');
            foreach (string entry in entries)
            {
                string entry2 = entry.Trim();
                if (!string.IsNullOrWhiteSpace(entry2))
                ReadEntry(entry2);
            }
            return data;

            void ReadEntry(string entry)
            {
                string[] split = entry.Split(':');
                string name = split[0].ToLower();
                string value = split[1];
                data.SetValue(name, value);
            }
        }
    }
}
