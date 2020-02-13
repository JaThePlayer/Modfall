using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
#pragma warning disable CS0108

namespace TowerFall
{
    class patch_MainMenu : MainMenu
    {
        public patch_MainMenu(MenuState state) : base(state)
        {
            // no-op. MonoMod ignores this - we only need this to make the compiler shut up.
        }
        [MonoModIgnore]
        public extern void MainOptions();
        [MonoModIgnore]
        public extern void MainCredits();
        [MonoModIgnore]
        public extern void MainQuit();
        public extern void orig_CreateMain();
        [MonoModIgnore]
        public extern void TweenBGCameraToY(int y);
        [MonoModIgnore]
        public MenuItem ToStartSelected { get; private set; }
        //public extern MenuItem ToStartSelected;

        public void ModOptions()
        {
            State = MenuState.None;
            //Draw.Rect
            //Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            //Draw.TextRight(TFGame.Font, "MODFALL: V0.1", new Vector2(70f, 10f), Color.White);
            //Draw.SpriteBatch.End();
        }

        /// <summary>
        /// Actually CreateModOptions
        /// </summary>
        public void CreateNone()
        {
            BackState = MenuState.Main;
            float nextHeight = 50f;
            foreach (Mod mod in ModLoader.mm.ModLoader.Mods)
            {
                nextHeight += 10f;
            }

            if (ModLoader.mm.ModLoader.CheckForModfallUpdate())
            {
                nextHeight += 15f;
                if (updateModfallButton == null)
                {
                    updateModfallButton = new BladeButton(nextHeight, "UPDATE", delegate
                    {
                        string args = $"-v n -exe \"{Path.Combine(ModLoader.mm.ModLoader.PathGame, "TowerFall.exe")}\"";
                        Logger.Log($"[Modfall] Updating to version {ModLoader.mm.ModLoader.NewestVersion}");
                        Process.Start(Path.Combine(ModLoader.mm.ModLoader.PathGame, "Modfall.CmdInstaller.exe"), args);
                        Engine.Instance.Exit();
                    });
                    updateModfallButton.Visible = true;
                    updateModfallButton.Selected = true;

                }
            }
            nextHeight += 15f;
            modOptionsButtons = new List<BladeButton>();
            modOptionsButtons.Add(new BladeButton(nextHeight, "MOD UPDATES", delegate
            {
                inModUpdateScreen = true;
                
                foreach (BladeButton button in modOptionsButtons)
                {
                    if (button != null)
                    {
                        button.Visible = false;
                        button.Selected = false;
                    }
                }
                foreach (Mod mod in ModLoader.mm.ModLoader.Mods)
                {
                    ModLoader.mm.ModLoader.CheckForModUpdate(mod.Data);
                    
                }
                float nextH = 60f;
                modUpdateButtons = new List<BladeButton>();
                foreach (Update update in ModLoader.mm.ModLoader.ModUpdates.Values)
                {
                    modUpdateButtons.Add(new BladeButton(nextH, update.Name.ToUpper(), delegate
                    {
                        string args = $"-mod {update.DownloadUrl} \"{ModLoader.mm.ModLoader.ModPaths[update.Name]}\"";
                        Logger.Log($"[Modfall] Updating mod {update.Name} to {update.NewVersion}");
                        Process.Start(Path.Combine(ModLoader.mm.ModLoader.PathGame, "Modfall.CmdInstaller.exe"), args);
                        Engine.Instance.Exit();
                    }));
                    nextH += 20f;
                }
                if (modUpdateButtons.Count > 0)
                {
                    modUpdateButtons[0].Selected = true;
                    for (int i = 0; i < modUpdateButtons.Count; i++)
                    {
                        if (i > 0)
                        {
                            modUpdateButtons[i].UpItem = modUpdateButtons[i - 1];
                        }
                        if (i + 1 < modUpdateButtons.Count)
                        {
                            modUpdateButtons[i].DownItem = modUpdateButtons[i + 1];
                        }
                    }
                }
            }));
            if (updateModfallButton != null)
            {
                updateModfallButton.DownItem = modOptionsButtons[0];
                modOptionsButtons[0].UpItem = updateModfallButton;
            }
            else
            {
                modOptionsButtons[0].Selected = true;
            }
            nextHeight += 10f;
        }

        static bool inModUpdateScreen;
        public void DestroyNone()
        {

        }

