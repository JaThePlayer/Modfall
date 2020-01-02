using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.

namespace TowerFall
{
    /*
    // The patch_ class is in the same namespace as the original class.
    // This can be bypassed by placing it anywhere else and using [MonoModPatch("global::Celeste.Player")]

    // Visibility defaults to "internal", which hides your patch from runtime mods.
    // If you want to "expose" new members to runtime mods, create extension methods in a public static class PlayerExt
    class patch_Player : Player
    { // : Player lets us reuse any of its visible members without redefining them.

        public patch_Player(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, HatStates hatState, bool frozen, bool flash, bool indicator)
            : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen, flash, indicator)
        {
            // no-op. MonoMod ignores this - we only need this to make the compiler shut up.
        }

        // MonoMod creates a copy of the original method, called orig_Added.
        public extern void orig_Jump(bool particles, bool canSuper, bool forceSuper, int ledgeDir, bool doubleJump);
        public new void Jump(bool particles, bool canSuper, bool forceSuper, int ledgeDir, bool doubleJump)
        {
            
            orig_Jump(particles, canSuper, forceSuper, ledgeDir, doubleJump);
            Speed.Y -= 10f;
        }
    }
    */
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
            /*
            Draw.SpriteBatch.Begin();
            //Draw.Rect(0, 0, 320, 240, Color.LightBlue);
            //Draw.Text(TFGame.Font, "MODFALL: V0.1", new Vector2(0f, 10f), Color.White);
            Draw.Text(TFGame.Font, "MOD LIST:", new Vector2(0f, 20f), Color.White);
            float nextHeight = 30f;
            foreach (Mod mod in ModLoader.mm.ModLoader.Mods)
            {
                Draw.Text(TFGame.Font, $"{mod.Name.ToUpper()} V{mod.Version.ToUpper()}", new Vector2(0f, nextHeight), Color.White);
                nextHeight += 10f;
            }
            Draw.SpriteBatch.End(); */
            BackState = MenuState.Main;
            //testImage = new Image(ModAtlas.ModAtlases["EpicAtlas"]["yay"]);

            
        }

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
            Draw.Text(TFGame.Font, "MODFALL: V0.1", new Vector2(0f, 10f), Color.White);
            Draw.Text(TFGame.Font, $"MODS LOADED: {ModLoader.mm.ModLoader.Mods.Count}", new Vector2(0f, 20f), Color.White);
            if (ModLoader.mm.ModLoader.Errors.Count > 0)
            {
                Draw.Text(TFGame.Font, $"{ModLoader.mm.ModLoader.Errors.Count} MODS FAILED LOADING!", new Vector2(0f, 30f), Color.Red);
            }

            if (State == MenuState.None)
            {
                Draw.Text(TFGame.Font, "MOD LIST:", new Vector2(0f, 40f), Color.White);
                float nextHeight = 50f;
                foreach (Mod mod in ModLoader.mm.ModLoader.Mods)
                {
                    Draw.Text(TFGame.Font, $"{mod.Data.Name.ToUpper()} V{mod.Data.Version.ToUpper()}", new Vector2(0f, nextHeight), Color.White);
                    nextHeight += 10f;
                }
                BackState = MenuState.Main;
            }
            Draw.SpriteBatch.End();
        }
    }
}
