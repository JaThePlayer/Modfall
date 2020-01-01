using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using TowerFall.ModLoader.mm;

namespace TowerFall
{
    class patch_MatchVariants : MatchVariants
    {
        public patch_MatchVariants()
        {
            // no-op
        }

        public static List<Variant> ModVariants = new List<Variant> { };
        [MonoModIgnore]
        private List<Variant> canRandoms;

        public extern void orig_ctor(bool noPerPlayer = false);
        [MonoModConstructor]
        public void ctor(bool noPerPlayer = false)
        {
            orig_ctor(noPerPlayer);
            // Handle modded variants
            foreach (Variant variant in ModVariants)
            {
                Array.Resize(ref Variants, Variants.Length + 1);
                Variants[Variants.Length - 1] = variant;
                Logger.Log($"[Modfall] Adding variant {variant.Title} to MatchVariants");
                if (variant.CanRandom)
                {
                    canRandoms.Add(variant);
                }
            }
            LogVariants();
        }

    }
}
