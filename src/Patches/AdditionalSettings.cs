using SFS.World;

namespace VanillaUpgrades;

[HarmonyPatch(typeof(EffectManager), "CreateExplosion")]
public class StopExplosions
{
    [HarmonyPrefix]
    static bool Prefix()
    {
        return Config.settings.explosions;
    }
}

[HarmonyPatch(typeof(EffectManager), "CreatePartOverheatEffect")]
public class StopEntryExplosions
{
    [HarmonyPrefix]
    static bool Prefix()
    {
        return Config.settings.explosions;
    }
}