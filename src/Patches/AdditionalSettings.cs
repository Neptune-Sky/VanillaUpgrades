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
            if (!Config.settings.explosionShake)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreateExplosion")]
    public class StopExplosions
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (!Config.settings.explosions)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreatePartOverheatEffect")]
    public class StopEntryExplosions
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (!Config.settings.explosions)
            {
                return true;
            }
            return true;
        }
    }

}
