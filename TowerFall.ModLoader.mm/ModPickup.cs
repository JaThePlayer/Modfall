using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TowerFall.ModLoader.mm
{
    public class ModPickup : Pickup
    {
        public ModPickup(Vector2 position, Vector2 targetPosition, int playerIndex) : base(position, targetPosition)
        {
        }
    }
}
