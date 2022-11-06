using SFS.World;
using UITools;
using Type = SFS.UI.ModGUI.Type;

namespace VanillaUpgrades;

public class WorldClockDisplay : MonoBehaviour
{
    const string PositionKey = "VU.WorldClockWindow";

    public long subtractor;

    readonly string defaultTime = "000d 00h 00m 00s";
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
        clockHolder = UIExtensions.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "Clock Holder");
        WorldTime.main.realtimePhysics.OnChange += OnTimewarpButton;

        ShowGUI();
        Config.settings.showTime.OnChange += ToggleChecks;
        Config.settings.alwaysShowTime.OnChange += ToggleChecks;
        Config.settings.showWorldTime.OnChange += ToggleChecks;

        Config.settings.persistentVars.windowScale.OnChange += () =>
        {
            clockWindow.ScaleWindow();
            clockWindow.ClampWindow();
        };
    }

    public void Update()
    {
        timestamp = TimeSpanConv(new TimeSpan((long)WorldTime.main.worldTime * 10000000));
        timewarpTime = TimeSpanConv(new TimeSpan((long)WorldTime.main.worldTime * 10000000 - subtractor));

        worldTimeLabel.Text = timestamp;

        if (WorldTime.main.timewarpIndex != 0 && Config.settings.showTime)
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
        Vector2Int pos = new((int)UIExtensions.CanvasPixelSize.x, (int)UIExtensions.CanvasPixelSize.y - 70);

        clockWindow = Builder.CreateWindow(clockHolder.gameObject.transform, 0, 300, 220, pos.x, pos.y,
            true, false, 1, "World Clock");
        clockWindow.RegisterOnDropListener(() => clockWindow.ClampWindow());
        clockWindow.RegisterPermanentSaving(PositionKey);
        RectTransform titleSize = clockWindow.rectTransform.Find("Title") as RectTransform;
        titleSize.sizeDelta = new Vector2(titleSize.sizeDelta.x, 30);

        clockWindow.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 10,
            new RectOffset(0, 0, 3, 0));

        Builder.CreateSeparator(clockWindow, 280);

        worldTimeContainer = Builder.CreateContainer(clockWindow);
        worldTimeContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 0);
        UIExtensions.AlignedLabel(worldTimeContainer, 280, 28, "World Time:");
        worldTimeLabel = UIExtensions.AlignedLabel(worldTimeContainer, 280, 32);

        timewarpContainer = Builder.CreateContainer(clockWindow);
        timewarpContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 0);
        UIExtensions.AlignedLabel(timewarpContainer, 280, 28, "Timewarp Span:");
        timewarpTimeLabel = UIExtensions.AlignedLabel(timewarpContainer, 280, 32);


        ToggleChecks();

        clockWindow.ScaleWindow();
        clockWindow.ClampWindow();
    }

    void OnTimewarpButton()
    {
        if (Config.settings.alwaysShowTime || !Config.settings.showTime) return;
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
        if (!Config.settings.showTime ||
            (WorldTime.main.timewarpIndex == 0 && !Config.settings.alwaysShowTime))
            clockHolder.SetActive(false);

        if (Config.settings.showTime && Config.settings.alwaysShowTime) clockHolder.SetActive(true);

        worldTimeContainer.gameObject.SetActive(Config.settings.showWorldTime);
        if (!Config.settings.showWorldTime)
            clockWindow.Size = new Vector2(clockWindow.Size.x, 150);
        else
            clockWindow.Size = new Vector2(clockWindow.Size.x, 220);
    }

    static string TimeSpanConv(TimeSpan span)
    {
        long milleniaNum = (long)span.Days / 365 / 1000;
        long yearNum = (long)span.Days / 365 - milleniaNum * 1000;

        string millenia = milleniaNum != 0 ? milleniaNum + "M " : "";
        string years = yearNum != 0 ? yearNum + "y " : "";
        string days = (span.Days - (yearNum + milleniaNum * 1000) * 365).ToString("000") + "d ";

        return millenia + years + days + span.Hours.ToString("00") + "h " + span.Minutes.ToString("00") + "m " +
               span.Seconds.ToString("00") + "s";
    }
}