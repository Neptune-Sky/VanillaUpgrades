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

        public Rect windowRect = new Rect((float)WindowManager.settings["worldTime"]["x"], (float)WindowManager.settings["worldTime"]["y"], 150f * WindowManager.scale.x, 30f * WindowManager.scale.y);

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

            midAlign.fontSize = (int)(14 * WindowManager.scale.y);
            midAlign.alignment = TextAnchor.MiddleCenter;
            midAlign.normal.textColor = Color.white;

            bold.fontSize = (int)(15 * WindowManager.scale.y);
            bold.fontStyle = FontStyle.Bold;
            bold.normal.textColor = Color.white;
            bold.alignment = TextAnchor.MiddleCenter;

            if ((bool)Config.settings["worldTime"])
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

            if (((WorldTime.main.timewarpIndex != 0 && (bool)Config.settings["showTime"]) || (bool)Config.settings["alwaysShowTime"]) && VideoSettingsPC.main.uiOpacitySlider.value != 0)
            {
                if (subtractor == 0) subtractor = WorldTime.main.worldTime * 10000000;
                GUI.color = Config.windowColor;
                Rect oldRect = windowRect;
                windowRect = GUILayout.Window(WindowManager.GetValidID(), windowRect, windowFunc, "World Clock", GUILayout.MaxWidth(Screen.width * 0.125f), GUILayout.MaxHeight(Screen.height * 0.06f), GUILayout.MinHeight(30));
                windowRect = WindowManager.ConfineRect(windowRect);
                if (oldRect != windowRect) WindowManager.settings["worldTime"]["x"] = windowRect.x; WindowManager.settings["worldTime"]["y"] = windowRect.y;

            }
            else subtractor = 0;
        }
        
    }
        
}
