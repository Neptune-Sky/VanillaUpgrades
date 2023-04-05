using System.Globalization;
using HarmonyLib;
using SFS.Builds;
using SFS.Translations;
using SFS.UI;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Units), nameof(Units.ToDistanceString))]
    internal static class DistanceUnits
    {
        [HarmonyPrefix]
        private static bool Prefix(this double a, ref string __result)
        {
            if (double.IsInfinity(a)) return true;
            switch (a)
            {
                case >= 9460730472580800 when Config.settings.lyUnits:
                    __result = (a / 946073047258080).ToString("F1", CultureInfo.InvariantCulture) + "ly";
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
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToVelocityString))]
    internal static class VelocityUnits
    {
        [HarmonyPrefix]
        private static bool Prefix(this double a, ref string __result)
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
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToMassString))]
    internal static class KtMass
    {
        private static bool Prefix(this float a, bool forceDecimal, ref string __result)
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
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToThrustString))]
    internal static class KtThrust
    {
        private static bool Prefix(this float a, ref string __result)
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

    [HarmonyPatch(typeof(BuildStatsDrawer), "Draw")]
    internal static class KtMassThrustBuild
    {
        private static void Postfix(float ___mass, float ___thrust, ref TextAdapter ___massText, ref TextAdapter ___thrustText)
        {
            if (___mass > 10000 && Config.settings.ktUnits)
            {
                ___massText.Text = (___mass / 1000).ToString("F1") + "kt";
            }
            if (___thrust > 10000 && Config.settings.ktUnits)
            {
                ___thrustText.Text = (___thrust / 1000).ToString("F") + "kt";
            }
        }
    }
}

