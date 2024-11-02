using SFS.UI;
using SFS.World;
using static VanillaUpgrades.HoverHandler;

namespace VanillaUpgrades
{
    internal static partial class WorldManager
    {
        public static Rocket currentRocket;

        private static void UpdatePlayer()
        {
            currentRocket = PlayerController.main.player.Value != null
                ? PlayerController.main.player.Value as Rocket
                : null;
            ToggleTorque.Set(false);
            EnableHoverMode(false, false);
        }

        public static void Setup()
        {
            UpdatePlayer();
            HideTopLeftButtonText();
            PlayerController.main.player.OnChange += UpdatePlayer;
            Config.settings.allowTimeSlowdown.OnChange += TimeManipulation.ToggleChange;
            Config.settings.hideTopLeftButtonText.OnChange += HideTopLeftButtonText;
            UpdateInGame.execute += () =>
            {
                if (hoverMode) TwrTo1();
            };
        }
    }
}

