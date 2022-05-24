using System;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine;
using SFS;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace VanillaUpgrades
{
    public class DVCalc : MonoBehaviour
    {
        public Rect calcRect = new Rect(Screen.width - 175f, 50f, 175f, 200f);
        public Rect ispRect = new Rect(Screen.width - 175f, 270f, 175f, 160f);

        public static bool showCalc = (bool)Config.settings["showCalc"];

        public string isp;
        public string wetMass;
        public string dryMass;
        public string result;
        public bool showIsp = (bool)Config.settings["showAverager"];

        public static void toggleCalc()
        {
            showCalc = !showCalc;
        }

        public void calcFunc(int windowID)
        {
            GUILayout.BeginHorizontal(); 
            GUILayout.Label("ISP:", GUILayout.MaxWidth(65));
            isp = GUILayout.TextField(isp);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Wet Mass:", GUILayout.MaxWidth(65));
            wetMass = GUILayout.TextField(wetMass);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Dry Mass:", GUILayout.MaxWidth(65));
            dryMass = GUILayout.TextField(dryMass);
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Result:", GUILayout.MaxWidth(65));
            GUILayout.Label(result, "TextField");
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Calculate", GUILayout.Width(75)))
            {
                bool valid = true;
                try
                {
                    if (!Regex.IsMatch(isp, "^\\d*\\.?\\d*$") || isp == null)
                    {
                        isp = "";
                        valid = false;
                    } 
                    if (!Regex.IsMatch(wetMass, "^\\d*\\.?\\d*$") || wetMass == null)
                    {
                        wetMass = "";
                        valid = false;
                    } 
                    if (!Regex.IsMatch(dryMass, "^\\d*\\.?\\d*$") || dryMass == null)
                    {
                        dryMass = "";
                        valid = false;
                    }
                }
                catch (Exception)
                {
                    valid = false;
                }
                ;

                if (!valid)
                {
                    result = "Error";
                    return;
                }

                result = (9.8f * float.Parse(isp, CultureInfo.InvariantCulture) * (Mathf.Log(float.Parse(wetMass, CultureInfo.InvariantCulture) / float.Parse(dryMass, CultureInfo.InvariantCulture)) / Mathf.Log((float)Math.E))).ToString() + "m/s";
                
                
            }
            if (GUILayout.Button("Clear", GUILayout.Width(75)))
            {
                isp = null;
                dryMass = null;
                wetMass = null;
                result = null;

            }
            GUILayout.EndHorizontal();

            showIsp = GUILayout.Toggle(showIsp, " ISP Averager");

            GUI.DragWindow();

        }

        public List<JObject> engines = new List<JObject>();
        public Vector2 scroll;

        public string isp2;
        public string thrust;
        public string ispResult = null;

        public void ispFunc(int windowID)
        {
            int count = 0;
            int dupeIndex = -1;
            int removeIndex = -1;
            int minHeight = 75;
            switch (engines.Count)
            {
                case 1:
                    minHeight = 85;
                    break;
                case 2:
                    minHeight = 170;
                    break;
                default:
                    minHeight = 250;
                    break;
            }
            if (engines.Count > 0)
            {
                scroll = GUILayout.BeginScrollView(scroll, GUILayout.MinHeight(minHeight));

                foreach (var e in engines)
                {
                    GUILayout.Label($"Engine {count += 1}\nThrust: {e["thrust"]}\nISP: {e["isp"]}");

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Remove", GUILayout.MaxHeight(20), GUILayout.MaxWidth(65)))
                    {
                        removeIndex = count - 1;
                    }
                    if (GUILayout.Button("Copy", GUILayout.MaxHeight(20), GUILayout.MaxWidth(55)))
                    {
                        dupeIndex = count - 1;
                    }
                    GUILayout.EndHorizontal();
                }


                GUILayout.EndScrollView();
            }

            if (dupeIndex != -1)
            {
                engines.Add(engines[dupeIndex]);
            }
            if (removeIndex != -1)
            {
                engines.RemoveAt(removeIndex);
                if (engines.Count == 0)
                {
                    ispRect.height = 160;
                    ispRect.width = 175;
                }
            }

            GUILayout.Label("New Engine:");
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Thrust:", GUILayout.MaxWidth(65));
            thrust = GUILayout.TextArea(thrust);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("ISP:", GUILayout.MaxWidth(65));
            isp2 = GUILayout.TextArea(isp2);
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Add Engine", GUILayout.MaxWidth(155)))
            {
                bool valid = true;
                try
                {
                    if (!Regex.IsMatch(isp2, "^\\d*\\.?\\d*$"))
                    {
                        isp2 = "Error";
                        valid = false;
                    }
                    if (!Regex.IsMatch(thrust, "^\\d*\\.?\\d*$"))
                    {
                        thrust = "Error";
                        valid = false;
                    }
                }
                catch (Exception)
                {
                    isp2 = "Error";
                    thrust = "Error";
                    valid = false;
                }
                if (valid)
                {
                    JObject toAdd = JObject.Parse("{ isp: " + float.Parse(isp2, CultureInfo.InvariantCulture) + ", thrust:" + float.Parse(thrust, CultureInfo.InvariantCulture) + "}");
                    engines.Add(toAdd);
                    isp2 = null;
                    thrust = null;
                }
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Calculate", GUILayout.Width(75)) && engines.Count > 0)
            {
                float dividend = 0;
                float divisor = 0;

                foreach (var engine in engines)
                {
                    dividend += (float)engine["thrust"];
                    divisor += (float)engine["thrust"] / (float)engine["isp"];
                }

                ispResult = (dividend / divisor).Round(0.01f).ToString();
            }

            if (GUILayout.Button("Clear", GUILayout.Width(75)))
            {
                engines.Clear();
                isp2 = null;
                thrust = null;
                ispResult = null;
                ispRect.height = 160;
                ispRect.width = 175;
            }
            GUILayout.EndHorizontal();

            if (ispResult != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Average ISP:");
                GUILayout.Label(ispResult, "TextField", GUILayout.Width(90));
                GUILayout.EndHorizontal();
            }



            GUI.DragWindow();
        }

        public void OnGUI()
        { 
            if (showCalc && !Main.menuOpen && VideoSettingsPC.main.uiOpacitySlider.value != 0)
            {
                GUI.color = Config.windowColor;
                calcRect = GUI.Window(WindowManager.GetValidID(), calcRect, calcFunc, "ΔV Calculator");
                calcRect = WindowManager.ConfineRect(calcRect);
                if (showIsp)
                {
                    ispRect = GUILayout.Window(WindowManager.GetValidID(), ispRect, ispFunc, "ISP Averager", GUILayout.MaxHeight(300), GUILayout.Width(175));
                    ispRect = WindowManager.ConfineRect(ispRect);
                }
            }
        }
    }
}
