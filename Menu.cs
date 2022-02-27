using System;
using HarmonyLib;
using ModLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using SFS.Builds;

namespace ASoD_s_VanillaUpgrades
{
    internal class Menu : MonoBehaviour
    {
        // Token: 0x0400136C RID: 4972
        public static Rect windowRect = new Rect((float)Screen.width * 0.05f, (float)Screen.height * 0.92f, 200f, 200f);

        // Token: 0x0400136D RID: 4973
        public static bool snapping;

        // Token: 0x0400136E RID: 4974
        public static bool noAdaptation;

        public static void windowFunc(int windowID)
        {
            snapping = GUI.Toggle(new Rect(30f, 20f, 160f, 20f), snapping, " Disable Snapping");
            noAdaptation = GUI.Toggle(new Rect(30f, 40f, 160f, 20f), noAdaptation, " Disable Adapting");
        }

        public static void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, new GUI.WindowFunction(windowFunc), "Build Settings");
            Event current = Event.current;
            bool flag = current.keyCode == KeyCode.L;
            if (flag)
            {
                BuildManager.main.Launch();
            }
        }
    }
}
