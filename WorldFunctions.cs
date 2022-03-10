using SFS.UI;
using SFS.World;
using System;
using UnityEngine;

namespace ASoD_s_VanillaUpgrades
{

    public class WorldFunctions : MonoBehaviour
    {
        public static Rect windowRect = new Rect(0f, Screen.height * 0.05f, 150f, 220f);

        public static Rocket currentRocket;

        public double apoapsis;

        public double periapsis;

        public double angle;

        public double displayEcc;

        public static bool disableKt;

        public static WorldFunctions instance;


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
            if (value < 10000)
            {
                return value.Round(0.1).ToString(1, true) + "m";
            }
            else
            {
                if (value > 10000000 && Config.mmUnits)
                {
                    return (value / 1000000).Round(0.1).ToString(1, true) + "Mm";
                }
                return (value / 1000).Round(0.1).ToString(1, true) + "km";
            }
        }

        public void windowFunc(int windowID)
        {
            GUI.Label(new Rect(10f, 20f, 160f, 20f), "Apoapsis:");
            GUI.Label(new Rect(10f, 40f, 160f, 20f), displayify(apoapsis));
            GUI.Label(new Rect(10f, 70f, 160f, 20f), "Periapsis:");
            GUI.Label(new Rect(10f, 90f, 160f, 20f), displayify(periapsis));
            GUI.Label(new Rect(10f, 120f, 160f, 25f), "Eccentricity:");
            GUI.Label(new Rect(10f, 140f, 160f, 25f), displayEcc.Round(0.001).ToString(3, true));
            GUI.Label(new Rect(10f, 170f, 160f, 25f), "Angle:");
            GUI.Label(new Rect(10f, 190f, 160f, 20f), angle.Round(0.1).ToString(1, true) + "°");

            GUI.DragWindow();
        }

        public static void StopTimewarp(bool showmsg)
        {
            if (WorldTime.main.timewarpIndex.timewarpIndex == 0) return;

            WorldTime.main.timewarpIndex.timewarpIndex = 0;
            WorldTime.main.SetState(1, true, false);
            TimewarpToClass.timewarpTo = false;
            if (showmsg)
            {
                MsgDrawer.main.Log("Time acceleration stopped");
            }

        }

        public static void Throttle01()
        {
            currentRocket.throttle.throttlePercent.Value = 0.0005f;
        }


        public void OnGUI()
        {



            if (Main.menuOpen || !Config.showAdvanced) return;

            var player = (PlayerController.main.player.Value as Rocket);
            currentRocket = GameManager.main.rockets[GameManager.main.rockets.IndexOf(player)];
            var sma = currentRocket.location.planet.Value.mass / -(2.0 * (Math.Pow(currentRocket.location.velocity.Value.magnitude, 2.0) / 2.0 - currentRocket.location.planet.Value.mass / currentRocket.location.Value.Radius));
            Double3 @double = Double3.Cross(currentRocket.location.position, currentRocket.location.velocity);
            Double2 double2 = (Double2)(Double3.Cross((Double3)currentRocket.location.velocity.Value, @double) / currentRocket.location.planet.Value.mass) - currentRocket.location.position.Value.normalized;
            var ecc = double2.magnitude;
            displayEcc = ecc;


            apoapsis = (Kepler.GetApoapsis(sma, ecc) - currentRocket.location.planet.Value.Radius);
            periapsis = (Kepler.GetPeriapsis(sma, ecc) - currentRocket.location.planet.Value.Radius);

            if (apoapsis == double.PositiveInfinity)
            {

                if (currentRocket.physics.location.velocity.Value.normalized.magnitude > 0)
                {
                    apoapsis = double.NegativeInfinity;
                }
                else
                {
                    apoapsis = 0;
                }

            }
            if (periapsis < 0) { periapsis = 0; }




            var trueAngle = currentRocket.partHolder.transform.eulerAngles.z;

            if (trueAngle > 180) { angle = 360 - trueAngle; }
            if (trueAngle < 180) { angle = -trueAngle; }

            windowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), windowRect, new GUI.WindowFunction(windowFunc), "Advanced");

        }
    }
}
