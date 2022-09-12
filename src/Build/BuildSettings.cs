using System.Collections.Generic;
using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.UI;
using SFS.UI.ModGUI;
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

    public static class BuildSettings
    {
        private const string posKey = "BuildSettingsWindow";
        public static GameObject windowHolder;

        static readonly int MainWindowID = Builder.GetRandomID();
        static Window window;

        static ToggleWithLabel snapToggle;
        static ToggleWithLabel adaptToggle;
        // static ToggleWithLabel calcToggle;

        public static bool snapping;
        public static bool noAdaptation;
        public static bool noAdaptOverride;

        static int height = 170;
        static string title = "Build Settings";
        static Vector2Int defaultPos = new Vector2Int(300, height);

        public static void Setup()
        {
            ShowGUI();
            ScaleWindow(window);
            OnToggle();
            Config.settingsData.showBuildGui.OnChange += OnToggle;
            Config.settingsData.showBuildGui.Value &= !Main.buildSettingsPresent;

            Config.settingsData.persistentVars.windowScale.OnChange += () => ScaleWindow(window);

        }

        static void OnToggle()
        {
            windowHolder.SetActive(Config.settingsData.showBuildGui);
        }

        static void ScaleWindow(Window input)
        {
            input.rectTransform.localScale = new Vector2(Config.settingsData.persistentVars.windowScale.Value, Config.settingsData.persistentVars.windowScale.Value);
            WindowManager.ClampWindow(input);
            WindowManager.Save(posKey, input);
        }

        static void ShowGUI()
        {
            Vector2Int pos = Config.settingsData.windowsSavedPosition.GetValueOrDefault(posKey, defaultPos);
            windowHolder = CustomUI.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "Build Settings");

            window = Builder.CreateWindow(windowHolder.transform, MainWindowID, 375, height, pos.x, pos.y, true, true, 0.95f, title);
            window.gameObject.GetComponent<DraggableWindowModule>().OnDropAction += () =>
            {
                WindowManager.ClampWindow(window);
                WindowManager.Save(posKey, window);
            };

            window.CreateLayoutGroup(Type.Vertical, padding: new RectOffset(0, 0, 7, 0));

            if (!Main.buildSettingsPresent)
            {
                snapToggle = Builder.CreateToggleWithLabel(window, 320, 35, () => !snapping, () => snapping = !snapping, 0, 0, "Snap to Parts");
                adaptToggle = Builder.CreateToggleWithLabel(window, 320, 35, () => !noAdaptation, () => noAdaptation = !noAdaptation, 0, 0, "Part Adaptation");
            }
            // calcToggle = Builder.CreateToggleWithLabel(window, 320, 35, () => DVCalc.showCalc, DVCalc.toggleCalc, 0, 0, "ΔV Calculator");

            ScaleWindow(window);
        }

        public static void Launch()
        {
            BuildState.main.UpdatePersistent();
            Base.sceneLoader.LoadWorldScene(true);
        }
    }
}