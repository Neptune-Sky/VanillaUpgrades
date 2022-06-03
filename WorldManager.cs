using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SFS;
using SFS.World;

namespace VanillaUpgrades
{
    public class WorldManager : MonoBehaviour
    {
        public static Rocket currentRocket = (PlayerController.main.player.Value as Rocket);

        public static void Throttle01()
        {
            if (currentRocket == null) return;
            currentRocket.throttle.throttlePercent.Value = 0.0005f;
        }

        public void Update()
        {
            if (!(bool)Config.settings["allowTimeSlowdown"] && TimeDecelMain.timeDecelIndex != 0)
            {
                WorldTime.main.SetState(1, true, false);
                TimeDecelMain.timeDecelIndex = 0;
            }

            if (TimeDecelMain.timeDecelIndex != 0 && WorldTime.main.timewarpIndex != 0)
            {
                TimeDecelMain.timeDecelIndex = 0;
            }

            if (PlayerController.main.player.Value == null)
            {
                currentRocket = null;
                return;
            } else
            {
                currentRocket = (PlayerController.main.player.Value as Rocket);
            }
        }
    }
}
