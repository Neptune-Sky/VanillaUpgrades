using HarmonyLib;
using ModLoader;
using SFS;
using SFS.IO;
using System.IO;
using System.Reflection;
using ModLoader.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VanillaUpgrades
{
    public class Main : Mod
    {
        public static Main main;
        public override string ModNameID => "VanUp";
        public override string DisplayName => "Vanilla Upgrades";
        public override string Author => "ASoD";
        public override string MinimumGameVersionNecessary => "1.5.7";
        public override string ModVersion => "v3.1";
        public override string Description => "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features.";

        public override void Early_Load()
        {
            main = this;
            modFolder = new FolderPath(ModFolder);
            patcher = new Harmony("mods.ASoD.VanUp");
            patcher.PatchAll();
            SubscribeToScenes();
            Application.quitting += OnQuit;
        }

        public override void Load()
        {
            mainObject = new GameObject("ASoDMainObject", typeof(WindowManager), typeof(Config), typeof(ErrorNotification));
            Object.DontDestroyOnLoad(mainObject);
            mainObject.SetActive(true);
        }

        void SubscribeToScenes()
        {
            SceneHelper.OnBuildSceneLoaded += () => buildObject = new GameObject("ASoDBuildObject", typeof(BuildSettings), typeof(DVCalc));
            SceneHelper.OnWorldSceneLoaded += () => worldObject = new GameObject("ASoDWorldObject", typeof(WorldManager), typeof(AdvancedInfo), typeof(FaceDirection), typeof(WorldClockDisplay));
        }

        void OnQuit()
        {
            File.WriteAllText(WindowManager.inst.windowDir, WindowManager.settings.ToString());
        }

        public static bool menuOpen;

        public static GameObject mainObject;

        public static GameObject buildObject;

        public static GameObject worldObject;

        public static Harmony patcher;

        public static FolderPath modFolder;
    }
}
