using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SFS.UI;

namespace ASoD_s_VanillaUpgrades
{
    
    
    [HarmonyPatch(typeof(TextInputMenu), nameof(TextInputMenu.OnOpen))]
    public class StopKeyDetection
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Main.KeyBool.StopKeys = true;
        }
    }

    [HarmonyPatch(typeof(TextInputMenu), nameof(TextInputMenu.OnClose))]
    public class RestartKeyDetection
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Main.KeyBool.StopKeys = false;
        }

    }
}
