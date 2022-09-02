using HarmonyLib;
using SFS;
using System.IO;
using SFS.IO;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(VideoSettingsPC), "UIOpacityChanged")]
    public class OpacityChangeListener
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!Main.menuOpen) return;
            Config.settings["persistentVars"]["opacity"] = VideoSettingsPC.main.uiOpacitySlider.value;
            File.WriteAllText(Config.configPath, Config.settings.ToString());
        }
    }

    public class OpacityChanger
    {
        public static bool hidden;
        public static float uiOpacity = (float)Config.settings["persistentVars"]["opacity"];

        public static void HideUI()
        {
            hidden = !hidden;
            float toChange = 0;
            if (hidden) toChange = (float)Config.settings["persistentVars"]["opacity"];
            VideoSettingsPC.main.uiOpacitySlider.value = toChange;
            Config.settings["guiHidden"] = hidden;
            File.WriteAllText(Config.configPath, Config.settings.ToString());
        }
    }
}
