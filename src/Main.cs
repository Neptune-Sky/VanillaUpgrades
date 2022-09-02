using HarmonyLib;
using ModLoader;
using SFS;
using SFS.IO;
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
<<<<<<< Updated upstream
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
=======
        public override string ModNameID => "VanUp";
        public override string DisplayName => "Vanilla Upgrades";
        public override string Author => "ASoD";
        public override string MinimumGameVersionNecessary => "1.5.7";
        public override string ModVersion => "v3.1";
        public override string Description => "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features.";

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            UnityEngine.Object.DontDestroyOnLoad(mainObject);
        }

        void SubscribeToScenes()
        {
            SceneHelper.OnBuildSceneLoaded += () => buildObject = new GameObject("ASoDBuildObject", typeof(BuildSettings), typeof(DVCalc));
            SceneHelper.OnWorldSceneLoaded += () => worldObject = new GameObject("ASoDWorldObject", typeof(WorldManager), typeof(AdvancedInfo), typeof(FaceDirection), typeof(WorldClockDisplay));
=======
            Object.DontDestroyOnLoad(mainObject);
            mainObject.SetActive(true);
        }
       
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            switch (scene.name)
            {
                case "Build_PC":
                    buildObject = new GameObject("ASoDBuildObject");
                    buildObject.AddComponent<BuildSettings>();
                    buildObject.AddComponent<DVCalc>();
                    buildObject.SetActive(true);
                    UnityEngine.Object.Destroy(worldObject);
                    return;

                case "World_PC":
                    worldObject = new GameObject("ASoDWorldObject");
                    worldObject.AddComponent<WorldManager>();
                    worldObject.AddComponent<AdvancedInfo>();
                    // worldViewObject.AddComponent<TimewarpToClass>();
                    worldObject.AddComponent<FaceDirection>();
                    worldObject.AddComponent<WorldClockDisplay>();
                    worldObject.SetActive(true);
                    UnityEngine.Object.Destroy(buildObject);
                    return;
                default:
                    UnityEngine.Object.Destroy(buildObject);
                    UnityEngine.Object.Destroy(worldObject);

                    return;
            }
>>>>>>> Stashed changes
        }

        void OnQuit()
        {
            File.WriteAllText(WindowManager.inst.windowDir, WindowManager.settings.ToString());
        }

<<<<<<< Updated upstream
=======


>>>>>>> Stashed changes
        public static bool menuOpen;

        public static GameObject mainObject;

        public static GameObject buildObject;

        public static GameObject worldObject;

        public static Harmony patcher;

        public static FolderPath modFolder;
    }
}
