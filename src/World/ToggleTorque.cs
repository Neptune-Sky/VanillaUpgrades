using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using SFS.World;
using SFS;
using SFS.UI;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Rocket), "GetTurnAxis")]
    public class TorquePatch
    {
        [HarmonyPrefix]
        static bool Prefix(bool useStopRotation)
        {
            if (ToggleTorque.disableTorque && useStopRotation && !WorldManager.currentRocket.arrowkeys.rcs.Value)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(GameManager), "ClearWorld")]
    public class ClearWorld
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }
    [HarmonyPatch(typeof(GameManager), "ExitToBuild")]
    public class ExitToBuild
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }
    [HarmonyPatch(typeof(GameManager), "ExitToHub")]
    public class ExitToHub
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }
    [HarmonyPatch(typeof (GameManager), "RevertToLaunch")]
    public class RevertLaunch
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }
    [HarmonyPatch(typeof(GameManager), "RevertToBuild")]
    public class RevertBuild
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            ToggleTorque.disableTorque = false;
        }
    }
    public class ToggleTorque
    {
        public static bool disableTorque;

        public static void toggleTorque()
        {
            string enabled;

            disableTorque = !disableTorque;
            if (disableTorque)
            {
                enabled = "Disabled";
            }
            else
            {
                enabled = "Enabled";
            }

            MsgDrawer.main.Log("Torque " + enabled);
        }
    }
}
