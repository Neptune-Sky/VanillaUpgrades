using System.Collections.Generic;
using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.UI;
using SFS.WorldBase;
using UnityEngine;

// ReSharper disable InconsistentNaming

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
            if (!BuildSettings.noSnapping)
                return true;
            __result = new List<Vector2>();
            return false;
        }
    }

    [HarmonyPatch(typeof(BuildStatsDrawer), "Draw")]
    internal class DisplayCorrectTWR
    {
        static void Postfix(float ___mass, float ___thrust, TextAdapter ___thrustToWeightText)
        {
            var spaceCenter = Base.planetLoader.spaceCenter;
            var gravityAtLaunchpad = spaceCenter.address.GetPlanet().GetGravity(spaceCenter.LaunchPadLocation.position.magnitude);
            var TWR = ___thrust * 9.8 / (___mass * gravityAtLaunchpad);
            ___thrustToWeightText.Text = TWR.ToString(2, true);
        }
    }
}