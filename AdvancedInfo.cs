using SFS;
using SFS.UI;
using SFS.World;
using System;
using UnityEngine;

namespace VanillaUpgrades
{

    public class AdvancedInfo : MonoBehaviour
    {
        public static Rect windowRect = new Rect((float)WindowManager.settings["advancedInfo"]["x"], (float)WindowManager.settings["advancedInfo"]["y"], 10f, 10f);

        public double apoapsis;

        public double periapsis;

        public double angle;

        public double displayEcc;

        public bool maximized = true;


        public static AdvancedInfo instance;

        public void Awake()
        {
            instance = this;
        }

        public static string displayify(double value)
        {
            if (double.IsNegativeInfinity(value))
            {
                return "Escape";
            }
            if (double.IsNaN(value))
            {
                return "Error";
            }
            if (value < 10000)
            {
                return value.Round(0.1).ToString(1, true) + "m";
            }
            else
            {
                if (value > 100000000 && (bool)Config.settings["mmUnits"])
                {
                    return (value / 1000000).Round(0.1).ToString(1, true) + "Mm";
                }
                return (value / 1000).Round(0.1).ToString(1, true) + "km";
            }
        }

        public void windowFunc(int windowID)
        {
            bool oldMax = maximized;

            maximized = GUI.Toggle(new Rect(1, -1, 30, 30), maximized, "");

            if (maximized)
            {
                int height = 220;
                int width = 150;

                if (oldMax == false && maximized == true && windowRect.y >= Screen.height - 215)
                {
                    windowRect.y = windowRect.y - (115 - 42);
                }
                if (windowRect.y >= Screen.height - 215 || windowRect.y <= 10 || (bool)Config.settings["alwaysCompact"])
                {
                    height = 115;
                    width = 200;

                    windowRect.height = height;
                    windowRect.width = width;

                    GUIStyle rightAlign = new GUIStyle();
                    rightAlign.alignment = TextAnchor.LowerRight;
                    rightAlign.normal.textColor = Color.white;

                    GUIStyle leftAlign = new GUIStyle();
                    leftAlign.alignment = TextAnchor.LowerLeft;
                    leftAlign.normal.textColor = Color.white;

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Apoapsis:", leftAlign);
                    GUILayout.Label(displayify(apoapsis), rightAlign);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Periapsis:", leftAlign);
                    GUILayout.Label(displayify(periapsis), rightAlign);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Eccentricity:", leftAlign);
                    GUILayout.Label(displayifyEcc(displayEcc), rightAlign);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Angle:", leftAlign);
                    GUILayout.Label(angle.Round(0.1).ToString(1, true) + "°", rightAlign);
                    GUILayout.EndHorizontal();
                }
                else
                {
                    windowRect.height = height;
                    windowRect.width = width;

                    GUILayout.Label("Apoapsis:");
                    GUILayout.Space(-5);
                    GUILayout.Label(displayify(apoapsis));
                    GUILayout.Space(5);

                    GUILayout.Label("Periapsis:");
                    GUILayout.Space(-5);
                    GUILayout.Label(displayify(periapsis));
                    GUILayout.Space(5);

                    GUILayout.Label("Eccentricity:");
                    GUILayout.Space(-5);
                    GUILayout.Label(displayifyEcc(displayEcc));
                    GUILayout.Space(5);

                    GUILayout.Label("Angle:");
                    GUILayout.Space(-5);
                    GUILayout.Label(angle.Round(0.1).ToString(1, true) + "°");
                }

                

                
            }
            else
            {
                if (oldMax == true && maximized == false && windowRect.y >= Screen.height - 215)
                {
                    windowRect.y = windowRect.y + (115 - 42);
                }

                windowRect.height = 42;
                windowRect.width = 150;
            }

            GUI.DragWindow();
        }

        public string displayifyEcc(double value)
        {
            if (value > 1000000 || double.IsNaN(value)) return "Error";
            return value.Round(0.001).ToString(3, true);
        }
        public void Update()
        {

            if (Main.menuOpen || !(bool)Config.settings["showAdvanced"] || VideoSettingsPC.main.uiOpacitySlider.value == 0) return;


            var sma = WorldManager.currentRocket.location.planet.Value.mass / -(2.0 * (Math.Pow(WorldManager.currentRocket.location.velocity.Value.magnitude, 2.0) / 2.0 - WorldManager.currentRocket.location.planet.Value.mass / WorldManager.currentRocket.location.Value.Radius));
            Double3 @double = Double3.Cross(WorldManager.currentRocket.location.position, WorldManager.currentRocket.location.velocity);
            Double2 double2 = (Double2)(Double3.Cross((Double3)WorldManager.currentRocket.location.velocity.Value, @double) / WorldManager.currentRocket.location.planet.Value.mass) - WorldManager.currentRocket.location.position.Value.normalized;
            var ecc = double2.magnitude;
            displayEcc = ecc;


            apoapsis = (Kepler.GetApoapsis(sma, ecc) - WorldManager.currentRocket.location.planet.Value.Radius);
            periapsis = (Kepler.GetPeriapsis(sma, ecc) - WorldManager.currentRocket.location.planet.Value.Radius);

            if (apoapsis == double.PositiveInfinity)
            {

                if (WorldManager.currentRocket.physics.location.velocity.Value.normalized.magnitude > 0)
                {
                    apoapsis = double.NegativeInfinity;
                }
                else
                {
                    apoapsis = 0;
                }

            }
            if (periapsis < 0) { periapsis = 0; }




            var trueAngle = WorldManager.currentRocket.partHolder.transform.eulerAngles.z;

            if (trueAngle > 180) { angle = 360 - trueAngle; }
            if (trueAngle < 180) { angle = -trueAngle; }


        }

        public void OnGUI()
        {
            if (Main.menuOpen || !(bool)Config.settings["showAdvanced"] || VideoSettingsPC.main.uiOpacitySlider.value == 0 || WorldManager.currentRocket == null) return;

            Rect oldRect = windowRect;
            GUI.color = Config.windowColor;
            windowRect = WindowManager.ConfineRect(windowRect);
            windowRect = GUI.Window(WindowManager.GetValidID(), windowRect, windowFunc, "Advanced");
            if (oldRect != windowRect) WindowManager.settings["advancedInfo"]["x"] = windowRect.x; WindowManager.settings["advancedInfo"]["y"] = windowRect.y;
        }
    }
}
