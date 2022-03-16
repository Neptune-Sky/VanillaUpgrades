using HarmonyLib;
using SFS.Translations;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(Units), nameof(Units.ToDistanceString))]
    public static class ReimplementMm
    {
        [HarmonyPrefix]
        static bool Prefix(this double a, ref string __result)
        {
            if (!(bool)Config.settings["mmUnits"]) return true;

            if (a >= 10000000 && !double.IsInfinity(a))
            {
                __result = (a / 1000000).Round(0.1).ToString(1, true) + "Mm";
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToVelocityString))]
    public static class KmVelocity
    {
        [HarmonyPrefix]
        static bool Prefix(this double a, ref string __result)
        {
            if (!(bool)Config.settings["kmsUnits"])
            {
                return true;
            }

            if (a >= 10000 && !double.IsInfinity(a))
            {
                __result = (a / 1000).Round(0.1).ToString(1, true) + "km/s";
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToMassString))]
    public static class KtMass
    {
        [HarmonyPrefix]
        static bool Prefix(this float a, bool forceDecimal, ref string __result)
        {
            if (!(bool)Config.settings["ktUnits"]) return false;

            if (a >= 5000 && !float.IsInfinity(a) && !AdvancedInfo.disableKt)
            {
                var newVar = (a / 1000).Round(0.01f).ToString(2, forceDecimal) + "k";
                __result = Loc.main.Mass.Inject(newVar, "value");
                return false;
            }
            return true;
        }


    }

    [HarmonyPatch(typeof(Units), nameof(Units.ToThrustString))]
    public static class KtThrust
    {
        [HarmonyPrefix]
        static bool Prefix(this float a, ref string __result)
        {
            if (!(bool)Config.settings["ktUnits"]) return false;

            if (a >= 10000 && !float.IsInfinity(a) && !AdvancedInfo.disableKt)
            {
                var newVar = (a / 1000).Round(0.01f).ToString(2, false) + "k";
                __result = Loc.main.Thrust.Inject(newVar, "value");
                return false;
            }
            return true;
        }
    }
}
