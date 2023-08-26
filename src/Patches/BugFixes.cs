using HarmonyLib;
using SFS.Builds;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;

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
}