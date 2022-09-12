using HarmonyLib;
using SFS.World;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(PlayerController), "CreateShakeEffect")]
    public class RemoveShake
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return Config.settingsData.explosionShake;

        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreateExplosion")]
    public class StopExplosions
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            return Config.settingsData.explosions;
        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreatePartOverheatEffect")]
    public class StopEntryExplosions
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            return Config.settingsData.explosions;
        }
    }

}
