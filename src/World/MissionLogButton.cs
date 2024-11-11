using SFS.Audio;
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
                if (selected is MapPlayer)
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
            MapRocket mapObject = GameSelector.main.selected.Value != null ? GameSelector.main.selected.Value as MapRocket: Map.view.view.target.Value as MapRocket;
            
            mapObject.rocket.stats.branch
            
        }
    }
}