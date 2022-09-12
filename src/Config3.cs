using HarmonyLib;
using Newtonsoft.Json.Linq;
using SFS;
using SFS.Builds;
using SFS.UI;
using System;
using System.IO;
using SFS.IO;
using UnityEngine;

namespace VanillaUpgrades
{

    public class Config3 : MonoBehaviour
    {
        public static FilePath configPath = Main.modFolder.ExtendToFile("Config.txt");

        public static JObject defaultConfig = JObject.Parse("{ " +
            "persistentVars: { " +
            "opacity: 1, " +
            "windowColor: {" +
            "r: 1.0, " +
            "g: 1.0, " +
            "b: 1.0 " +
            "} " +
            "}, " +
            "guiHidden: false," +
            "showBuildGUI: true, " +
            "showAdvanced: true, " +
            "alwaysCompact: false, " +
            "showCalc: false, " +
            "showAverager: false, " +
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

        public static JObject settings2;

        public static bool showSettings;

        public bool gui = false;
        public bool units = false;
        public bool functions = false;
        public bool more = false;
        public bool cheats = false;
        public bool color = false;

        public static int test = 3;

        public static float windowHeight = 350f;

        public static Color windowColor;

        public Vector2 scroll = Vector2.zero;

        public Rect windowRect = new Rect((float)WindowManager2.settings["config"]["x"], (float)WindowManager2.settings["config"]["y"], 245f, windowHeight);

        public void Awake()
        {




            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, defaultConfig.ToString());
            }

            try
            {
                settings2 = JObject.Parse(File.ReadAllText(configPath));
            }
            catch (Exception)
            {
                File.WriteAllText(configPath, defaultConfig.ToString());
                ErrorNotification.Error("Config file was of an invalid format, and was reset to defaults.");
                settings2 = defaultConfig;
            }

            if (settings2["persistentVars"] == null) settings2["persistentVars"] = defaultConfig["persistentVars"];

            if (settings2["persistentVars"]["opacity"] == null) settings2["persistentVars"]["opacity"] = defaultConfig["persistentVars"]["opacity"];

            if (settings2["persistentVars"]["windowColor"] == null) settings2["persistentVars"]["windowColor"] = defaultConfig["persistentVars"]["windowColor"];

            try
            {
                Vector3 check = new Vector3((float)settings2["persistentVars"]["windowColor"]["r"], (float)settings2["persistentVars"]["windowColor"]["g"], (float)settings2["persistentVars"]["windowColor"]["b"]);
            }
            catch (Exception)
            {
                ErrorNotification.Error("Window color data was of an invalid format, and was reset to defaults.");
                settings2["persistentVars"]["windowColor"] = defaultConfig["persistentVars"]["windowColor"];
            }

            if (settings2["guiHidden"] == null) settings2["guiHidden"] = defaultConfig["guiHidden"];

            if (settings2["showBuildGUI"] == null) settings2["showBuildGUI"] = defaultConfig["showBuildGUI"];

            if (settings2["showAdvanced"] == null) settings2["showAdvanced"] = defaultConfig["showAdvanced"];

            if (settings2["alwaysCompact"] == null) settings2["alwaysCompact"] = defaultConfig["alwaysCompact"];

            if (settings2["showCalc"] == null) settings2["showCalc"] = defaultConfig["showCalc"];

            if (settings2["showAverager"] == null) settings2["showAverager"] = defaultConfig["showAverager"];

            if (settings2["showTime"] == null) settings2["showTime"] = defaultConfig["showTime"];

            if (settings2["worldTime"] == null) settings2["worldTime"] = defaultConfig["worldTime"];

            if (settings2["alwaysShowTime"] == null) settings2["alwaysShowTime"] = defaultConfig["alwaysShowTime"];

            if (settings2["mmUnits"] == null) settings2["mmUnits"] = defaultConfig["mmUnits"];

            if (settings2["kmsUnits"] == null) settings2["kmsUnits"] = defaultConfig["kmsUnits"];

            if (settings2["cUnits"] == null) settings2["cUnits"] = defaultConfig["cUnits"];

            if (settings2["ktUnits"] == null) settings2["ktUnits"] = defaultConfig["ktUnits"];

            if (settings2["shakeEffects"] == null) settings2["shakeEffects"] = defaultConfig["shakeEffects"];

