using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SFS.Translations;
using UnityEngine;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(Units), nameof(Units.ToDistanceString))]
    public static class ReimplementMm
    {
        [HarmonyPrefix]
        static bool Prefix(this double a, ref string __result)
        {
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
            if (a >= 10000 && !double.IsInfinity(a))
            {
                __result = (a / 1000).Round(0.1).ToString(1, true) + "km/s";
                return false;
            }
            return true;
        }
    }

}
