using SFS.UI;
using SFS.World;

namespace VanillaUpgrades
{
    internal static class WorldManager
    {
        public static Rocket currentRocket;
        

        public static void Throttle01()
        {
            if (currentRocket == null || !PlayerController.main.HasControl(MsgDrawer.main)) return;
            currentRocket.throttle.throttlePercent.Value = 0.0005f;
        }

        private static void UpdatePlayer()
        {
            currentRocket = PlayerController.main.player.Value != null
                ? PlayerController.main.player.Value as Rocket
                : null;
        }

        public static void Setup()
        {
            UpdatePlayer();
            PlayerController.main.player.OnChange += UpdatePlayer;
            Config.settings.allowTimeSlowdown.OnChange += TimeManipulation.ToggleChange;
        }
    }
}

