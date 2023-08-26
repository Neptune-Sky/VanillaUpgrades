using System.Collections.Generic;
using HarmonyLib;
using SFS.Builds;
using SFS.Parts.Modules;
using UnityEngine;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    internal class StopAdaptation
    {
        private static bool Prefix()
        {
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    internal class AdaptPartPicker
    {
        private static void Prefix()
        {
            BuildSettings.noAdaptOverride = true;
        }
        private static void Postfix()
        {
            BuildSettings.noAdaptOverride = false;
        }
    }


    [HarmonyPatch(typeof(AdaptModule), "UpdateAdaptation")]
    internal class FixCucumber
    {
        private static bool Prefix()
        {
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
        }
    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    public class KillMagnet
    {
        private static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (!BuildSettings.snapping)
                return true;
            __result = new List<Vector2>();
            return false;
        }
    }
}