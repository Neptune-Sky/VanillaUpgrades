using SFS.UI.ModGUI;
using SFS.World;
using UITools;
using UnityEngine;
using Type = SFS.UI.ModGUI.Type;

namespace VanillaUpgrades
{
    public class AdvancedInfo : MonoBehaviour
    {
        const string PositionKey = "VU.AdvancedInfoWindow";

        public GameObject windowHolder;
        public Window advancedInfoWindow;
        Label angleHorizontal;
        Label angleVertical;
        Label apoapsisHorizontal;

        Label apoapsisVertical;
        Label eccentricityHorizontal;
        Label eccentricityVertical;
        Container horizontal;
        Label periapsisHorizontal;
        Label periapsisVertical;

        Container vertical;

        void Awake()
        {
            windowHolder = UIExtensions.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "AdvancedInfoHolder");

            CreateWindow();
            VerticalGUI();
            HorizontalGUI();
            CheckHorizontalToggle();

            PlayerController.main.player.OnChange += OnPlayerChange;
            Config.settings.horizontalMode.OnChange += CheckHorizontalToggle;
            Config.settings.persistentVars.windowScale.OnChange += () => advancedInfoWindow.ScaleWindow();

            if (!Config.settings.showAdvanced) windowHolder.SetActive(false);
        }

        void Update()
        {
            if (WorldManager.currentRocket == null) return;

            if (Config.settings.horizontalMode)
                RefreshLabels(apoapsisHorizontal, periapsisHorizontal, eccentricityHorizontal, angleHorizontal);
            else
                RefreshLabels(apoapsisVertical, periapsisVertical, eccentricityVertical, angleVertical);
        }

        void CreateWindow()
        {
            Config.settings.showAdvanced.OnChange += () => { windowHolder.SetActive(Config.settings.showAdvanced); };

            advancedInfoWindow = Builder.CreateWindow(windowHolder.gameObject.transform, 0, 220, 350, 200, 1200, true,
                false, 1, "Advanced Info");
            advancedInfoWindow.RegisterPermanentSaving(PositionKey);

            RectTransform titleSize = advancedInfoWindow.rectTransform.Find("Title") as RectTransform;
            titleSize.sizeDelta = new Vector2(titleSize.sizeDelta.x, 30);

            advancedInfoWindow.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 0, new RectOffset(5, 0, 0, 0));

            advancedInfoWindow.RegisterOnDropListener(() => advancedInfoWindow.ClampWindow());

            vertical = Builder.CreateContainer(advancedInfoWindow);
            vertical.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);

            horizontal = Builder.CreateContainer(advancedInfoWindow);
            horizontal.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10);

            vertical.rectTransform.localScale = new Vector2(1, 0.99f);
            horizontal.rectTransform.localScale = new Vector2(1, 0.98f);

            advancedInfoWindow.ScaleWindow();
            advancedInfoWindow.ClampWindow();
        }

        void OnPlayerChange()
        {
            if (PlayerController.main == null) return;
            if (WorldManager.currentRocket == null)
            {
                windowHolder.SetActive(false);
                return;
            }

            if (Config.settings.showAdvanced) windowHolder.SetActive(true);
        }

        void CheckHorizontalToggle()
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

        void VerticalGUI()
        {
            Builder.CreateSeparator(vertical, 205);

            UIExtensions.AlignedLabel(vertical, 140, 30, "Apoapsis:");
            apoapsisVertical = UIExtensions.AlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            UIExtensions.AlignedLabel(vertical, 140, 30, "Periapsis:");
            periapsisVertical = UIExtensions.AlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            UIExtensions.AlignedLabel(vertical, 140, 30, "Eccentricity:");
            eccentricityVertical = UIExtensions.AlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            UIExtensions.AlignedLabel(vertical, 140, 30, "Angle:");
            angleVertical = UIExtensions.AlignedLabel(vertical, 175, 30);
        }

        void HorizontalGUI()
        {
            Builder.CreateSeparator(horizontal, 335);
            Container apoapsisContainer = Builder.CreateContainer(horizontal);
            apoapsisContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(apoapsisContainer, 150, 30, "Apoapsis:");
            apoapsisHorizontal = UIExtensions.AlignedLabel(apoapsisContainer, 175, 30);

            Container periapsisContainer = Builder.CreateContainer(horizontal);
            periapsisContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(periapsisContainer, 150, 30, "Periapsis:");
            periapsisHorizontal = UIExtensions.AlignedLabel(periapsisContainer, 175, 30);

            Container eccentricityContainer = Builder.CreateContainer(horizontal);
            eccentricityContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(eccentricityContainer, 150, 30, "Eccentricity:");
            eccentricityHorizontal = UIExtensions.AlignedLabel(eccentricityContainer, 175, 30);

            Container angleContainer = Builder.CreateContainer(horizontal);
            angleContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(angleContainer, 150, 30, "Angle:");
            angleHorizontal = UIExtensions.AlignedLabel(angleContainer, 175, 30);
        }

        void RefreshLabels(Label apoapsis, Label periapsis, Label eccentricity, Label angle)
        {
            if (WorldManager.currentRocket.physics.GetTrajectory().paths[0] is Orbit orbit)
            {
                apoapsis.Text = (orbit.apoapsis - WorldManager.currentRocket.location.planet.Value.Radius)
                    .ToDistanceString();

                double truePeriapsis =
                    orbit.periapsis < WorldManager.currentRocket.location.planet.Value.Radius
                        ? 0
                        : orbit.periapsis - WorldManager.currentRocket.location.planet.Value.Radius;
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
    }
}

