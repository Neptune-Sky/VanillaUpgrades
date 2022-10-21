using System.Collections.Generic;
using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.Parts.Modules;
using SFS.UI;
using SFS.UI.ModGUI;
using UnityEngine;
namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(PickCategoriesMenu), "Start")]
    class OpenPickCategories
    {
        public static PickCategoriesMenu inst;
        static void Prefix (PickCategoriesMenu __instance)
        {
            inst = __instance;
        }
    }

    public class KeybindMethods
    {
        public static void Launch()
        {
            BuildState.main.UpdatePersistent();
            Base.sceneLoader.LoadWorldScene(true);
        }

        public static void OpenPickCategory()
        {

        }
    }
}
