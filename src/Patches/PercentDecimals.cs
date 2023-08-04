using HarmonyLib;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Units), nameof(Units.ToPercentString), typeof(double), typeof(bool))]
    public class PercentDecimals
    {
        // ReSharper disable once RedundantAssignment
        private static bool Prefix(ref string __result, double a, bool forceDecimals = true)
        {
            if (!Config.settings.morePrecisePercentages) return true;
            a *= 100;

            switch (a)
            {
                case >= 100:
                case 0:
                    __result = a.ToString(0, false);
                    break;
                case >= 10:
                    __result = a.ToString(1, true);
                    break;
                case < 0.01:
                    a = 0.01;
                    __result = a.ToString(2, true);
                    break;
                
                default:
                    __result = a.ToString(2, true);
                    break;
            }

            __result += "%";
            return false;
        }
    }
}