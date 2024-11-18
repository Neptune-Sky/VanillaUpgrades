using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModLoader.IO;
using SFS.Audio;
using SFS.Logs;
using SFS.Stats;
using SFS.UI;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;

namespace VanillaUpgrades
{
    public static class MissionLogButton
    {
        private static readonly IconButton missionLog = new();
        public static void Create()
        {
            
            GameObject parent = GameSelector.main.focusButton.button.gameObject;
            missionLog.button = Object.Instantiate(parent).GetComponent<Button>();
            GameObject gameObject = missionLog.button.gameObject;
            gameObject.transform.SetParent(parent.transform.parent);
            gameObject.transform.localScale = Vector3.one;
            missionLog.text = missionLog.button.gameObject.GetComponentInChildren<TextAdapter>();
            missionLog.text.Text = "Mission Log";
            missionLog.Show = false;
            GameSelector.main.selected.OnChange += selected =>
            {
                var endMissionButton = GameObject.Find("End Mission Button");
                bool endMissionActive = endMissionButton != null && endMissionButton.activeSelf;
                if (selected is MapPlayer && ! endMissionActive)
                {
                    missionLog.Show = true;
                    return;
                }
                missionLog.Show = false;
            };
            missionLog.button.onClick += Clicked;
        }

        private static void Clicked()
        {
            var mapRocket = GameSelector.main.selected.Value as MapRocket;
            if (mapRocket == null) return;
            Rocket rocket = mapRocket.rocket;
            MethodInfo logsMethod = EndMissionMenu.main.GetType()
                .GetMethod("ReplayMission", BindingFlags.NonPublic | BindingFlags.Static);
            
            var logs = (List<(string, double, LogId)>)logsMethod.Invoke(EndMissionMenu.main, new object[]  {rocket.stats.branch, rocket.location.Value, null, null, null});
               
            foreach (var log in logs)
            {
                Debug.Log(log); 
            }
        }
    }
}