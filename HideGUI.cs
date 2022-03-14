using HarmonyLib;
using SFS;
using UnityEngine;
using System.IO;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(VideoSettingsPC), "UIOpacityChanged")]
    public class OpacityChangeListener
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Config.settings["persistentVars"]["opacity"] = 0;
            File.WriteAllText(Config.configPath, Config.settings.ToString());
        }
    }

    public class OpacityChanger
    {
        public static float uiOpacity = (float)Config.settings["persistentVars"]["opacity"];

        public static void HideUI()
        {

            var oldSettings = VideoSettingsPC.main.uiOpacitySlider.value;
            VideoSettingsPC.main.uiOpacitySlider.value = (float)Config.settings["persistentVars"]["opacity"];
            Config.settings["persistentVars"]["opacity"] = oldSettings;
            File.WriteAllText(Config.configPath, Config.settings.ToString());
        }
    }
}
