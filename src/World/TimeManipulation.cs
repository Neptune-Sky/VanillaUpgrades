using HarmonyLib;
using JetBrains.Annotations;
using SFS.UI;
using SFS.World;

namespace VanillaUpgrades
{
    [HarmonyPatch]
    internal static class TimeDecelerationPatch
    {
        [HarmonyPatch(typeof(WorldTime), nameof(WorldTime.DecelerateTime))]
        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool TimeSlowdown()
        {
            if (WorldTime.main.timewarpIndex != 0) return true;
            TimeManipulation.SlowTime();
            return false;
        }

        [HarmonyPatch(typeof(WorldTime), nameof(WorldTime.AccelerateTime))]
        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool EndDeceleration()
        {
            if (TimeManipulation.timeDecelIndex <= 0) return true;

            WorldTime.main.SetState(1, true, false);
            MsgDrawer.main.Log("Time restored to normal");
            TimeManipulation.timeDecelIndex = 0;
            return false;
        }
    }

    public static class TimeManipulation
    {
        public static int timeDecelIndex;
        private static readonly double[] decelSpeeds = { 0.75, 0.5, 0.25, 0.1, 0 };

        public static void SlowTime()
        {
            if (!Config.settings.allowTimeSlowdown) return;
            if (timeDecelIndex >= decelSpeeds.Length) return;
            timeDecelIndex++;
            var defaultMessage = true;
            var speed = decelSpeeds[timeDecelIndex - 1];
            if (speed == 0)
            {
                defaultMessage = false;
                MsgDrawer.main.Log("Time frozen");
            }

            WorldTime.main.SetState(speed, true, defaultMessage);
        }

        public static void ToggleChange()
        {
            if (!Config.settings.allowTimeSlowdown && timeDecelIndex != 0)
            {
                WorldTime.main.SetState(1, true, false);
                timeDecelIndex = 0;
            }

            if (timeDecelIndex != 0 && WorldTime.main.timewarpIndex != 0) timeDecelIndex = 0;
        }

        public static void StopTimewarp(bool showmsg)
        {
            if (WorldTime.main.timewarpIndex == 0 && timeDecelIndex == 0) return;
            WorldTime.main.timewarpIndex = 0;
            WorldTime.main.SetState(1, true, false);
            timeDecelIndex = 0;
            if (showmsg) MsgDrawer.main.Log("Time acceleration stopped");
        }
    }
}