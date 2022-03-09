using HarmonyLib;
using ModLoader;
using SFS.World;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ASoD_s_VanillaUpgrades
{
    public class Main : SFSMod
    {
		public string getModAuthor()
		{
			return "ASoD";
		}

		public string getModName()
		{
			return "VanillaUpgrades";
		}

		public void load()
		{
			Main.patcher = new Harmony("mods.ASoD.VanUp");
			Main.patcher.PatchAll();
			Loader.modLoader.suscribeOnChangeScene(new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded));

			mainObject = new GameObject("ASoDMainObject");
			mainObject.AddComponent<Config>();
			UnityEngine.Object.DontDestroyOnLoad(mainObject);
			mainObject.SetActive(true);
			return;
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
					worldViewObject.AddComponent<Throttle>();
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

		public void unload()
		{
			throw new NotImplementedException();
		}
		public static bool menuOpen;

		public static GameObject mainObject;

		public static GameObject buildMenuObject;

		public static GameObject worldViewObject;

		public static Harmony patcher;

		public static class KeyBool
		{
			public static bool StopKeys = false;
		}
	}
}
