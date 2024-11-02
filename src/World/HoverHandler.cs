using SFS;
using SFS.Parts.Modules;
using SFS.UI;
using SFS.World;
using UnityEngine;

namespace VanillaUpgrades
{
    internal static class HoverHandler
    {
        public static void ToggleHoverMode() => EnableHoverMode(!hoverMode);
        public static bool hoverMode;
        public static void EnableHoverMode(bool enable = true, bool showMsg = true)
        {
            if (WorldTime.main.realtimePhysics.Value)
            {
                if (hoverMode == enable) return;
                hoverMode = enable;
                if (hoverMode) TwrTo1();
                if (showMsg) MsgDrawer.main.Log(hoverMode ? "Hovering..." : "Exit hover mode");
                return;
            }
            MsgDrawer.main.Log("Cannot hover while timewarping");
        }

        public static void TwrTo1()
        {
            if (WorldManager.currentRocket == null || !PlayerController.main.HasControl(MsgDrawer.main))
            {
                EnableHoverMode(false, false);
                return;
            }

            var mass = WorldManager.currentRocket.rb2d.mass;
            var localGravity = (float)(WorldManager.currentRocket.location.planet.Value.GetGravity(WorldManager.currentRocket.location.position.Value.magnitude));

            // Calculate thrust at 100% throttle (this method takes into account each engine's direction)
            // Note: Boosters are ignored. But honestly you wouldn't use that functionality with boosters...
            var thrustAt100PerCent = new Vector2(0.0f, 0.0f);

            foreach (var engineModule in WorldManager.currentRocket.partHolder.GetModules<EngineModule>())
            {
                if (!engineModule.engineOn.Value) continue; // engine has to be on
                Vector2 direction = engineModule.transform.TransformVector(engineModule.thrustNormal.Value);

                // For cheaters...
                var stretchFactor = 1.0f;
                if (Base.worldBase.AllowsCheats) stretchFactor = direction.magnitude;

                var engineThrust = engineModule.thrust.Value * direction.normalized * stretchFactor;
                thrustAt100PerCent += engineThrust;
            }

            var twrAt100PerCent = thrustAt100PerCent.magnitude / mass * 9.8f / localGravity;

            if(twrAt100PerCent < 1.0)
            {
                // Throttle at 100% is not enough to reach TWR of 1 -> set it at 100% (Note: also works if thrust is 0)
                WorldManager.currentRocket.throttle.throttlePercent.Value = 1.0f;
            }
            else
            {
                // Adjust the throttle at the suited value
                WorldManager.currentRocket.throttle.throttlePercent.Value = 1.0f / twrAt100PerCent;
            }
        }
    }
}

