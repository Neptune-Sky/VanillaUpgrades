using SFS.World;

namespace VanillaUpgrades
{
    public class WorldManager
    {
        public static Rocket currentRocket;

        public static void Throttle01()
        {
            if (currentRocket == null) return;
            currentRocket.throttle.throttlePercent.Value = 0.0005f;
        }

        static void UpdatePlayer()
        {
            currentRocket = PlayerController.main.player.Value != null ? PlayerController.main.player.Value as Rocket : null;
        }

        public static void Setup()
        {
            UpdatePlayer();
            PlayerController.main.player.OnChange += UpdatePlayer;
            Config.settingsData.allowTimeSlowdown.OnChange += TimeDecelMain.ToggleChange;
        }
    }
}
