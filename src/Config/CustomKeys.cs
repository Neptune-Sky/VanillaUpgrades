using ModLoader;
using ModLoader.Helpers;
using SFS;
using SFS.Builds;
using UnityEngine;
using static SFS.Input.KeybindingsPC;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable IdentifierTypo

namespace VanillaUpgrades
{
    public class VuKeybindings : ModKeybindings
    {
        private static VuKeybindings _main;

        public static void LoadKeybindings()
        {
            _main = SetupKeybindings<VuKeybindings>(Main.main);

            SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
            SceneHelper.OnWorldSceneLoaded += OnWorldLoad;

            AddStaticKeybindings();
        }

        private static void AddStaticKeybindings()
        {
            AddOnKeyDown(_main.HideUI, CustomKeyExecution.HideUI);
            AddOnKeyDown(_main.ToggleWindowed, CustomKeyExecution.ToggleWindowed);
        }

        private static void OnBuildLoad()
        {
            AddOnKeyDown_Build(_main.Launch, KeyMethods.Launch);
            AddOnKeyDown_Build(_main.OpenCategories, KeyMethods.pickCategoriesMenu.expandMenu.ToggleExpanded);
            AddOnKeyDown_Build(_main.ToggleSymmetry, BuildManager.main.ToggleSymmetryMode);
            AddOnKeyDown_Build(_main.ToggleInterior, InteriorManager.main.ToggleInteriorView);
        }

        private static void OnWorldLoad()
        {
            AddOnKeyDown_World(_main.StopTimewarp, () => TimeManipulation.StopTimewarp(true));
            AddOnKeyDown_World(_main.Throttle01, WorldManager.Throttle01);
            AddOnKeyDown_World(_main.ToggleTorque, VanillaUpgrades.ToggleTorque.Toggle);
        }

        public override void CreateUI()
        {
            CreateUI_Text("VanillaUpgrades Keybindings");
            CreateUI_Keybinding(HideUI, KeyCode.F2, "Hide UI");
            CreateUI_Keybinding(ToggleWindowed, KeyCode.F11, "Toggle Windowed Mode");
            CreateUI_Space();
            CreateUI_Text("Build");
            CreateUI_Keybinding(ToggleSymmetry, KeyCode.Z, "Toggle symmetry mode");
            CreateUI_Keybinding(ToggleInterior, KeyCode.X, "Toggle interior view");
            CreateUI_Keybinding(OpenCategories, KeyCode.Tab, "Open Pick Categories");
            CreateUI_Keybinding(Launch, KeyCode.L, "Launch");
            CreateUI_Space();
            CreateUI_Text("World");
            CreateUI_Keybinding(StopTimewarp, KeyCode.Slash, "Stop Timewarp");
            CreateUI_Keybinding(Throttle01, KeyCode.C, "Throttle To 0.1%");
            CreateUI_Keybinding(ToggleTorque, KeyCode.T, "Toggle torque");
            CreateUI_Space();
        }

        #region Keys

        public Key HideUI = KeyCode.F2;
        
        public Key ToggleWindowed = KeyCode.F11;

        public Key ToggleSymmetry = KeyCode.Z;

        public Key ToggleInterior = KeyCode.X;

        public Key Launch = KeyCode.L;

        public Key OpenCategories = KeyCode.Tab;

        public Key StopTimewarp = KeyCode.Slash;

        public Key ToggleTorque = KeyCode.T;

        public Key Throttle01 = KeyCode.C;

        #endregion
    }
    
    public static class CustomKeyExecution
    {

        public static void HideUI()
        {
            var pickGrid = GameObject.Find("Canvas - PickGrid");
            var ui = GameObject.Find("--- UI ---");

            if (pickGrid != null)
            {
                pickGrid.GetComponent<Canvas>().enabled = !pickGrid.GetComponent<Canvas>().enabled;
            }

            if (ui != null)
            {
                ui.GetComponent<Canvas>().enabled = !ui.GetComponent<Canvas>().enabled;
            }
        }

        public static void ToggleWindowed()
        {
            VideoSettingsPC.main.windowModeDropdown.value = VideoSettingsPC.main.settings.windowMode is 0 or 2 ? 2 : 1;
        }
    }
    
}

