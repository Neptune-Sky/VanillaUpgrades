using HarmonyLib;
using ModLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VanillaUpgrades
{
    public class Main : SFSMod
    {
        public Main() : base(
           "VanUp", // Mod id
           "VanillaUpgrades", // Mod Name
           "ASoD", // Mod Author
           "v1.3.2", // Mod loader version
           "v2.3.5", // Mod version
           "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features."
           )
        { }
        public override void early_load()
        {
            patcher = new Harmony("mods.ASoD.VanUp");
            patcher.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.quitting += OnQuit;
            return;
        }

        public override void load()
        {
            mainObject = new GameObject("ASoDMainObject");
            mainObject.AddComponent<WindowManager>();
            mainObject.AddComponent<Config>();
            mainObject.AddComponent<ErrorNotification>();
            UnityEngine.Object.DontDestroyOnLoad(mainObject);
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
        }

        private void OnQuit()
        {
            File.WriteAllText(WindowManager.inst.windowDir, WindowManager.settings.ToString());
        }

        public override void unload()
        {
            throw new NotImplementedException();
        }



        public static bool menuOpen;

        public static GameObject mainObject;

        public static GameObject buildObject;

        public static GameObject worldObject;

        public static Harmony patcher;

        public static string modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
    }
}
