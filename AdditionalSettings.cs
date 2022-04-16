using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using SFS.World;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(PlayerController), "CreateShakeEffect")]
    public class RemoveShake
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (!(bool)Config.settings["shakeEffects"])
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreateExplosion")]
    public class StopExplosions
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (!(bool)Config.settings["explosions"])
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(EffectManager), "CreatePartOverheatEffect")]
    public class StopEntryExplosions
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (!(bool)Config.settings["explosions"])
            {
                return true;
            }
            return true;
        }
    }
}
