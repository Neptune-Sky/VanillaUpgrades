using System;
using System.Collections.Generic;
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
    public class SettingsData
    {
        // random vars
        public PersistentVars persistentVars = new PersistentVars();
        public bool guiHidden;

        // GUI
        public Dictionary<string, Vector2> windowsSavedPosition = new Dictionary<string, Vector2>(); //name MUST be unique
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

    public static class Config
    {
        static readonly FilePath Path = Main.modFolder.ExtendToFile("Config2.txt");
        
        public static void Load()
        {
            if (!JsonWrapper.TryLoadJson(Path, out settingsData) && Path.FileExists())
            {
                ErrorNotification.Error("An error occured while trying to load Config, so it was reset to defaults.");
            }
            settingsData = settingsData ?? new SettingsData();
            Save();
        }

        public static SettingsData settingsData;

        public static void Save()
        {
            Path.WriteText(JsonWrapper.ToJson(settingsData, true));
        }
    }
}
