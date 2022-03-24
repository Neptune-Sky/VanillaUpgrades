using HarmonyLib;
using SFS.Audio;
using SFS;
using SFS.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFS.Builds;
using System.Reflection;

namespace ASoD_s_VanillaUpgrades
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
            "mmUnits: true, " +
            "kmsUnits: true, " +
            "cUnits: true, " +
            "ktUnits: true, " +
            "stopTimewarpOnEncounter: true, " +
            "moreCameraZoom: true, " +
            "moreCameraMove: true, " +
            "allowTimeSlowdown: false, " +
            "higherPhysicsWarp: false }");

        public static JObject settings;

        public static bool showSettings;
        
        public static float windowHeight = 300f;

        public static Color windowColor;

        public Vector2 scroll = Vector2.zero;

        public Rect windowRect = new Rect((float)WindowManager.settings["config"]["x"], (float)WindowManager.settings["config"]["y"], 235f, windowHeight);

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
            } catch (Exception e)
            {
                ErrorNotification.Error("Window color data was of an invalid format, and was reset to defaults.");
                settings["persistentVars"]["windowColor"] = defaultConfig["persistentVars"]["windowColor"];
            }

            if (settings["showBuildGUI"] == null) settings["showBuildGUI"] = defaultConfig["showBuildGUI"];

            if (settings["showAdvanced"] == null) settings["showAdvanced"] = defaultConfig["showAdvanced"];

            if (settings["mmUnits"] == null) settings["mmUnits"] = defaultConfig["mmUnits"];

            if (settings["kmsUnits"] == null) settings["kmsUnits"] = defaultConfig["kmsUnits"];

            if (settings["cUnits"] == null) settings["cUnits"] = defaultConfig["cUnits"];

            if (settings["ktUnits"] == null) settings["ktUnits"] = defaultConfig["ktUnits"];

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
            scroll = GUI.BeginScrollView(new Rect(0, 20, windowRect.width - 2, windowHeight - 50), scroll, new Rect(0, 0, windowRect.width - 20, 430));

            GUI.Label(new Rect(15f, 0f, 110f, 20f), "GUI:");

            settings["showBuildGUI"] = GUI.Toggle(new Rect(15f, 20f, 210f, 20f), (bool)settings["showBuildGUI"], " Show Build Settings");

            settings["showAdvanced"] = GUI.Toggle(new Rect(15f, 40f, 190f, 20f), (bool)settings["showAdvanced"], " Show Advanced Info");

            GUI.Label(new Rect(15f, 70f, 210f, 20f), "Units:");

            settings["mmUnits"] = GUI.Toggle(new Rect(15f, 90f, 210f, 20f), (bool)settings["mmUnits"], " Megameters (Mm)");

            settings["kmsUnits"] = GUI.Toggle(new Rect(15f, 110f, 210f, 20f), (bool)settings["kmsUnits"], " Kilometers per Second (km/s)");

            settings["cUnits"] = GUI.Toggle(new Rect(15f, 130f, 210f, 20f), (bool)settings["cUnits"], " % Speed of Light (c)");

            settings["ktUnits"] = GUI.Toggle(new Rect(15f, 150f, 210f, 20f), (bool)settings["ktUnits"], " Kilotonnes (kt)");

            GUI.Label(new Rect(15f, 180f, 210f, 20f), "Functions:");

            settings["stopTimewarpOnEncounter"] = GUI.Toggle(new Rect(15f, 200f, 210f, 20f), (bool)settings["stopTimewarpOnEncounter"], " Stop Timewarp on Encounter");

            settings["moreCameraZoom"] = GUI.Toggle(new Rect(15f, 220f, 210f, 20f), (bool)settings["moreCameraZoom"], " Camera Zoom Increase");

            settings["moreCameraMove"] = GUI.Toggle(new Rect(15f, 240f, 210f, 20f), (bool)settings["moreCameraMove"], " Camera Movement Increase");

            GUI.Label(new Rect(15f, 270f, 210f, 20f), "\"Cheaty\" Functions:");

            settings["allowTimeSlowdown"] = GUI.Toggle(new Rect(15f, 290f, 210f, 20f), (bool)settings["allowTimeSlowdown"], " Allow Time Slowdown");

            settings["higherPhysicsWarp"] = GUI.Toggle(new Rect(15f, 310f, 210f, 20f), (bool)settings["higherPhysicsWarp"], " Higher Physics Timewarps");

            GUI.Label(new Rect(15f, 340f, 210f, 20f), "Window Color:");

            GUI.Label(new Rect(15f, 365f, 210f, 20f), "R");
            settings["persistentVars"]["windowColor"]["r"] = GUI.HorizontalSlider(new Rect(30f, 370f, 140f, 20f), (float)settings["persistentVars"]["windowColor"]["r"], 0, 1);
            GUI.Label(new Rect(175f, 365f, 210f, 20f), $"{((float)settings["persistentVars"]["windowColor"]["r"] * 100).Round(1f)}%");

            GUI.Label(new Rect(15f, 385f, 210f, 20f), "G");
            settings["persistentVars"]["windowColor"]["g"] = GUI.HorizontalSlider(new Rect(30f, 390f, 140f, 20f), (float)settings["persistentVars"]["windowColor"]["g"], 0, 1);
            GUI.Label(new Rect(175f, 385f, 210f, 20f), $"{((float)settings["persistentVars"]["windowColor"]["g"] * 100).Round(1f)}%");

            GUI.Label(new Rect(15f, 405f, 210f, 20f), "B");
            settings["persistentVars"]["windowColor"]["b"] = GUI.HorizontalSlider(new Rect(30f, 410f, 140f, 20f), (float)settings["persistentVars"]["windowColor"]["b"], 0, 1);
            GUI.Label(new Rect(175f, 405f, 210f, 20f), $"{((float)settings["persistentVars"]["windowColor"]["b"] * 100).Round(1f)}%");


            GUI.EndScrollView();

            if (GUI.Button(new Rect(25f, windowHeight - 30, 180f, 25f), "Default Settings"))
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
