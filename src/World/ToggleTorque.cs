using HarmonyLib;
using SFS.UI;
using SFS.World;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Rocket), "GetTorque")]
    public static class ToggleTorque
    {
        public static bool disableTorque;

        public static void Toggle()
        {
            if (!PlayerController.main.HasControl(MsgDrawer.main)) return;
            disableTorque = !disableTorque;
            MsgDrawer.main.Log("Torque " + (disableTorque ? "Disabled" : "Enabled"));
        }
        private static bool Prefix(ref float __result)
        {
            if (!disableTorque) return true;
            __result = 0f;
            return false;
        }
    }
}

