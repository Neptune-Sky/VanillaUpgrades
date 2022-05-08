using HarmonyLib;
using Newtonsoft.Json.Linq;
using SFS;
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
                Config.showSettings = true;
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
            Config.showSettings = false;
            Main.menuOpen = false;
            File.WriteAllText(Config.configPath, Config.settings.ToString());
            if (Main.buildMenuObject != null)
            {
                if ((bool)Config.settings["moreCameraZoom"])
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
            Config.windowColor = new Color((float)Config.settings["persistentVars"]["windowColor"]["r"], (float)Config.settings["persistentVars"]["windowColor"]["g"], (float)Config.settings["persistentVars"]["windowColor"]["b"], VideoSettingsPC.main.uiOpacitySlider.value);
        }

    }

    public class Config : MonoBehaviour
    {
        public static string configPath = Main.modDir + "Config.txt";

        public static JObject defaultConfig = JObject.Parse("{ " +
            "persistentVars: { " +
            "opacity: 0, " +
            "windowColor: {" +
            "r: 1.0, " +
            "g: 1.0, " +
            "b: 1.0 " +
            "} " +
            "}, " +
            "showBuildGUI: true, " +
            "showAdvanced: true, " +
            "showTime: true, " +
            "worldTime: true," +
            "alwaysShowTime: false," +
            "mmUnits: true, " +
            "kmsUnits: true, " +
            "cUnits: true, " +
            "ktUnits: true, " +
            "shakeEffects: true," +
            "explosions: true," +
            "explosionShake: true, " +
            "stopTimewarpOnEncounter: true, " +
            "moreCameraZoom: true, " +
            "moreCameraMove: true, " +
            "allowTimeSlowdown: false, " +
            "higherPhysicsWarp: false }");

        public static JObject settings;

        public static bool showSettings;

        public static int test = 3;

        public static float windowHeight = 350f;

        public static Color windowColor;

        public Vector2 scroll = Vector2.zero;

        public Rect windowRect = new Rect((float)WindowManager.settings["config"]["x"], (float)WindowManager.settings["config"]["y"], 245f, windowHeight);

        public void Awake()
        {
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, defaultConfig.ToString());
            }

            try
            {
                settings = JObject.Parse(File.ReadAllText(configPath));
            }
            catch (Exception e)
            {
                File.WriteAllText(configPath, defaultConfig.ToString());
                ErrorNotification.Error("Config file was of an invalid format, and was reset to defaults.");
                settings = defaultConfig;
            }

            if (settings["persistentVars"] == null) settings["persistentVars"] = defaultConfig["persistentVars"];

            if (settings["persistentVars"]["opacity"] == null) settings["persistentVars"]["opacity"] = defaultConfig["persistentVars"]["opacity"];

            if (settings["persistentVars"]["windowColor"] == null) settings["persistentVars"]["windowColor"] = defaultConfig["persistentVars"]["windowColor"];

            try
            {
                Vector3 check = new Vector3((float)settings["persistentVars"]["windowColor"]["r"], (float)settings["persistentVars"]["windowColor"]["g"], (float)settings["persistentVars"]["windowColor"]["b"]);
            }
            catch (Exception e)
            {
                ErrorNotification.Error("Window color data was of an invalid format, and was reset to defaults.");
                settings["persistentVars"]["windowColor"] = defaultConfig["persistentVars"]["windowColor"];
            }

            if (settings["showBuildGUI"] == null) settings["showBuildGUI"] = defaultConfig["showBuildGUI"];

            if (settings["showAdvanced"] == null) settings["showAdvanced"] = defaultConfig["showAdvanced"];

            if (settings["showTime"] == null) settings["showTime"] = defaultConfig["showTime"];

            if (settings["worldTime"] == null) settings["worldTime"] = defaultConfig["worldTime"];

            if (settings["alwaysShowTime"] == null) settings["alwaysShowTime"] = defaultConfig["alwaysShowTime"];

            if (settings["mmUnits"] == null) settings["mmUnits"] = defaultConfig["mmUnits"];

            if (settings["kmsUnits"] == null) settings["kmsUnits"] = defaultConfig["kmsUnits"];

            if (settings["cUnits"] == null) settings["cUnits"] = defaultConfig["cUnits"];

            if (settings["ktUnits"] == null) settings["ktUnits"] = defaultConfig["ktUnits"];

            if (settings["shakeEffects"] == null) settings["shakeEffects"] = defaultConfig["shakeEffects"];

            if (settings["explosions"] == null) settings["explosions"] = defaultConfig["explosions"];

            if (settings["explosionShake"] == null) settings["explosionShake"] = defaultConfig["explosionShake"];

            if (settings["stopTimewarpOnEncounter"] == null) settings["stopTimewarpOnEncounter"] = defaultConfig["stopTimewarpOnEncounter"];

            if (settings["moreCameraZoom"] == null) settings["moreCameraZoom"] = defaultConfig["moreCameraZoom"];

            if (settings["moreCameraMove"] == null) settings["moreCameraMove"] = defaultConfig["moreCameraMove"];

            if (settings["allowTimeSlowdown"] == null) settings["allowTimeSlowdown"] = defaultConfig["allowTimeSlowdown"];

            if (settings["higherPhysicsWarp"] == null) settings["higherPhysicsWarp"] = defaultConfig["higherPhysicsWarp"];

            windowColor = new Color((float)settings["persistentVars"]["windowColor"]["r"], (float)settings["persistentVars"]["windowColor"]["g"], (float)settings["persistentVars"]["windowColor"]["b"], VideoSettingsPC.main.uiOpacitySlider.value);

            File.WriteAllText(Config.configPath, Config.settings.ToString());
        }

        public void windowFunc(int windowID)
        {
            scroll = GUILayout.BeginScrollView(scroll);

            GUILayout.Label("GUI:");

            settings["showBuildGUI"] = GUILayout.Toggle((bool)settings["showBuildGUI"], " Show Build Settings");

            settings["showAdvanced"] = GUILayout.Toggle((bool)settings["showAdvanced"], " Show Advanced Info");

            settings["showTime"] = GUILayout.Toggle((bool)settings["showTime"], " World Clock During Timewarp");

            if ((bool)settings["showTime"])
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                settings["worldTime"] = GUILayout.Toggle((bool)settings["worldTime"], " World Time In World Clock");
                GUILayout.EndHorizontal();
            }

            if (!(bool)settings["showTime"] || !(bool)settings["worldTime"])
            {
                settings["alwaysShowTime"] = false;
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                settings["alwaysShowTime"] = GUILayout.Toggle((bool)settings["alwaysShowTime"], " Always Show World Time");
                GUILayout.EndHorizontal();
            }

            GUILayout.Label("\nUnits:");

            settings["mmUnits"] = GUILayout.Toggle((bool)settings["mmUnits"], " Megameters (Mm)");

            settings["kmsUnits"] = GUILayout.Toggle((bool)settings["kmsUnits"], " Kilometers per Second (km/s)");

            settings["cUnits"] = GUILayout.Toggle((bool)settings["cUnits"], " % Speed of Light (c)");

            settings["ktUnits"] = GUILayout.Toggle((bool)settings["ktUnits"], " Kilotonnes (kt)");


            GUILayout.Label("\nFunctions:");

            settings["stopTimewarpOnEncounter"] = GUILayout.Toggle((bool)settings["stopTimewarpOnEncounter"], " Stop Timewarp on Encounter");

            settings["moreCameraZoom"] = GUILayout.Toggle((bool)settings["moreCameraZoom"], " Camera Zoom Increase");

            settings["moreCameraMove"] = GUILayout.Toggle((bool)settings["moreCameraMove"], " Camera Movement Increase");

            GUILayout.Label("\n\"Cheaty\" Functions:");

            settings["allowTimeSlowdown"] = GUILayout.Toggle((bool)settings["allowTimeSlowdown"], " Allow Time Slowdown");

            settings["higherPhysicsWarp"] = GUILayout.Toggle((bool)settings["higherPhysicsWarp"], " Higher Physics Timewarps");


            GUILayout.Label("\n Additional Settings:");

            settings["shakeEffects"] = GUILayout.Toggle((bool)settings["shakeEffects"], " Toggle Shake Effects");

            if (!(bool)settings["shakeEffects"])
            {
                settings["explosionShake"] = false;
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                settings["explosionShake"] = GUILayout.Toggle((bool)settings["explosionShake"], " Toggle Explosion Shake");
                GUILayout.EndHorizontal();
            }

            settings["explosions"] = GUILayout.Toggle((bool)settings["explosions"], " Toggle Explosion Effects");


            GUILayout.Label("\nWindow Color:");


            GUILayout.BeginHorizontal();
            GUILayout.Label("R", GUILayout.MaxWidth(15));

            GUILayout.BeginVertical();
            GUILayout.Space(9);
            settings["persistentVars"]["windowColor"]["r"] = GUILayout.HorizontalSlider((float)settings["persistentVars"]["windowColor"]["r"], 0, 1, GUILayout.Width(140));
            GUILayout.EndVertical();

            GUILayout.Label($"{((float)settings["persistentVars"]["windowColor"]["r"] * 100).Round(1f)}%");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("G", GUILayout.MaxWidth(15));

            GUILayout.BeginVertical();
            GUILayout.Space(9);
            settings["persistentVars"]["windowColor"]["g"] = GUILayout.HorizontalSlider((float)settings["persistentVars"]["windowColor"]["g"], 0, 1, GUILayout.Width(140));
            GUILayout.EndVertical();

            GUILayout.Label($"{((float)settings["persistentVars"]["windowColor"]["g"] * 100).Round(1f)}%");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("B", GUILayout.MaxWidth(15));

            GUILayout.BeginVertical();
            GUILayout.Space(9);
            settings["persistentVars"]["windowColor"]["b"] = GUILayout.HorizontalSlider((float)settings["persistentVars"]["windowColor"]["b"], 0, 1, GUILayout.Width(140));
            GUILayout.EndVertical();

            GUILayout.Label($"{((float)settings["persistentVars"]["windowColor"]["b"] * 100).Round(1f)}%");
            GUILayout.EndHorizontal();


            GUILayout.EndScrollView();

            if (GUILayout.Button("Default Settings"))
            {
                File.WriteAllText(configPath, defaultConfig.ToString());
                settings = defaultConfig;
            }

            GUI.DragWindow();

        }

        public void OnGUI()
        {
            if (WindowManager.inst == null) return;
            if (!showSettings) return;
            Rect oldRect = windowRect;
            windowColor.a = 1;
            GUI.color = windowColor;
            windowRect = GUI.Window(WindowManager.GetValidID(), windowRect, windowFunc, "VanillaUpgrades Config");
            windowColor = new Color((float)settings["persistentVars"]["windowColor"]["r"], (float)settings["persistentVars"]["windowColor"]["g"], (float)settings["persistentVars"]["windowColor"]["b"]);
            windowRect = WindowManager.ConfineRect(windowRect);
            if (oldRect != windowRect) WindowManager.settings["config"]["x"] = windowRect.x; WindowManager.settings["config"]["y"] = windowRect.y;
        }
    }
}
