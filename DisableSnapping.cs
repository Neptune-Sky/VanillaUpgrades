using System;
using SFS.Builds;
using HarmonyLib;
using SFS.Parts.Modules;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(HoldGrid), "GetSnapPosition_Old")]
    public class NotifyMagnet
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            BuildMenuFunctions.partMagnetizing = true;
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            BuildMenuFunctions.partMagnetizing = false;
        }

    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetSnapOffsets))]
    public class KillMagnet
    {
        [HarmonyPrefix]
        static void Prefix(MagnetModule A, MagnetModule B, ref float snapDistance)
        {
            if (BuildMenuFunctions.snapping)
            {
                snapDistance = 0.0f;
            }
        }
    }
}
