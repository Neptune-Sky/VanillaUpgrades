using HarmonyLib;
using SFS.Achievements;
using SFS.World;
using SFS.World.Maps;
using SFS.WorldBase;

namespace VanillaUpgrades
{
    [HarmonyPatch]
    public class StopTimewarpOnEncounter
    {
        private static TimewarpTo TimewarpTo;
        
        [HarmonyPatch(typeof(TimewarpTo), "Start")]
        [HarmonyPrefix]
        private static void GetTimewarpTo(TimewarpTo __instance)
        {
            TimewarpTo = __instance;
        }
        
        [HarmonyPatch(typeof(AchievementsModule), "Log_Planet")]
        [HarmonyPostfix]
        private static void StopTimewarp(Planet planet, Planet planet_Old)
        {
            if (TimewarpTo != null && TimewarpTo.warp != null) return;
            if (planet.parentBody != planet_Old || !Config.settings.stopTimewarpOnEncounter) return;
            WorldTime.main.SetState(2, false, false);

            TimeManipulation.StopTimewarp(false);
        }
    }
}

