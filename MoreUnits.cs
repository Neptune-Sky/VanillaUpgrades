using HarmonyLib;
using SFS.Translations;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(Units), nameof(Units.ToDistanceString))]
    public static class DistanceUnits
    {
        [HarmonyPrefix]
        static bool Prefix(this double a, ref string __result)
        {
            if (!(bool)Config.settings["mmUnits"]) return true;

            if (a >= 100000000 && !double.IsInfinity(a))
            {
                __result = (a / 1000000).Round(0.1).ToString(1, true) + "Mm";
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
            if (!(bool)Config.settings["kmsUnits"] && !(bool)Config.settings["cUnits"])
            {
                return true;
            }

            if (a >= 10000 && !double.IsInfinity(a))
            {
                if (a > 2997924 && (bool)Config.settings["cUnits"])
                {
                    __result = (a / 299792458).Round(0.001).ToString(3, true) + "c";
                    return false;
                }
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
