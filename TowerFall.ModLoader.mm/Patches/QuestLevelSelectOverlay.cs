using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.
#pragma warning disable CS0169
#pragma warning disable CS0649

namespace TowerFall
{
    class patch_QuestLevelSelectOverlay : QuestLevelSelectOverlay
    {
        public patch_QuestLevelSelectOverlay() : base(null)
        {

        }
        [MonoMod.MonoModIgnore]
        private int statsID;
        [MonoMod.MonoModIgnore]
        private MapScene map;

        public extern void orig_Update();
        public override void Update()
        {

            if (statsID > 0)
                orig_Update();

        }
        public extern void orig_Render();
        public override void Render()
        {
            //if (statsID > 0)
            if (statsID > 0)
                orig_Render();
        }
    }
}
