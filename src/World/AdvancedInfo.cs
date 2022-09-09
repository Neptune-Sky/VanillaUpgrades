using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VanillaUpgrades
{
    public class AdvancedInfo : MonoBehaviour
    {
        private readonly int windowID = Builder.GetRandomID();
        public GameObject windowHolder;
        public Window advancedInfoWindow;
        bool compacted;
        Label apoapsis;
        Label periapsis;
        Label eccentricity;
        Label angle;
        List<Container> labels = new List<Container>();

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

            advancedInfoWindow = Builder.CreateWindow(windowHolder.gameObject.transform, windowID, 220, 350, (int)pos.x, (int)pos.y, true, true, 1, "Advanced Info");
            WindowManager.ClampWindow(advancedInfoWindow);

            RectTransform titleSize = advancedInfoWindow.rectTransform.Find("Title") as RectTransform;
            titleSize.sizeDelta = new Vector2(titleSize.sizeDelta.x, 30);

            advancedInfoWindow.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 10, new RectOffset(5, 0, 3, 3));

            Container apoapsisContainer = Builder.CreateContainer(advancedInfoWindow);
            apoapsisContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);

            Label apoapsisLabel = Builder.CreateLabel(apoapsisContainer, 150, 30, text: "Apoapsis:");
            apoapsisLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            apoapsis = Builder.CreateLabel(apoapsisContainer, 175, 30, text: "");
            apoapsis.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

            Builder.CreateSeparator(advancedInfoWindow, 210);

            Container periapsisContainer = Builder.CreateContainer(advancedInfoWindow);
            periapsisContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);

            Label periapsisLabel = Builder.CreateLabel(periapsisContainer, 150, 30, text: "Periapsis:");
            periapsisLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            periapsis = Builder.CreateLabel(periapsisContainer, 175, 30, text: "");
            periapsis.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

            Builder.CreateSpace(advancedInfoWindow, 0, 10);

            Container eccentricityContainer = Builder.CreateContainer(advancedInfoWindow);
            eccentricityContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);

            Label eccentricityLabel = Builder.CreateLabel(eccentricityContainer, 150, 30, text: "Eccentricity:");
            eccentricityLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            eccentricity = Builder.CreateLabel(eccentricityContainer, 175, 30, text: "");
            eccentricity.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

            Builder.CreateSpace(advancedInfoWindow, 0, 10);

            Container angleContainer = Builder.CreateContainer(advancedInfoWindow);
            angleContainer.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);

            Label angleLabel = Builder.CreateLabel(angleContainer, 150, 30, text: "Angle:");
            angleLabel.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            angle = Builder.CreateLabel(angleContainer, 175, 30, text: "");
            angle.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

            labels.Add(apoapsisContainer);
            labels.Add(periapsisContainer);
            labels.Add(eccentricityContainer);
            labels.Add(angleContainer);
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

            if ((int)advancedInfoWindow.Position.y < 380 || (int)advancedInfoWindow.Position.y > WindowManager.gameSize.y - 30)
            {
                if (!compacted)
                {
                    labels.ForEach(e =>
                    {
                        if (!e.gameObject.HasComponent<VerticalLayoutGroup>() || e.gameObject.HasComponent<HorizontalLayoutGroup>()) return;
                        Destroy(e.gameObject.GetComponent<VerticalLayoutGroup>());
                        e.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 10);
                    });
                    advancedInfoWindow.Size = new Vector2(400, 250);
                    WindowManager.ClampWindow(advancedInfoWindow);
                    compacted = true;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(advancedInfoWindow.rectTransform);
                }
            }
            else
            {
                if (compacted)
                {
                    labels.ForEach(e =>
                    {
                        if (e.gameObject.HasComponent<VerticalLayoutGroup>() || !e.gameObject.HasComponent<HorizontalLayoutGroup>()) return;
                        Destroy(e.gameObject.GetComponent<HorizontalLayoutGroup>());
                        e.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);
                    });
                    advancedInfoWindow.Size = new Vector2(220, 350);
                    WindowManager.ClampWindow(advancedInfoWindow);
                    compacted = false;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(advancedInfoWindow.rectTransform);
                }
            }
        }
    }
}
