using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace TowerFall.ModLoader.mm
{
    public class ModLoader
    {
        public readonly static Version ModfallVersion = new Version(0, 8);
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

        public static void LoadModAssembly(Assembly asm, ModData modData)
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (Exception e)
            {
                Logger.Log($"Failed reading assembly: {e}");
                Errors.Add($"{modData.Name} threw an exception when loading: {e}");
                return;
            }
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (typeof(Mod).IsAssignableFrom(type))
                {
                    var mod = (Mod)type.GetConstructor(_EmptyTypeArray).Invoke(_EmptyObjectArray);
                    mod.Data = modData;
                    Mods.Add(mod);
                }
            }
        }


        public static void DumpTextures()
        {
            // Dump textures
            Logger.Log("[Modfall] Dumping Textures");
            string dumpDir = Path.Combine(PathGame, "DUMP", "Sprites");
            Directory.CreateDirectory(dumpDir);
            Directory.CreateDirectory(Path.Combine(dumpDir, "Atlas"));
            foreach (KeyValuePair<string, Subtexture> subtexture in TFGame.Atlas.SubTextures)
            {
                Dump(subtexture, Path.Combine(dumpDir, "Atlas"));
            }
            foreach (KeyValuePair<string, Subtexture> subtexture in TFGame.BGAtlas.SubTextures)
            {
                Dump(subtexture, Path.Combine(dumpDir, "BGAtlas"));
            }
            foreach (KeyValuePair<string, Subtexture> subtexture in TFGame.BossAtlas.SubTextures)
            {
                Dump(subtexture, Path.Combine(dumpDir, "BossAtlas"));
            }
            foreach (KeyValuePair<string, Subtexture> subtexture in TFGame.MenuAtlas.SubTextures)
            {
                Dump(subtexture, Path.Combine(dumpDir, "MenuAtlas"));
            }

            if (GameData.DarkWorldDLC)
            {
                foreach (KeyValuePair<string, Subtexture> subtexture in TFGame.EditorAtlas.SubTextures)
                {
                    Dump(subtexture, Path.Combine(dumpDir, "EditorAtlas"));
                }
            }

            void Dump(KeyValuePair<string, Subtexture> subtexture, string targetDir)
            {
                Subtexture texture = subtexture.Value;
                Rectangle sourceRectangle = texture.Rect;
                Texture2D cropTexture = new Texture2D(TFGame.Instance.GraphicsDevice, texture.Width, texture.Height);
                Color[] data = new Color[texture.Width * texture.Height];
                texture.Texture2D.GetData(0, sourceRectangle, data, 0, data.Length);
                cropTexture.SetData(data);
                string dir = Path.Combine(targetDir, Path.GetDirectoryName(subtexture.Key));
                Directory.CreateDirectory(dir);
                Stream stream = File.Create(Path.Combine(targetDir, $"{subtexture.Key}.png"));
                cropTexture.SaveAsPng(stream, cropTexture.Width, cropTexture.Height);
                stream.Dispose();
                cropTexture.Dispose();
                GC.Collect();
            }
        }

        static List<string> Blacklist = new List<string>();
        public static void LoadMods()
        {
            Directory.CreateDirectory(PathMods = Path.Combine(PathGame, "Mods")); // Make sure it exists before mods get loaded

            string settingsFile = Path.Combine(PathGame, "modLoaderSettings.xml");
            string settingsFileTxt = Path.Combine(PathGame, "modLoaderSettings.txt");
            string fileVer = "1";
            if (File.Exists(settingsFileTxt))
            {
                string text = File.ReadAllText(settingsFileTxt);
                string[] lines = text.Trim().Split(';');
                foreach (string line in lines)
                {
                    string[] vals = line.Split(':');
                    switch (vals[0].Trim())
                    {
                        case "LogToFile":
                            ModLoaderSettings.LogToFile = bool.Parse(vals[1].Trim());
                            break;
                        case "DumpTextures":
                            DumpTextures();
                            break;
                    }
                }
                File.Delete(settingsFileTxt);
                Logger.Log("[Modfall] Creating modLoaderSettings.xml");
                XmlDocument doc = new XmlDocument();
                XmlElement settings = doc.CreateElement("settings");
                doc.AppendChild(settings);
                settings.CreateChild("fileVer").InnerText = fileVer;
                settings.CreateChild("logToFile").InnerText = ModLoaderSettings.LogToFile.ToString();
                settings.CreateChild("dumpTextures").InnerText = "false";
                doc.Save(settingsFile);
            }
            if (File.Exists(settingsFile))
            {
                XmlElement settings = Calc.LoadXML(settingsFile)["settings"];
                foreach (XmlNode node in settings)
                {
                    if (node is XmlElement)
                    {
                        switch (node.Name)
                        {
                            case "logToFile":
                                ModLoaderSettings.LogToFile = bool.Parse(node.InnerText);
                                break;
                            case "dumpTextures":
                                if (bool.Parse(node.InnerText))
                                    DumpTextures();
                                break;
                        }
                    }
                }
            } else
            {
                Logger.Log("[Modfall] Creating modLoaderSettings.xml");
                XmlDocument doc = new XmlDocument();
                XmlElement settings = doc.CreateElement("settings");
                doc.AppendChild(settings);
                settings.CreateChild("fileVer").InnerText = fileVer;
                settings.CreateChild("logToFile").InnerText = "true";
                settings.CreateChild("dumpTextures").InnerText = "false";
                doc.Save(settingsFile);
                //File.WriteAllText(settingsFile, text);
            }

            if (File.Exists(Path.Combine(PathMods, "blacklist.txt")))
            {
                Logger.Log("[Modfall] Reading blacklist.txt");
                string text = File.ReadAllText(Path.Combine(PathMods, "blacklist.txt"));
                string[] lines = text.Trim().Split('\n');
                foreach (string line in lines)
                {
                    string lineTrim = line.Trim().Trim(';');
                    if (!lineTrim.StartsWith("#"))
                    {
                        Blacklist.Add(Path.Combine(PathMods, lineTrim));
                    }
                }
            } else
            {
                Logger.Log("[Modfall] Creating blacklist.txt");
                string text = $"# This is the blacklist. Type in the names of directories you don't want to load mods from here, seperated by a new line {Environment.NewLine}# Lines starting with # are ignored.";
                File.WriteAllText(Path.Combine(PathMods, "blacklist.txt"), text);
            }
            Logger.Log("[Modfall] Extracting mod .zip files");
            foreach (string modPath in Directory.GetFiles(PathMods))
            {
                if (modPath.EndsWith(".zip") && !Blacklist.Contains(modPath))
                {
                    ZipFile zip = ZipFile.Read(modPath);
                    zip.ExtractAll(Path.Combine(Path.GetDirectoryName(modPath), Path.GetFileNameWithoutExtension(modPath)));
                    zip.Dispose();
                    File.Delete(modPath);
                }
            }

            Logger.Log("[Modfall] Loading Mods");
            foreach (string modPath in Directory.GetDirectories(PathMods))
            {
                if (!Blacklist.Contains(modPath))
                    LoadDir(modPath);
            }

            foreach (Mod mod in Mods)
            {
                Logger.Log($"[Modfall] Initializing Mod: {mod.Data.Name}");
                mod.Load();
            }

        }

        public static void LoadDir(string path)
        {
            ModData modData = GetModData(path);
            string modName = modData.Name;
            if (modData.MinVersion > ModfallVersion)
            {
                Logger.Log($"[Modfall] Mod {modName} requires a newer version of Modfall: {modData.MinVersion}");
                Errors.Add($"{modName} requires a newer version of Modfall: {modData.MinVersion}");
                return;
            }
            Logger.Log($"[Modfall] Loading Mod: {modName}");
            ModPaths.Add(modName, path);
            ModPathsInv.Add(path, modName);
            // Code mods
            if (!string.IsNullOrEmpty(modData.DLL))
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(Path.Combine(path, modData.DLL) + ".dll");
                    
                    LoadModAssembly(asm, modData);
                }
                catch (Exception ex)
                {
                    Errors.Add($"{modName} threw an exception when loading: {ex.Message}");
                    Logger.Log(ex.Message);
                }
            } else
            {
                // No dll, so make a null module
                Mod mod = new NullMod();
                mod.Data = modData;
                Mods.Add(mod);
            }
            // Graphics
            string graphicsPath = Path.Combine(path, "Content", "Graphics");
            if (Directory.Exists(graphicsPath))
            {
                /*
                // Files directly in the Graphics/ folder
                foreach (string file in Directory.GetFiles(graphicsPath))
                {
                    AddSprite(file);
                }
                // files in subfolders
                foreach (string dir in Directory.GetDirectories(graphicsPath))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        AddSprite(file);
                    }
                } */
                Crawl(graphicsPath, delegate (string s) { AddSprite(s); });

                void AddSprite(string spritePath)
                {
                    if (spritePath.EndsWith(".png"))
                    {
                        string virtualPath = spritePath.Substring(graphicsPath.Length + 1);
                        virtualPath = virtualPath.Remove(virtualPath.Length - 4, 4).Replace('\\', '/');
                        Monocle.Texture texture = new Monocle.Texture(spritePath, true);
                        Logger.Log($"[Modfall] Adding graphic {virtualPath}");
                        if (TFGame.Atlas.SubTextures.ContainsKey(virtualPath))
                        {
                            TFGame.Atlas.SubTextures[virtualPath] = new Subtexture(texture);
                        } else
                        {
                            TFGame.Atlas.SubTextures.Add(virtualPath, new Subtexture(texture));
                        }
                    }
                    
                }
            }
            // Atlases
            string atlasPath = Path.Combine(path, "Content", "Atlases");
            // SpriteData
            string spriteDataPath = Path.Combine(atlasPath, "SpriteData");
            if (Directory.Exists(spriteDataPath))
            {
                //foreach (string file in Directory.GetFiles(spriteDataPath))
                Crawl(spriteDataPath, delegate (string file)
                {
                    if (file.EndsWith(".xml"))
                    {
                        Logger.Log("[Modfall] Loading SpriteData: " + Path.GetFileNameWithoutExtension(file));
                        XmlNode xmlNode = Calc.LoadXML(file);

                        foreach (object obj in xmlNode["SpriteData"])
                        {
                            if (obj is XmlElement)
                            {
                                string id = (obj as XmlElement).Attr("id");
                                Logger.Log($"[Modfall] Adding sprite {id} to SpriteData");
                                ((patch_SpriteData)TFGame.SpriteData).Add(id, obj as XmlElement);
                            }
                        }
                    }
                });
            }
            // SFX
            string SFXPath = Path.Combine(path, "Content", "SFX");
            if (Directory.Exists(SFXPath))
            {
                Crawl(SFXPath, delegate (string file)
                {
                    string virtualPath = file.Substring(SFXPath.Length + 1);
                    int extLength = Path.GetExtension(virtualPath).Length;
                    virtualPath = virtualPath.Remove(virtualPath.Length - extLength, extLength).Replace('\\', '/');
                    Logger.Log($"[Modfall] Adding SFX {virtualPath}");
                    SFX sfx = new SFX(file);
                    patch_Sounds.ModSFX.Add(virtualPath, sfx);
                });
            }
        }
        
        public static void Crawl(string dir, Action<string> onFile)
        {
            foreach (string file in Directory.GetFiles(dir))
            {
                onFile(file);
            }
            foreach (string dir2 in Directory.GetDirectories(dir))
            {
                Crawl(dir2, onFile);
            }
        }

        /// <summary>
        /// read the modfall.xml file
        /// </summary>
        /// <param name="modFolderPath"></param>
        /// <returns></returns>
        public static ModData GetModData(string modFolderPath)
        {
            Logger.Log($"[Modfall] Loading ModData from: {modFolderPath}");
            
            ModData data = new ModData();

            // Failsafe if mod.xml doesn't specify version
            data.Version = "1.0.0";
            if (File.Exists(Path.Combine(modFolderPath, "modfall.txt")))
            {
                Logger.Log($"[Modfall] [WARN] The modfall.txt format is deprecated. Please switch to the .xml format!");
                string file = File.ReadAllText(Path.Combine(modFolderPath, "modfall.txt"));
                string[] entries = file.Split(';');
                foreach (string entry in entries)
                {
                    string entry2 = entry.Trim();
                    if (!string.IsNullOrWhiteSpace(entry2))
                        ReadEntry(entry2);
                }
                

                void ReadEntry(string entry)
                {
                    string[] split = entry.Split(':');
                    string name = split[0].ToLower();
                    string value = split[1];
                    data.SetValue(name, value);
                }
                
            }
            // No old .txt file, now look for new xml file
            else if (File.Exists(Path.Combine(modFolderPath, "modfall.xml")))
            {
                XmlElement file = Calc.LoadXML(Path.Combine(modFolderPath, "modfall.xml"))["mod"];
                foreach (XmlNode node in file.ChildNodes)
                {
                    if (node is XmlElement)
                    {
                        XmlElement element = (XmlElement)node;
                        data.SetValue(element.Name.ToLower(), element.InnerText);
                    }
                    
                }
            } else
            {
                data.Name = Path.GetFileNameWithoutExtension(modFolderPath);
                Logger.Log($"[Modfall] [WARN] Didn't find a modfall file! Assuming name {data.Name}!");
            }

            return data;
        }

        public static bool CheckForModfallUpdate()
        {
            if (UpdateAvailable)
            {
                return true;
            }
            if (UpdateChecked)
            {
                return false;
            }
            string url = "https://api.github.com/repos/JaThePlayer/Modfall/releases/latest";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
           | (SecurityProtocolType)768//.Tls11
           | (SecurityProtocolType)3072//.Tls12
           | SecurityProtocolType.Ssl3;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    /*
                    example response:
                    {
                        "url": "https://api.github.com/repos/JaThePlayer/Modfall/releases/23506762",
                        "assets_url": "https://api.github.com/repos/JaThePlayer/Modfall/releases/23506762/assets",
                        "upload_url": "https://uploads.github.com/repos/JaThePlayer/Modfall/releases/23506762/assets{?name,label}",
                        "html_url": "https://github.com/JaThePlayer/Modfall/releases/tag/v0.5b",
                        "id": 23506762,
                        "node_id": "MDc6UmVsZWFzZTIzNTA2NzYy",
                        "tag_name": "v0.5b",
                        ...
                        we want the tag_name
                    */
                    string res;
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        res = reader.ReadToEnd();
                    }
                    string tagName = res.Split(new string[] { "\"tag_name\": " }, StringSplitOptions.None)[1];
                    tagName = tagName.Remove(tagName.IndexOf(',')).Trim().Trim('"').TrimStart('v').TrimEnd('b');
                    Version version = new Version(tagName);
                    if (version > ModfallVersion)
                    {
                        Logger.Log($"[Modfall] [Update Check] New version available!");
                        UpdateAvailable = true;
                        NewestVersion = version;
                        return true;
                    }
                }
            } catch (Exception e)
            {
                Logger.Log($"[Modfall] Update Check failed!");
                Logger.Log($"{e.Message}\n{e.StackTrace}");
                UpdateChecked = true;
            }
            UpdateChecked = true;
            return false;
        }

        
        private static bool UpdateChecked;
        public static Version NewestVersion = ModfallVersion;
        private static bool UpdateAvailable;

        public static bool CheckForModUpdate(ModData data)
        {
            if (ModUpdates.ContainsKey(data.Name))
            {
                return true;
            }
            if (data.GithubLink == null)
            {
                return false;
            }
            string url = $"https://api.github.com/repos/{data.GithubLink.Replace("https://", "").Replace("github.com/", "")}/releases/latest";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
           | (SecurityProtocolType)768//.Tls11
           | (SecurityProtocolType)3072//.Tls12
           | SecurityProtocolType.Ssl3;
            Logger.Log($"[Modfall] [Mod Update] Connecting to {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string res;
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        res = reader.ReadToEnd();
                    }
                    string tagName = res.Split(new string[] { "\"tag_name\": " }, StringSplitOptions.None)[1];
                    tagName = tagName.Remove(tagName.IndexOf(',')).Trim().Trim('"').TrimStart('v').TrimEnd('b');
                    Version version = new Version(tagName);
                    Version curVersion = new Version(data.Version);
                    if (version > curVersion)
                    {
                        Logger.Log($"[Modfall] [Mod Update] New version for mod {data.Name} available!");
                        string download = res.Split(new string[] { "\"browser_download_url\": " }, StringSplitOptions.None)[1];
                        Update update = new Update()
                        {
                            NewVersion = version,
                            OldVersion = curVersion,
                            Name = data.Name,
                            DownloadUrl = download.Remove(download.IndexOf('}')).Trim().Trim('"')
                        };
                        ModUpdates.Add(data.Name, update);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"[Modfall] Update Check for mod {data.Name} failed!");
                Logger.Log($"{e.Message}\n{e.StackTrace}");
            }
            return false;
        }

        /// <summary>
        /// modName, Update
        /// </summary>
        public static Dictionary<string, Update> ModUpdates = new Dictionary<string, Update>();
    }

    public struct Update
    {
        public Version NewVersion;
        public Version OldVersion;
        public string DownloadUrl;
        public string Name;
    }
}
