using HarmonyLib;
using SFS.Builds;
using SFS.World;
using UnityEngine;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(PlayerController), "ClampTrackingOffset")]
    public class MoreCameraMove
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Vector2 __result, Vector2 newValue)
        {
            if (!Config.settings.moreCameraMove) return true;
            if (PlayerController.main.player.Value == null) return true;
            PlayerController.main.player.Value.ClampTrackingOffset(ref newValue, -30);
            __result = newValue;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerController), "ClampCameraDistance")]
    public class MoreCameraZoom
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, float newValue)
        {
            if (!Config.settings.moreCameraZoom) return true;
            if (PlayerController.main.player.Value == null) return true;
            __result = Mathf.Clamp(newValue, 0.05f, 2.5E+10f);
            return false;
        }
    }
}

