using HarmonyLib;
using SFS.Achievements;
using SFS.UI;
using SFS.World;
using SFS.WorldBase;
using System;
using UnityEngine;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(AchievementsModule), "Log_Planet")]
    public class StopTimewarpOnEncounter
    {
        [HarmonyPostfix]
        public static void Postfix(Planet planet, Planet planet_Old)
        {
            if (planet.parentBody == planet_Old && Config.settings.stopTimewarpOnEncounter)
            {
                WorldTime.main.SetState(2, false, false);
                
                TimeManipulation.StopTimewarp(false);
                return;
            }

        }
    }
}
