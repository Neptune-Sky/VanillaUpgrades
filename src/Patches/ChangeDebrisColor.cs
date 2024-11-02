using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SFS.World;
using SFS.World.Maps;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(MapIcon), "UpdateAlpha")]
    internal static class Alpha
    {
        private static MapIcon _obj;

        private static void Prefix(MapIcon __instance)
        {
            _obj = __instance;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].OperandIs(1f))
                    codes[i] = new CodeInstruction(OpCodes.Call, typeof(Alpha).GetMethod("CheckForControl",
                        BindingFlags.Static | BindingFlags.Public));

            return codes.AsEnumerable();
        }

        public static float CheckForControl()
        {
            var rocket = _obj.gameObject.GetComponent<Rocket>();
            if (rocket == null || !Config.settings.darkenDebris) return 1f;
            return rocket.hasControl ? 1f : 0.5f;
        }
    }

    [HarmonyPatch(typeof(RocketManager), "CreateRocket")]
    internal static class ChildRocketsGetColored
    {
        private static void Postfix(Rocket __result)
        {
            Traverse.Create(__result.mapIcon).Method("UpdateAlpha").GetValue();
        }
    }

    [HarmonyPatch(typeof(Staging), "OnSplit")]
    internal static class NewRocketsGetColored
    {
        private static void Postfix(Rocket parentRocket, Rocket childRocket)
        {
            Traverse.Create(parentRocket.mapIcon).Method("UpdateAlpha").GetValue();
            Traverse.Create(childRocket.mapIcon).Method("UpdateAlpha").GetValue();
        }
    }

    [HarmonyPatch(typeof(RocketManager), "MergeRockets")]
    internal static class DockedRocketsGetColored
    {
        private static void Postfix(Rocket rocket_A)
        {
            Traverse.Create(rocket_A.mapIcon).Method("UpdateAlpha").GetValue();
        }
    }
}