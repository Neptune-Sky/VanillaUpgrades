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
        public Rect calcRect = new Rect((float)WindowManager2.settings["dvCalc"]["x"], (float)WindowManager2.settings["dvCalc"]["y"], 190f * WindowManager2.scale.y, 200f * WindowManager2.scale.y);
        public Rect ispRect = new Rect((float)WindowManager2.settings["ispAverager"]["x"], (float)WindowManager2.settings["ispAverager"]["y"], 190f * WindowManager2.scale.y, 160f * WindowManager2.scale.y);

        public int ispHeight = (int)(160 * WindowManager2.scale.y);

        public static bool showCalc = (bool)Config3.settings2["showCalc"];

        public string isp;
        public string wetMass;
        public string dryMass;
        public string result;
        public bool showIsp = (bool)Config3.settings2["showAverager"];
        
        public void Update()
        {
            ispHeight = (int)(160 * WindowManager2.scale.y);
            calcRect.width = 190f * WindowManager2.scale.y;
            calcRect.height = 200f * WindowManager2.scale.y;
        }

        public static void toggleCalc()
        {
            showCalc = !showCalc;
        }

        public void calcFunc(int windowID)
        {
            GUIStyle leftAlign = new GUIStyle();
            leftAlign.alignment = TextAnchor.MiddleLeft;
            leftAlign.normal.textColor = Color.white;
            leftAlign.fontSize = (int)(14 * WindowManager2.scale.y);

            GUIStyle leftAlignTextField = new GUIStyle(GUI.skin.textField);
            leftAlignTextField.alignment = TextAnchor.UpperLeft;
            leftAlignTextField.normal.textColor = Color.white;
            leftAlignTextField.fontSize = (int)(14 * WindowManager2.scale.y);


            GUIStyle midAlign = new GUIStyle(GUI.skin.button);
            midAlign.alignment = TextAnchor.MiddleCenter;
            midAlign.normal.textColor = Color.white;
            midAlign.fontSize = (int)(14 * WindowManager2.scale.y);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ISP:", leftAlign, GUILayout.Width(65 * WindowManager2.scale.y));
            GUILayout.EndVertical();
            isp = GUILayout.TextArea(isp, leftAlignTextField, GUILayout.MinWidth(85 * WindowManager2.scale.y), GUILayout.ExpandHeight(false));
            GUILayout.EndHorizontal();

            GUILayout.Space(5 * WindowManager2.scale.y);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Wet Mass:", leftAlign, GUILayout.Width(65 * WindowManager2.scale.y));
            GUILayout.EndVertical();
            wetMass = GUILayout.TextField(wetMass, leftAlignTextField, GUILayout.MinWidth(85 * WindowManager2.scale.y), GUILayout.ExpandHeight(false));
            GUILayout.EndHorizontal();

            GUILayout.Space(5 * WindowManager2.scale.y);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Dry Mass:", leftAlign, GUILayout.Width(65 * WindowManager2.scale.y));
            GUILayout.EndVertical();
            dryMass = GUILayout.TextField(dryMass, leftAlignTextField, GUILayout.MinWidth(85 * WindowManager2.scale.y), GUILayout.ExpandHeight(false));
            GUILayout.EndHorizontal();

            GUILayout.Space(7 * WindowManager2.scale.y);
            GUILayout.Label(result, leftAlignTextField);

            GUILayout.Space(3 * WindowManager2.scale.y);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Result", midAlign, GUILayout.Width(83 * WindowManager2.scale.y)))
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
            if (GUILayout.Button("Clear", midAlign, GUILayout.Width(82 * WindowManager2.scale.y)))
            {
                isp = null;
                dryMass = null;
                wetMass = null;
                result = null;

            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Toggle ISP Averager", midAlign))
            {
                showIsp = !showIsp;
            }

            GUI.DragWindow();

        }

        public List<JObject> engines = new List<JObject>();
        public Vector2 scroll;

        public string isp2;
        public string thrust;
        public string ispResult = null;

        public void ispFunc(int windowID)
        {
            ispRect.width = 190f * WindowManager2.scale.y;

            GUIStyle leftAlign = new GUIStyle();
            leftAlign.alignment = TextAnchor.MiddleLeft;
            leftAlign.normal.textColor = Color.white;
            leftAlign.fontSize = (int)(14 * WindowManager2.scale.y);

            GUIStyle leftAlignTextField = new GUIStyle(GUI.skin.textField);
            leftAlignTextField.alignment = TextAnchor.UpperLeft;
            leftAlignTextField.normal.textColor = Color.white;
            leftAlignTextField.fontSize = (int)(14 * WindowManager2.scale.y);

            GUIStyle midAlign = new GUIStyle(GUI.skin.button);
            midAlign.alignment = TextAnchor.MiddleCenter;
            midAlign.normal.textColor = Color.white;
            midAlign.fontSize = (int)(14 * WindowManager2.scale.y);

            int count = 0;
            int dupeIndex = -1;
            int removeIndex = -1;
            int minHeight = (int)(75 * WindowManager2.scale.y);
            switch (engines.Count)
            {
                case 1:
                    minHeight = (int)(85 * WindowManager2.scale.y);
                    break;
                case 2:
                    minHeight = (int)(170 * WindowManager2.scale.y);
                    break;
                default:
                    minHeight = (int)(250 * WindowManager2.scale.y);
                    break;
            }
            if (engines.Count > 0)
            {
                ispRect.height = ispHeight + minHeight;
                scroll = GUILayout.BeginScrollView(scroll, GUILayout.MinHeight(minHeight), GUILayout.MaxWidth(180 * WindowManager2.scale.y));
                

                foreach (var e in engines)
                {
                    GUILayout.Label($"Engine {count += 1}\nThrust: {e["thrust"]}\nISP: {e["isp"]}", leftAlign);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Remove", midAlign, GUILayout.MaxHeight(20 * WindowManager2.scale.y), GUILayout.MaxWidth(65 * WindowManager2.scale.y)))
                    {
                        removeIndex = count - 1;
                    }
                    if (GUILayout.Button("Copy", midAlign, GUILayout.MaxHeight(20 * WindowManager2.scale.y), GUILayout.MaxWidth(55 * WindowManager2.scale.y)))
                    {
                        dupeIndex = count - 1;
                    }
                    GUILayout.EndHorizontal();
                }


                GUILayout.EndScrollView();
            }
            else
            {
                ispRect.height = ispHeight;
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
                    ispRect.height = ispHeight;
                    ispRect.width = 190 * WindowManager2.scale.y;
                }
            }

            GUILayout.Label("New Engine:", leftAlign);
            GUILayout.Space(6 * WindowManager2.scale.y); 
            
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Thrust:", leftAlign, GUILayout.MaxWidth(80 * WindowManager2.scale.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            thrust = GUILayout.TextArea(thrust, leftAlignTextField, GUILayout.MaxWidth(100 * WindowManager2.scale.y));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ISP:", leftAlign, GUILayout.MaxWidth(80 * WindowManager2.scale.y));
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            isp2 = GUILayout.TextArea(isp2, leftAlignTextField, GUILayout.MaxWidth(100 * WindowManager2.scale.y));
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Add Engine", midAlign, GUILayout.MaxWidth(175 * WindowManager2.scale.y)))
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
            if (GUILayout.Button("Result", midAlign, GUILayout.Width(83 * WindowManager2.scale.y)) && engines.Count > 0)
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

            if (GUILayout.Button("Clear", midAlign, GUILayout.Width(82 * WindowManager2.scale.y)))
            {
                engines.Clear();
                isp2 = null;
                thrust = null;
                ispResult = null;
                ispRect.height = 160 * WindowManager2.scale.y;
                ispRect.width = 190 * WindowManager2.scale.y;
            }
            GUILayout.EndHorizontal();

            if (ispResult != null)
            {
                ispRect.height += 20 * WindowManager2.scale.y;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Avg. ISP:", leftAlign);
                GUILayout.EndVertical();
                GUILayout.Label(ispResult, leftAlignTextField, GUILayout.Width(100 * WindowManager2.scale.y));
                GUILayout.EndHorizontal();
            }

            GUI.DragWindow();
        }

        public void OnGUI()
        { 
            if (showCalc && !Main.menuOpen && VideoSettingsPC.main.uiOpacitySlider.value != 0)
            {
                GUI.color = Config3.windowColor;
                Rect oldCalc = calcRect;
                calcRect = GUI.Window(WindowManager2.GetValidID(), calcRect, calcFunc, "ΔV Calculator");
                calcRect = WindowManager2.ConfineRect(calcRect);
                if (calcRect != oldCalc)
                {
                    WindowManager2.settings["dvCalc"]["x"] = calcRect.x;
                    WindowManager2.settings["dvCalc"]["y"] = calcRect.y;
                }
                if (showIsp)
                {
                    Rect oldIsp = ispRect;
                    ispRect = GUI.Window(WindowManager2.GetValidID(), ispRect, ispFunc, "ISP Averager");
                    ispRect = WindowManager2.ConfineRect(ispRect);
                    if (ispRect != oldIsp)
                    {
                        WindowManager2.settings["ispAverager"]["x"] = ispRect.x;
                        WindowManager2.settings["ispAverager"]["y"] = ispRect.y;
                    }
                }
            }
        }
    }
}
