using System;
using System.Collections.Generic;
using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS.IO;
using UITools;
using UnityEngine;
using Console = ModLoader.IO.Console;
using Object = UnityEngine.Object;

namespace VanillaUpgrades
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Main : Mod, IUpdatable
    {

        public static Main main;

        public static bool menuOpen;

        public static bool buildSettingsPresent;

        private static GameObject mainObject;

        private static Harmony patcher;

        public static FolderPath modFolder;
        public override string ModNameID => "VanUp";
        public override string DisplayName => "Vanilla Upgrades";
        public override string Author => "StarMods";
        public override string MinimumGameVersionNecessary => "1.5.10.2";
        public override string ModVersion => "v5.1.2.1";

        public override string Description =>
            "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features.";

        public override Action LoadKeybindings => VU_Keybindings.LoadKeybindings;

        public override Dictionary<string, string> Dependencies => new() { { "UITools", "1.0" } };
        
        public Dictionary<string, FilePath> UpdatableFiles => new() { { "https://github.com/Neptune-Sky/VanillaUpgrades/releases/latest/download/VanillaUpgrades.dll", new FolderPath(ModFolder).ExtendToFile("VanillaUpgrades.dll") } };

        public override void Early_Load()
        {
            main = this;
            modFolder = new FolderPath(ModFolder);
            patcher = new Harmony("mods.ASoD.VanUp");
            patcher.PatchAll();
            SubscribeToScenes();
            Application.quitting += OnQuit;
            Config.Load();
        }

        public override void Load()
        {
            Console.commands.Add(Command);
            ConfigUI.Setup();
            Application.runInBackground = Config.settings.allowBackgroundProcess;
            Config.settings.allowBackgroundProcess.OnChange +=
                () => Application.runInBackground = Config.settings.allowBackgroundProcess;
            mainObject = new GameObject("ASoDMainObject", typeof(ErrorNotification));
            Object.DontDestroyOnLoad(mainObject);
            mainObject.SetActive(true);
        }

        private static void SubscribeToScenes()
        {
            SceneHelper.OnBuildSceneLoaded += BuildSettings.Setup;
            SceneHelper.OnWorldSceneLoaded += () =>
            {
                WorldManager.Setup();
                new GameObject("ASoDWorldObject", typeof(AdvancedInfo), typeof(WorldClockDisplay));
            };
        }

        private static void OnQuit()
        {
            Config.Save();
        }

        private static bool Command(string str)
        {
            if (!str.StartsWith("vu")) return false;
            Debug.Log("Hi");

            return true;

        }
    }

    [HarmonyPatch(typeof(Loader), "Initialize_Load")]
    internal class ModCheck
    {
        private static void Postfix(List<Mod> ___loadedMods)
        {
            if (___loadedMods.FindIndex(e => e.ModNameID == "BuildSettings") == -1) return;
            Main.buildSettingsPresent = true;
            Debug.Log(
                "VanillaUpgrades: BuildSettings mod was detected, disabling own Build Settings features to avoid conflicts.");
        }
    }
}

