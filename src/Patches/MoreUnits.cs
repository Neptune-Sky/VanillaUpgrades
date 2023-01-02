using System.Globalization;
using HarmonyLib;
using SFS.Translations;
using SFS.UI;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Units), nameof(Units.ToDistanceString))]
    public static class DistanceUnits
    {
        [HarmonyPrefix]
        static bool Prefix(this double a, ref string __result)
        {
            if (!Config.settings.mmUnits) return true;

            if (a >= 100000000 && !double.IsInfinity(a))
            {
                __result = (a / 1000000).Round(0.1).ToString("F1", CultureInfo.InvariantCulture) + "Mm";
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToVelocityString))]
    public static class VelocityUnits
    {
        [HarmonyPrefix]
        static bool Prefix(this double a, ref string __result)
        {
            if (!Config.settings.kmsUnits && !Config.settings.cUnits) return true;

            if (a >= 10000 && !double.IsInfinity(a))
            {
                if (a > 2997924 && Config.settings.cUnits)
                {
                    __result = (a / 299792458).Round(0.001).ToString("F3", CultureInfo.InvariantCulture) + "c";
                    return false;
                }

                __result = (a / 1000).Round(0.1).ToString("F1", CultureInfo.InvariantCulture) + "km/s";
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToMassString))]
    public static class KtMass
    {
        static bool Prefix(this float a, bool forceDecimal, ref string __result)
        {
            if (!Config.settings.ktUnits) return true;

            if (a >= 10000 && !float.IsInfinity(a))
            {
                string b = (a / 1000).Round(0.01f).ToString(forceDecimal ? "F1" : "F", CultureInfo.InvariantCulture);
                __result = Loc.main.Mass.Inject(b + "k", "value");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToThrustString))]
    public static class KtThrust
    {
        static bool Prefix(this float a, ref string __result)
        {
            if (!Config.settings.ktUnits) return true;

            if (a >= 10000 && !float.IsInfinity(a))
            {
                string b = (a / 1000).Round(0.01f).ToString("F", CultureInfo.InvariantCulture);
                __result = Loc.main.Thrust.Inject(b + "k", "value");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(SFS.Builds.BuildStatsDrawer), "Draw")]
    static class KtMassThrustBuild
    {
        static void Postfix(float ___mass, float ___thrust, ref TextAdapter ___massText, ref TextAdapter ___thrustText)
        {
            if (___mass > 10000 && Config.settings.ktUnits)
            {
                ___massText.Text = (___mass / 1000).ToString("F1") + "kt";
            }
            if (___thrust > 10000 && Config.settings.ktUnits)
            {
                ___thrustText.Text = (___thrust / 1000).ToString() + "kt";
            }
        }
    }
}

