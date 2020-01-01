using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerFall.ModLoader.mm
{
    public class Mod
    {
        public string Name;
        public string Version = "1.0.0";
        public virtual void Load()
        {
        }
        public virtual void Unload()
        {
        }
        public Mod()
        {
        }

        public int RegisterArrowType(Type type, Subtexture HUDTexture, Color color, Color colorB)
        {
            int index = ModArrow.ModArrowTypes.Count + 11;
            Logger.Log($"[Modfall] Registering Arrow: {type.FullName} as index {index}");
            ModArrow.ModArrowTypes.Add(index, type);
            ModArrow.ModArrowHUDTextures.Add(index, HUDTexture);
            ModArrow.ModArrowColors.Add(index, color);
            ModArrow.ModArrowColorsB.Add(index, colorB);
            Array.Resize(ref patch_Arrow.Colors, patch_Arrow.Colors.Length + 1);
            patch_Arrow.Colors[patch_Arrow.Colors.Length - 1] = color;
            return index;
        }

        public int RegisterPickup(Type PickupType, bool isArrowPickup = false)
        {
            int index = patch_Pickup.ModPickupTypes.Count + 21;
            Logger.Log($"[Modfall] Registering Pickup: {PickupType.FullName} as index {index}");
            patch_Pickup.ModPickupTypes.Add(index, PickupType);
            if (isArrowPickup)
            {
                Logger.Log($"[Modfall] Registering Arrow Pickup: {PickupType.FullName} as index {index}");
                patch_Pickup.ModArrowPickupTypes.Add(index, PickupType);
            }
            return index;
        }

        public void RegisterVariant(Variant variant)
        {
            Logger.Log($"[Modfall] Registering Variant: {variant.Title}");
            patch_MatchVariants.ModVariants.Add(variant);
        }
    }
}
