using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerFall.ModLoader.mm
{
    public class ModArrow : Arrow
    {
        public static Dictionary<int, Type> ModArrowTypes = new Dictionary<int, Type>();
        public static Dictionary<int, Subtexture> ModArrowHUDTextures = new Dictionary<int, Subtexture>();
        public static Dictionary<int, Color> ModArrowColors = new Dictionary<int, Color>();
        public static Dictionary<int, Color> ModArrowColorsB = new Dictionary<int, Color>();
        public override ArrowTypes ArrowType => ArrowTypes.Normal;

        protected override void CreateGraphics()
        {
            
        }

        protected override void InitGraphics()
        {
            
        }

        protected override void SwapToBuriedGraphics()
        {
            
        }

        protected override void SwapToUnburiedGraphics()
        {
            
        }
    }
}
