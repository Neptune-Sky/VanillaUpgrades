using Newtonsoft.Json.Linq;
using SFS.IO;
using System;
using System.IO;
using UnityEngine;
using SFS.UI.ModGUI;
using SFS.Parsers.Json;
using UnityEngine.UI;

namespace VanillaUpgrades
{
    [Serializable]
    public class Windows
    {
        public Vector2 buildSettings = new Vector2(Screen.width * 0.05f, Screen.height * 0.94f);

        public Vector2 advancedInfo = new Vector2(0f, Screen.height * 0.05f);

        public Vector2 config = new Vector2(300, 1000);

        public Vector2 worldTime = new Vector2(Screen.width * 0.958f, Screen.height * 0.045f);

        public Vector2 calc = new Vector2(Screen.width - 175f, 50f);

        public Vector2 averager = new Vector2(Screen.width - 175f, 270f);
    }


    public class WindowManager : MonoBehaviour
    {
        public static Vector2 gameSize;

        public static Windows windows;

        public static FilePath windowPath = Main.modFolder.ExtendToFile("WindowPositions.txt");

        GameObject sizeTracking;

        void Awake()
        {
            sizeTracking = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "sizeTracking");
        }

        void Update()
        {
            gameSize = new Vector2((sizeTracking.GetComponentInParent<CanvasScaler>().referenceResolution.y / Screen.height) * Screen.width, sizeTracking.GetComponentInParent<CanvasScaler>().referenceResolution.y);
        }

        public static void ClampWindow(Window window)
        {
            Vector2 pos = window.Position;
            Vector2 size = window.Size;

            size.x = size.x * Config.settingsData.persistentVars.windowScale;
            size.y = size.y * Config.settingsData.persistentVars.windowScale;

            pos.x = Mathf.Clamp(pos.x, size.x / 2, gameSize.x - size.x / 2);
            pos.y = Mathf.Clamp(pos.y, size.y, gameSize.y);
            window.Position = pos;
        }

        public static void Save(string key, Window window)
        {
            if (Config.settingsData.windowsSavedPosition.ContainsKey(key))
                Config.settingsData.windowsSavedPosition[key] = window.Position;
            else
                Config.settingsData.windowsSavedPosition.Add(key, window.Position);
        }
    }


    public class WindowManager2 : MonoBehaviour
    {
        public Vector2 defaultBuildSettings = new Vector2(Screen.width * 0.05f, Screen.height * 0.94f);

        public Vector2 defaultAdvancedInfo = new Vector2(0f, Screen.height * 0.05f);

        public Vector2 defaultWorldTime = new Vector2(Screen.width * 0.958f, Screen.height * 0.045f);

        public Vector2 defaultCalc = new Vector2(Screen.width - 175f, 50f);

        public Vector2 defaultAverager = new Vector2(Screen.width - 175f, 270f);

        public static Vector2 scale = new Vector2(1080f / Screen.currentResolution.height, 1920f / Screen.currentResolution.width);

        public JObject defaults = JObject.Parse("{buildSettings:{}, advancedInfo:{}, config:{}, worldTime:{}, dvCalc: {}, ispAverager: {}}");

        public static JObject settings;

        public string windowDir = Main.modFolder.ExtendToFile("WindowPositions.txt");

        public static WindowManager2 inst;

        // Keeps windows from being moved off the screen.
        public static Rect ConfineRect(Rect window)
        {
            window.x = Mathf.Clamp(window.x, -7, Screen.width - window.width + 7);
            window.y = Mathf.Clamp(window.y, 0, Screen.height - window.height + 7);
            return window;
        }

        // Returns a window ID that will not conflict with other windows.
        public static int GetValidID()
        {
            return GUIUtility.GetControlID(FocusType.Passive);
        }

        public void Update()
        {
            scale = new Vector2((float)(Screen.currentResolution.width / 1920f), (float)(Screen.currentResolution.height / 1080f));

            if (scale.x < 1) scale.x = scale.x * 1.15f;
            if (scale.y < 1) scale.y = scale.y * 1.15f;

            if (scale.y > 1) scale.y = 1;
            if (scale.x > 1) scale.x = 1;

            if (scale.x < scale.y) scale.x = scale.y;
        }

        public void Awake()
        {
            defaults["buildSettings"]["x"] = defaultBuildSettings.x;
            defaults["buildSettings"]["y"] = defaultBuildSettings.y;
            defaults["advancedInfo"]["x"] = defaultAdvancedInfo.x;
            defaults["advancedInfo"]["y"] = defaultAdvancedInfo.y;
            defaults["worldTime"]["x"] = defaultWorldTime.x;
            defaults["worldTime"]["y"] = defaultWorldTime.y;
            defaults["dvCalc"]["x"] = defaultCalc.x;
            defaults["dvCalc"]["y"] = defaultCalc.y;
            defaults["ispAverager"]["x"] = defaultAverager.x;
            defaults["ispAverager"]["y"] = defaultAverager.y;

            if (!File.Exists(windowDir))
            {
                File.WriteAllText(windowDir, defaults.ToString());
            }

            try
            {
                settings = JObject.Parse(File.ReadAllText(windowDir));
                Vector2 check;
                check = new Vector2((float)settings["buildSettings"]["x"], (float)settings["buildSettings"]["y"]);
                check = new Vector2((float)settings["advancedInfo"]["x"], (float)settings["advancedInfo"]["y"]);
                check = new Vector2((float)settings["config"]["x"], (float)settings["config"]["y"]);
                check = new Vector2((float)settings["worldTime"]["x"], (float)settings["worldTime"]["y"]);
                check = new Vector2((float)settings["dvCalc"]["x"], (float)settings["dvCalc"]["y"]);
                check = new Vector2((float)settings["ispAverager"]["x"], (float)settings["ispAverager"]["y"]);

                /*
                if (settings["buildSettings"] == null)
                {
                    settings["buildSettings"]["x"] = defaultBuildSettings.x;
                    settings["buildSettings"]["y"] = defaultBuildSettings.y;
                }
                if (settings["advancedInfo"] == null)
                {
                    settings["advancedInfo"]["x"] = defaultAdvancedInfo.x;
                    settings["advancedInfo"]["y"] = defaultAdvancedInfo.y;
                }
                if (settings["config"] == null)
                {
                    settings["config"] = defaultConfig.x;
                    settings["config"]["y"] = defaultConfig.y;
                }
                if (settings["worldTime"] == null)
                {
                    settings["worldTime"]["x"] = defaultWorldTime.x;
                    settings["worldTime"]["y"] = defaultWorldTime.y;
                }
                */
            }
            catch (Exception)
            {
                File.WriteAllText(windowDir, defaults.ToString());
                // ErrorNotification.Error("Window positions file was of an invalid format, and was reset to defaults.");
                settings = defaults;
            }

            inst = this;
        }
    }
}
