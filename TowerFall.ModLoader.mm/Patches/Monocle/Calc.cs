using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        // new
        /// <summary>
        /// Converts a string formatted as x,y to a Vector2
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector2 StringToVec2(string str)
        {
            string[] split = str.Split(',');
            return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
        }

        public static Delegate GetMethod<T>(object obj, string method) where T : class
        {
            if (obj.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) == null)
            {
                // Check the base class as well, for custom enemy classes extending vanilla enemy classes
                MethodInfo baseMethod = obj.GetType().BaseType.GetMethod(method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (baseMethod == null) {
                    return null;
                } else {
                    Delegate.CreateDelegate(typeof(T), obj, baseMethod);
                }
            }
            return Delegate.CreateDelegate(typeof(T), obj, method);
        }
    }
}
