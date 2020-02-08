using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using MonoMod;

namespace TowerFall
{
    class patch_EnemyArrowHUD : EnemyArrowHUD
    {
        public patch_EnemyArrowHUD() : base(null)
        {
            // no-op
        }

        [MonoModIgnore]
        private Subtexture[] images;

        public static List<Subtexture> modImages = new List<Subtexture>();
        public extern void orig_ctor(Enemy enemy);
        [MonoModConstructor]
        public void ctor(Enemy enemy)
        {
            orig_ctor(enemy);
            for (int i = 0; i < modImages.Count; i++)
            {
                AddImage(modImages[i]);
            }
        }

        private void AddImage(Subtexture texture)
        {
            Array.Resize(ref images, images.Length + 1);
            images[images.Length-1] = texture;
        }
    }
}
