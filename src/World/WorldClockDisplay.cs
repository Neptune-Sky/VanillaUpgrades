using System;
using SFS.UI.ModGUI;
using SFS.World;
using UITools;
using UnityEngine;
using VanillaUpgrades.Utility;
using Type = SFS.UI.ModGUI.Type;
using UIExtensions = VanillaUpgrades.Utility.UIExtensions;

namespace VanillaUpgrades;

public class WorldClockDisplay : MonoBehaviour
{
    private const string PositionKey = "VU.WorldClockWindow";

    private const string defaultTime = "000d 00h 00m 00s";

    public double subtractor;
    private GameObject clockHolder;
    private Window clockWindow;

    private string timestamp;
    private Container timewarpContainer;
    private string timewarpTime;

    private Label timewarpTimeLabel;
    private Container worldTimeContainer;
    private Label worldTimeLabel;

    private void Awake()
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
        timestamp = TimeSpanConv(WorldTime.main.worldTime);
        timewarpTime = TimeSpanConv(WorldTime.main.worldTime - subtractor);

        worldTimeLabel.Text = timestamp;

        if (WorldTime.main.timewarpIndex != 0 && Config.settings.showTime)
        {
            if (subtractor == 0) subtractor = WorldTime.main.worldTime;
            timewarpTimeLabel.Text = timewarpTime;
        }
        else
        {
            subtractor = 0;
            timewarpTimeLabel.Text = defaultTime;
        }
    }

    private void ShowGUI()
    {
        Vector2Int pos = new((int)UIExtensions.CanvasPixelSize.x, (int)UIExtensions.CanvasPixelSize.y - 70);

        clockWindow = Builder.CreateWindow(clockHolder.gameObject.transform, 0, 300, 220, pos.x, pos.y,
            true, false, 1, "World Clock");
        clockWindow.RegisterOnDropListener(() => clockWindow.ClampWindow());
        clockWindow.RegisterPermanentSaving(PositionKey);
        var titleSize = clockWindow.rectTransform.Find("Title") as RectTransform;
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

    private void OnTimewarpButton()
    {
        if (Config.settings.alwaysShowTime || !Config.settings.showTime) return;
        var timewarpIndex = WorldTime.main.timewarpIndex;
        if (timewarpIndex != 0)
        {
            if (!clockHolder.gameObject.activeSelf) clockHolder.gameObject.SetActive(true);
        }
        else
        {
            clockHolder.gameObject.SetActive(false);
        }
    }

    private void ToggleChecks()
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

    private static string TimeSpanConv(double totalSeconds)
    {
        const double secondsPerMinute = 60;
        const double secondsPerHour = secondsPerMinute * 60;
        const double secondsPerDay = secondsPerHour * 24;
        const double secondsPerYear = secondsPerDay * 365.25; // Account for leap years
        const double secondsPerMillennium = secondsPerYear * 1000;
        const double secondsPerMegaannum = secondsPerYear * 1000000;

        // Calculate each time unit and ensure fractions are passed down
        var megaannums = Math.Floor(totalSeconds / secondsPerMegaannum);
        totalSeconds %= secondsPerMegaannum;

        var millennia = Math.Floor(totalSeconds / secondsPerMillennium);
        totalSeconds %= secondsPerMillennium;

        var years = Math.Floor(totalSeconds / secondsPerYear);
        totalSeconds %= secondsPerYear;

        var days = Math.Floor(totalSeconds / secondsPerDay);
        totalSeconds %= secondsPerDay;

        var hours = Math.Floor(totalSeconds / secondsPerHour);
        totalSeconds %= secondsPerHour;

        var minutes = Math.Floor(totalSeconds / secondsPerMinute);
        totalSeconds %= secondsPerMinute;

        // Remaining seconds as an integer (no decimals)
        var seconds = (int)Math.Floor(totalSeconds);

        // Build the output string using the shorter units
        return $"{(megaannums > 0 ? $"{megaannums}Ma " : "")}" +
               $"{(millennia > 0 ? $"{millennia}M " : "")}" +
               $"{(years > 0 ? $"{years}y " : "")}" +
               $"{days}d " + // Always show days, even if 0
               $"{hours}h " +
               $"{minutes}m " +
               $"{seconds}s".TrimEnd();
    }
}