        public void CreateMain()
        {
            BladeButton buttonMainQuit = null;
            List<MenuItem> list = new List<MenuItem>();
            // FightButton fightButton = new FightButton(new Vector2(100f, 140f), new Vector2(-160f, 120f));
            FightButton fightButton = new FightButton(new Vector2(160f, 140f), new Vector2(-160f, 120f));
            list.Add(fightButton);
            // CoOpButton coOpButton = new CoOpButton(new Vector2(220f, 140f), new Vector2(480f, 120f));
            CoOpButton coOpButton = new CoOpButton(new Vector2(240f, 140f), new Vector2(480f, 120f));
            list.Add(coOpButton);
            WorkshopButton workshopButton = new WorkshopButton(new Vector2(270f, 210f), new Vector2(270f, 300f));
            list.Add(workshopButton);
            ArchivesButton archivesButton = new ArchivesButton(new Vector2(200f, 210f), new Vector2(200f, 300f));
            list.Add(archivesButton);
            TrialsButton trialsButton = new TrialsButton(new Vector2(130f, 210f), new Vector2(130f, 300f));
            list.Add(trialsButton);
            BladeButton buttonMainOptions;
            BladeButton buttonMainCredits;
            BladeButton buttonModOptions;
            // h = 18
            if (MainMenu.NoQuit)
            {
                buttonModOptions = new BladeButton(188f, "MOD OPTIONS", new Action(this.MainOptions));
                list.Add(buttonModOptions);
                buttonMainOptions = new BladeButton(206f, "OPTIONS", new Action(this.MainOptions));
                list.Add(buttonMainOptions);
                buttonMainCredits = new BladeButton(224f, "CREDITS", new Action(this.MainCredits));
                list.Add(buttonMainCredits);
            }
            else
            {
                buttonModOptions = new BladeButton(174f, "MOD OPTIONS", new Action(ModOptions));
                list.Add(buttonModOptions);
                buttonMainOptions = new BladeButton(192f, "OPTIONS", new Action(this.MainOptions));
                list.Add(buttonMainOptions);
                buttonMainCredits = new BladeButton(210f, "CREDITS", new Action(this.MainCredits));
                list.Add(buttonMainCredits);
                buttonMainQuit = new BladeButton(228f, "QUIT", new Action(this.MainQuit));
                list.Add(buttonMainQuit);
            }
            base.Add<MenuItem>(list);
            fightButton.DownItem = trialsButton;//buttonModOptions;
            fightButton.LeftItem = buttonModOptions;
            fightButton.RightItem = coOpButton;
            coOpButton.DownItem = trialsButton;
            coOpButton.LeftItem = fightButton;
            //buttonMainOptions.UpItem = fightButton;
            buttonMainOptions.UpItem = buttonModOptions;
            buttonMainOptions.DownItem = buttonMainCredits;
            buttonMainOptions.RightItem = trialsButton;
            buttonMainCredits.UpItem = buttonMainOptions;
            buttonMainCredits.RightItem = trialsButton;
            // MOD OPTIONS
            buttonModOptions.UpItem = fightButton;
            buttonModOptions.DownItem = buttonMainOptions;
            buttonModOptions.RightItem = fightButton;
            if (!MainMenu.NoQuit)
            {
                buttonMainCredits.DownItem = buttonMainQuit;
                buttonMainQuit.UpItem = buttonMainCredits;
                buttonMainQuit.RightItem = trialsButton;
            }
            trialsButton.RightItem = archivesButton;
            trialsButton.LeftItem = buttonMainOptions;
            trialsButton.UpItem = coOpButton;
            archivesButton.LeftItem = trialsButton;
            archivesButton.UpItem = coOpButton;
            if (workshopButton != null)
            {
                archivesButton.RightItem = workshopButton;
                coOpButton.DownItem = archivesButton;
                trialsButton.UpItem = fightButton;
                workshopButton.RightItem = null;
                workshopButton.LeftItem = archivesButton;
                workshopButton.UpItem = coOpButton;
            }
            if (this.OldState == MainMenu.MenuState.Options)
            {
                this.ToStartSelected = buttonMainOptions;
            }
            else if (this.OldState == MainMenu.MenuState.Archives)
            {
                this.ToStartSelected = archivesButton;
            }
            else if (this.OldState == MainMenu.MenuState.Workshop)
            {
                this.ToStartSelected = workshopButton;
            }
            else if (this.OldState == MainMenu.MenuState.Credits)
            {
                this.ToStartSelected = buttonMainCredits;
            }
            else if (this.OldState == MainMenu.MenuState.CoOp || MainMenu.RollcallMode == MainMenu.RollcallModes.Quest || MainMenu.RollcallMode == MainMenu.RollcallModes.DarkWorld)
            {
                this.ToStartSelected = coOpButton;
            }
            else if (MainMenu.RollcallMode == MainMenu.RollcallModes.Trials)
            {
                this.ToStartSelected = trialsButton;
            }
            else
            {
                this.ToStartSelected = fightButton;
            }
            this.BackState = MainMenu.MenuState.PressStart;
            this.TweenBGCameraToY(0);
            MainMenu.CurrentMatchSettings = null;
        }

