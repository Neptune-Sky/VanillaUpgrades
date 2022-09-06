using System;
using System.IO;
using SFS.IO;
using UnityEngine;
using ModLoader;
using SFS.UI.ModGUI;
using SFS.Parsers.Json;
using SFS.UI;

namespace VanillaUpgrades
{
    [Serializable]
    public class PersistentVars
    {
        public float opacity = 1;
        public Color windowColor = new Color(0.007843138f, 0.09019608f, 0.18039216f, 1f);
        public float windowScale = 1;
    }
    [Serializable]
    public class Settings
    {
        // random vars
        public PersistentVars persistentVars = new PersistentVars();
        public bool guiHidden;

        // GUI
        public bool showBuildGui = true;
        public bool showAdvanced = true;
        public bool alwaysCompact;
        public bool showCalc;
        public bool showAverager;
        public bool showTime = true;
        public bool worldTime = true;
        public bool alwaysShowTime;

        // Units
        public bool mmUnits = true;
        public bool kmsUnits = true;
        public bool cUnits = true;
        public bool ktUnits = true;

        // Misc
        public bool explosions = true;
        public bool explosionShake = true;
        public bool stopTimewarpOnEncounter = true;
        public bool moreCameraZoom = true;
        public bool moreCameraMove = true;

        // Cheats
        public bool allowTimeSlowdown;
        public bool higherPhysicsWarp;
    }
    public class Config : MonoBehaviour
    {
        private const int normalToggle = 575;
        private const int toggleHeight = 42;
        private const int subToggle = 575;

        public static Settings settings;
        public static GameObject configHolder;
        public static Window settingsWindow;
        public static Box settingsHolder;

        public static FilePath configPath = Main.modFolder.ExtendToFile("Config2.txt");


        void Awake()
        {
            configHolder = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "ConfigHolder");
            var rectTransform = configHolder.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.position = Vector2.zero;
            DontDestroyOnLoad(configHolder.gameObject);

            if (!JsonWrapper.TryLoadJson(configPath, out settings) && File.Exists(configPath))
            {
                ErrorNotification.Error("An error occured while trying to load Config, so it was reset to defaults.");
            }
            settings = settings ?? new Settings();
            Save();
            ShowGUI(true);


        }

        public static void ShowGUI(bool init)
        {
            settingsWindow = Builder.CreateWindow(configHolder.gameObject.transform, Builder.GetRandomID(), 750, 670, 320, 320, true, true, 1, "VanillaUpgrades Config");
            settingsWindow.CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal, TextAnchor.UpperCenter);
            settingsWindow.gameObject.GetComponent<DraggableWindowModule>().OnDropAction += OnDragDrop;

            Builder.CreateSpace(settingsWindow, 0, 0);
            Container categories = Builder.CreateContainer(settingsWindow);
            categories.CreateLayoutGroup(SFS.UI.ModGUI.Type.Vertical, spacing: 10);

            Builder.CreateSpace(categories, 0, 10);
            Builder.CreateButton(categories, 100, 50, 0, 0, () => RefreshGUI("gui"), "GUI");
            Builder.CreateButton(categories, 100, 50, 0, 0, () => RefreshGUI("units"), "Units");
            Builder.CreateButton(categories, 100, 50, 0, 0, () => RefreshGUI("misc"), "Misc");
            Builder.CreateButton(categories, 100, 50, 0, 0, () => RefreshGUI("cheats"), "Cheats");
            RefreshGUI("gui", init);

