using Microsoft.Xna.Framework;
using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerFall
{
    class patch_TreasureChest : TreasureChest
    {
        public patch_TreasureChest() : base(Vector2.Zero, Types.AutoOpen, AppearModes.Enemies, Pickups.Arrows)
        {
            // no-op
        }

        [MonoModIgnore]
        private List<Pickups> pickups;

        public List<Pickups> GetPickups()
        {
            return pickups;
        }

        public void AddPickup(Pickups pickup)
        {
            pickups.Add(pickup);
        }

        public void SetPickups(List<Pickups> pickups)
        {
            this.pickups = pickups;
        }
    }
}
