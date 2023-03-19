using HarmonyLib;
using SFS.World;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(EffectManager), "CreateExplosion")]
    public class StopExplosions
    {
        [HarmonyPrefix]
        private static bool Prefix()
        {
            return Config.settings.explosions;
        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreatePartOverheatEffect")]
    public class StopEntryExplosions
    {
        [HarmonyPrefix]
        private static bool Prefix()
        {
            return Config.settings.explosions;
        }
    }
}

