using HarmonyLib;
using SFS;
using SFS.Builds;

namespace VanillaUpgrades
{

    [HarmonyPatch(typeof(PickCategoriesMenu), "Start")]
    static class OpenPickCategories
    {
        public static PickCategoriesMenu inst;

        static void Prefix(PickCategoriesMenu __instance)
        {
            inst = __instance;
        }
    }

    public static class KeyMethods
    {
        public static void Launch()
        {
            BuildState.main.UpdatePersistent();
            Base.sceneLoader.LoadWorldScene(true);
        }
    }
}