            if (settings2["explosions"] == null) settings2["explosions"] = defaultConfig["explosions"];

            if (settings2["explosionShake"] == null) settings2["explosionShake"] = defaultConfig["explosionShake"];

            if (settings2["stopTimewarpOnEncounter"] == null) settings2["stopTimewarpOnEncounter"] = defaultConfig["stopTimewarpOnEncounter"];

            if (settings2["moreCameraZoom"] == null) settings2["moreCameraZoom"] = defaultConfig["moreCameraZoom"];

            if (settings2["moreCameraMove"] == null) settings2["moreCameraMove"] = defaultConfig["moreCameraMove"];

            if (settings2["allowTimeSlowdown"] == null) settings2["allowTimeSlowdown"] = defaultConfig["allowTimeSlowdown"];

            if (settings2["higherPhysicsWarp"] == null) settings2["higherPhysicsWarp"] = defaultConfig["higherPhysicsWarp"];

            windowColor = new Color((float)settings2["persistentVars"]["windowColor"]["r"], (float)settings2["persistentVars"]["windowColor"]["g"], (float)settings2["persistentVars"]["windowColor"]["b"], VideoSettingsPC.main.uiOpacitySlider.value);

            VideoSettingsPC.main.uiOpacitySlider.value = -1;
            VideoSettingsPC.main.uiOpacitySlider.value = (float)settings2["persistentVars"]["opacity"];
            settings2["guiHidden"] = false;

            File.WriteAllText(Config3.configPath, Config3.settings2.ToString());


        }
        public string Arrow(bool open)
        {
            if (open)
            {
                return "▼    ";
            }
            return "▶    ";
        }
        public void windowFunc(int windowID)
        {
            scroll = GUILayout.BeginScrollView(scroll);

            GUIStyle button = new GUIStyle(GUI.skin.button);

            button.alignment = TextAnchor.MiddleLeft;

            button.normal.textColor = Color.white;

            if (GUILayout.Button(Arrow(gui) + "GUI", button))
            {
                gui = !gui;
            }

            if (gui)
            {
                settings2["showBuildGUI"] = GUILayout.Toggle((bool)settings2["showBuildGUI"], " Show Build Settings");

                settings2["showAdvanced"] = GUILayout.Toggle((bool)settings2["showAdvanced"], " Show Advanced Info");

                if ((bool)settings2["showAdvanced"])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    settings2["alwaysCompact"] = GUILayout.Toggle((bool)settings2["alwaysCompact"], " Force Compact Mode");
                    GUILayout.EndHorizontal();
                }

                settings2["showCalc"] = GUILayout.Toggle((bool)settings2["showCalc"], " ΔV Calc Default");

                if ((bool)settings2["showCalc"])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    settings2["showAverager"] = GUILayout.Toggle((bool)settings2["showAverager"], " ISP Averager Default");
                    GUILayout.EndHorizontal();
                }
                else
                {
                    settings2["showAverager"] = false;
                }

                settings2["showTime"] = GUILayout.Toggle((bool)settings2["showTime"], " World Clock During Timewarp");

