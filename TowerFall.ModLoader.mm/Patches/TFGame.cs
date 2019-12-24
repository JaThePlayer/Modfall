using System;
using System.Collections.Generic;
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
    }
}
