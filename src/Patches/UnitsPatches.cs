using System.Globalization;
using HarmonyLib;
using SFS.Builds;
using SFS.Translations;
using SFS.UI;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Units))]
    internal class UnitsPatches
    {
        [HarmonyPatch(nameof(Units.ToDistanceString))]
        [HarmonyPrefix]
        private static bool DistanceUnits(double a, ref string __result)
        {
            if (double.IsInfinity(a)) return true;
            switch (a)
            {
                case >= 94607304725800440 when Config.settings.lyUnits:
                    __result = (a / 9460730472580044).ToString("F1", CultureInfo.InvariantCulture) + "ly";
                    return false;
                case >= 100000000000 when Config.settings.gmUnits:
                    __result = (a / 1000000000).ToString("F1", CultureInfo.InvariantCulture) + "Gm";
                    return false;
                case >= 100000000 when Config.settings.mmUnits:
                    __result = (a / 1000000).ToString("F1", CultureInfo.InvariantCulture) + "Mm";
                    return false;
                default:
                    return true;
            }
        }
        
        [HarmonyPatch(nameof(Units.ToVelocityString))]
        [HarmonyPrefix]
        private static bool VelocityUnits(double a, ref string __result)
        {
            if (double.IsInfinity(a)) return true;
            switch (a)
            {
                case >= 2997924 when Config.settings.cUnits:
                    __result = (a / 299792458).ToString("F3", CultureInfo.InvariantCulture) + "c";
                    return false;
                case >= 10000 when Config.settings.kmsUnits:
                    __result = (a / 1000).ToString("F1", CultureInfo.InvariantCulture) + "km/s";
                    return false;
                default:
                    return true;
            }
        }
        
        [HarmonyPatch(nameof(Units.ToMassString))]
        [HarmonyPrefix]
        private static bool MassUnits(float a, bool forceDecimal, ref string __result)
        {
            if (float.IsInfinity(a)) return true;
            switch (a)
            {
                case >= 10000 when Config.settings.ktUnits:
                {
                    var b = (a / 1000).ToString(forceDecimal ? "F1" : "F", CultureInfo.InvariantCulture);
                    __result = Loc.main.Mass.Inject(b + "k", "value");
                    return false;
                }
                default:
                    return true;
            }
        }
        
        [HarmonyPatch(nameof(Units.ToThrustString))]
        [HarmonyPrefix]
        private static bool Prefix(float a, ref string __result)
        {
            if (float.IsInfinity(a)) return true;
            switch (a)
            {
                case >= 10000 when Config.settings.ktUnits:
                {
                    var b = (a / 1000).ToString("F1", CultureInfo.InvariantCulture);
                    __result = Loc.main.Thrust.Inject(b + "k", "value");
                    return false;
                }
                default:
                    return true;
            }
        }
    }
    
    [HarmonyPatch(typeof(Units), nameof(Units.ToPercentString), typeof(double), typeof(bool))]
    public class PercentDecimals
    {
        // ReSharper disable once RedundantAssignment
        private static bool Prefix(ref string __result, double a, bool forceDecimals = true)
        {
            if (!Config.settings.morePrecisePercentages) return true;
            a *= 100;

            switch (a)
            {
                case >= 100:
                case 0:
                    __result = a.ToString(0, false);
                    break;
                case >= 10:
                    __result = a.ToString(1, true);
                    break;
                case < 0.01:
                    a = 0.01;
                    __result = a.ToString(2, true);
                    break;
                
                default:
                    __result = a.ToString(2, true);
                    break;
            }

            __result += "%";
            return false;
        }
    }

    [HarmonyPatch(typeof(BuildStatsDrawer), "Draw")]
    internal static class KtMassThrustBuild
    {
        // This is required to make mass and thrust units actually work in the build area because they don't
        // use the existing methods to get their text output by default.
        private static void Postfix(float ___mass, float ___thrust, ref TextAdapter ___massText, ref TextAdapter ___thrustText)
        {
            ___massText.Text = ___mass.ToMassString(false, 4).Split(" ")[1];
            ___thrustText.Text = ___thrust.ToThrustString().Split(" ")[1];
        }
    }
}

