using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SFS.World;
using System.IO;
using System.Reflection.Emit;

namespace ASoD_s_VanillaUpgrades
{
    /*
    [HarmonyPatch(typeof(Trajectory), "CalculatePaths")]
    public class MoreTrajectories
    {
        
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            // codes[14].opcode = OpCodes.Ldsfld;
            // codes[14].operand = Config.test;
            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof (Trajectory), "GetNextPath")]
    public class AllowSameSOIPaths
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            codes.RemoveRange(40, 2);
            return codes.AsEnumerable();
        }
    }
    */
}
