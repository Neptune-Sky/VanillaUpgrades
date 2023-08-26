using HarmonyLib;
using SFS.Logs;
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
        
        [HarmonyPatch(typeof(LogsModule), "Log_Planet")]
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
    internal class PhysicsTimewarpIfTurning
    {

        private static void Postfix(ref bool __result)
        {
            if (WorldManager.currentRocket.arrowkeys.turnAxis == 0) return; 
            WorldTime.ShowCannotTimewarpMsg(Field.Text("Cannot timewarp faster than %speed%x while turning"), MsgDrawer.main);
            __result = false;
        }
    }
    
    [HarmonyPatch(typeof(EffectManager))]
    public class StopExplosions
    {
        [HarmonyPatch("CreateExplosion")]
        [HarmonyPatch("CreatePartOverheatEffect")]
        [HarmonyPrefix]
        private static bool Prefix()
        {
            return Config.settings.explosions;
        }
    }
    
    [HarmonyPatch(typeof(WorldTime))]
    public class RaiseMaxPhysicsTimewarp
    {
        [HarmonyPatch("GetTimewarpSpeed_Physics")]
        [HarmonyPrefix]
        public static bool AddMoreIndexes(ref double __result, int timewarpIndex_Physics)
        {
            if (!Config.settings.higherPhysicsWarp) return true;
            __result = new[] { 1, 2, 3, 5, 10, 25 }[timewarpIndex_Physics];
            return false;

        }

        [HarmonyPatch("MaxPhysicsIndex", MethodType.Getter)]
        [HarmonyPrefix]
        public static bool AllowUsingIndexes(ref int __result)
        {
            if (!Config.settings.higherPhysicsWarp) return true;
            __result = 5;
            return false;

        }
    }
}

