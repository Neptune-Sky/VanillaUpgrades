using SFS.Builds;
using SFS.UI;
using UnityEngine;
using Button = SFS.UI.ModGUI.Button;
using UIExtensions = VanillaUpgrades.Utility.UIExtensions;

namespace VanillaUpgrades
{
    public static class BuildSettings
    {
        private static GameObject[] buttons;

        public static bool noSnapping;
        public static bool noAdaptation;
        public static bool noAdaptOverride;

        public static void Setup()
        {
            noSnapping = false;
            noAdaptation = false;
            CreateGUI();
            Config.settings.showBuildGui.OnChange += OnToggle;
            Config.settings.showBuildGui.Value &= !Main.buildSettingsPresent;
            
            if (!Config.settings.moreCameraZoom) return;
            BuildManager.main.buildCamera.maxCameraDistance = 300;
            BuildManager.main.buildCamera.minCameraDistance = 0.1f;
        }

        private static void OnToggle()
        {
            buttons.ForEach(e => e.SetActive(Config.settings.showBuildGui.Value));
        }
        
        private static void CreateGUI()
        {
            Transform topRight = GameObject.Find("Top Right").transform;
            
            Button adaptModButton = UIExtensions.ButtonForVanillaUI(out GameObject adaptingButton, topRight, 130, 50, 25, null, "Adapting");
            adaptingButton.transform.SetAsFirstSibling();
            adaptModButton.OnClick = () =>
            {
                noAdaptation ^= true;
                MsgDrawer.main.Log("Part Adaptation " + (noAdaptation ? "Disabled" : "Enabled"));
                adaptModButton.SetSelected(!noAdaptation);
            };
            adaptModButton.SetSelected();
            
            Button snapModButton = UIExtensions.ButtonForVanillaUI(out GameObject snappingButton, topRight, 130, 50, 25, null, "Snapping");
            snappingButton.transform.SetAsFirstSibling();
            snapModButton.OnClick = () =>
            {
                noSnapping ^= true;
                MsgDrawer.main.Log("Part Snapping " + (noSnapping ? "Disabled" : "Enabled"));
                snapModButton.SetSelected(!noSnapping);
            };
            snapModButton.SetSelected();
            
            buttons = new [] {adaptingButton, snappingButton};
        }
    }
}