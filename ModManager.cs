using HarmonyLib;
using SFS.Audio;
using SFS.UI;
using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnOpen))]
    public class ShowSettings
    {
        [HarmonyPostfix]
        public static void Postfix(BasicMenu __instance)
        {
            if (__instance.GetComponent<AudioSettings>())
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
        }

    }

    public class Config : MonoBehaviour
    {
        public static string configPath = Main.modDir + "Config.txt";

        public static JObject defaultConfig = JObject.Parse("{ persistentVars: { opacity: 0 }, showBuildGUI: true, showAdvanced: true, mmUnits: true, kmsUnits: true, ktUnits: true, stopTimewarpOnEncounter: true, moreCameraZoom: true, moreCameraMove: true, allowTimeSlowdown: false, higherPhysicsWarp: false }");
        public static JObject settings;

        public static bool showSettings;
        
        static float windowHeight = 370f;
        public Rect windowRect = new Rect(10f, (float)Screen.height - windowHeight, 230f, windowHeight);

        public static GUIStyle title = new GUIStyle();
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
                Debug.LogWarning("VanillaUpgrades config format found to be invalid, resetting to default.");
                settings = defaultConfig;
            }
            if (settings["persistentVars"] == null) settings["persistentVars"] = defaultConfig["persistentVars"];

            if (settings["persistentVars"]["opacity"] == null) settings["persistentVars"]["opacity"] = defaultConfig["persistentVars"]["opacity"];

            if (settings["showBuildGUI"] == null) settings["showBuildGUI"] = defaultConfig["showBuildGUI"];

            if (settings["showAdvanced"] == null) settings["showAdvanced"] = defaultConfig["showAdvanced"];

            if (settings["mmUnits"] == null) settings["mmUnits"] = defaultConfig["mmUnits"];

            if (settings["kmsUnits"] == null) settings["kmsUnits"] = defaultConfig["kmsUnits"];

            if (settings["ktUnits"] == null) settings["ktUnits"] = defaultConfig["ktUnits"];

            if (settings["stopTimewarpOnEncounter"] == null) settings["stopTimewarpOnEncounter"] = defaultConfig["stopTimewarpOnEncounter"];

            if (settings["moreCameraZoom"] == null) settings["moreCameraZoom"] = defaultConfig["moreCameraZoom"];

            if (settings["moreCameraMove"] == null) settings["moreCameraMove"] = defaultConfig["moreCameraMove"];

            if (settings["allowTimeSlowdown"] == null) settings["allowTimeSlowdown"] = defaultConfig["allowTimeSlowdown"];

            if (settings["higherPhysicsWarp"] == null) settings["higherPhysicsWarp"] = defaultConfig["higherPhysicsWarp"];

            File.WriteAllText(Config.configPath, Config.settings.ToString());
        }

        public void windowFunc(int windowID)
        {

            GUI.Label(new Rect(20f, 20f, 110f, 20f), "GUI:");

            settings["showBuildGUI"] = GUI.Toggle(new Rect(20f, 40f, 210f, 20f), (bool)settings["showBuildGUI"], " Show Build Settings");

            settings["showAdvanced"] = GUI.Toggle(new Rect(20f, 60f, 190f, 20f), (bool)settings["showAdvanced"], " Show Advanced Info");

            GUI.Label(new Rect(20f, 90f, 210f, 20f), "Units:");

            settings["mmUnits"] = GUI.Toggle(new Rect(20f, 110f, 210f, 20f), (bool)settings["mmUnits"], " Megameters (Mm)");

            settings["kmsUnits"] = GUI.Toggle(new Rect(20f, 130f, 210f, 20f), (bool)settings["kmsUnits"], " Kilometers per Second (km/s)");

            settings["ktUnits"] = GUI.Toggle(new Rect(20f, 150f, 210f, 20f), (bool)settings["ktUnits"], " Kilotonnes (kt)");

            GUI.Label(new Rect(20f, 180f, 210f, 20f), "Functions:");

            settings["stopTimewarpOnEncounter"] = GUI.Toggle(new Rect(20f, 200f, 210f, 20f), (bool)settings["stopTimewarpOnEncounter"], " Stop Timewarp on Encounter");

            settings["moreCameraZoom"] = GUI.Toggle(new Rect(20f, 220f, 210f, 20f), (bool)settings["moreCameraZoom"], " Camera Zoom Increase");

            settings["moreCameraMove"] = GUI.Toggle(new Rect(20f, 240f, 210f, 20f), (bool)settings["moreCameraMove"], " Camera Movement Increase");

            GUI.Label(new Rect(20f, 270f, 210f, 20f), "\"Cheaty\" Functions:");

            settings["allowTimeSlowdown"] = GUI.Toggle(new Rect(20f, 290f, 210f, 20f), (bool)settings["allowTimeSlowdown"], " Allow Time Slowdown");

            settings["higherPhysicsWarp"] = GUI.Toggle(new Rect(20f, 310f, 210f, 20f), (bool)settings["higherPhysicsWarp"], " Higher Physics Timewarps");

            if (GUI.Button(new Rect(25f, windowHeight - 30, 180f, 20f), "Defaults"))
            {
                File.WriteAllText(configPath, defaultConfig.ToString());
                settings = defaultConfig;
            }

            GUI.DragWindow();

        }

        public void OnGUI()
        {

            if (!showSettings) return;

            windowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), windowRect, windowFunc, "VanillaUpgrades Config");
        }
    }
}
