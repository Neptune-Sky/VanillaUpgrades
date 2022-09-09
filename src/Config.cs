using System;
using System.Collections.Generic;
using SFS.IO;
using UnityEngine;
using SFS.Parsers.Json;
using SFS.Variables;
using SFS.UI.ModGUI;

namespace VanillaUpgrades
{
    [Serializable]
    public class PersistentVars
    {
        public float opacity = 1;
        public Color windowColor = new Color(0.007843138f, 0.09019608f, 0.18039216f, 1f);
        public Float_Local windowScale = new Float_Local { Value = 1 };
    }
    [Serializable]
    public class SettingsData
    {
        // random vars
        public PersistentVars persistentVars = new PersistentVars();
        public bool guiHidden;

        // GUI
        public Dictionary<string, Vector2> windowsSavedPosition = new Dictionary<string, Vector2>(); //name MUST be unique

        public Bool_Local showBuildGui = new Bool_Local { Value = true };
        public Bool_Local showAdvanced = new Bool_Local { Value = true };
        public Bool_Local horizontalMode = new Bool_Local();
        public bool showCalc;
        public bool showAverager;
        public Bool_Local showTime = new Bool_Local { Value = true };
        public Bool_Local worldTime = new Bool_Local { Value = true };
        public Bool_Local alwaysShowTime = new Bool_Local();

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
        public Bool_Local allowTimeSlowdown = new Bool_Local();
        public bool higherPhysicsWarp;
    }

    public static class Config
    {
        static readonly FilePath Path = Main.modFolder.ExtendToFile("Config.txt");
        
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
