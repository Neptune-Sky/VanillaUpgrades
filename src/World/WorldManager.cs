using SFS.World;

namespace VanillaUpgrades
{
    public class WorldManager
    {
        public static Rocket currentRocket = (PlayerController.main.player.Value as Rocket);

        public static void Throttle01()
        {
            if (currentRocket == null) return;
            currentRocket.throttle.throttlePercent.Value = 0.0005f;
        }

        public static void Setup()
        {
            PlayerController.main.player.OnChange += () => currentRocket = PlayerController.main.player.Value as Rocket;
            Config.settingsData.allowTimeSlowdown.OnChange += TimeDecelMain.ToggleChange;
        }
    }
}
