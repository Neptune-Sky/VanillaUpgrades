﻿using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS;
using SFS.Builds;
using UnityEngine;
using UnityEngine.UI;
using static SFS.Input.KeybindingsPC;
// ReSharper disable MemberCanBePrivate.Global

namespace VanillaUpgrades
{
    public class VU_Keybindings : ModKeybindings
    {
        private static VU_Keybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<VU_Keybindings>(Main.main);

            SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
            SceneHelper.OnWorldSceneLoaded += OnWorldLoad;

            AddStaticKeybindings();
        }

        private static void AddStaticKeybindings()
        {
            AddOnKeyDown(main.Hide_UI, CustomKeyExecution.HideUI);
            AddOnKeyDown(main.Toggle_Windowed, CustomKeyExecution.ToggleWindowed);
        }

        private static void OnBuildLoad()
        {
            AddOnKeyDown_Build(main.Launch, KeyMethods.Launch);
            AddOnKeyDown_Build(main.OpenCategories, KeyMethods.pickCategoriesMenu.expandMenu.ToggleExpanded);
            AddOnKeyDown_Build(main.Toggle_Symmetry, BuildManager.main.ToggleSymmetryMode);
            AddOnKeyDown_Build(main.Toggle_Interior, InteriorManager.main.ToggleInteriorView);
        }

        private static void OnWorldLoad()
        {
            AddOnKeyDown_World(main.Stop_Timewarp, () => TimeManipulation.StopTimewarp(true));
            AddOnKeyDown_World(main.Throttle01, WorldManager.Throttle01);
            AddOnKeyDown_World(main.ToggleTorque, VanillaUpgrades.ToggleTorque.Toggle);
        }

        public override void CreateUI()
        {
            CreateUI_Text("VanillaUpgrades Keybindings");
            CreateUI_Keybinding(Hide_UI, KeyCode.F2, "Hide UI");
            CreateUI_Keybinding(Toggle_Windowed, KeyCode.F11, "Toggle Windowed Mode");
            CreateUI_Space();
            CreateUI_Text("Build Mode");
            CreateUI_Keybinding(Toggle_Symmetry, KeyCode.Z, "Toggle symmetry mode");
            CreateUI_Keybinding(Toggle_Interior, KeyCode.X, "Toggle interior view");
            CreateUI_Keybinding(OpenCategories, KeyCode.Tab, "Open Pick Categories");
            CreateUI_Keybinding(Launch, KeyCode.L, "Launch");
            CreateUI_Space();
            CreateUI_Text("World View");
            CreateUI_Keybinding(Stop_Timewarp, KeyCode.Slash, "Stop Timewarp");
            CreateUI_Keybinding(Throttle01, KeyCode.C, "Throttle To 0.1%");
            CreateUI_Keybinding(ToggleTorque, KeyCode.T, "Toggle torque");
            CreateUI_Space();
        }

        #region Keys

        public Key Hide_UI = KeyCode.F2;
        
        public Key Toggle_Windowed = KeyCode.F11;

        public Key Toggle_Symmetry = KeyCode.Z;

        public Key Toggle_Interior = KeyCode.X;

        public Key Launch = KeyCode.L;

        public Key OpenCategories = KeyCode.Tab;

        public Key CalcToggle = KeyCode.C;

        public Key Stop_Timewarp = KeyCode.Slash;

        public Key Timewarp_To = KeyCode.Semicolon;

        public Key ToggleTorque = KeyCode.T;

        public Key Throttle01 = KeyCode.C;

        #endregion
    }
    
    public static class CustomKeyExecution
    {

        public static void HideUI()
        {
            GameObject pickGrid = GameObject.Find("Canvas - PickGrid");
            GameObject ui = GameObject.Find("--- UI ---");

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

