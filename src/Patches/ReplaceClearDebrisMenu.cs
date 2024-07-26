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
        private static readonly List<Rocket> Rockets = GameManager.main.rockets;
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
                        ClearDebris(false, true);
                    }, CloseMode.Stack),
                
                ButtonBuilder.CreateButton(null,
                    () => "Cancel", null, CloseMode.Current)
                );
            return false;
        }

        private static void ClearDebris(bool onlyUnnamed = true, bool destroyControllable = false)
        {
            for (var num = Rockets.Count - 1; num >= 0; num--)
            {
                if (onlyUnnamed && Rockets[num].rocketName != "") continue;
                if (!destroyControllable && Rockets[num].hasControl.Value) continue;
                
                RocketManager.DestroyRocket(Rockets[num], DestructionReason.Intentional);
            }
        }
    }
}