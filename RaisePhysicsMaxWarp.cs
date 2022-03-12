using HarmonyLib;
using SFS.World;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(TimewarpIndex), "GetTimewarpSpeed_Physics")]
    public class AddMoreIndexes
    {
        [HarmonyPrefix]
        public static bool Prefix(ref double __result, int timewarpIndex_Physics)
        {
            if (Config.higherPhysicsWarp)
            {
                __result = new int[] { 1, 2, 5, 10, 25 }[timewarpIndex_Physics];
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TimewarpIndex), "get_MaxTimewarpIndex_Physics")]
    public class AllowUsingIndexes
    {
        [HarmonyPrefix]
        public static bool Prefix(ref int __result)
        {
            if (Config.higherPhysicsWarp)
            {
                __result = 4;
                return false;
            }
            return true;
        }
    }
}
