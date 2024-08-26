using SFS;
using SFS.Parts.Modules;
using SFS.UI;
using SFS.World;
using UnityEngine;

namespace VanillaUpgrades
{
    internal static class WorldManager
    {
        public static Rocket currentRocket;

        public static bool hoverMode = false;

        public static void Throttle01()
        {
            if (currentRocket == null || !PlayerController.main.HasControl(MsgDrawer.main)) return;
            if (!SFS.World.WorldTime.main.realtimePhysics.Value)
            {
                MsgDrawer.main.Log("Cannot throttle while timewarping");
                return;
            }
            ExitHoverMode(true);
            currentRocket.throttle.throttlePercent.Value = 0.0005f;
        }

        public static void EnterHoverMode()
        {
            if (!SFS.World.WorldTime.main.realtimePhysics.Value)
            {
                MsgDrawer.main.Log("Cannot hover while timewarping");
            }
            else
            {
                MsgDrawer.main.Log("Hover mode active");
                hoverMode = true;
                TwrTo1();
            }
        }

        public static void ExitHoverMode(bool showMsg)
        {
            if(hoverMode && showMsg) MsgDrawer.main.Log("Exit hover mode");
            hoverMode = false;
        }

        public static void TwrTo1()
        {
            if (currentRocket == null || !PlayerController.main.HasControl(MsgDrawer.main))
            {
                ExitHoverMode(false);
                return;
            }

            float mass = currentRocket.rb2d.mass;
            float localGravity = (float)(currentRocket.location.planet.Value.GetGravity(currentRocket.location.position.Value.magnitude));

            // Calculate thrust at 100% throttle (this method takes into account each engine's direction)
            // Note: Boosters are ignored. But honestly you wouldn't use that functionality with boosters...
            Vector2 thrustAt100PerCent = new Vector2(0.0f, 0.0f);

            foreach (EngineModule engineModule in currentRocket.partHolder.GetModules<EngineModule>())
            {
                if(engineModule.engineOn.Value) // engine has to be on
                {
                    Vector2 direction = (Vector2)engineModule.transform.TransformVector(engineModule.thrustNormal.Value);

                    // For cheaters...
                    float stretchFactor = 1.0f;
                    if (Base.worldBase.AllowsCheats) stretchFactor = direction.magnitude;

                    Vector2 engineThrust = engineModule.thrust.Value * direction.normalized * stretchFactor;
                    thrustAt100PerCent += engineThrust;
                }
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
            ExitHoverMode(false);
        }

        public static void Setup()
        {
            UpdatePlayer();
            PlayerController.main.player.OnChange += UpdatePlayer;
            Config.settings.allowTimeSlowdown.OnChange += TimeManipulation.ToggleChange;
        }
    }
}

