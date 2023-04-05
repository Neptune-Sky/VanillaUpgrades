using HarmonyLib;
using SFS.Achievements;
using SFS.Translations;
using SFS.UI;
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

    [HarmonyPatch(typeof(Rocket), "CanTimewarp")]
    class PhysicsTimewarpIfTurning
    {

        private static void Postfix(ref bool __result)
        {
            if (WorldManager.currentRocket.arrowkeys.turnAxis == 0) return; 
            WorldTime.ShowCannotTimewarpMsg(Field.Text("Cannot timewarp faster than %speed%x while turning"), MsgDrawer.main);
            __result = false;
        }
    }
}

