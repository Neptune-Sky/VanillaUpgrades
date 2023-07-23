using HarmonyLib;
using SFS;
using UnityEngine;

namespace VanillaUpgrades
{
    public static class OpacityChanger
    {

        public static void HideUI()
        {
            GameObject pickGrid = GameObject.Find("Canvas - PickGrid");
            GameObject ui = GameObject.Find("--- UI ---");

            if (pickGrid != null)
            {
                pickGrid.GetComponent<Canvas>().enabled = !pickGrid.GetComponent<Canvas>().enabled;
            }

            if (ui != null)
            {
                ui.GetComponent<Canvas>().enabled = !ui.GetComponent<Canvas>().enabled;
            }
        }
    }
}

