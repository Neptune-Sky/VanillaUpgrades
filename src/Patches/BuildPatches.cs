using System;
using System.Collections.Generic;
using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using SFS.WorldBase;
using UnityEngine;
using UnityEngine.UI;
using static SFS.UI.ModGUI.Builder;
using Button = SFS.UI.ModGUI.Button;
using Type = SFS.UI.ModGUI.Type;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(PartGrid), "UpdateAdaptation")]
    internal class StopAdaptation
    {
        private static bool Prefix()
        {
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
        }
    }

    [HarmonyPatch(typeof(HoldGrid), "TakePart_PickGrid")]
    internal class AdaptPartPicker
    {
        private static void Prefix()
        {
            BuildSettings.noAdaptOverride = true;
        }

        private static void Postfix()
        {
            BuildSettings.noAdaptOverride = false;
        }
    }


    [HarmonyPatch(typeof(AdaptModule), "UpdateAdaptation")]
    internal class FixCucumber
    {
        private static bool Prefix()
        {
            return !BuildSettings.noAdaptation || BuildSettings.noAdaptOverride;
        }
    }

    [HarmonyPatch(typeof(MagnetModule), nameof(MagnetModule.GetAllSnapOffsets))]
    internal class KillMagnet
    {
        private static bool Prefix(MagnetModule A, MagnetModule B, float snapDistance, ref List<Vector2> __result)
        {
            if (!BuildSettings.noSnapping)
                return true;
            __result = new List<Vector2>();
            return false;
        }
    }

    [HarmonyPatch(typeof(BuildStatsDrawer), "Draw")]
    internal class DisplayCorrectTWR
    {
        private static void Postfix(float ___mass, float ___thrust, TextAdapter ___thrustToWeightText)
        {
            SpaceCenterData spaceCenter = Base.planetLoader.spaceCenter;
            var gravityAtLaunchpad = spaceCenter.address.GetPlanet()
                .GetGravity(spaceCenter.LaunchPadLocation.position.magnitude);
            var TWR = ___thrust * 9.8 / (___mass * gravityAtLaunchpad);
            ___thrustToWeightText.Text = TWR.ToString(2, true);
        }
    }

    [HarmonyPatch(typeof(PickCategoriesMenu))]
    internal static class ReplacePickCategories
    {
        private static Window window;
        private static PickCategoriesMenu inst;

        private static int categories;

        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        private static void Start(PickCategoriesMenu __instance)
        {
            inst = __instance;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetupElements")]
        private static bool SetupElements(IReadOnlyCollection<PickGridUI.CategoryParts> picklists)
        {
            categories = picklists.Count;
            if (categories <= 24) return true;
            List<Button> buttons = new();
            GameObject gameObject = inst.gameObject;
            var rows = Math.Clamp(picklists.Count, 1, 24);
            window = CreateWindow(gameObject.transform, GetRandomID(), 240, 57 * rows, opacity: 0f);

            window.gameObject.transform.Find("Back (Game)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Back (InGame)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Title").gameObject.SetActive(false);
            window.gameObject.transform.Find("Mask").GetComponent<RectMask2D>().rectTransform.offsetMax +=
                new Vector2(0, 40);
            window.gameObject.Rect().offsetMax =
                new Vector2(window.gameObject.Rect().offsetMax.x, 10);

            window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 7f, new RectOffset(0, 0, 0, 0));
            window.EnableScrolling(Type.Vertical);
            foreach (PickGridUI.CategoryParts categoryParts in picklists)
            {
                Button button = CreateButton(window, 225, 50, text: categoryParts.tag.displayName.Field);
                button.gameObject.GetComponentInChildren<TextAdapter>().transform.localScale = new Vector3(0.8f, 0.85f);
                button.OnClick += () =>
                {
                    inst.SelectCategory(categoryParts);
                    foreach (Button b in buttons) b.gameObject.GetComponent<ButtonPC>().SetSelected(false);

                    button.gameObject.GetComponent<ButtonPC>().SetSelected(true);
                };
                buttons.Add(button);
            }

            buttons[0].gameObject.GetComponent<ButtonPC>().SetSelected(true);
            return false;
        }

        // ReSharper disable once RedundantAssignment
        [HarmonyPrefix]
        [HarmonyPatch("SelectCategory")]
        private static bool SelectCategory(PickGridUI.CategoryParts newSelected,
            ref PickGridUI.CategoryParts ___selected)
        {
            if (categories <= 24) return true;
            inst.expandMenu.Close();
            ___selected = newSelected;
            BuildManager.main.pickGrid.OpenCategory(newSelected);
            return false;
        }
    }
}