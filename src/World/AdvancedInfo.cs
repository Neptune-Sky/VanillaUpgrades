using System.Collections.Generic;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using UnityEngine;

namespace VanillaUpgrades
{
    public class AdvancedInfo : MonoBehaviour
    {
        private const string posKey = "AdvancedInfoWindow";
        readonly int windowID = Builder.GetRandomID();

        public GameObject windowHolder;
        public Window advancedInfoWindow;

        Container vertical;
        Container horizontal;

        Label apoapsisVertical;
        Label periapsisVertical;
        Label eccentricityVertical;
        Label angleVertical;
        Label apoapsisHorizontal;
        Label periapsisHorizontal;
        Label eccentricityHorizontal;
        Label angleHorizontal;

        void Awake()
        {
            windowHolder = CustomUI.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "AdvancedInfoHolder");

            CreateWindow();
            VerticalGUI(); 
            HorizontalGUI();
            CheckHorizontalToggle();

            PlayerController.main.player.OnChange += OnPlayerChange;
            Config.settingsData.horizontalMode.OnChange += CheckHorizontalToggle;
            Config.settingsData.persistentVars.windowScale.OnChange += () =>
            {
                advancedInfoWindow.rectTransform.localScale = new Vector2(Config.settingsData.persistentVars.windowScale, Config.settingsData.persistentVars.windowScale);
                WindowManager.ClampWindow(advancedInfoWindow);
                WindowManager.Save(posKey, advancedInfoWindow);
            };

            if (!Config.settingsData.showAdvanced) windowHolder.SetActive(false);
        }

        void CreateWindow()
        {
            Config.settingsData.showAdvanced.OnChange += () => 
            { 
                windowHolder.SetActive(Config.settingsData.showAdvanced); 
            };

            Vector2Int pos = Config.settingsData.windowsSavedPosition.GetValueOrDefault(posKey, new Vector2Int(200, 1200));

            advancedInfoWindow = Builder.CreateWindow(windowHolder.gameObject.transform, windowID, 220, 350, pos.x, pos.y, true, true, 1, "Advanced Info");

            RectTransform titleSize = advancedInfoWindow.rectTransform.Find("Title") as RectTransform;
            titleSize.sizeDelta = new Vector2(titleSize.sizeDelta.x, 30);

            advancedInfoWindow.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 0, new RectOffset(5, 0, 0, 0));

            advancedInfoWindow.gameObject.GetComponent<DraggableWindowModule>().OnDropAction += () =>
            {
                WindowManager.ClampWindow(advancedInfoWindow);
                WindowManager.Save(posKey, advancedInfoWindow);
            };

            vertical = Builder.CreateContainer(advancedInfoWindow);
            vertical.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);

            horizontal = Builder.CreateContainer(advancedInfoWindow);
            horizontal.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10);

            vertical.rectTransform.localScale = new Vector2(1, 0.99f);
            horizontal.rectTransform.localScale = new Vector2(1, 0.98f);

            advancedInfoWindow.rectTransform.localScale = new Vector2(Config.settingsData.persistentVars.windowScale, Config.settingsData.persistentVars.windowScale);

            WindowManager.ClampWindow(advancedInfoWindow);
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

        void CheckHorizontalToggle()
        {
            if (Config.settingsData.horizontalMode)
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
            WindowManager.ClampWindow(advancedInfoWindow);
        }

        void VerticalGUI()
        {
            Builder.CreateSeparator(vertical, 205);

            CustomUI.LeftAlignedLabel(vertical, 140, 30, "Apoapsis:");
            apoapsisVertical = CustomUI.LeftAlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            CustomUI.LeftAlignedLabel(vertical, 140, 30, "Periapsis:");
            periapsisVertical = CustomUI.LeftAlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            CustomUI.LeftAlignedLabel(vertical, 140, 30, "Eccentricity:");
            eccentricityVertical = CustomUI.LeftAlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            CustomUI.LeftAlignedLabel(vertical, 140, 30, "Angle:");
            angleVertical = CustomUI.LeftAlignedLabel(vertical, 175, 30);
        }

        void HorizontalGUI()
        {
            Builder.CreateSeparator(horizontal, 335);
            Container apoapsisContainer = Builder.CreateContainer(horizontal);
            apoapsisContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            CustomUI.LeftAlignedLabel(apoapsisContainer, 150, 30, "Apoapsis:");
            apoapsisHorizontal = CustomUI.LeftAlignedLabel(apoapsisContainer, 175, 30);

            Container periapsisContainer = Builder.CreateContainer(horizontal);
            periapsisContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            CustomUI.LeftAlignedLabel(periapsisContainer, 150, 30, "Periapsis:");
            periapsisHorizontal = CustomUI.LeftAlignedLabel(periapsisContainer, 175, 30);

            Container eccentricityContainer = Builder.CreateContainer(horizontal);
            eccentricityContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            CustomUI.LeftAlignedLabel(eccentricityContainer, 150, 30, "Eccentricity:");
            eccentricityHorizontal = CustomUI.LeftAlignedLabel(eccentricityContainer, 175, 30);

            Container angleContainer = Builder.CreateContainer(horizontal);
            angleContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            CustomUI.LeftAlignedLabel(angleContainer, 150, 30, "Angle:");
            angleHorizontal = CustomUI.LeftAlignedLabel(angleContainer, 175, 30);
        }

        void RefreshLabels(Label apoapsis, Label periapsis, Label eccentricity, Label angle)
        {
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

            float trueAngle = WorldManager.currentRocket.partHolder.transform.eulerAngles.z;

            if (trueAngle > 180) angle.Text = (360 - trueAngle).ToString("F1") + "°";
            else angle.Text = (-trueAngle).ToString("F1") + "°";
        }

        void Update()
        {
            if (WorldManager.currentRocket == null) return;

            if (Config.settingsData.horizontalMode) 
                RefreshLabels(apoapsisHorizontal, periapsisHorizontal, eccentricityHorizontal, angleHorizontal);
            else 
                RefreshLabels(apoapsisVertical, periapsisVertical, eccentricityVertical, angleVertical);

        }
    }
}
