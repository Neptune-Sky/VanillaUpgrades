using System.Collections.Generic;
using HarmonyLib;
using SFS.Input;
using SFS.UI;
using SFS.World;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(GameManager), "OpenClearDebrisMenu")]
    internal static class ReplaceClearDebrisMenu
    {
        // ReSharper disable once UnusedMember.Local
        private static bool Prefix()
        {
            MenuGenerator.ShowChoices(() => "What objects would you like to clear?", 
                ButtonBuilder.CreateButton(null,
                    () => "All Unnamed Debris", () =>
                    {
                        ClearDebris();
                    }, CloseMode.Stack),
                ButtonBuilder.CreateButton(null,
                    () => "All Debris", () =>
                    {
                        ClearDebris(false);
                    }, CloseMode.Stack),
                ButtonBuilder.CreateButton(null,
                    () => "All Unnamed Rockets", () =>
                    {
                        ClearDebris(true, true);
                    }, CloseMode.Stack),
                ButtonBuilder.CreateButton(null,
                    () => "Everything", () =>
                    {
                        MenuGenerator.OpenConfirmation(CloseMode.None, () => 
                            "Are you sure you want to delete EVERYTHING in your world? This includes all named rockets and debris.",
                        () => "Yes", () =>
                        {
                            ClearDebris(false, true); 
                            ScreenManager.main.CloseStack();
                        },
                        () => "No", ScreenManager.main.CloseCurrent);
                    }, CloseMode.None),
                
                ButtonBuilder.CreateButton(null,
                    () => "Cancel", null, CloseMode.Current)
                );
            return false;
        }

        private static void ClearDebris(bool onlyUnnamed = true, bool destroyControllable = false)
        {
            var rockets = GameManager.main.rockets;
            for (var num = rockets.Count - 1; num >= 0; num--)
            {
                if (onlyUnnamed && rockets[num].rocketName != "") continue;
                if (!destroyControllable && rockets[num].hasControl.Value) continue;
                
                RocketManager.DestroyRocket(rockets[num], DestructionReason.Intentional);
            }
        }
    }
}