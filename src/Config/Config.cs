﻿using System;
using SFS.IO;
using SFS.Variables;
using UITools;
using UnityEngine;

namespace VanillaUpgrades
{
    [Serializable]
    public class PersistentVars
    {
        public Float_Local windowScale = new() { Value = 1 };
        public float minutesUntilAutosave = 10f;
        public int allowedAutosaveSlots = 5;
    }

    [Serializable]
    public class SettingsData
    {
        // random vars
        public PersistentVars persistentVars = new();
        
        // UI
        public Bool_Local showBuildGui = new() { Value = true };
        public Bool_Local showAdvanced = new() { Value = true };
        public Bool_Local showAdvancedInSeparateWindow = new();
        public Bool_Local horizontalMode = new();
        public Bool_Local showTime = new() { Value = true };
        public Bool_Local showWorldTime = new() { Value = true };
        public Bool_Local alwaysShowTime = new();

        // Units
        public bool lyUnits = true;
        public bool gmUnits = true;
        public bool mmUnits = true;
        public bool kmsUnits = true;
        public bool cUnits = true;
        public bool ktUnits = true;

        // Misc
        public bool explosions = true;
        public bool morePrecisePercentages = true;
        public bool darkenDebris = true;
        public bool stopTimewarpOnEncounter = true;
        public bool moreCameraZoom = true;
        public bool moreCameraMove = true;
        public Bool_Local allowBackgroundProcess = new();

        // Cheats
        public Bool_Local allowTimeSlowdown = new();
        public bool higherPhysicsWarp;
        
        
    }

    public class Config : ModSettings<SettingsData>
    {
        private static Config main;

        private Action saveAction;

        protected override FilePath SettingsFile { get; } = Main.modFolder.ExtendToFile("Config.txt");

        public static void Load()
        {
            main = new Config();
            main.Initialize();
        }

        public static void Save()
        {
            main.saveAction?.Invoke();
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            saveAction = onChange;
            Application.quitting += onChange;
        }
    }
}

