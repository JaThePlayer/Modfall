using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerFall;
using TowerFall.ModLoader.mm;

namespace Monocle
{
    static class patch_Calc
    {
        // Quest Treasure chests read the pickup type using this function, but the call is stuck deep in compiler-generated hell
        // Instead, I patch this function to check for modded pickups
        public static T StringToEnum<T>(string str) where T : struct
        {
            if (Enum.IsDefined(typeof(T), str))
            {
                return (T)((object)Enum.Parse(typeof(T), str));
            }
            if (typeof(T) == typeof(Pickups))
            {
                for (int i = 0; i < patch_Pickup.ModPickupTypes.Count; i++)
                {
                    //Type type = patch_Pickup.ModPickupTypes.Values.ToList()[i];
                    string name = patch_Pickup.ModPickupNames.Values.ToList()[i];
                    if (str == name)
                    {
                        return (T)(object)(i + 21);
                    }
                }
                Logger.Log($"[Modfall] [ERROR] Pickup {str} not registered!");
                throw new Exception($"Pickup {str} not registered!");
            }
            throw new Exception("The string cannot be converted to the enum type.");
        }
    }
}
