using System;
using UnityEngine;
using UnityEngine.UI;

namespace ASoD_s_VanillaUpgrades
{
    public class ErrorNotification : MonoBehaviour
    {
        public static string errors;

        public static void Error(string error)
        {
            errors += $"- {error}\n\n";
        }

        public static Vector2 errorSize = new Vector2(350f, 300f);
        public Rect windowRect = new Rect((Screen.width * 0.5f) - errorSize.x * 0.5f, (Screen.height * 0.5f) - errorSize.y * 0.5f, errorSize.x, errorSize.y);

        public void windowFunc(int windowID)
        {
            GUI.Box(new Rect(10, 20, windowRect.width - 20, windowRect.height - 50), "");
            GUI.Label(new Rect(20, 20, 310, 50), "An error occured while loading VanillaUpgrades.");
            GUI.Label(new Rect(20, 70, 310, 220), errors);

            if (GUI.Button(new Rect(290, 275, 50, 20), "Okay"))
            {
                errors = null;
            }

            GUI.DragWindow();
        }
        public void OnGUI()
        {
            if (errors == null) return;

            windowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), windowRect, windowFunc, "Uh oh!");
        }
    }
}
