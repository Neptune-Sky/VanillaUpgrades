using SFS.UI;
using SFS.World;
using VanillaUpgrades.Utility;
using Type = SFS.UI.ModGUI.Type;

namespace VanillaUpgrades;

public class WorldClockDisplay : MonoBehaviour
{
    const string posKey = "WorldClockWindow";

    public long subtractor;

    readonly string defaultTime = "000d 00h 00m 00s";
    readonly int MainWindowID = Builder.GetRandomID();
    GameObject clockHolder;
    Window clockWindow;

    string timestamp;
    Container timewarpContainer;
    string timewarpTime;

    Label timewarpTimeLabel;
    Container worldTimeContainer;
    Label worldTimeLabel;

    void Awake()
    {
        clockHolder = CustomUI.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "Clock Holder");
        WorldTime.main.realtimePhysics.OnChange += OnTimewarpButton;

        ShowGUI();
        Config.settingsData.showTime.OnChange += ToggleChecks;
        Config.settingsData.alwaysShowTime.OnChange += ToggleChecks;
        Config.settingsData.showWorldTime.OnChange += ToggleChecks;

        Config.settingsData.persistentVars.windowScale.OnChange += () =>
        {
            clockWindow.rectTransform.localScale = new Vector2(Config.settingsData.persistentVars.windowScale,
                Config.settingsData.persistentVars.windowScale);
            WindowManager.ClampWindow(clockWindow);
            WindowManager.Save(posKey, clockWindow);
        };
    }

    public void Update()
    {
        timestamp = TimeSpanConv(new TimeSpan((long)WorldTime.main.worldTime * 10000000));
        timewarpTime = TimeSpanConv(new TimeSpan((long)WorldTime.main.worldTime * 10000000 - subtractor));

        worldTimeLabel.Text = timestamp;

        if (WorldTime.main.timewarpIndex != 0 && Config.settingsData.showTime)
        {
            if (subtractor == 0) subtractor = (long)WorldTime.main.worldTime * 10000000;
            timewarpTimeLabel.Text = timewarpTime;
        }
        else
        {
            subtractor = 0;
            timewarpTimeLabel.Text = defaultTime;
        }
    }

    void ShowGUI()
    {
        Vector2Int pos = Config.settingsData.windowsSavedPosition.GetValueOrDefault(posKey,
            new Vector2Int((int)WindowManager.gameSize.x, (int)WindowManager.gameSize.y - 70));
        clockWindow = Builder.CreateWindow(clockHolder.gameObject.transform, MainWindowID, 300, 220, pos.x, pos.y,
            true, true, 1, "World Clock");
        clockWindow.gameObject.GetComponent<DraggableWindowModule>().OnDropAction += () =>
        {
            WindowManager.ClampWindow(clockWindow);
            WindowManager.Save(posKey, clockWindow);
        };

        RectTransform titleSize = clockWindow.rectTransform.Find("Title") as RectTransform;
        titleSize.sizeDelta = new Vector2(titleSize.sizeDelta.x, 30);

        clockWindow.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 10,
            new RectOffset(0, 0, 3, 0));

        Builder.CreateSeparator(clockWindow, 280);

        worldTimeContainer = Builder.CreateContainer(clockWindow);
        worldTimeContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 0);
        CustomUI.AlignedLabel(worldTimeContainer, 280, 28, "World Time:");
        worldTimeLabel = CustomUI.AlignedLabel(worldTimeContainer, 280, 32);

        timewarpContainer = Builder.CreateContainer(clockWindow);
        timewarpContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 0);
        CustomUI.AlignedLabel(timewarpContainer, 280, 28, "Timewarp Span:");
        timewarpTimeLabel = CustomUI.AlignedLabel(timewarpContainer, 280, 32);


        ToggleChecks();

        clockWindow.rectTransform.localScale = new Vector2(Config.settingsData.persistentVars.windowScale,
            Config.settingsData.persistentVars.windowScale);
        WindowManager.ClampWindow(clockWindow);
    }

    void OnTimewarpButton()
    {
        if (Config.settingsData.alwaysShowTime || !Config.settingsData.showTime) return;
        int timewarpIndex = WorldTime.main.timewarpIndex;
        if (timewarpIndex != 0)
        {
            if (!clockHolder.gameObject.activeSelf) clockHolder.gameObject.SetActive(true);
        }
        else
        {
            clockHolder.gameObject.SetActive(false);
        }
    }

    void ToggleChecks()
    {
        if (!Config.settingsData.showTime ||
            (WorldTime.main.timewarpIndex == 0 && !Config.settingsData.alwaysShowTime))
            clockHolder.SetActive(false);

        if (Config.settingsData.showTime && Config.settingsData.alwaysShowTime) clockHolder.SetActive(true);

        worldTimeContainer.gameObject.SetActive(Config.settingsData.showWorldTime);
        if (!Config.settingsData.showWorldTime)
            clockWindow.Size = new Vector2(clockWindow.Size.x, 150);
        else
            clockWindow.Size = new Vector2(clockWindow.Size.x, 220);
    }

    public string TimeSpanConv(TimeSpan span)
    {
        long milleniaNum = (long)span.Days / 365 / 1000;
        long yearNum = (long)span.Days / 365 - milleniaNum * 1000;

        string millenia = milleniaNum != 0 ? milleniaNum + "M " : "";
        string years = yearNum != 0 ? yearNum + "y " : "";
        string days = (span.Days - (yearNum + milleniaNum * 1000) * 365).ToString("000") + "d ";

        return millenia + years + days + span.Hours.ToString("00") + "h " + span.Minutes.ToString("00") + "m " +
               span.Seconds.ToString("00") + "s";
    }

    /*
    public void windowFunc(int windowID)
    {
        var bold = new GUIStyle();
        var midAlign = new GUIStyle();

        midAlign.fontSize = (int)(14 * WindowManager2.scale.y);
        midAlign.alignment = TextAnchor.MiddleCenter;
        midAlign.normal.textColor = Color.white;

        bold.fontSize = (int)(15 * WindowManager2.scale.y);
        bold.fontStyle = FontStyle.Bold;
        bold.normal.textColor = Color.white;
        bold.alignment = TextAnchor.MiddleCenter;

        if (Config.settingsData.worldTime)
        {
            GUILayout.Label("World Time:", midAlign);
            GUILayout.Label(timestamp, bold);
            GUILayout.Space(4);
        }
        if (WorldTime.main.timewarpIndex != 0)
        {
            GUILayout.Space(4);
            GUILayout.Label("Timewarp Span:", midAlign);
            GUILayout.Label(timewarpTime, bold);
        }

        GUI.DragWindow();
    }
    */
}