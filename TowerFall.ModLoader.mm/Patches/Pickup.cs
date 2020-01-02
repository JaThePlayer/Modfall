using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS0626 // orig_ method is marked external and has no attributes on it.

namespace TowerFall
{
    class patch_Pickup : Pickup
    {
        public patch_Pickup() : base(Vector2.Zero, Vector2.Zero)
        {
            // no-op
        }
        public static Dictionary<int, Type> ModPickupTypes = new Dictionary<int, Type>();
        public static Dictionary<int, Type> ModArrowPickupTypes = new Dictionary<int, Type>();
        public static Type[] ModPickupArgs = new Type[] { typeof(Vector2), typeof(Vector2), typeof(int) };
        public static extern Pickup orig_CreatePickup(Vector2 position, Vector2 targetPosition, Pickups type, int playerIndex);
        public static Pickup CreatePickup(Vector2 position, Vector2 targetPosition, Pickups type, int playerIndex)
        {
            Pickup pickup;
            if ((int)type > 20)
            {
                Type ctype = ModPickupTypes[(int)type];
                pickup = (Pickup)ctype.GetConstructor(ModPickupArgs).Invoke(new object[3] { position, targetPosition, playerIndex });

            } else
            {
                pickup = orig_CreatePickup(position, targetPosition, type, playerIndex);
            }
            return pickup;
        }
    }
}
