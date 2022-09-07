using SFS;
using SFS.World;
using System;
using UnityEngine;

namespace VanillaUpgrades
{
    public class WorldClockDisplay : MonoBehaviour
    {
        
        public static string timestamp;
        public static string timewarpTime;

        public Rect windowRect = new Rect((float)WindowManager2.settings["worldTime"]["x"], (float)WindowManager2.settings["worldTime"]["y"], 150f * WindowManager2.scale.x, 30f * WindowManager2.scale.y);

        public string TimeSpanConv(TimeSpan span)
        {
            long milleniaNum = ((long)span.Days / 365) / 1000;
            long yearNum = ((long)span.Days / 365) - milleniaNum * 1000;

            string millenia = milleniaNum != 0 ? milleniaNum.ToString() + "M " : "";
            string years = yearNum != 0 ? yearNum.ToString() + "y " : "";
            string days = (span.Days - (yearNum + milleniaNum * 1000) * 365).ToString("000") + "d ";

            return millenia + years + days + span.Hours.ToString("00") + "h " + (span.Minutes).ToString("00") + "m " + span.Seconds.ToString("00") + "s";
        }
        public void Update()
        {
            /*
            windowRect.width = 150f * WindowManager.scale.x;
            windowRect.height = 30f * WindowManager.scale.y;
            */

            var span = new TimeSpan((long)WorldTime.main.worldTime * 10000000);
            var span2 = new TimeSpan((long)WorldTime.main.worldTime * 10000000 - (long)subtractor);

            timestamp = TimeSpanConv(span);
            timewarpTime = TimeSpanConv(span2);
        }

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

        public double subtractor;
        public void OnGUI()
        {
            if (Main.menuOpen) return;

            if (((WorldTime.main.timewarpIndex != 0 && Config.settingsData.showTime) || Config.settingsData.alwaysShowTime) && VideoSettingsPC.main.uiOpacitySlider.value != 0)
            {
                if (subtractor == 0) subtractor = WorldTime.main.worldTime * 10000000;
                GUI.color = Config.settingsData.persistentVars.windowColor;
                Rect oldRect = windowRect;
                windowRect = GUILayout.Window(WindowManager2.GetValidID(), windowRect, windowFunc, "World Clock", GUILayout.MaxWidth(Screen.width * 0.125f), GUILayout.MaxHeight(Screen.height * 0.06f), GUILayout.MinHeight(30));
                windowRect = WindowManager2.ConfineRect(windowRect);
                if (oldRect != windowRect) WindowManager2.settings["worldTime"]["x"] = windowRect.x; WindowManager2.settings["worldTime"]["y"] = windowRect.y;

            }
            else subtractor = 0;
        }
        
    }
        
}
