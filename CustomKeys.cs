using HarmonyLib;
using SFS.Builds;
using SFS.Input;
using SFS.IO;
using SFS.World;
using System;
using UnityEngine;
using static SFS.Input.KeybindingsPC;

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
            public Key Hide_UI = KeyCode.F2;

            public Key Launch = KeyCode.L;

            public Key Stop_Timewarp = KeyCode.Slash;

            public Key Timewarp_To = KeyCode.Semicolon;

            public Key Throttle01 = KeyCode.C;

            public Key Prograde = KeyCode.Alpha1;

            public Key Retrograde = KeyCode.Alpha2;

            public Key Rad_In = KeyCode.Alpha3;

            public Key Rad_Out = KeyCode.Alpha4;

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
            Traverse createTraverse = Traverse.Create(__instance).Method("Create", new object[] { custom_keys.Launch, defaultData.Launch, "Launch" });
            Traverse createSpaceTraverse = Traverse.Create(__instance).Method("CreateSpace");
            Traverse createTextTraverse = Traverse.Create(__instance).Method("CreateText", "VanillaUpgrades Keybinds");

            // Finally actually call the code
            createTextTraverse.GetValue("VanillaUpgrades Keybinds");
            createTraverse.GetValue(new object[] { custom_keys.Hide_UI, defaultData.Hide_UI, "Hide UI" });
            createSpaceTraverse.GetValue();
            createTraverse.GetValue(new object[] { custom_keys.Launch, defaultData.Launch, "Launch" });
            createSpaceTraverse.GetValue();
            createTraverse.GetValue(new object[] { custom_keys.Stop_Timewarp, defaultData.Stop_Timewarp, "Stop_Timewarp" });
            createTraverse.GetValue(new object[] { custom_keys.Timewarp_To, defaultData.Timewarp_To, "Timewarp_To" });
            createTraverse.GetValue(new object[] { custom_keys.Throttle01, defaultData.Throttle01, "Throttle_To_0.1%" });
            createSpaceTraverse.GetValue();
            createSpaceTraverse.GetValue();

            /* Pissed me off, will work on it later.
            createTraverse.GetValue(new object[] { custom_keys.Prograde, defaultData.Prograde, "Face prograde" });
            createTraverse.GetValue(new object[] { custom_keys.Retrograde, defaultData.Retrograde, "Face retrograde" });
            createTraverse.GetValue(new object[] { custom_keys.Rad_In, defaultData.Rad_In, "Face radial in" });
            createTraverse.GetValue(new object[] { custom_keys.Rad_Out, defaultData.Rad_Out, "Face radial out" });
            */

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

            keysNode.AddOnKeyDown(CustomKeybinds.custom_keys.Launch, () => BuildSettings.Launch());
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
                AdvancedInfo.StopTimewarp(true);
            });
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Throttle01, new Action(AdvancedInfo.Throttle01));
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Timewarp_To, new Action(TimewarpToClass.TimewarpTo));
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Hide_UI, new Action(OpacityChanger.HideUI));
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Prograde, delegate
            {
                FaceDirection.Mode(1);
            });
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Retrograde, delegate
            {
                FaceDirection.Mode(2);
            });
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Rad_In, delegate
            {
                FaceDirection.Mode(3);
            });
            GameManager.AddOnKeyDown(CustomKeybinds.custom_keys.Rad_Out, delegate
            {
                FaceDirection.Mode(4);
            });
        }
    }
}
