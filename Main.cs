using System;
using HarmonyLib;
using ModLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ASoD_s_VanillaUpgrades
{
    public class Main
    {
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public string getModAuthor()
		{
			return "ASoD";
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public string getModName()
		{
			return "VanillaUpgrades";
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000205E File Offset: 0x0000025E
		public void load()
		{
			Main.patcher = new Harmony("mods.ASoD.VanUp");
			Main.patcher.PatchAll();
			Loader.modLoader.suscribeOnChangeScene(new UnityAction<Scene, LoadSceneMode>(this.OnSceneLoaded));
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002090 File Offset: 0x00000290
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "Build_PC")
			{
				GameObject gameObject = new GameObject("ASoDBuildObject");
				gameObject.AddComponent<Menu>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				gameObject.SetActive(true);
				Main.menuObject = gameObject;
				return;
			}
			UnityEngine.Object.Destroy(Main.menuObject);
		}

		public void unload()
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000001 RID: 1
		public static GameObject menuObject;

		// Token: 0x04000002 RID: 2
		public static Harmony patcher;
	}
}
