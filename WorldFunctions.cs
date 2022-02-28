﻿using System;
using SFS.World;
using UnityEngine;
using System.Collections.Generic;

namespace ASoD_s_VanillaUpgrades
{
    public class WorldFunctions : MonoBehaviour
    {
        public static Rect windowRect = new Rect(0f, Screen.height * 0.05f, 150f, 230f);

        public static Rocket currentRocket;

        public double apoapsis;

        public double periapsis;

        public double angle;

        public double displayEcc;

        public static string displayify(double value)
        {
            if (double.IsNegativeInfinity(value))
            {
                return "Escape";
            }
            if (value < 10000)
            {
                return value.ToString() + "m";
            }
            else
            {
                if (value > 10000000)
                {
                    return (value / 1000000).Round(0.1).ToString() + "Mm";
                }
                return (value / 1000).Round(0.1).ToString() + "km";
            }
        }

        public void windowFunc(int windowID)
        {
            GUI.Label(new Rect(20f, 20f, 160f, 20f), "Apoapsis:");
            GUI.Label(new Rect(20f, 40f, 160f, 20f), displayify(apoapsis));
            GUI.Label(new Rect(20f, 70f, 160f, 20f), "Periapsis:");
            GUI.Label(new Rect(20f, 90f, 160f, 20f), displayify(periapsis));
            GUI.Label(new Rect(20f, 120f, 160f, 25f), "Eccentricity:");
            GUI.Label(new Rect(20f, 140f, 160f, 25f), displayEcc.ToString());
            GUI.Label(new Rect(20f, 170f, 160f, 25f), "Angle:");
            GUI.Label(new Rect(20f, 190f, 160f, 20f), angle.Round(0.1).ToString() + "°");
            
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
            displayEcc = ecc.Round(0.001);

            apoapsis = (Kepler.GetApoapsis(sma, ecc) - currentRocket.location.planet.Value.Radius).Round(0.1);
            periapsis = (Kepler.GetPeriapsis(sma, ecc) - currentRocket.location.planet.Value.Radius).Round(0.1);
            if (apoapsis == double.PositiveInfinity) { 
                if (currentRocket.physics.location.Value.velocity.magnitude > 0)
                {
                    apoapsis = double.NegativeInfinity;
                } else
                {
                    apoapsis = 0;
                }
                
            }
            if (periapsis < 0) { periapsis = 0; }


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
