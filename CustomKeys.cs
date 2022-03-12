using System;
using SFS.Builds;
using SFS.Input;
using SFS.IO;
using UnityEngine;
using HarmonyLib;
using static SFS.Input.KeybindingsPC;
using SFS.World;

namespace ASoD_s_VanillaUpgrades
{
    
    public static class KeybindsHolder
    {
        public static CustomKeybinds keybinds = new CustomKeybinds();
    }

    public class CustomKeybinds : Settings<CustomKeybinds.DefaultData>
    {
        protected override string FileName => "VanillaUpgradesKeybinds";

        public static DefaultData custom_keys = new DefaultData();
        public class DefaultData
        {

            public Key Launch = KeyCode.L;

            public Key Throttle01 = KeyCode.BackQuote;

            public Key Stop_Timewarp = KeyCode.Slash;

            public Key Timewarp_To = KeyCode.Semicolon;

            public Key Hide_UI = KeyCode.F2;

        }

        // Loads automatically(?), no need for a hook here
        protected override void OnLoad()
        {
            custom_keys = settings;
        }

        public void AwakeHook(KeybindingsPC __instance)
        {

            // Load the saved and default keys
            Load();
            DefaultData defaultData = new DefaultData();

            // Reflection needed since these are private
            Traverse createTraverse = Traverse.Create(__instance).Method("Create", new object[] {custom_keys.Launch, defaultData.Launch, "Launch"});
            Traverse createSpaceTraverse = Traverse.Create(__instance).Method("CreateSpace");

            // Finally actually call the code
            createTraverse.GetValue(new object[] { custom_keys.Launch, defaultData.Launch, "Launch" });
            createTraverse.GetValue(new object[] { custom_keys.Throttle01, defaultData.Throttle01, "Throttle_To_0.1%" });
            createTraverse.GetValue(new object[] { custom_keys.Stop_Timewarp, defaultData.Stop_Timewarp, "Stop_Timewarp" });
            createTraverse.GetValue(new object[] { custom_keys.Timewarp_To, defaultData.Timewarp_To, "Timewarp_To" });
            createTraverse.GetValue(new object[] { custom_keys.Hide_UI, defaultData.Hide_UI, "Hide UI" });
            createSpaceTraverse.GetValue();
            createSpaceTraverse.GetValue();
            createSpaceTraverse.GetValue();
        }

        public void SaveData()
        {
            Save();
        }

    }

    // Save hook for... Saving
    [HarmonyPatch(typeof(Settings<Data>), "Save")]
    public class KeybindsSaveHook
    {
        [HarmonyPostfix]
        public static void SaveHook(Settings<Data> __instance)
        {
            // This works. I don't know why. At least it sucessfully prevents infinite loops
            if (__instance is KeybindingsPC)
            {
                KeybindsHolder.keybinds.SaveData();
            }
        }
    }

    // Awake hook to add the menu items
    [HarmonyPatch(typeof(KeybindingsPC), "Awake")]
    public class KeybindsAwakeHook
    {
        [HarmonyPrefix]
        public static void Prefix(KeybindingsPC __instance)
        {
            KeybindsHolder.keybinds.AwakeHook(__instance);
        }
    }

    [HarmonyPatch(typeof(BuildMenus), "Start")]
    public static class AddBuildKeybinds
    {
        [HarmonyPrefix]
        public static void Prefix(ref BuildMenus __instance)
        {
            KeysNode keysNode = BuildManager.main.build_Input.keysNode;

            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Launch, () => BuildMenuFunctions.Launch());
            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Hide_UI, () => OpacityChanger.HideUI());
        }
    }

    [HarmonyPatch(typeof(GameManager), "Start")]
    public class AddWorldKeybinds
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Stop_Timewarp, delegate
            {
                WorldFunctions.StopTimewarp(true);
            });
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Throttle01, new Action(WorldFunctions.Throttle01));
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Timewarp_To, new Action(TimewarpToClass.TimewarpTo));
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Hide_UI, new Action(OpacityChanger.HideUI));
        }
    }
}
