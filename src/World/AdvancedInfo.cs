using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using TMPro;
using UITools;
using UnityEngine;
using UnityEngine.UI;
using VanillaUpgrades.Utility;
using Type = SFS.UI.ModGUI.Type;
using UIExtensions = VanillaUpgrades.Utility.UIExtensions;

namespace VanillaUpgrades
{
    public partial class AdvancedInfo : MonoBehaviour
    {
        private const string PositionKey = "VU.AdvancedInfoWindow";

        public GameObject windowHolder;
        private Window advancedInfoWindow;

        private Dictionary<string, Label> infoLabels = new()
        {
            {"apopsisHorizontal", null},
            {"apoapsisVertical", null},
            {"periapsisHorizontal", null},
            {"periapsisVertical", null},
            {"eccentricityHorizontal", null},
            {"eccentricityVertical", null},
            {"angleHorizontal", null},
            {"angleVertical", null},
            {"angleTitleHorizontal", null},
            {"angleTitleVertical", null},
        };
        private static Dictionary<string, TextAdapter> newStats = new()
        { 
            {"Apoapsis", null},
            {"Periapsis", null},
            {"Eccentricity", null},
            {"Angle", null},
            {"AngleTitle", null}
        };
        private static readonly List<GameObject> infoObjects = new();

        private Container vertical;
        private Container horizontal;

        private void Awake()
        {
            windowHolder = UIExtensions.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "AdvancedInfoHolder");
            
            AddToVanillaGUI();
            CreateWindow();
            VerticalGUI();
            HorizontalGUI();
            CheckHorizontalToggle();
            

            PlayerController.main.player.OnChange += OnPlayerChange;
            Config.settings.horizontalMode.OnChange += CheckHorizontalToggle;
            Config.settings.persistentVars.windowScale.OnChange += () => advancedInfoWindow.ScaleWindow();
            Config.settings.showAdvanced.OnChange += OnToggle;
            Config.settings.showAdvancedInSeparateWindow.OnChange += OnToggle;

            OnToggle();
        }

        private void Update()
        {
            if (WorldManager.currentRocket == null) return;
            if (Config.settings.showAdvancedInSeparateWindow)
                RefreshLabels(infoLabels);
            else RefreshLabels(newStats) ;
            
        }
        private void OnPlayerChange()
        {
            if (PlayerController.main == null) return;
            OnToggle();
            ToggleTorque.disableTorque = false;
        }

        private void OnToggle()
        {
            if (PlayerController.main == null) return;
            bool value = WorldManager.currentRocket != null && Config.settings.showAdvanced;
            windowHolder.SetActive(value && Config.settings.showAdvancedInSeparateWindow);
            infoObjects.ForEach(e => e.SetActive(value && !Config.settings.showAdvancedInSeparateWindow));
        }

        private void CheckHorizontalToggle()
        {
            if (Config.settings.horizontalMode)
            {
                vertical.gameObject.SetActive(false);
                horizontal.gameObject.SetActive(true);
                advancedInfoWindow.Size = new Vector2(350, 230);
            }
            else
            {
                horizontal.gameObject.SetActive(false);
                vertical.gameObject.SetActive(true);
                advancedInfoWindow.Size = new Vector2(220, 350);
            }

            advancedInfoWindow.ClampWindow();
        }

        private static void GetValues(out string apoapsis, out string periapsis, out string eccentricity,
            out string angleTitle, out string angle)
        {
            Rocket rocket = WorldManager.currentRocket;
            if (rocket.physics.GetTrajectory().paths[0] is Orbit orbit)
            {
                apoapsis = (orbit.apoapsis - rocket.location.planet.Value.Radius)
                    .ToDistanceString();

                double truePeriapsis =
                    orbit.periapsis < rocket.location.planet.Value.Radius
                        ? 0
                        : orbit.periapsis - rocket.location.planet.Value.Radius;
                periapsis = truePeriapsis.ToDistanceString();

                eccentricity = orbit.ecc.ToString("F3", CultureInfo.InvariantCulture);
                
            }
            else
            {
                apoapsis = "0.0m";
                periapsis = "0.0m";
                eccentricity = "0.000";
            }

            float globalAngle = rocket.partHolder.transform.eulerAngles.z;
            Location location = rocket.location.Value;
            
            Vector2 orbitAngleVector = new Vector2(Mathf.Cos((float)location.position.AngleRadians), Mathf.Sin((float)location.position.AngleRadians)).Rotate_Radians(270 * Mathf.Deg2Rad);
            var facing = new Vector2(Mathf.Cos(globalAngle * Mathf.Deg2Rad), Mathf.Sin(globalAngle * Mathf.Deg2Rad));
            float trueAngle = Vector2.SignedAngle(facing, orbitAngleVector);
            
            if (location.TerrainHeight < location.planet.TimewarpRadius_Ascend - rocket.location.planet.Value.Radius)
            {
                angle = trueAngle.ToString("F1", CultureInfo.InvariantCulture) + "°";
                angleTitle = "Local Angle";
                return;
            }
            if (globalAngle > 180) angle = (360 - globalAngle).ToString("F1", CultureInfo.InvariantCulture) + "°";
            else angle = (-globalAngle).ToString("F1", CultureInfo.InvariantCulture) + "°";

            angleTitle = "Angle";
        }
        private static void RefreshLabels<T>(Dictionary<string, T> texts)
        {
            GetValues(out string apo, out string peri, out string ecc, out string angTitle, out string ang);

            if (texts is Dictionary<string, Label> labelDict)
            {
                labelDict["apoapsisVertical"].Text = labelDict["apoapsisHorizontal"].Text = apo;
                labelDict["periapsisVertical"].Text = labelDict["periapsisHorizontal"].Text = peri;
                labelDict["eccentricityVertical"].Text = labelDict["eccentricityHorizontal"].Text = ecc;
                labelDict["angleVertical"].Text = labelDict["angleHorizontal"].Text = ang;
                labelDict["angleTitleVertical"].Text = labelDict["angleTitleHorizontal"].Text = angTitle;
                return;
            }

            if (texts is not Dictionary<string, TextAdapter> adapterDict) throw new ArgumentNullException(nameof(adapterDict));
            adapterDict["Apoapsis"].Text = apo;
            adapterDict["Periapsis"].Text = peri;
            adapterDict["Eccentricity"].Text = ecc;
            adapterDict["Angle"].Text = ang;
            adapterDict["AngleTitle"].Text = angTitle;
        }

        private void OnDestroy()
        {
            infoObjects.Clear();
            PlayerController.main.player.OnChange -= OnPlayerChange;
            Config.settings.horizontalMode.OnChange -= CheckHorizontalToggle;
            Config.settings.persistentVars.windowScale.OnChange -= () => advancedInfoWindow.ScaleWindow();
            Config.settings.showAdvanced.OnChange -= OnToggle;
            Config.settings.showAdvancedInSeparateWindow.OnChange -= OnToggle;
            
        infoLabels = new()
        {
            {"apopsisHorizontal", null},
            {"apoapsisVertical", null},
            {"periapsisHorizontal", null},
            {"periapsisVertical", null},
            {"eccentricityHorizontal", null},
            {"eccentricityVertical", null},
            {"angleHorizontal", null},
            {"angleVertical", null},
            {"angleTitleHorizontal", null},
            {"angleTitleVertical", null},
        };
        newStats = new()
        { 
            {"Apoapsis", null},
            {"Periapsis", null},
            {"Eccentricity", null},
            {"Angle", null},
            {"AngleTitle", null}
        };
        }
    }
}

