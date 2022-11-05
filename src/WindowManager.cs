using SFS.IO;

namespace VanillaUpgrades;

[Serializable]
public class Windows
{
    public Vector2 buildSettings = new(Screen.width * 0.05f, Screen.height * 0.94f);

    public Vector2 advancedInfo = new(0f, Screen.height * 0.05f);

    public Vector2 config = new(300, 1000);

    public Vector2 worldTime = new(Screen.width * 0.958f, Screen.height * 0.045f);

    public Vector2 calc = new(Screen.width - 175f, 50f);

    public Vector2 averager = new(Screen.width - 175f, 270f);
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
        Vector2 refRes = sizeTracking.GetComponentInParent<CanvasScaler>().referenceResolution;
        gameSize = new Vector2(refRes.y / Screen.height * Screen.width, refRes.y);
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
            Config.settingsData.windowsSavedPosition[key] = Vector2Int.RoundToInt(window.Position);
        else
            Config.settingsData.windowsSavedPosition.Add(key, Vector2Int.RoundToInt(window.Position));
    }
}