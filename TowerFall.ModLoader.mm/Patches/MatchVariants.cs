using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoMod;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null

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
                if (variant.CanRandom)
                {
                    canRandoms.Add(variant);
                }
            }
        }

    }
}
