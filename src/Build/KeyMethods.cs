using HarmonyLib;
using SFS;
using SFS.Builds;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(PickCategoriesMenu), "Start")]
    public static class KeyMethods
    {
        public static PickCategoriesMenu pickCategoriesMenu;

        static void Prefix(PickCategoriesMenu __instance)
        {
            pickCategoriesMenu = __instance;
        }
        public static void Launch()
        {
            BuildState.main.UpdatePersistent();
            Base.sceneLoader.LoadWorldScene(true);
        }
    }
}