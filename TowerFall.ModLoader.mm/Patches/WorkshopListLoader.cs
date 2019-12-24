using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TowerFall.ModLoader.mm;

namespace TowerFall
{
    class patch_WorkshopListLoader : WorkshopListLoader
    {
        public patch_WorkshopListLoader() : base(null)
        {

        }
        [MonoMod.MonoModIgnore]
        private MapScene map;
        [MonoMod.MonoModIgnore]
        private List<MapButton> buttons;

        public extern void orig_Load();

        public void Load()
        {   
            if (map.Mode != MainMenu.RollcallModes.Quest)
            {
                orig_Load();
                return;
            }
            int num = 0;
            int id = 14;
            foreach (string file in ModLoader.mm.ModLoader.ModPaths.Values)
            {
                // Load custom Co-Op levels
                
                if (Directory.Exists(Path.Combine(file, "Levels", "Quest")))
                {
                    string questPath = Path.Combine(file, "Levels", "Quest");
                    foreach (string file3 in Directory.EnumerateFiles(questPath, "*.tower"))
                    {
                        // if there isn't a .oel file already, convert a .tower file to .oel
                        string pathNoExt = Path.Combine(questPath, Path.GetFileNameWithoutExtension(file3));
                        if (!File.Exists(pathNoExt + ".oel"))
                        {
                            TowerToOel.Convert(pathNoExt);
                        }
                    }
                    foreach (string file2 in Directory.EnumerateFiles(questPath, "*.oel"))
                    {
                        try
                        {
                            Logger.Log("Trying to load:" + file2);
                            patch_QuestLevelData data2 = (patch_QuestLevelData)GameData.QuestLevels[0];
                            QuestLevelData data = data2.DeepClone();
                            data.Path = file2;
                            data.DataPath = Path.Combine(Path.GetDirectoryName(file2), Path.GetFileNameWithoutExtension(file2)) + "data.xml";
                            data.ID.X = id;

                            XmlElement xmlElement = Calc.LoadXML(data.Path)["level"];

                            // Theme preset
                            if (xmlElement.HasChild("theme"))
                            {
                                data.Theme = new TowerTheme(GameData.Themes[xmlElement.ChildText("theme")]);
                            } else
                            {
                                data.Theme = new TowerTheme(GameData.Themes["SacredGround"]);
                            }

                            // Build Icon
                            if (xmlElement.HasChild("icon"))
                            {
                                Texture2D icon = TowerMapData.BuildIcon(xmlElement["icon"].ChildText("data"), data.Theme.TowerType);
                                Monocle.Texture texture = new Monocle.Texture(icon);
                                data.Theme.Icon = new Subtexture(texture);
                            }
                            // Get name
                            if (xmlElement.HasChild("title"))
                            {
                                data.Theme.Name = xmlElement.ChildText("title");
                            }
                            else {
                                data.Theme.Name = Path.GetFileNameWithoutExtension(file2).ToUpper() + " - " + ModLoader.mm.ModLoader.ModPathsInv[file].ToUpper();
                            }

                            Array.Resize(ref GameData.QuestLevels, id+1);
                            Array.Resize(ref SaveData.Instance.Quest.Towers, id+1);
                            
                            GameData.QuestLevels[id] = data;
                            
                            SaveData.Instance.Quest.Towers[id] = new QuestTowerStats();
                            SaveData.Instance.Quest.Towers[id].Revealed = true;
                            QuestMapButton item = new QuestMapButton(data);
                            //if (xmlElement.HasChild("author"))
                            //{
                            //    item.Data.Author = xmlElement.ChildText("author");
                            //}
                            item.Data.Author = "FROM: " + ModLoader.mm.ModLoader.ModPathsInv[file].ToUpper();
                            ((patch_MapButton)(MapButton)item).SetAuthor("FROM: " + ModLoader.mm.ModLoader.ModPathsInv[file].ToUpper());
                            buttons.Add(item);
                            id++;
                        }
                        catch (Exception e)
                        {
                            num++;
                            Logger.Log(e.Message + e.StackTrace);
                        }
                    }
                }
                
            }
            
        }
    }
}
