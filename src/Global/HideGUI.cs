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
                Config.settings.persistentVars.opacity = 1;
                VideoSettingsPC.main.uiOpacitySlider.value = 0;
                Config.settings.guiHidden = true;
            }
            else
            {
                Config.settings.persistentVars.opacity = VideoSettingsPC.main.uiOpacitySlider.value;
                Config.settings.guiHidden = false;
            }

            File.WriteAllText(Config3.configPath, Config3.settings2.ToString());
        }
    }

    public static class OpacityChanger
    {
        public static bool hidden;
        public static float uiOpacity = Config.settings.persistentVars.opacity;

        public static void HideUI()
        {
            hidden = !hidden;
            float toChange = 0;
            if (hidden) toChange = Config.settings.persistentVars.opacity;
            VideoSettingsPC.main.uiOpacitySlider.value = toChange;
            Config.settings.guiHidden = hidden;
            File.WriteAllText(Config3.configPath, Config3.settings2.ToString());
        }
    }
}
