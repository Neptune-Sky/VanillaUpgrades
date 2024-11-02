using SFS.UI;
using SFS.World;
using UnityEngine;
using static VanillaUpgrades.HoverHandler;

namespace VanillaUpgrades
{
    internal static partial class WorldManager
    {
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

        private static void HideTopLeftButtonText()
        {
            GameObject TopLeftPanel = GameObject.Find("Top Left Panel");
            foreach (var textAdapter in TopLeftPanel.gameObject.GetComponentsInChildren<TextAdapter>(true))
            {
                textAdapter.gameObject.SetActive(!Config.settings.hideTopLeftButtonText);
            }
        }
    }
}