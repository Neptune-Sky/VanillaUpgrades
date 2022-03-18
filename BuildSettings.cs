using SFS;
using SFS.Builds;
using SFS.Parts.Modules;
using UnityEngine;
using HarmonyLib;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    class StopAdaptation
    {

        [HarmonyPrefix]
        static bool Prefix()
        {
            if (BuildSettings.noAdaptation && !BuildSettings.noAdaptOverride)
            {
                return false;
            }
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

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetSnapOffsets))]
    public class KillMagnet
    {
        [HarmonyPrefix]
        static void Prefix(MagnetModule A, MagnetModule B, ref float snapDistance)
        {
            if (BuildSettings.snapping)
            {
                snapDistance = 0.0f;
            }
        }
    }
    public class BuildSettings : MonoBehaviour
    {
        // Token: 0x0400136C RID: 4972
        public static Rect windowRect = new Rect((float)WindowManager.settings["buildSettings"]["x"], (float)WindowManager.settings["buildSettings"]["y"], 180f, 70f);

        // Token: 0x0400136D RID: 4973
        public static bool snapping;

        public static bool partMagnetizing;

        // Token: 0x0400136E RID: 4974
        public static bool noAdaptation;

        public static bool noAdaptOverride;

        public static void Launch()
        {
            BuildState.main.SaveLaunchData(updatePersistent: true);
            Base.sceneLoader.LoadWorldScene(launch: true);
        }

        public void windowFunc(int windowID)
        {
            snapping = GUI.Toggle(new Rect(10f, 20f, 120f, 20f), snapping, " Disable Snapping");
            noAdaptation = GUI.Toggle(new Rect(10f, 40f, 120f, 20f), noAdaptation, " Disable Adapting");


            GUI.DragWindow();
        }
        public void Update()
        {
           if ((bool)Config.settings["moreCameraZoom"])
            {
                BuildManager.main.buildCamera.maxCameraDistance = 300;
                BuildManager.main.buildCamera.minCameraDistance = 0.1f;
            } else
            {
                BuildManager.main.buildCamera.maxCameraDistance = 60;
                BuildManager.main.buildCamera.minCameraDistance = 10f;
            }
        }
        public void OnGUI()
        {
            if (Main.menuOpen || !(bool)Config.settings["showBuildGUI"] || VideoSettingsPC.main.uiOpacitySlider.value == 0) return;
            Rect oldRect = windowRect;
            GUI.color = new Color((float)Config.settings["persistentVars"]["windowColor"]["r"], (float)Config.settings["persistentVars"]["windowColor"]["g"], (float)Config.settings["persistentVars"]["windowColor"]["b"], VideoSettingsPC.main.uiOpacitySlider.value);
            windowRect = GUI.Window(WindowManager.GetValidID(), windowRect, new GUI.WindowFunction(windowFunc), "Build Settings");
            windowRect = WindowManager.ConfineRect(windowRect);
            if (oldRect != windowRect) WindowManager.settings["buildSettings"]["x"] = windowRect.x; WindowManager.settings["buildSettings"]["y"] = windowRect.y;

        }
    }
}
