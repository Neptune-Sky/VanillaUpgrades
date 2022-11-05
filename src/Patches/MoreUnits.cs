using System.Globalization;

namespace VanillaUpgrades;

[HarmonyPatch(typeof(Units), nameof(Units.ToDistanceString))]
public static class DistanceUnits
{
    [HarmonyPrefix]
    static bool Prefix(this double a, ref string __result)
    {
        if (!Config.settingsData.mmUnits) return true;

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
        if (!Config.settingsData.kmsUnits && !Config.settingsData.cUnits) return true;

        if (a >= 10000 && !double.IsInfinity(a))
        {
            if (a > 2997924 && Config.settingsData.cUnits)
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
    [HarmonyPrefix]
    static bool Prefix(this float a, bool forceDecimal, ref string __result)
    {
        if (!Config.settingsData.ktUnits) return true;

        if (a >= 10000 && !float.IsInfinity(a) && Config.settingsData.ktUnits)
        {
            __result = (a / 1000).Round(0.01f).ToString(forceDecimal ? "F1" : "F", CultureInfo.InvariantCulture) +
                       "kt";
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
        if (!Config.settingsData.ktUnits) return true;

        if (a >= 10000 && !float.IsInfinity(a) && Config.settingsData.ktUnits)
        {
            __result = (a / 1000).Round(0.01f).ToString("F", CultureInfo.InvariantCulture) + "kt";
            return false;
        }

        return true;
    }
}