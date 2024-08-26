using SFS;
using SFS.Parts.Modules;
using SFS.Sharing;
using SFS.UI;
using SFS.World;
using UnityEngine;

namespace VanillaUpgrades
{
    internal static class WorldManager
    {
        public static Rocket currentRocket;

        public static bool hoverMode;

        public static void Throttle01()
        {
            if (currentRocket == null || !PlayerController.main.HasControl(MsgDrawer.main)) return;
            if (!WorldTime.main.realtimePhysics.Value)
            {
                MsgDrawer.main.Log("Cannot throttle while timewarping");
                return;
            }
            EnableHoverMode(false);
            currentRocket.throttle.throttlePercent.Value = 0.0005f;
        }

        public static void ToggleHoverMode()
        {
            if (hoverMode)
            {
                EnableHoverMode(false);
                return;
            }
            EnableHoverMode();
        }

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
            if (currentRocket == null || !PlayerController.main.HasControl(MsgDrawer.main))
            {
                EnableHoverMode(false, false);
                return;
            }

            float mass = currentRocket.rb2d.mass;
            var localGravity = (float)(currentRocket.location.planet.Value.GetGravity(currentRocket.location.position.Value.magnitude));

            // Calculate thrust at 100% throttle (this method takes into account each engine's direction)
            // Note: Boosters are ignored. But honestly you wouldn't use that functionality with boosters...
            var thrustAt100PerCent = new Vector2(0.0f, 0.0f);

            foreach (EngineModule engineModule in currentRocket.partHolder.GetModules<EngineModule>())
            {
                if (!engineModule.engineOn.Value) continue; // engine has to be on
                Vector2 direction = engineModule.transform.TransformVector(engineModule.thrustNormal.Value);

                // For cheaters...
                var stretchFactor = 1.0f;
                if (Base.worldBase.AllowsCheats) stretchFactor = direction.magnitude;

                Vector2 engineThrust = engineModule.thrust.Value * direction.normalized * stretchFactor;
                thrustAt100PerCent += engineThrust;
            }

            float TwrAt100PerCent = thrustAt100PerCent.magnitude / mass * 9.8f / localGravity;

            if(TwrAt100PerCent < 1.0)
            {
                // Throttle at 100% is not enough to reach TWR of 1 -> set it at 100% (Note: also works if thrust is 0)
                currentRocket.throttle.throttlePercent.Value = 1.0f;
            }
            else
            {
                // Adjust the throttle at the suited value
                currentRocket.throttle.throttlePercent.Value = 1.0f / TwrAt100PerCent;
            }
        }

        private static void UpdatePlayer()
        {
            currentRocket = PlayerController.main.player.Value != null
                ? PlayerController.main.player.Value as Rocket
                : null;
            ToggleTorque.disableTorque = false;
            EnableHoverMode(false, false);
        }

        public static void Setup()
        {
            UpdatePlayer();
            PlayerController.main.player.OnChange += UpdatePlayer;
            Config.settings.allowTimeSlowdown.OnChange += TimeManipulation.ToggleChange;
        }
    }
}

