using HarmonyLib;
using SFS.World;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(WorldTime), "GetTimewarpSpeed_Physics")]
    public class AddMoreIndexes
    {
        [HarmonyPrefix]
        public static bool Prefix(ref double __result, int timewarpIndex_Physics)
        {
            if (!Config.settings.higherPhysicsWarp) return true;
            __result = new[] { 1, 2, 3, 5, 10, 25 }[timewarpIndex_Physics];
            return false;

        }
    }

    [HarmonyPatch(typeof(WorldTime))]
    [HarmonyPatch("MaxPhysicsIndex", MethodType.Getter)]
    public class AllowUsingIndexes
    {
        [HarmonyPrefix]
        public static bool Prefix(ref int __result)
        {
            if (!Config.settings.higherPhysicsWarp) return true;
            __result = 5;
            return false;

        }
    }
}

