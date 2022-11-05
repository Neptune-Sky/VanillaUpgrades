using SFS.IO;
using SFS.Parsers.Json;
using SFS.Variables;

namespace VanillaUpgrades;

[Serializable]
public class PersistentVars
{
    public float opacity = 1;
    public Color windowColor = new(0.007843138f, 0.09019608f, 0.18039216f, 1f);
    public Float_Local windowScale = new() { Value = 1 };
}

[Serializable]
public class SettingsData
{
    // random vars
    public PersistentVars persistentVars = new();
    public bool guiHidden;

    public Bool_Local showBuildGui = new() { Value = true };
    public Bool_Local showAdvanced = new() { Value = true };
    public Bool_Local horizontalMode = new();
    public bool showCalc;
    public bool showAverager;
    public Bool_Local showTime = new() { Value = true };
    public Bool_Local showWorldTime = new() { Value = true };
    public Bool_Local alwaysShowTime = new();

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
    public Bool_Local allowTimeSlowdown = new();
    public bool higherPhysicsWarp;

    // GUI
    public Dictionary<string, Vector2Int>
        windowsSavedPosition = new(); //name MUST be unique
}

public static class Config
{
    static readonly FilePath Path = Main.modFolder.ExtendToFile("Config.txt");

    public static SettingsData settingsData;

    public static void Load()
    {
        if (!JsonWrapper.TryLoadJson(Path, out settingsData) && Path.FileExists())
            ErrorNotification.Error("An error occured while trying to load Config, so it was reset to defaults.");

        settingsData = settingsData ?? new SettingsData();
        Save();
    }

    public static void Save()
    {
        if (settingsData == null)
            Load();
        Path.WriteText(JsonWrapper.ToJson(settingsData, true));
    }
}