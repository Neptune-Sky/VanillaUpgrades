using SFS;
using SFS.Builds;
using UnityEngine;

namespace ASoD_s_VanillaUpgrades
{
    public class BuildMenuFunctions : MonoBehaviour
    {
        // Token: 0x0400136C RID: 4972
        public static Rect windowRect = new Rect((float)Screen.width * 0.05f, (float)Screen.height * 0.94f, 180f, 70f);

        // Token: 0x0400136D RID: 4973
        public static bool snapping;

        public static bool partMagnetizing;

        // Token: 0x0400136E RID: 4974
        public static bool noAdaptation;

        public static bool noAdaptOverride;

        public static void Launch()
        {
            BuildState.main.SaveLaunchData(updatePersistent: true);
            Base.sceneLoader.LoadWorldScene(launch: true);
        }

        public void windowFunc(int windowID)
        {
            snapping = GUI.Toggle(new Rect(10f, 20f, 120f, 20f), snapping, " Disable Snapping");
            noAdaptation = GUI.Toggle(new Rect(10f, 40f, 120f, 20f), noAdaptation, " Disable Adapting");


            GUI.DragWindow();
        }

        public void OnGUI()
        {
            if (Main.menuOpen || !(bool)Config.settings["showBuildGUI"] || VideoSettingsPC.main.uiOpacitySlider.value == 0) return;

            windowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), windowRect, new GUI.WindowFunction(windowFunc), "Build Settings");

        }
    }
}
