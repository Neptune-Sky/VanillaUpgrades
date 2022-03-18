﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS.UI;
using UnityEngine;
using SFS.World;
using SFS.Variables;

namespace ASoD_s_VanillaUpgrades
{
    public class FaceDirection : MonoBehaviour
    {
        public Float_Local turn_Axis = new Float_Local();
        public static Arrowkeys_Local arrowkeys = new Arrowkeys_Local();
        public static int mode = 0;
        public static float e;

        public static void Mode(int toMode)
        {
            return;
            if (toMode == mode)
            {
                mode = 0;
                MsgDrawer.main.Log("Manual control restored");
                arrowkeys.Value.turnAxis.Value = 0f;
                return;
            } else
            {
                mode = toMode;
            }
            switch (mode)
            {
                case 1:
                    MsgDrawer.main.Log("Facing prograde...");
                    break;
                case 2:
                    MsgDrawer.main.Log("Facing retrograde...");
                    break;
                case 3:
                    MsgDrawer.main.Log("Facing radial in...");
                    break;
                case 4:
                    MsgDrawer.main.Log("Facing radial out...");
                    break;
            }
        }
        public void Update()
        {
            return;
            int subtractor = 0;
            int forceDir = 0;
            Vector2 velo = AdvancedInfo.currentRocket.physics.location.velocity.Value;
            e = Mathf.Atan2(velo.x, velo.y) * Mathf.Rad2Deg;
            switch (mode)
            {
                case 0:
                    return;
                case 1:
                    break;
                case 2:
                    e += 180;
                    if (e > 360) e -= 360;
                    break;
                case 3:
                    e += 90;
                    if (e > 360) e -= 360;
                    break;
                case 4:
                    e -= 90;
                    if (e < 0) e += 360;
                    break;
            }
            var angularVelo = AdvancedInfo.currentRocket.rb2d.angularVelocity;
            
            
            arrowkeys.Value = AdvancedInfo.currentRocket.arrowkeys;
            var angle = AdvancedInfo.instance.angle;
            if (angle > e + 1)
            {
                arrowkeys.Value.turnAxis.Value = -1f;
            }
            else if (angle < e - 1)
            {
                arrowkeys.Value.turnAxis.Value = 1f;
            } else
            {
                arrowkeys.Value.turnAxis.Value = 0f;
            }
            

            if (angle < e + 1 && angle > e - 1)
            {

                AdvancedInfo.currentRocket.rb2d.angularVelocity = 0;
            }
        }
    }
}