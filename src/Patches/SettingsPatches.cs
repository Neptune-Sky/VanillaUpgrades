using HarmonyLib;
using Newtonsoft.Json.Linq;
using SFS;
using SFS.Parsers.Json;
using SFS.Builds;
using SFS.UI;
using System;
using System.IO;
using UnityEngine;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnOpen))]
    public class ShowSettings
    {
        [HarmonyPostfix]
        public static void Postfix(BasicMenu __instance)
        {
            if (__instance.GetComponent<VideoSettingsPC>())
            {
                Config.settingsWindow.gameObject.SetActive(true);
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
            Config.settingsWindow.gameObject.SetActive(false);
            Main.menuOpen = false;
            Config.Save();
            if (Main.buildObject != null)
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
            Config3.windowColor = new Color((float)Config3.settings2["persistentVars"]["windowColor"]["r"], (float)Config3.settings2["persistentVars"]["windowColor"]["g"], (float)Config3.settings2["persistentVars"]["windowColor"]["b"], VideoSettingsPC.main.uiOpacitySlider.value);
        }

    }
}
