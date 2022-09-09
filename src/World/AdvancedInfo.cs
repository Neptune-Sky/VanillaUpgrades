using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VanillaUpgrades
{
    public class AdvancedInfo : MonoBehaviour
    {
        public GameObject windowHolder;
        public Window advancedInfoWindow;
        Label apoapsis;
        Label periapsis;
        Label eccentricity;
        Label angle;

        void Awake()
        {
            windowHolder = Builder.CreateHolder(Builder.SceneToAttach.CurrentScene, "AdvancedInfoHolder");
            var rect = windowHolder.gameObject.AddComponent<RectTransform>();
            var zero = Vector2.zero;
            rect.anchorMax = zero;
            rect.anchorMin = zero;
            rect.position = zero;

            PlayerController.main.player.OnChange += OnPlayerChange;
            Config.settingsData.showAdvanced.OnChange += OnToggle;
            ShowGUI();

            advancedInfoWindow.gameObject.GetComponent<DraggableWindowModule>().OnDropAction += () => WindowManager.ClampWindow(advancedInfoWindow);

            if (!Config.settingsData.showAdvanced) windowHolder.SetActive(false);
        }

        void OnPlayerChange()
        {
            if (WorldManager.currentRocket == null)
            {
                windowHolder.SetActive(false);
                return;
            }
            if (Config.settingsData.showAdvanced) windowHolder.SetActive(true);
        }

        void OnToggle()
        {
            windowHolder.SetActive(Config.settingsData.showAdvanced);
        }

        void ShowGUI()
        {
            Vector2 pos = Config.settingsData.windowsSavedPosition.GetValueOrDefault("AdvancedInfoWindow", new Vector2(200, 1200));
            advancedInfoWindow = Builder.CreateWindow(windowHolder.gameObject.transform, Builder.GetRandomID(), 250, 350, (int)pos.x, (int)pos.y, true, true, 1, string.Empty);

            advancedInfoWindow.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 0, new RectOffset(5, 0, 3, 3));

            var apoapsisLabel = Builder.CreateLabel(advancedInfoWindow, 150, 30, text: "Apoapsis:");
            apoapsisLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            apoapsis = Builder.CreateLabel(advancedInfoWindow, 175, 30, text: "");
            apoapsis.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;


            Builder.CreateSpace(advancedInfoWindow, 0, 10);

            var periapsisLabel = Builder.CreateLabel(advancedInfoWindow, 150, 30, text: "Periapsis:");
            periapsisLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            periapsis = Builder.CreateLabel(advancedInfoWindow, 175, 30, text: "");
            periapsis.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

            Builder.CreateSpace(advancedInfoWindow, 0, 10);

            var eccentricityLabel = Builder.CreateLabel(advancedInfoWindow, 150, 30, text: "Eccentricity:");
            eccentricityLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            eccentricity = Builder.CreateLabel(advancedInfoWindow, 175, 30, text: "");
            eccentricity.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

            Builder.CreateSpace(advancedInfoWindow, 0, 10);

            var angleLabel = Builder.CreateLabel(advancedInfoWindow, 150, 30, text: "Angle:");
            angleLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            angle = Builder.CreateLabel(advancedInfoWindow, 175, 30, text: "");
            angle.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
        }

        void ScaledWidth(Label label)
        {
            label.Size = new Vector2(label.Text.Length * 12, 30);
        }

        void Update()
        {
            if (WorldManager.currentRocket == null) return;



            if (WorldManager.currentRocket.physics.GetTrajectory().paths[0] is Orbit orbit)
            {
                apoapsis.Text = (orbit.apoapsis - WorldManager.currentRocket.location.planet.Value.Radius).ToDistanceString();

                double truePeriapsis = 
                orbit.periapsis < WorldManager.currentRocket.location.planet.Value.Radius 
                    ? 0 : (orbit.periapsis - WorldManager.currentRocket.location.planet.Value.Radius);
                periapsis.Text = truePeriapsis.ToDistanceString();

                eccentricity.Text = orbit.ecc.ToString("F3");
            }
            else
            {
                apoapsis.Text = "0.0m";
                periapsis.Text = "0.0m";
                eccentricity.Text = "0.000";
            }

            var trueAngle = WorldManager.currentRocket.partHolder.transform.eulerAngles.z;

            if (trueAngle > 180) angle.Text = (360 - trueAngle).ToString("F1") + "°";
            else angle.Text = (-trueAngle).ToString("F1") + "°";
        }
    }
}
