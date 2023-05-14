using System.Collections.Generic;
using HarmonyLib;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.UI.ModGUI;
using UITools;
using UnityEngine;
using VanillaUpgrades.Utility;
using UIExtensions = VanillaUpgrades.Utility.UIExtensions;

namespace VanillaUpgrades
{

    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    internal class StopAdaptation
    {
        private static bool Prefix()
        {
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    internal class AdaptPartPicker
    {
        private static void Prefix()
        {
            BuildSettings.noAdaptOverride = true;
        }
        private static void Postfix()
        {
            BuildSettings.noAdaptOverride = false;
        }
    }


    [HarmonyPatch(typeof(AdaptModule), "UpdateAdaptation")]
    internal class FixCucumber
    {
        private static bool Prefix()
        {
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
        }
    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    public class KillMagnet
    {
        private static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (!BuildSettings.snapping)
                return true;
            __result = new List<Vector2>();
            return false;
        }
    }

    public static class BuildSettings
    {
        private const string PositionKey = "VU.BuildSettingsWindow";

        private const int Height = 170;

        private static GameObject windowHolder;

        private static Window window;

        public static bool snapping;
        public static bool noAdaptation;
        public static bool noAdaptOverride;
        private static readonly Vector2Int DefaultPos = new (300, Height);

        public static void Setup()
        {
            CreateGUI();
            Config.settings.showBuildGui.OnChange += OnToggle;
            Config.settings.showBuildGui.Value &= !Main.buildSettingsPresent;
            Config.settings.persistentVars.windowScale.OnChange += () => window.ScaleWindow();
            
            if (!Config.settings.moreCameraZoom) return;
            BuildManager.main.buildCamera.maxCameraDistance = 300;
            BuildManager.main.buildCamera.minCameraDistance = 0.1f;
        }

        private static void OnToggle()
        {
            windowHolder.SetActive(Config.settings.showBuildGui);
        }

        private static void CreateGUI()
        {
            windowHolder = UIExtensions.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "Build Settings");

            window = Builder.CreateWindow(windowHolder.transform, 0, 375, Height, DefaultPos.x, DefaultPos.y, true, false,
                0.95f, "Build Settings");
            window.RegisterPermanentSaving(PositionKey);
            window.RegisterOnDropListener(window.ClampWindow);

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