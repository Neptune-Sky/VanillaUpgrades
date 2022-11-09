using System.Collections.Generic;
using HarmonyLib;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.UI.ModGUI;
using UITools;
using UnityEngine;
using Type = SFS.UI.ModGUI.Type;

namespace VanillaUpgrades
{

    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    class StopAdaptation
    {
        [HarmonyPrefix]
        static bool Prefix()
        {
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
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
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
        }
    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    public class KillMagnet
    {
        [HarmonyPrefix]
        static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (!BuildSettings.snapping)
                return true;
            __result = new List<Vector2>();
            return false;
        }
    }

    public static class BuildSettings
    {
        const string PositionKey = "VU.BuildSettingsWindow";

        const int Height = 170;

        static GameObject windowHolder;

        static Window window;

        public static bool snapping;
        public static bool noAdaptation;
        public static bool noAdaptOverride;
        static readonly Vector2Int DefaultPos = new (300, Height);

        public static void Setup()
        {
            CreateGUI();
            Config.settings.showBuildGui.OnChange += OnToggle;
            Config.settings.showBuildGui.Value &= !Main.buildSettingsPresent;
            Config.settings.persistentVars.windowScale.OnChange += () => window.ScaleWindow();
            if (Config.settings.moreCameraZoom)
            {
                BuildManager.main.buildCamera.maxCameraDistance = 300;
                BuildManager.main.buildCamera.minCameraDistance = 0.1f;
            }
        }

        static void OnToggle()
        {
            windowHolder.SetActive(Config.settings.showBuildGui);
        }

        static void CreateGUI()
        {
            windowHolder = UIExtensions.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "Build Settings");

            window = Builder.CreateWindow(windowHolder.transform, 0, 375, Height, DefaultPos.x, DefaultPos.y, true, false,
                0.95f, "Build Settings");
            window.RegisterPermanentSaving(PositionKey);
            window.RegisterOnDropListener(() => window.ClampWindow());

            window.CreateLayoutGroup(Type.Vertical, padding: new RectOffset(0, 0, 7, 0));

            if (!Main.buildSettingsPresent)
            {
                Builder.CreateToggleWithLabel(window, 320, 35, () => !snapping, () => snapping ^= true, 0, 0,
                    "Snap to Parts");
                Builder.CreateToggleWithLabel(window, 320, 35, () => !noAdaptation, () => noAdaptation ^= true, 0, 0,
                    "Part Adaptation");
            }

            window.ScaleWindow();
        }
    }
}