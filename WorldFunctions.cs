using System;
using SFS.World;
using UnityEngine;
using System.Collections.Generic;

namespace ASoD_s_VanillaUpgrades
{
    public class WorldFunctions : MonoBehaviour
    {
        public static Rect windowRect = new Rect(Screen.width * 0.7f, 0f, 200f, 170f);

        public static Rocket currentRocket;

        public double apoapsis;

        public double periapsis;

        public double angle;

        public static string displayify(double value)
        {
            if (value < 10000)
            {
                return value.ToString() + "m";
            }
            else
            {
                return (value / 1000).Round(0.1).ToString() + "km";
            }
        }

        public void windowFunc(int windowID)
        {
            GUI.Label(new Rect(20f, 20f, 160f, 20f), "Apoapsis:");
            GUI.Label(new Rect(20f, 40f, 160f, 20f), displayify(apoapsis));
            GUI.Label(new Rect(20f, 70f, 160f, 20f), "Periapsis:");
            GUI.Label(new Rect(20f, 90f, 160f, 20f), displayify(periapsis));
            GUI.Label(new Rect(20f, 120f, 160f, 25f), "Angle:");
            GUI.Label(new Rect(20f, 140f, 160f, 20f), angle.Round(0.1).ToString() + "°");
            GUI.DragWindow();
        }


        
        public void OnGUI()
        {
            var player = (PlayerController.main.player.Value as Rocket);
            currentRocket = GameManager.main.rockets[GameManager.main.rockets.IndexOf(player)];
            var sma = currentRocket.location.planet.Value.mass / -(2.0 * (Math.Pow(currentRocket.location.velocity.Value.magnitude, 2.0) / 2.0 - currentRocket.location.planet.Value.mass / currentRocket.location.Value.Radius));
            Double3 @double = Double3.Cross(currentRocket.location.position, currentRocket.location.velocity);
            Double2 double2 = (Double2)(Double3.Cross((Double3)currentRocket.location.velocity.Value, @double) / currentRocket.location.planet.Value.mass) - currentRocket.location.position.Value.normalized;
            var ecc = double2.magnitude;

            apoapsis = (Kepler.GetApoapsis(sma, ecc) - currentRocket.location.planet.Value.Radius).Round(0.1);
            periapsis = (Kepler.GetPeriapsis(sma, ecc) - currentRocket.location.planet.Value.Radius).Round(0.1);
            if (apoapsis == double.PositiveInfinity) { apoapsis = 0; }
            if (periapsis == double.PositiveInfinity || periapsis < 0) { periapsis = 0; }

            angle = currentRocket.partHolder.transform.eulerAngles.z;

            if (angle > 180) { angle = 360 - angle; }
            
            Event current = Event.current;
            if (current.keyCode == KeyCode.Slash)
            {
                WorldTime.main.SetState(1, true, true);
            }
            if (current.keyCode == KeyCode.BackQuote)
            {
                currentRocket.throttle.throttlePercent.Value = 0.0005f;
            }
            windowRect = GUI.Window(0, windowRect, new GUI.WindowFunction(windowFunc), "Advanced Info");
            
        }
    }
}
