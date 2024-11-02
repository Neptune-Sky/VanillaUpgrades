using ModLoader;
using ModLoader.Helpers;
using SFS;
using SFS.Builds;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable IdentifierTypo

namespace VanillaUpgrades;

public class VuKeybindings : ModKeybindings
{
    private static VuKeybindings main;

    public static void LoadKeybindings()
    {
        main = SetupKeybindings<VuKeybindings>(Main.inst);

        SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
        SceneHelper.OnWorldSceneLoaded += OnWorldLoad;

        AddStaticKeybindings();
    }

    private static void AddStaticKeybindings()
    {
        AddOnKeyDown(main.hideUI, CustomKeyExecution.HideUI);
        AddOnKeyDown(main.toggleWindowed, CustomKeyExecution.ToggleWindowed);
    }

    private static void OnBuildLoad()
    {
        AddOnKeyDown_Build(main.launch, KeyMethods.Launch);
        AddOnKeyDown_Build(main.openCategories, KeyMethods.pickCategoriesMenu.expandMenu.ToggleExpanded);
        AddOnKeyDown_Build(main.toggleSymmetry, BuildManager.main.ToggleSymmetryMode);
        AddOnKeyDown_Build(main.toggleInterior, InteriorManager.main.ToggleInteriorView);
    }

    private static void OnWorldLoad()
    {
        AddOnKeyDown_World(main.stopTimewarp, () => TimeManipulation.StopTimewarp(true));
        AddOnKeyDown_World(main.throttle01, WorldManager.Throttle01);
        AddOnKeyDown_World(main.hoverMode, HoverHandler.ToggleHoverMode);
        AddOnKeyDown_World(main.toggleTorque, VanillaUpgrades.ToggleTorque.Toggle);
    }

    public override void CreateUI()
    {
        CreateUI_Text("VanillaUpgrades Keybindings");
            
        CreateUI_Keybinding(hideUI, KeyCode.F2, "Hide UI");
        CreateUI_Keybinding(toggleWindowed, KeyCode.F11, "Toggle Windowed Mode");
        CreateUI_Space();
            
        CreateUI_Text("Build");
            
        CreateUI_Keybinding(toggleSymmetry, KeyCode.Z, "Toggle symmetry mode");
        CreateUI_Keybinding(toggleInterior, KeyCode.X, "Toggle interior view");
        CreateUI_Keybinding(openCategories, KeyCode.Tab, "Open Pick Categories");
        CreateUI_Keybinding(launch, KeyCode.L, "Launch");
        CreateUI_Space();
            
        CreateUI_Text("World");
            
        CreateUI_Keybinding(stopTimewarp, KeyCode.Slash, "Stop Timewarp");
        CreateUI_Keybinding(throttle01, KeyCode.C, "Throttle To 0.1%");
        CreateUI_Keybinding(hoverMode, KeyCode.V, "Hover mode");
        CreateUI_Keybinding(toggleTorque, KeyCode.T, "Toggle torque");
        CreateUI_Space();
    }

    #region Keys

    public Key hideUI = KeyCode.F2;

    public Key toggleWindowed = KeyCode.F11;

    public Key toggleSymmetry = KeyCode.Z;

    public Key toggleInterior = KeyCode.X;

    public Key launch = KeyCode.L;

    public Key openCategories = KeyCode.Tab;

    public Key stopTimewarp = KeyCode.Slash;

    public Key toggleTorque = KeyCode.T;

    public Key throttle01 = KeyCode.C;

    public Key hoverMode = KeyCode.V;

    #endregion
}

public static class CustomKeyExecution
{
    public static void HideUI()
    {
        GameObject pickGrid = GameObject.Find("Canvas - PickGrid");
        GameObject ui = GameObject.Find("--- UI ---");

        if (pickGrid != null) pickGrid.GetComponent<Canvas>().enabled = !pickGrid.GetComponent<Canvas>().enabled;

        if (ui != null) ui.GetComponent<Canvas>().enabled = !ui.GetComponent<Canvas>().enabled;
    }

    public static void ToggleWindowed()
    {
        VideoSettingsPC.main.windowModeDropdown.value = VideoSettingsPC.main.settings.windowMode is 0 or 2 ? 2 : 1;
    }
}