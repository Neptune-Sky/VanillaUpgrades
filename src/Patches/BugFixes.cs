using HarmonyLib;
using SFS.Builds;
using SFS.UI;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace VanillaUpgrades
{
    // Stops the camera from zooming while the cursor isn't in the game window.
    [HarmonyPatch]
    internal class ZoomFix
    {
        private static bool CheckMouseBounds()
        {
            Rect screenRect = new (0, 0, Screen.width, Screen.height);
            return screenRect.Contains(Input.mousePosition);
        }

        [HarmonyPatch(typeof(BuildManager), "OnZoom")]
        [HarmonyPatch(typeof(PlayerController), "OnZoom")]
        [HarmonyPatch(typeof(MapView), "OnZoom")]
        [HarmonyPrefix]
        private static bool WorldMap()
        {
            return CheckMouseBounds();
        }
    }
    
    // Fixes throttle fill bar being offset from actual value
    [HarmonyPatch(typeof(ThrottleDrawer), "UpdatePercentUI")]
    internal class ThrottleFillBarAccuracyFix
    {
        private static bool Prefix(Throttle_Local ___throttle, FillSlider ___throttleSlider, TextAdapter ___throttlePercentText)
        {
            var value = ___throttle.Value.throttlePercent.Value;
            ___throttlePercentText.Text = value.ToPercentString();
            ___throttleSlider.SetFillAmount(0.16f + (value * 0.68f), false);
            return false;
        }
    }
}