            settingsWindow.rectTransform.localScale = new Vector2(settings.persistentVars.windowScale, settings.persistentVars.windowScale);
        }

        public static void RefreshGUI(string options, bool init = false)
        {
            if (!init)
            {
                Destroy(settingsHolder.gameObject);
                settingsHolder = null;
            }
            
            settingsHolder = Builder.CreateBox(settingsWindow, 600, 600, 0, 0, 0.7f);
            settingsHolder.CreateLayoutGroup(SFS.UI.ModGUI.Type.Vertical, spacing: 0, childAlignment: TextAnchor.UpperCenter);

            switch (options)
            {
                case "gui":
                    Builder.CreateLabel(settingsHolder, 300, 50, 0, 0, "GUI");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.showBuildGui, () => settings.showBuildGui = !settings.showBuildGui, 0, 0, 
                    "Show Build Settings");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.showAdvanced, () => settings.showAdvanced = !settings.showAdvanced, 0, 0,
                    "Show Advanced Info");
                    Space(false);
                    SubToggle(settingsHolder, subToggle, toggleHeight, () => settings.alwaysCompact, () => settings.alwaysCompact = !settings.alwaysCompact, 0, 0,
                    "    Force Compact Mode");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.showCalc, () => settings.showCalc = !settings.showCalc, 0, 0,
                    "Show dV Calc by Default");
                    Space(false);

                    SubToggle(settingsHolder, subToggle, toggleHeight, () => settings.showAverager, () => settings.showAverager = !settings.showAverager, 0, 0,
                    "    Averager Default");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.showTime, () => settings.showTime = !settings.showTime, 0, 0,
                    "Show World Clock While Timewarping");
                    Space(false);
                    SubToggle(settingsHolder, subToggle, toggleHeight, () => settings.worldTime, () => settings.worldTime = !settings.worldTime, 0, 0,
                    "    Show World Time in Clock");
                    Space(false);
                    SubToggle(settingsHolder, subToggle, toggleHeight, () => settings.alwaysShowTime, () => settings.alwaysShowTime = !settings.alwaysShowTime, 0, 0,
                    "    Always Show World Clock");
                    break;
                case "units":
                    Builder.CreateLabel(settingsHolder, 300, 50, 0, 0, "Units");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.mmUnits, () => settings.mmUnits = !settings.mmUnits, 0, 0,
                    "Megameters (Mm)");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.kmsUnits, () => settings.kmsUnits = !settings.kmsUnits, 0, 0,
                    "Kilometers/Second (km/s)");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.cUnits, () => settings.cUnits = !settings.cUnits, 0, 0,
                    "% Speed of Light (c)");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.ktUnits, () => settings.ktUnits = !settings.ktUnits, 0, 0,
                    "Kilotonnes (kt)");
                    break;
                case "misc":
                    Builder.CreateLabel(settingsHolder, 300, 50, 0, 0, "Miscellaneous");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.explosions, () => settings.explosions = !settings.explosions, 0, 0,
                    "Explosion Effects");
                    Space(false);
                    SubToggle(settingsHolder, subToggle, toggleHeight, () => settings.explosionShake, () => settings.explosionShake = !settings.explosionShake, 0, 0,
                    "    Explosion Shake");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.stopTimewarpOnEncounter, () => settings.stopTimewarpOnEncounter = !settings.stopTimewarpOnEncounter, 0, 0,
                    "Stop Timewarp On Encounter");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.moreCameraZoom, () => settings.moreCameraZoom = !settings.moreCameraZoom, 0, 0,
                    "More Camera Zoom");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.moreCameraMove, () => settings.moreCameraMove = !settings.moreCameraMove, 0, 0,
                    "More Camera Movement");
                    break;
                case "cheats":
                    Builder.CreateLabel(settingsHolder, 300, 50, 0, 0, "Cheats");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.allowTimeSlowdown, () => settings.allowTimeSlowdown = !settings.allowTimeSlowdown, 0, 0,
                    "Allow Time Slowdown");
                    Space(true);
                    Builder.CreateToggleWithLabel(settingsHolder, normalToggle, toggleHeight, () => settings.higherPhysicsWarp, () => settings.higherPhysicsWarp = !settings.higherPhysicsWarp, 0, 0,
                    "Higher Physics Timewarps");
                    break;
            }
        }

        public static void Space(bool more)
        {
            if (more)
            {
                Builder.CreateSpace(settingsHolder, 30, 30);
                return;
            }
            Builder.CreateSpace(settingsHolder, 15, 15);
        }

        public static void SubToggle(Transform parent, int width, int height, Func<bool> getToggleValue, Action onChange, int posX, int posY, string label)
        {
            Container container = Builder.CreateContainer(settingsHolder);
            container.CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal);

            Builder.CreateSpace(container, 55, 0);
            Builder.CreateToggleWithLabel(parent, width, height, getToggleValue, onChange, posX, posY, label);

        }

        public static void Save()
        {
            JsonWrapper.SaveAsJson(configPath, settings, true);
        }

        static void OnDragDrop()
        {
            WindowManager.windows.config = settingsWindow.Position;
            WindowManager.ClampWindow(settingsWindow);
        }
    }
}
