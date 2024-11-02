using System;
using HarmonyLib;
using JetBrains.Annotations;
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
    
    [HarmonyPatch(typeof(GameManager))]
    public class UpdateInGame
    {
        public static Action execute = () => { };
        [HarmonyPatch("Update")]
        public static void Postfix()
        {
            execute.Invoke();
        }
    }

    [HarmonyPatch(typeof(ThrottleDrawer))]
    public class ManageHoverMode
    {
        [HarmonyPatch("ToggleThrottle")]
        [HarmonyPostfix]
        public static void ToggleThrottle_Postfix(ref Throttle_Local ___throttle)
        {
            if(!___throttle.Value.throttleOn)
            {
                // Do this only if throttle has been turned off (keep hover mode on if this was set before throttle is turned on)
                HoverHandler.EnableHoverMode(false, false);
            }
        }

        [HarmonyPatch("AdjustThrottleRaw")]
        [HarmonyPostfix]
        public static void AdjustThrottleRaw_Postfix()
        {
            HoverHandler.EnableHoverMode(false);
        }

        [HarmonyPatch("SetThrottleRaw")]
        [HarmonyPostfix]
        public static void SetThrottleRaw_Postfix()
        {
            HoverHandler.EnableHoverMode(false);
        }
    }

    [HarmonyPatch(typeof(ThrottleDrawer), "UpdatePercentUI")]
    class ThrottleFillbarAccuracyFix
    {
        [UsedImplicitly]
        static bool Prefix(Throttle_Local ___throttle, FillSlider ___throttleSlider, TextAdapter ___throttlePercentText)
        {
            var value = ___throttle.Value.throttlePercent.Value;
            ___throttlePercentText.Text = value.ToPercentString();
            ___throttleSlider.SetFillAmount(0.16f + (value * 0.68f), false);
            return false;
        }
    }
    
    [HarmonyPatch(typeof(FlightInfoDrawer), "Update")]
    class LimitDecimalsOfTimewarpText
    {
        private static void Postfix(TextAdapter ___timewarpText)
        {
            ___timewarpText.Text = WorldTime.main.timewarpSpeed.Round(2) + "x";
        }
    }
    
    [HarmonyPatch(typeof(Rocket), "GetTorque")]
    public static class ToggleTorque
    {
        public static bool disableTorque;

        public static void Toggle()
        {
            if (!PlayerController.main.HasControl(MsgDrawer.main)) return;
            disableTorque = !disableTorque;
            MsgDrawer.main.Log("Torque " + (disableTorque ? "Disabled" : "Enabled"));
        }
        private static bool Prefix(ref float __result)
        {
            if (!disableTorque) return true;
            __result = 0f;
            return false;
        }
    }
}