using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.

namespace TowerFall
{
    class patch_ArrowHUD : ArrowHUD
    {
        [MonoModIgnore]
        private Subtexture[] images;

        public extern void orig_ctor();
        [MonoModConstructor]
        public void ctor()
        {
            orig_ctor();
            for (int i = 0; i < ModArrow.ModArrowHUDTextures.Count; i++)
            {
                Array.Resize(ref images, images.Length + 1);
                
                images[i + 11] = ModArrow.ModArrowHUDTextures[i + 11];
            }
        }
    }
}
