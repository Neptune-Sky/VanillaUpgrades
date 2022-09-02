using System.Collections.Generic;
using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.Parts.Modules;
using UnityEngine;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    class StopAdaptation
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            if (BuildSettings.noAdaptation && !BuildSettings.noAdaptOverride) return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    class AdaptPartPicker
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            BuildSettings.noAdaptOverride = true;
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            BuildSettings.noAdaptOverride = false;
        }
    }

<<<<<<< Updated upstream
=======
    [HarmonyPatch(typeof(AdaptModule), "UpdateAdaptation")] 
    class FixCucumber
    {
        static bool Prefix()
        {
            if (BuildSettings.noAdaptation && !BuildSettings.noAdaptOverride)
            {
                return false;
            }
            return true;
        }
    }

>>>>>>> Stashed changes
    /*
    [HarmonyPatch(typeof(HoldGrid), "GetSnapPosition_Old")]
    public class NotifyMagnet
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            BuildSettings.partMagnetizing = true;
        }

        [HarmonyPostfix]
        static void Postfix()
        {
            BuildSettings.partMagnetizing = false;
        }

    }
    */
    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    public class KillMagnet
    {
        [HarmonyPrefix]
        static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (BuildSettings.snapping)
            {
                __result = new List<Vector2>();
                return false;
            }

            return true;
        }
    }

    public class BuildSettings : MonoBehaviour
    {
        // Token: 0x0400136C RID: 4972
        public static Rect windowRect = new Rect((float)WindowManager.settings["buildSettings"]["x"],
            (float)WindowManager.settings["buildSettings"]["y"], 180f * WindowManager.scale.x,
            100f * WindowManager.scale.y);

        // Token: 0x0400136D RID: 4973
        public static bool snapping;

        public static bool partMagnetizing;

        // Token: 0x0400136E RID: 4974
        public static bool noAdaptation;

        public static bool noAdaptOverride;

        public void Update()
        {
            windowRect.width = 180f * WindowManager.scale.x;
            windowRect.height = 100f * WindowManager.scale.y;
        }

        public void OnGUI()
        {
            if (Main.menuOpen || !(bool)Config.settings["showBuildGUI"] ||
                VideoSettingsPC.main.uiOpacitySlider.value == 0) return;
            Rect oldRect = windowRect;
            GUI.color = Config.windowColor;
            windowRect = GUI.Window(WindowManager.GetValidID(), windowRect, windowFunc, "Build Settings");
            windowRect = WindowManager.ConfineRect(windowRect);
            if (oldRect != windowRect) WindowManager.settings["buildSettings"]["x"] = windowRect.x;
            WindowManager.settings["buildSettings"]["y"] = windowRect.y;
        }

        public static void Launch()
        {
            BuildState.main.UpdatePersistent();
            Base.sceneLoader.LoadWorldScene(true);
        }

        public string Toggle(bool enabled)
        {
            if (enabled) return " Disabled";
            return " Enabled";
        }

        public string Hide(bool shown)
        {
            if (shown) return "Hide ";
            return "Show ";
        }

        public void windowFunc(int windowID)
        {
            GUIStyle leftAlign = new GUIStyle();
            leftAlign.alignment = TextAnchor.LowerLeft;
            leftAlign.normal.textColor = Color.white;
            leftAlign.fontSize = (int)(14 * WindowManager.scale.y);

            GUIStyle midAlign = new GUIStyle(GUI.skin.button);
            midAlign.alignment = TextAnchor.MiddleCenter;
            midAlign.normal.textColor = Color.white;
            midAlign.fontSize = (int)(12 * WindowManager.scale.y);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Snapping" + Toggle(snapping), midAlign)) snapping = !snapping;
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Adapting" + Toggle(noAdaptation), midAlign)) noAdaptation = !noAdaptation;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Hide(DVCalc.showCalc) + "ΔV Calculator", midAlign)) DVCalc.showCalc = !DVCalc.showCalc;
            GUILayout.EndHorizontal();


            GUI.DragWindow();
        }
    }
}