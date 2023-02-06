using HarmonyLib;
using SFS.Builds;
using SFS.UI;
using UnityEngine.SceneManagement;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnOpen))]
    public class ShowSettings
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Main.menuOpen = true;
        }
    }

    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnClose))]
    public class HideSettings
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Main.menuOpen = false;
            Config.Save();
            if (SceneManager.GetActiveScene().name == "Build_PC")
            {
                if (Config.settings.moreCameraZoom)
                {
                    if (BuildManager.main.buildCamera.maxCameraDistance == 300) return;
                    BuildManager.main.buildCamera.maxCameraDistance = 300;
                    BuildManager.main.buildCamera.minCameraDistance = 0.1f;
                }
                else
                {
                    if (BuildManager.main.buildCamera.maxCameraDistance == 60) return;
                    BuildManager.main.buildCamera.maxCameraDistance = 60;
                    BuildManager.main.buildCamera.minCameraDistance = 10f;
                }
            }
        }
    }
    /*
    [HarmonyDebug]
    [HarmonyPatch(typeof(BuildManager), "ExitToHub")]
    public static class ExitToMainMenu_Build
    {
        static bool Prefix()
        {

            MenuGenerator.ShowChoices(null, ButtonBuilder.CreateButton(null, () => "Space Center", () =>
            {
                BuildState.main.UpdatePersistent();
                Base.sceneLoader.LoadHubScene();
            }, CloseMode.Current), ButtonBuilder.CreateButton(null, () => "Main Menu", () =>
            {
                BuildState.main.UpdatePersistent();
                Base.sceneLoader.LoadHomeScene(null);
            }, CloseMode.Current));

            Debug.Log("fuck");
            return false;
        }
    }*/


}