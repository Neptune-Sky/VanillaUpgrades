using HarmonyLib;
using System;
using ModLoader;
using System.IO;
using System.Reflection;
using ModLoader.Helpers;
using SFS.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VanillaUpgrades
{
    public class Main : Mod
    {
        public Main() : base(
           "VanUp", // Mod id
           "VanillaUpgrades", // Mod Name
           "ASoD", // Mod Author
           "0.5.7", // Game version
           "v3.0", // Mod version
           "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features."
           )
        { }

        public static Main main;
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
            mainObject = new GameObject("ASoDMainObject");
            mainObject.AddComponent<WindowManager>();
            mainObject.AddComponent<Config>();
            mainObject.AddComponent<ErrorNotification>();
            UnityEngine.Object.DontDestroyOnLoad(mainObject);
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
