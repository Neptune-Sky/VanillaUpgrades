using HarmonyLib;
using SFS.Achievements;
using SFS.UI;
using SFS.World;
using SFS.WorldBase;
using System;
using UnityEngine;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(TimewarpIndex), "ApplyState")]
    public class ResetVars
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            TimewarpToClass.timewarpTo = false;
        }
    }

    [HarmonyPatch(typeof(AchievementsModule), "Log_Planet")]
    public class StopTimewarpOnEncounter
    {
        [HarmonyPostfix]
        public static void Postfix(Planet planet, Planet planet_Old)
        {
            if (planet.parentBody == planet_Old && (bool)Config.settings["stopTimewarpOnEncounter"])
            {
                TimewarpToClass.ChangeTimewarp(1, true);
                return;
            }
            if (TimewarpToClass.timewarpTo == true)
            {
                AdvancedInfo.StopTimewarp(false);
            }

        }
    }
    public class TimewarpToClass : MonoBehaviour
    {
        public static bool timewarpTo;

        public static int decider;

        public static bool heightIncreasing;

        public static double height;

        public static double threshold;

        public static double target;

        public static double lastHeight;

        public static int index;

        private static double GetTimewarpSpeed_Rails(int timewarpIndex_Rails)
        {
            return (new int[]
            {
                1,
                5,
                25
            })[timewarpIndex_Rails % 3] * Math.Pow(100.0, (double)((int)((float)timewarpIndex_Rails / 3f)));
        }

        public static void ChangeTimewarp(int amount, bool set)
        {
            if (PlayerController.main.player.Value == null) return;

            if (set)
            {
                index = amount;
            }
            else
            {
                index = WorldTime.main.timewarpIndex.timewarpIndex + amount;
            }

            bool real;
            if (index <= 0)
            {
                index = 0;
                real = true;
            }
            else
            {
                real = false;
            }
            if (index > WorldTime.main.timewarpIndex.GetMaxTimewarpIndex()) index = WorldTime.main.timewarpIndex.GetMaxTimewarpIndex();

            WorldTime.main.timewarpIndex.timewarpIndex = index;
            WorldTime.main.SetState(GetTimewarpSpeed_Rails(index), real, false);
        }

        public static void TimewarpTo()
        {
            if (PlayerController.main.player.Value == null) return;

            ChangeTimewarp(1, true);

            int maxIndex = WorldTime.main.timewarpIndex.GetMaxTimewarpIndex() - 1;

            double usedIndex = (AdvancedInfo.instance.apoapsis.Round(1).ToString().Length + AdvancedInfo.instance.periapsis.Round(1).ToString().Length) / 2.5;
            index = (int)usedIndex;
            if (usedIndex > maxIndex)
            {
                usedIndex = maxIndex;
            }
            if (usedIndex < 4) usedIndex = 4;

            if (WorldTime.CanTimewarp(false, false))
            {
                var name = heightIncreasing ? "apoapsis" : "periapsis";
                if (AdvancedInfo.displayify(AdvancedInfo.instance.apoapsis) == "Escape" && heightIncreasing)
                {
                    name = "escape";
                }
                timewarpTo = true;
                ChangeTimewarp((int)usedIndex, true);
                MsgDrawer.main.Log("Timewarping to " + name + "...");

                threshold = (target / Math.Pow(2, target.Round(1).ToString().Length)) * AdvancedInfo.instance.displayEcc;
                TimewarpToClass.lastHeight = Math.Abs(height - target);
            }

        }

        public void Update()
        {
            if (PlayerController.main.player.Value == null || AdvancedInfo.instance == null || AdvancedInfo.currentRocket == null) return;

            bool forceIncreasing;
            bool forceDecreasing;
            if (Math.Abs(height - AdvancedInfo.instance.periapsis) < threshold) { forceIncreasing = true; forceDecreasing = false; }
            else if (Math.Abs(height - AdvancedInfo.instance.apoapsis) < threshold) { forceIncreasing = false; forceDecreasing = true; } else { forceIncreasing = false; forceDecreasing = false; }

            if (!forceIncreasing && !forceDecreasing)
            {


                if (height.Round(0.01) < AdvancedInfo.currentRocket.physics.location.position.Value.magnitude.Round(0.01) - AdvancedInfo.currentRocket.physics.location.planet.Value.Radius)
                {
                    heightIncreasing = true;
                }
                if (height.Round(0.01) > AdvancedInfo.currentRocket.physics.location.position.Value.magnitude.Round(0.01) - AdvancedInfo.currentRocket.physics.location.planet.Value.Radius)
                {
                    heightIncreasing = false;
                }

            }
            else
            {
                if (forceIncreasing)
                {
                    heightIncreasing = true;
                }
                else if (forceDecreasing)
                {
                    heightIncreasing = false;
                }
            }

            height = AdvancedInfo.currentRocket.physics.location.position.Value.magnitude - AdvancedInfo.currentRocket.physics.location.planet.Value.Radius;
            target = heightIncreasing ? AdvancedInfo.instance.apoapsis : AdvancedInfo.instance.periapsis;

            int minIndex;
            double velocity = AdvancedInfo.currentRocket.physics.location.velocity.Value.magnitude;
            if (velocity < 300)
            {
                if (velocity < 50 && AdvancedInfo.instance.displayEcc < 0.5)
                {
                    minIndex = 7;
                }
                else
                {
                    minIndex = 6;
                }

            }
            else
            {
                if (target > 10000000)
                {
                    minIndex = 9;
                }
                else minIndex = 4;
            }

            if (timewarpTo == true)
            {

                if (Math.Abs(height - target) < lastHeight * (0.03 * index) && index >= minIndex)
                {
                    ChangeTimewarp(-1, false);
                    lastHeight = Math.Abs(height - target);
                }
                if (Math.Abs(height - target) < threshold)
                {
                    AdvancedInfo.StopTimewarp(true);
                }
            }


        }

        public Rect debugWindow = new Rect(20f, 500f, 200f, 200f);
        public bool showDebug = false;

        public void debugWindowFunc(int windowID)
        {
            GUI.Label(new Rect(20f, 20f, 1000f, 20f), "height: " + height.ToString());
            GUI.Label(new Rect(20f, 40f, 1000f, 20f), "target: " + target.ToString());
            GUI.Label(new Rect(20f, 60f, 1000f, 20f), "threshold: " + threshold.ToString());
            GUI.Label(new Rect(20f, 80f, 1000f, 20f), "heightIncreasing: " + heightIncreasing.ToString());
            GUI.Label(new Rect(20f, 100f, 1000f, 20f), "lastHeight: " + lastHeight.ToString());
            GUI.Label(new Rect(20f, 120f, 1000f, 20f), "index: " + index.ToString());
            GUI.Label(new Rect(20f, 140f, 1000f, 20f), "e: " + FaceDirection.e.ToString());
        }

        public void OnGUI()
        {
            if (showDebug)
            {
                debugWindow = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), debugWindow, debugWindowFunc, "VanUp Debug");
            }
        }
    }
}
