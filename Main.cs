using System;
using HarmonyLib;
using ModLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using SFS.World;

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
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "Build_PC")
			{
				GameObject gameObject = new GameObject("ASoDBuildObject");
				gameObject.AddComponent<BuildMenuFunctions>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				gameObject.SetActive(true);
				Main.buildMenuObject = gameObject;
				return;
			}

			UnityEngine.Object.Destroy(Main.buildMenuObject);

			if (scene.name == "World_PC")
			{
				worldViewObject = new GameObject("ASoDWorldObject");
				worldViewObject.AddComponent<WorldFunctions>();
				worldViewObject.AddComponent<Throttle>();
				UnityEngine.Object.DontDestroyOnLoad(worldViewObject);
				worldViewObject.SetActive(true);
				return;
			}
			
			UnityEngine.Object.Destroy(Main.worldViewObject);
		}

		public void unload()
		{
			throw new NotImplementedException();
		}

		public static GameObject buildMenuObject;

		public static GameObject worldViewObject;

		public static Harmony patcher;

		public static class KeyBool
		{
			public static bool StopKeys = false;
		}
	}
}
