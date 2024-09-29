using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SFS;
using SFS.Builds;
using SFS.Input;
using SFS.Translations;
using SFS.UI;
using SFS.World;
using UnityEngine.SceneManagement;

// ReSharper disable UnusedMember.Global

namespace VanillaUpgrades
{
    
    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnClose))]
    public class UpdateConfigOnMenuClose
    {
        public static void Postfix()
        {
            Config.Save();
            if (SceneManager.GetActiveScene().name != "Build_PC") return;
            if (Config.settings.moreCameraZoom)
            {
                if (BuildManager.main.buildCamera.maxCameraDistance == 300) return;
                BuildManager.main.buildCamera.maxCameraDistance = 300;
                BuildManager.main.buildCamera.minCameraDistance = 0.1f;
            }
            else
            {
                if (BuildManager.main.buildCamera.maxCameraDistance == 60) return;
                BuildManager.main.buildCamera.maxCameraDistance = 60;
                BuildManager.main.buildCamera.minCameraDistance = 10f;
            }
        }
    }
    
    [HarmonyPatch(typeof(BuildManager), nameof(BuildManager.OpenMenu))]
    public static class BuildManager_OpenMenu
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code)
        {
            MethodInfo method =
                typeof(BuildManager_OpenMenu).GetMethod(nameof(DoStuff), BindingFlags.Public | BindingFlags.Static);
            MethodInfo toArray = typeof(List<MenuElement>).GetMethod(nameof(List<MenuElement>.ToArray), BindingFlags.Public | BindingFlags.Instance);
            foreach (CodeInstruction instruction in code)
            {
                if (instruction.Calls(toArray))
                {
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 2);
                    yield return new CodeInstruction(OpCodes.Call, method);
                }
                
                yield return instruction;
            }
        }

        public static void DoStuff(ref List<MenuElement> elements, ref SizeSyncerBuilder.Carrier carrier2)
        {
            elements.Add(ButtonBuilder.CreateButton(carrier2, () => Loc.main.Exit_To_Main_Menu, () =>
            {
                BuildState.main.UpdatePersistent();
                Base.sceneLoader.LoadHomeScene(null);
            }, CloseMode.Current));
        }
    }
    
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.OpenMenu))]
    public static class GameManager_OpenMenu
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> code)
        {
            MethodInfo method =
                typeof(GameManager_OpenMenu).GetMethod(nameof(DoStuff), BindingFlags.Public | BindingFlags.Static);
            MethodInfo toArray = typeof(List<MenuElement>).GetMethod(nameof(List<MenuElement>.ToArray), BindingFlags.Public | BindingFlags.Instance);
            foreach (CodeInstruction instruction in code)
            {
                if (instruction.Calls(toArray))
                {
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 2);
                    yield return new CodeInstruction(OpCodes.Call, method);
                }
                
                yield return instruction;
            }
        }
        
        public static void DoStuff(ref List<MenuElement> elements, ref SizeSyncerBuilder.Carrier carrier2)
        {
            elements.Add(ButtonBuilder.CreateButton(carrier2, () => Loc.main.Exit_To_Main_Menu, () =>
            {
                Traverse.Create(GameManager.main).Method("UpdatePersistent", true, false, false).GetValue();
                Base.sceneLoader.LoadHomeScene(null);
            }, CloseMode.Current));
        }
    }
}