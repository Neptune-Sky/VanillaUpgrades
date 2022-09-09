using HarmonyLib;
using ModLoader;
using SFS.IO;
using ModLoader.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;
using System;

namespace VanillaUpgrades
{
    public class Main : Mod
    {
        public static Main main;
        public override string ModNameID => "VanUp";
        public override string DisplayName => "Vanilla Upgrades";
        public override string Author => "ASoD";
        public override string MinimumGameVersionNecessary => "1.5.7";
        public override string ModVersion => "v4.0";
        public override string Description => "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features.";
        // public override Func<ModKeybindings> OnLoadKeybindings => MyKeybindings.Setup;

        public override void Early_Load()
        {
            main = this;
            modFolder = new FolderPath(base.ModFolder);
            patcher = new Harmony("mods.ASoD.VanUp");
            patcher.PatchAll();
            SubscribeToScenes();
            Application.quitting += OnQuit;
            Config.Load();
        }

        public override void Load()
        {
            ConfigUI.Setup();
            mainObject = new GameObject("ASoDMainObject", typeof(WindowManager2), typeof(WindowManager), typeof(ErrorNotification));
            Object.DontDestroyOnLoad(mainObject);
            mainObject.SetActive(true);
        }

        void SubscribeToScenes()
        {
            SceneHelper.OnBuildSceneLoaded += () => buildObject = new GameObject("ASoDBuildObject", typeof(BuildSettings), typeof(DVCalc));
            SceneHelper.OnWorldSceneLoaded += () =>
            {
                WorldManager.Setup();
                worldObject = new GameObject("ASoDWorldObject", typeof(AdvancedInfo), typeof(WorldClockDisplay));
            };
        }

        void OnQuit()
        {
            Config.Save();
            WindowManager.Save();
        }
        public static bool menuOpen;

        public static GameObject mainObject;

        public static GameObject buildObject;

        public static GameObject worldObject;

        public static Harmony patcher;

        public static FolderPath modFolder;
    }
}
