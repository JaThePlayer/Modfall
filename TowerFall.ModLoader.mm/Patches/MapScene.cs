using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod;
using TowerFall.ModLoader.mm;

namespace TowerFall
{
    class patch_MapScene : MapScene
    {
        public patch_MapScene() : base(MainMenu.RollcallModes.DarkWorld)
        {

        }
        [MonoModIgnore]
        private extern void InitVersusButtons();

        bool loaded = false;
        public extern void orig_Begin();
        public override void Begin()
        {
            if (!loaded)
            {
                Entity entity = new Entity(-1);
                entity.Add(this.Renderer = new MapRenderer(false));
                base.Add<Entity>(entity);
            }
            
            this.Buttons = new List<MapButton>();
            if (this.Mode == MainMenu.RollcallModes.Versus)
            {
                InitVersusButtons();
            }
            else if (this.Mode == MainMenu.RollcallModes.Quest)
            {
                Buttons.Add(new GoToWorkshopMapButton());
                for (int i = 0; i < GameData.QuestLevels.Length; i++)
                {
                    if (SaveData.Instance.Unlocks.GetQuestTowerUnlocked(i))
                    {
                        this.Buttons.Add(new QuestMapButton(GameData.QuestLevels[i]));
                    }
                }
                
                LinkButtonsList();
            }
            else if (Mode == MainMenu.RollcallModes.DarkWorld)
            {
                for (int j = 0; j < GameData.DarkWorldTowers.Count; j++)
                {
                    if (SaveData.Instance.Unlocks.GetDarkWorldTowerUnlocked(j))
                    {
                        Buttons.Add(new DarkWorldMapButton(GameData.DarkWorldTowers[j]));
                    }
                }
                LinkButtonsList();
            }
            else
            {
                if (Mode != MainMenu.RollcallModes.Trials)
                {
                    throw new Exception("Mode not recognized!");
                }
                List<MapButton[]> list = new List<MapButton[]>();
                for (int k = 0; k < GameData.VersusTowers.Count; k++)
                {
                    if (SaveData.Instance.Unlocks.GetTowerUnlocked(k))
                    {
                        MapButton[] array = new MapButton[GameData.TrialsLevels.GetLength(1)];
                        for (int l = 0; l < array.Length; l++)
                        {
                            array[l] = new TrialsMapButton(GameData.TrialsLevels[k, l]);
                            Buttons.Add(array[l]);
                        }
                        for (int m = 0; m < array.Length; m++)
                        {
                            if (m > 0)
                            {
                                array[m].UpButton = array[m - 1];
                            }
                            if (m < array.Length - 1)
                            {
                                array[m].DownButton = array[m + 1];
                            }
                        }
                        list.Add(array);
                    }
                }
                for (int n = 0; n < list.Count; n++)
                {
                    if (n > 0)
                    {
                        for (int num = 0; num < list[n].Length; num++)
                        {
                            list[n][num].LeftButton = list[n - 1][num];
                        }
                    }
                    if (n < list.Count - 1)
                    {
                        for (int num2 = 0; num2 < list[n].Length; num2++)
                        {
                            list[n][num2].RightButton = list[n + 1][num2];
                        }
                    }
                    for (int num3 = 0; num3 < list[n].Length; num3++)
                    {
                        list[n][num3].MapXIndex = n;
                    }
                }
            }
            foreach (MapButton entity2 in this.Buttons)
            {
                Add(entity2);
            }
            MapButton mapButton;
            if (Mode == MainMenu.RollcallModes.Versus)
            {
                if (MainMenu.VersusMatchSettings.LevelSystem.CustomTower)
                {
                    mapButton = Buttons[0];
                }
                else if (MainMenu.VersusMatchSettings.RandomVersusTower)
                {
                    mapButton = Buttons[1];
                    for (int num4 = 0; num4 < this.Buttons.Count; num4++)
                    {
                        if (Buttons[num4] is VersusRandomSelect)
                        {
                            mapButton = Buttons[num4];
                            break;
                        }
                    }
                }
                else
                {
                    mapButton = GetButtonFromID(MainMenu.VersusMatchSettings.LevelSystem.ID);
                    if (mapButton == null)
                    {
                        mapButton = Buttons[1];
                    }
                }
            }
            else if (Mode == MainMenu.RollcallModes.Trials)
            {
                mapButton = GetButtonFromID(MainMenu.TrialsMatchSettings.LevelSystem.ID);
                if (mapButton == null)
                {
                    mapButton = Buttons[0];
                }
            }
            else if (Mode == MainMenu.RollcallModes.Quest)
            {
                mapButton = GetButtonFromID(MainMenu.QuestMatchSettings.LevelSystem.ID);
                if (mapButton == null)
                {
                    mapButton = Buttons[0];
                }
            }
            else
            {
                if (Mode != MainMenu.RollcallModes.DarkWorld)
                {
                    throw new Exception("Mode not recognized!");
                }
                mapButton = GetButtonFromID(MainMenu.DarkWorldMatchSettings.LevelSystem.ID);
                if (mapButton == null)
                {
                    mapButton = Buttons[0];
                }
            }
            if (mapButton.Data == null)
            {
                Renderer.OnStartSelection("");
            }
            else
            {
                Renderer.OnStartSelection(mapButton.Data.Title);
            }
            this.InitButtons(mapButton);
            this.CanAct = false;
            base.Add<CoroutineEntity>(new CoroutineEntity(this.IntroSequence()));
            base.Camera.Position = MapScene.ClampCamera(this.Selection.MapPosition);
            if (!loaded)
            {
                this.Cursor = new MapCursor(this.Selection);
                base.Add<MapCursor>(this.Cursor);
            }
            if (this.Mode == MainMenu.RollcallModes.Trials)
            {
                base.Add<TrialsLevelSelectOverlay>(this.trialsOverlay = new TrialsLevelSelectOverlay(this));
            }
            else if (this.Mode == MainMenu.RollcallModes.Quest)
            {
                base.Add<QuestLevelSelectOverlay>(this.questOverlay = new QuestLevelSelectOverlay(this));
            }
            else if (this.Mode == MainMenu.RollcallModes.DarkWorld)
            {
                base.Add<DarkWorldLevelSelectOverlay>(this.darkWorldOverlay = new DarkWorldLevelSelectOverlay(this));
            }
            if ((this.Mode == MainMenu.RollcallModes.Trials || this.Mode == MainMenu.RollcallModes.Versus) && !GameData.DarkWorldDLC)
            {
                base.Add<MapDarkWorldGate>(new MapDarkWorldGate(this));
            }
            //scene.begin
            UpdateEntityLists();
            foreach (KeyValuePair<int, Layer> keyValuePair in this.Layers)
            {
                keyValuePair.Value.Begin();
            }
            loaded = true;
        }

        private IEnumerator QuestIntroSequence()
        {
            int num;
            for (int i = 1; i < this.Buttons.Count; i = num + 1)
            {
                if (SaveData.Instance.Quest.ShouldRevealTower(this.Buttons[i].Data.ID.X))
                {
                    Music.Stop();
                    yield return this.Buttons[i].UnlockSequence(true);
                }
                num = i;
            }
            yield break;
        }

        public extern void orig_ExitWorkshop();
        public void ExitWorkshop()
        {
            if (Mode != MainMenu.RollcallModes.Quest)
            {
                orig_ExitWorkshop();
                return;
            }
            WorkshopLevels = false;
            TweenOutAllButtons();
            Buttons.Clear();
            Begin();
        }
        [MonoModIgnore]
        private QuestLevelSelectOverlay questOverlay;
        [MonoModIgnore]
        private TrialsLevelSelectOverlay trialsOverlay;
        [MonoModIgnore]
        private DarkWorldLevelSelectOverlay darkWorldOverlay;
        [MonoModIgnore]
        private extern IEnumerator IntroSequence();
    }
}
