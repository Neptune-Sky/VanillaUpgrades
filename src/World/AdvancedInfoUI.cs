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
    partial class AdvancedInfo
    {
        private void CreateWindow()
        {
            advancedInfoWindow = Builder.CreateWindow(windowHolder.gameObject.transform, 0, 220, 350, 200, 1200, true,
                false, 1, "Advanced Info");
            advancedInfoWindow.RegisterPermanentSaving(PositionKey);

            var titleSize = advancedInfoWindow.rectTransform.Find("Title") as RectTransform;
            titleSize.sizeDelta = new Vector2(titleSize.sizeDelta.x, 30);

            advancedInfoWindow.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 0, new RectOffset(5, 0, 0, 0));

            advancedInfoWindow.RegisterOnDropListener(advancedInfoWindow.ClampWindow);

            vertical = Builder.CreateContainer(advancedInfoWindow);
            vertical.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 0);

            horizontal = Builder.CreateContainer(advancedInfoWindow);
            horizontal.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleLeft, 10);

            vertical.rectTransform.localScale = new Vector2(1, 0.99f);
            horizontal.rectTransform.localScale = new Vector2(1, 0.98f);

            advancedInfoWindow.ScaleWindow();
            advancedInfoWindow.ClampWindow();
        }
        
        private void VerticalGUI()
        {
            Builder.CreateSeparator(vertical, 205);

            UIExtensions.AlignedLabel(vertical, 140, 30, "Apoapsis:");
            infoLabels["apoapsisVertical"] = UIExtensions.AlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            UIExtensions.AlignedLabel(vertical, 140, 30, "Periapsis:");
            infoLabels["periapsisVertical"] = UIExtensions.AlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            UIExtensions.AlignedLabel(vertical, 140, 30, "Eccentricity:");
            infoLabels["eccentricityVertical"] = UIExtensions.AlignedLabel(vertical, 175, 30);

            Builder.CreateSpace(vertical, 0, 10);

            infoLabels["angleTitleVertical"] = UIExtensions.AlignedLabel(vertical, 140, 30, "Angle:");
            infoLabels["angleVertical"] = UIExtensions.AlignedLabel(vertical, 175, 30);
        }

        private void HorizontalGUI()
        {
            Builder.CreateSeparator(horizontal, 335);
            Container apoapsisContainer = Builder.CreateContainer(horizontal);
            apoapsisContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(apoapsisContainer, 150, 30, "Apoapsis:");
            infoLabels["apoapsisHorizontal"] = UIExtensions.AlignedLabel(apoapsisContainer, 175, 30);

            Container periapsisContainer = Builder.CreateContainer(horizontal);
            periapsisContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(periapsisContainer, 150, 30, "Periapsis:");
            infoLabels["periapsisHorizontal"] = UIExtensions.AlignedLabel(periapsisContainer, 175, 30);

            Container eccentricityContainer = Builder.CreateContainer(horizontal);
            eccentricityContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(eccentricityContainer, 150, 30, "Eccentricity:");
            infoLabels["eccentricityHorizontal"] = UIExtensions.AlignedLabel(eccentricityContainer, 175, 30);

            Container angleContainer = Builder.CreateContainer(horizontal);
            angleContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            infoLabels["angleTitleHorizontal"] = UIExtensions.AlignedLabel(angleContainer, 150, 30, "Angle:");
            infoLabels["angleHorizontal"] = UIExtensions.AlignedLabel(angleContainer, 175, 30);
        }
        
        private static void AddToVanillaGUI()
        {
            GameObject thrust = GameObject.Find("Thrust (1)");
            GameObject separator = GameObject.Find("Separator (1)");
            GameObject holder = thrust.transform.parent.gameObject;
            for (var i = 0; i < newStats.Count - 1; i++)
            {
                string key = newStats.Keys.ToArray()[i];
                GameObject sep = GameObject.Instantiate(separator, holder.transform, true);
                infoObjects.Add(sep);
                GameObject Object = GameObject.Instantiate(thrust, holder.transform, true);
                infoObjects.Add(Object);
                
                Object.transform.GetChild(0).gameObject.GetComponent<TextAdapter>().Text = key;
                newStats[key] = Object.transform.GetChild(1).gameObject.GetComponent<TextAdapter>();

                if (i < 2)
                {

                    var rect = Object.transform.GetChild(0).GetComponent<RectTransform>();
                    Object.GetComponent<VerticalLayoutGroup>().childControlWidth = false;
                    rect.sizeDelta = new Vector2(150, rect.sizeDelta.y);
                    Object.transform.GetChild(1).GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
                }
                else if (key == "Angle")
                {
                    newStats["AngleTitle"] = Object.transform.GetChild(0).gameObject.GetComponent<TextAdapter>();
                }

            };
        }
    }
}