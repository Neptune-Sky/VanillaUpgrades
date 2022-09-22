using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.UI;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnOpen))]
    public class ShowSettings
    {
        [HarmonyPostfix]
        public static void Postfix(BasicMenu __instance)
        {
            if (__instance.gameObject.HasComponent<VideoSettingsPC>())
            {
                 ConfigUI.window.Enable();
            }
            Main.menuOpen = true;
        }
    }

    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnClose))]
    public class HideSettings
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            ConfigUI.window?.Disable();
            Main.menuOpen = false;
            Config.Save();
            if (Main.buildObject != null)
            {
                if (Config.settingsData.moreCameraZoom)
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
}
