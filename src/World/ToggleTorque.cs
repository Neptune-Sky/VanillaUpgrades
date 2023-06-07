using HarmonyLib;
using SFS.UI;
using SFS.World;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Rocket), "GetTurnAxis")]
    public class TorquePatch
    {
        private static void Postfix(ref float __result) => 
            __result = ToggleTorque.disableTorque && !WorldManager.currentRocket.arrowkeys.rcs.Value ? 0f : __result;
    }

    [HarmonyPatch(typeof(GameManager), "ClearWorld")]
    public class ClearWorld
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }

    [HarmonyPatch(typeof(GameManager), "ExitToBuild")]
    public class ExitToBuild
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }

    [HarmonyPatch(typeof(GameManager), "ExitToHub")]
    public class ExitToHub
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }

    [HarmonyPatch(typeof(GameManager), "RevertToLaunch")]
    public class RevertLaunch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }

    [HarmonyPatch(typeof(GameManager), "RevertToBuild")]
    public class RevertBuild
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }

    public static class ToggleTorque
    {
        public static bool disableTorque;

        public static void Toggle()
        {
            if (!PlayerController.main.HasControl(MsgDrawer.main)) return;

            disableTorque = !disableTorque;
            string enabled = disableTorque ? "Disabled" : "Enabled";

            MsgDrawer.main.Log("Torque " + enabled);
        }
    }
}

