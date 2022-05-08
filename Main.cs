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
           "v1.1.x", // Mod loader version
           "v2.2.7", // Mod version
           "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features."
           )
        { }
        public override void early_load()
        {
            Main.patcher = new Harmony("mods.ASoD.VanUp");
            Main.patcher.PatchAll();
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
                    GameObject gameObject = new GameObject("ASoDBuildObject");
                    gameObject.AddComponent<BuildSettings>();
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                    gameObject.SetActive(true);
                    Main.buildMenuObject = gameObject;
                    UnityEngine.Object.Destroy(Main.worldViewObject);
                    return;

                case "World_PC":
                    worldViewObject = new GameObject("ASoDWorldObject");
                    worldViewObject.AddComponent<AdvancedInfo>();
                    worldViewObject.AddComponent<TimewarpToClass>();
                    worldViewObject.AddComponent<FaceDirection>();
                    worldViewObject.AddComponent<WorldTimeDisplay>();
                    UnityEngine.Object.DontDestroyOnLoad(worldViewObject);
                    worldViewObject.SetActive(true);
                    UnityEngine.Object.Destroy(Main.buildMenuObject);
                    return;

                default:
                    UnityEngine.Object.Destroy(Main.buildMenuObject);
                    UnityEngine.Object.Destroy(Main.worldViewObject);
                    break;
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

        public static GameObject buildMenuObject;

        public static GameObject worldViewObject;

        public static Harmony patcher;

        public static string modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
    }
}