                if ((bool)settings2["showTime"])
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    settings2["worldTime"] = GUILayout.Toggle((bool)settings2["worldTime"], " World Time In World Clock");
                    GUILayout.EndHorizontal();
                }

                if (!(bool)settings2["showTime"] || !(bool)settings2["worldTime"])
                {
                    settings2["alwaysShowTime"] = false;
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    settings2["alwaysShowTime"] = GUILayout.Toggle((bool)settings2["alwaysShowTime"], " Always Show World Time");
                    GUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button(Arrow(units) + "Units", button))
            {
                units = !units;
            }

            if (units)
            {

                settings2["mmUnits"] = GUILayout.Toggle((bool)settings2["mmUnits"], " Megameters (Mm)");

                settings2["kmsUnits"] = GUILayout.Toggle((bool)settings2["kmsUnits"], " Kilometers per Second (km/s)");

                settings2["cUnits"] = GUILayout.Toggle((bool)settings2["cUnits"], " % Speed of Light (c)");

                settings2["ktUnits"] = GUILayout.Toggle((bool)settings2["ktUnits"], " Kilotonnes (kt)");
            }

            if (GUILayout.Button(Arrow(functions) + "Functions", button))
            {
                functions = !functions;
            }

            if (functions)
            {
                settings2["stopTimewarpOnEncounter"] = GUILayout.Toggle((bool)settings2["stopTimewarpOnEncounter"], " Stop Timewarp on Encounter");

                settings2["moreCameraZoom"] = GUILayout.Toggle((bool)settings2["moreCameraZoom"], " Camera Zoom Increase");

                settings2["moreCameraMove"] = GUILayout.Toggle((bool)settings2["moreCameraMove"], " Camera Movement Increase");
            }

            if (GUILayout.Button(Arrow(cheats) + "Cheats", button))
            {
                cheats = !cheats;
            }

            if (cheats)
            {
                settings2["allowTimeSlowdown"] = GUILayout.Toggle((bool)settings2["allowTimeSlowdown"], " Allow Time Slowdown");

                settings2["higherPhysicsWarp"] = GUILayout.Toggle((bool)settings2["higherPhysicsWarp"], " Higher Physics Timewarps");
            }


            if (GUILayout.Button(Arrow(more) + "Additional Settings", button))
            {
                more = !more;
            }

            if (more)
            {
                settings2["shakeEffects"] = GUILayout.Toggle((bool)settings2["shakeEffects"], " Toggle Shake Effects");

                if (!(bool)settings2["shakeEffects"])
                {
                    settings2["explosionShake"] = false;
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    settings2["explosionShake"] = GUILayout.Toggle((bool)settings2["explosionShake"], " Toggle Explosion Shake");
                    GUILayout.EndHorizontal();
                }

                settings2["explosions"] = GUILayout.Toggle((bool)settings2["explosions"], " Toggle Explosion Effects");
            }


            if (GUILayout.Button(Arrow(color) + "Window Color", button))
            {
                color = !color;
            }

            if (color)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("R", GUILayout.MaxWidth(15));

                GUILayout.BeginVertical();
                GUILayout.Space(9);
                settings2["persistentVars"]["windowColor"]["r"] = GUILayout.HorizontalSlider((float)settings2["persistentVars"]["windowColor"]["r"], 0, 1, GUILayout.Width(140));
                GUILayout.EndVertical();

                GUILayout.Label($"{((float)settings2["persistentVars"]["windowColor"]["r"] * 100).Round(1f)}%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("G", GUILayout.MaxWidth(15));

                GUILayout.BeginVertical();
                GUILayout.Space(9);
                settings2["persistentVars"]["windowColor"]["g"] = GUILayout.HorizontalSlider((float)settings2["persistentVars"]["windowColor"]["g"], 0, 1, GUILayout.Width(140));
                GUILayout.EndVertical();

                GUILayout.Label($"{((float)settings2["persistentVars"]["windowColor"]["g"] * 100).Round(1f)}%");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("B", GUILayout.MaxWidth(15));

                GUILayout.BeginVertical();
                GUILayout.Space(9);
                settings2["persistentVars"]["windowColor"]["b"] = GUILayout.HorizontalSlider((float)settings2["persistentVars"]["windowColor"]["b"], 0, 1, GUILayout.Width(140));
                GUILayout.EndVertical();

                GUILayout.Label($"{((float)settings2["persistentVars"]["windowColor"]["b"] * 100).Round(1f)}%");
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("Default Settings"))
            {
                File.WriteAllText(configPath, defaultConfig.ToString());
                settings2 = defaultConfig;
            }

            GUI.DragWindow();

        }

        public void OnGUI()
        {
            if (WindowManager2.inst == null) return;
            if (!showSettings) return;
            Rect oldRect = windowRect;
            windowColor.a = 1;
            GUI.color = windowColor;
            windowRect = GUI.Window(WindowManager2.GetValidID(), windowRect, windowFunc, "VanillaUpgrades Config");
            windowColor = new Color((float)settings2["persistentVars"]["windowColor"]["r"], (float)settings2["persistentVars"]["windowColor"]["g"], (float)settings2["persistentVars"]["windowColor"]["b"]);
            windowRect = WindowManager2.ConfineRect(windowRect);
            if (oldRect != windowRect) WindowManager2.settings["config"]["x"] = windowRect.x; WindowManager2.settings["config"]["y"] = windowRect.y;
        }
    }
}
