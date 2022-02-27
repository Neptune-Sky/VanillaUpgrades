using System;
using HarmonyLib;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.Parts;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    class StopAdaptation
    {
        
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (BuildMenuFunctions.noAdaptation && !BuildMenuFunctions.noAdaptOverride)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    class AdaptPartPicker
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            BuildMenuFunctions.noAdaptOverride = true;
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            BuildMenuFunctions.noAdaptOverride = false;
        }
    }
}
