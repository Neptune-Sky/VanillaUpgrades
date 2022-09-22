using SFS.Builds;
using SFS;
using ModLoader;
using ModLoader.Helpers;
using UnityEngine;
using static SFS.Input.KeybindingsPC;
// ReSharper disable InconsistentNaming

namespace VanillaUpgrades
{
    public class VU_Keybindings : ModKeybindings
    {
        #region Keys
        Key Hide_UI = KeyCode.F2;
        
        Key Toggle_Symmetry = KeyCode.J;
        
        Key Toggle_Interior = KeyCode.K;
        
        Key Launch = KeyCode.L;
        
        Key CalcToggle = KeyCode.C;
        
        Key Stop_Timewarp = KeyCode.Slash;
        
        Key Timewarp_To = KeyCode.Semicolon;
        
        Key ToggleTorque = KeyCode.T;
        
        Key Throttle01 = KeyCode.C;
        
        Key Prograde = KeyCode.Alpha1;
        
        Key Retrograde = KeyCode.Alpha2;
        
        Key Rad_In = KeyCode.Alpha3;
        
        Key Rad_Out = KeyCode.Alpha4;
        #endregion

        static VU_Keybindings main;
        
        public static void LoadKeybindings()
        {
            main = SetupKeybindings<VU_Keybindings>(Main.main);
            
            SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
            SceneHelper.OnWorldSceneLoaded += OnWorldLoad;
            
            AddStaticKeybindings();
        }

        static void AddStaticKeybindings()
        {
            AddOnKeyDown(main.Hide_UI, OpacityChanger.HideUI);
        }
        static void OnBuildLoad()
        {
            AddOnKeyDown_Build(main.Launch, BuildSettings.Launch);
            AddOnKeyDown_Build(main.Toggle_Symmetry, BuildManager.main.ToggleSymmetryMode);
            AddOnKeyDown_Build(main.Toggle_Interior, InteriorManager.main.ToggleInteriorView);
        }
        static void OnWorldLoad()
        {
            AddOnKeyDown_World(main.Stop_Timewarp, () => TimeManipulation.StopTimewarp(true));
            AddOnKeyDown_World(main.Throttle01, WorldManager.Throttle01);
            AddOnKeyDown_World(main.ToggleTorque, VanillaUpgrades.ToggleTorque.Toggle);
        }

        public override void CreateUI()
        {
            CreateUI_Text("VanillaUpgrades Keybindings");
            CreateUI_Keybinding(Hide_UI, KeyCode.F2, "Hide UI");
            CreateUI_Space();
            CreateUI_Text("Build Mode");
            CreateUI_Keybinding(Toggle_Symmetry, KeyCode.J, "Toggle symmetry mode");
            CreateUI_Keybinding(Toggle_Interior, KeyCode.K, "Toggle interior view");
            CreateUI_Keybinding(Launch, KeyCode.L,  "Launch");
            CreateUI_Space();
            CreateUI_Text("World View");
            CreateUI_Keybinding(Stop_Timewarp, KeyCode.Slash, "Stop Timewarp");
            CreateUI_Keybinding(Throttle01, KeyCode.C, "Throttle To 0.1%");
            CreateUI_Keybinding(ToggleTorque, KeyCode.T, "Toggle torque");
            CreateUI_Space();
        }
    }
}
