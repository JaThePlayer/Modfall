using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
#pragma warning disable CS0649

namespace TowerFall
{
    class patch_ScreenTitle : ScreenTitle
    {
        public patch_ScreenTitle() : base(MainMenu.MenuState.Archives)
        {
            // no-op
        }
        [MonoModIgnore]
        private Dictionary<MainMenu.MenuState, Subtexture> textures;

        public extern void orig_ctor(MainMenu.MenuState state);
        [MonoModConstructor]
        public void ctor(MainMenu.MenuState state)
        {
            orig_ctor(state);
            textures[MainMenu.MenuState.None] = TFGame.MenuAtlas["menuTitles/options"];
        }

    }
}
