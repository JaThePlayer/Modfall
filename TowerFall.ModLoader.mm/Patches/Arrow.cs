using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod;
using TowerFall.ModLoader.mm;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.

namespace TowerFall
{
    class patch_Arrow : Arrow
    {
        [MonoModIgnore]
        public override ArrowTypes ArrowType => ArrowTypes.Normal;
        [MonoModIgnore]
        protected override void CreateGraphics()
        {
            
        }
        [MonoModIgnore]
        protected override void InitGraphics()
        {
            
        }
        [MonoModIgnore]
        protected override void SwapToBuriedGraphics()
        {
            
        }
        [MonoModIgnore]
        protected override void SwapToUnburiedGraphics()
        {
        }
        [MonoModIgnore]
        private static Stack<Arrow>[] cached;
        //[MonoModIgnore]
        //extern new void Init(LevelEntity owner, Vector2 position, float direction);

        public static void Initialize()
        {
            // Handle modded arrows, or we will crash!
            patch_Arrow.cached = new Stack<Arrow>[Arrow.ARROW_TYPES + ModArrow.ModArrowTypes.Count];
            for (int i = 0; i < patch_Arrow.cached.Length; i++)
            {
                patch_Arrow.cached[i] = new Stack<Arrow>();
            }
        }

        public static extern Arrow orig_Create(ArrowTypes type, LevelEntity owner, Vector2 position, float direction, int? overrideCharacterIndex = null, int? overridePlayerIndex = null);
        public static Arrow Create(ArrowTypes type, LevelEntity owner, Vector2 position, float direction, int? overrideCharacterIndex = null, int? overridePlayerIndex = null)
        {
            patch_Arrow arrow;
            if ((int)type > 10)
            {
                // custom arrow
                Type ctype = ModArrow.ModArrowTypes[(int)type];
                arrow = (patch_Arrow)ctype.GetConstructor(ModLoader.mm.ModLoader._EmptyTypeArray).Invoke(ModLoader.mm.ModLoader._EmptyObjectArray);
                arrow.OverrideCharacterIndex = overrideCharacterIndex;
                arrow.OverridePlayerIndex = overridePlayerIndex;
                arrow.Init(owner, position, direction);
            } else
            {
                // vanilla arrow
                return orig_Create(type, owner, position, direction, overrideCharacterIndex, overridePlayerIndex);
            }
            return arrow;
        }

        public static Color[] Colors = new Color[]
        {
                Calc.HexToColor("F7EAC3"),
                Calc.HexToColor("F8B800"),
                Calc.HexToColor("F8B800"),
                Calc.HexToColor("B8F818"),
                Calc.HexToColor("F87858"),
                Calc.HexToColor("8EE8FF"),
                Calc.HexToColor("00FF4C"),
                Calc.HexToColor("FF6DFA"),
                Calc.HexToColor("BC70FF"),
                Calc.HexToColor("1BB7EE"),
                Calc.HexToColor("DB4ADB")
        };
        public static Color[] ColorsB = new Color[]
            {
                Calc.HexToColor("FFFFFF"),
                Calc.HexToColor("F7D883"),
                Calc.HexToColor("F7D883"),
                Calc.HexToColor("D0F76C"),
                Calc.HexToColor("F7B09E"),
                Calc.HexToColor("D8F7FF"),
                Calc.HexToColor("00D33B"),
                Calc.HexToColor("FFB5FC"),
                Calc.HexToColor("D5A5FF"),
                Calc.HexToColor("56D4FF"),
                Calc.HexToColor("FF52FF")
            };
    }
}
