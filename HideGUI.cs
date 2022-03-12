using HarmonyLib;
using SFS;
using UnityEngine;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(VideoSettingsPC), "UIOpacityChanged")]
    public class OpacityChangeListener
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            PlayerPrefs.SetFloat("ASoDLastOpacitySetting", 0f);
        }
    }

    public class OpacityChanger
    {
        public static void HideUI()
        {
            if (!PlayerPrefs.HasKey("ASoDLastOpacitySetting")) PlayerPrefs.SetFloat("ASoDLastOpacitySetting", 0f);
            var oldSettings = VideoSettingsPC.main.uiOpacitySlider.value;
            VideoSettingsPC.main.uiOpacitySlider.value = PlayerPrefs.GetFloat("ASoDLastOpacitySetting");
            PlayerPrefs.SetFloat("ASoDLastOpacitySetting", oldSettings);
        }
    }
}
