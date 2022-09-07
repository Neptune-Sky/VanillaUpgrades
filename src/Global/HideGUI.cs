using HarmonyLib;
using SFS;
using System.IO;
using SFS.IO;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(VideoSettingsPC), "UIOpacityChanged")]
    public static class OpacityChangeListener
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!Main.menuOpen) return;
            if (VideoSettingsPC.main.uiOpacitySlider.value < 0.001)
            {
                Config.settingsData.persistentVars.opacity = 1;
                VideoSettingsPC.main.uiOpacitySlider.value = 0;
                Config.settingsData.guiHidden = true;
            }
            else
            {
                Config.settingsData.persistentVars.opacity = VideoSettingsPC.main.uiOpacitySlider.value;
                Config.settingsData.guiHidden = false;
            }

            Config.Save();
        }
    }

    public static class OpacityChanger
    {
        public static bool hidden;
        public static float uiOpacity = Config.settingsData.persistentVars.opacity;

        public static void HideUI()
        {
            hidden = !hidden;
            float toChange = 0;
            if (hidden) toChange = Config.settingsData.persistentVars.opacity;
            VideoSettingsPC.main.uiOpacitySlider.value = toChange;
            Config.settingsData.guiHidden = hidden;
            Config.Save();
        }
    }
}
