using SFS.IO;
using Console = ModLoader.IO.Console;
using Object = UnityEngine.Object;

namespace VanillaUpgrades;

public class Main : Mod
{
    public static Main main;

    public static bool menuOpen;

    public static bool buildSettingsPresent;

    public static GameObject mainObject;

    public static GameObject buildObject;

    public static GameObject worldObject;

    public static Harmony patcher;

    public static FolderPath modFolder;
    public override string ModNameID => "VanUp";
    public override string DisplayName => "Vanilla Upgrades";
    public override string Author => "ASoD";
    public override string MinimumGameVersionNecessary => "1.5.8";
    public override string ModVersion => "v4.1";

    public override string Description =>
        "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features.";

    public override Action LoadKeybindings => VU_Keybindings.LoadKeybindings;

    public override Dictionary<string, string> Dependencies { get; } = new() { {"UITools", "1.0"} };

    public override void Early_Load()
    {
        main = this;
        modFolder = new FolderPath(ModFolder);
        patcher = new Harmony("mods.ASoD.VanUp");
        patcher.PatchAll();
        SubscribeToScenes();
        Application.quitting += OnQuit;
        Config.Load();
    }

    public override void Load()
    {
        Console.commands.Add(Command);

        ConfigUI.Setup();
        mainObject = new GameObject("ASoDMainObject", typeof(ErrorNotification));
        Object.DontDestroyOnLoad(mainObject);
        mainObject.SetActive(true);
    }

    void SubscribeToScenes()
    {
        SceneHelper.OnBuildSceneLoaded += () => { BuildSettings.Setup(); };
        SceneHelper.OnWorldSceneLoaded += () =>
        {
            WorldManager.Setup();
            worldObject = new GameObject("ASoDWorldObject", typeof(AdvancedInfo), typeof(WorldClockDisplay));
        };
    }

    void OnQuit()
    {
        Config.Save();
    }

    bool Command(string str)
    {
        if (str.StartsWith("vu"))
        {
            Debug.Log("Hi");

            return true;
        }

        return false;
    }
}

[HarmonyPatch(typeof(Loader), "Initialize_Load")]
class ModCheck
{
    static void Postfix(List<Mod> ___loadedMods)
    {
        List<Mod> modList = ___loadedMods;

        if (modList.FindIndex(e => { return e.ModNameID == "BuildSettings"; }) != -1)
        {
            Main.buildSettingsPresent = true;
            Debug.Log(
                "VanillaUpgrades: BuildSettings mod was detected, disabling own Build Settings features to avoid conflicts.");
        }
    }
}