        public extern void orig_Render();
        public override void Render()
        {
            orig_Render();

            //Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            Draw.SpriteBatch.Begin();
            Draw.Text(TFGame.Font, $"MODFALL: {ModLoader.mm.ModLoader.ModfallVersion.ToString()}", new Vector2(0f, 10f), Color.White);
            Draw.Text(TFGame.Font, $"MODS LOADED: {ModLoader.mm.ModLoader.Mods.Count}", new Vector2(0f, 20f), Color.White);
            int count = ModLoader.mm.ModLoader.Errors.Count;
            if (count > 0)
            {
                Draw.Text(TFGame.Font, $"{count} MOD{(count > 1 ? "S" : "")} FAILED LOADING!", new Vector2(0f, 30f), Color.Red);
            }

            // Mod Settings
            if (State == MenuState.None)
            {
                if (inModUpdateScreen)
                {
                    List<Update> updates = ModLoader.mm.ModLoader.ModUpdates.Values.ToList();
                    Draw.Text(TFGame.Font, "MOD UPDATES:", new Vector2(0f, 40f), Color.White);
                    //foreach (BladeButton button in modUpdateButtons)
                    for (int i = 0; i < modUpdateButtons.Count; i++)
                    {
                        BladeButton button = modUpdateButtons[i];
                        button.Update();
                        button.Render();
                        Draw.Text(TFGame.Font, $"{updates[i].OldVersion} -> {updates[i].NewVersion}".ToUpper(), button.Position + Vector2.UnitX * 100f, Color.Yellow);
                    }
                } else
                {
                    Draw.Text(TFGame.Font, "MOD LIST:", new Vector2(0f, 40f), Color.White);
                    float nextHeight = 50f;
                    foreach (Mod mod in ModLoader.mm.ModLoader.Mods)
                    {
                        Draw.Text(TFGame.Font, $"{mod.Data.Name.ToUpper()} V{mod.Data.Version.ToUpper()}", new Vector2(0f, nextHeight), Color.White);
                        nextHeight += 10f;
                    }
                    BackState = MenuState.Main;
                    if (ModLoader.mm.ModLoader.CheckForModfallUpdate())
                    {
                        Draw.Text(TFGame.Font, $"NEW MODFALL VERSION AVAILABLE: {ModLoader.mm.ModLoader.NewestVersion.ToString().ToUpper()}", new Vector2(0f, nextHeight), Color.Yellow);
                        nextHeight += 15f;
                        if (updateModfallButton == null)
                        {
                            updateModfallButton = new BladeButton(nextHeight, "UPDATE", delegate
                            {
                                string args = $"-v n -exe \"{Path.Combine(ModLoader.mm.ModLoader.PathGame, "TowerFall.exe")}\"";
                                Logger.Log($"[Modfall] Updating to version {ModLoader.mm.ModLoader.NewestVersion}");
                                Process.Start(Path.Combine(ModLoader.mm.ModLoader.PathGame, "Modfall.CmdInstaller.exe"), args);
                                Engine.Instance.Exit();
                            });
                            updateModfallButton.Visible = true;
                            updateModfallButton.Selected = true;

                        }
                        else
                        {
                            updateModfallButton.Update();
                            updateModfallButton.Render();
                        }
                        nextHeight += 10f;
                    }

                    foreach (BladeButton button in modOptionsButtons)
                    {
                        button.Update();
                        button.Render();
                    }
                }
            } else
            {
                inModUpdateScreen = false;
            }
                
            Draw.SpriteBatch.End();
        }

        static List<BladeButton> modOptionsButtons;
        static List<BladeButton> modUpdateButtons;
        BladeButton updateModfallButton;
    }
}
