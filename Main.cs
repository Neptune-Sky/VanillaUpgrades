using HarmonyLib;
using ModLoader;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.IO;

namespace ASoD_s_VanillaUpgrades
{
    public class Main : SFSMod
    {
        public Main() : base(
           "VanUp", // Mod id
           "VanillaUpgrades", // Mod Name
           "ASoD", // Mod Author
           "v1.1.x", // Mod loader version
           "v1.3" // Mod version
           )
        { }

        public override void early_load()
        {
            Main.patcher = new Harmony("mods.ASoD.VanUp");
            Main.patcher.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }

        public override void load()
        {
            mainObject = new GameObject("ASoDMainObject");
            mainObject.AddComponent<Config>();
            UnityEngine.Object.DontDestroyOnLoad(mainObject);
            mainObject.SetActive(true);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "Build_PC":
                    GameObject gameObject = new GameObject("ASoDBuildObject");
                    gameObject.AddComponent<BuildMenuFunctions>();
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                    gameObject.SetActive(true);
                    Main.buildMenuObject = gameObject;
                    UnityEngine.Object.Destroy(Main.worldViewObject);
                    return;

                case "World_PC":
                    worldViewObject = new GameObject("ASoDWorldObject");
                    worldViewObject.AddComponent<WorldFunctions>();
                    worldViewObject.AddComponent<TimewarpToClass>();
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
