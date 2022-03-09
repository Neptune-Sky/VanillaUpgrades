using HarmonyLib;
using SFS.Audio;
using SFS.UI;
using System;
using UnityEngine;

namespace ASoD_s_VanillaUpgrades
{
    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnOpen))]
    public class ShowSettings
    {
        [HarmonyPostfix]
        public static void Postfix(BasicMenu __instance)
        {
            if (__instance.GetComponent<AudioSettings>())
            {
                Config.showSettings = true;
            }
            Main.menuOpen = true;
        }
    }

    [HarmonyPatch(typeof(BasicMenu), nameof(BasicMenu.OnClose))]
    public class HideSettings
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            Config.showSettings = false;
            Main.menuOpen = false;
        }

    }

    public class Config : MonoBehaviour
    {
        public static string[] prefs;

        public static bool showSettings;
        public static bool showBuildGUI = true;
        public static bool showAdvanced = true;

        public static bool mmUnits = true;
        public static bool kmsUnits = true;
        public static bool ktUnits = true;



        static float windowHeight = 210f;
        public Rect windowRect = new Rect(10f, (float)Screen.height - windowHeight, 230f, windowHeight);

        public static GUIStyle title = new GUIStyle();
        public void Awake()
        {
            if (!PlayerPrefs.HasKey("ASoDVanUpConfig"))
            {
                PlayerPrefs.SetString("ASoDVanUpConfig", "True|True|True|True|True");
            }
            prefs = PlayerPrefs.GetString("ASoDVanUpConfig").Split(char.Parse("|"));

        }


        public void windowFunc(int windowID)
        {

            GUI.Label(new Rect(20f, 20f, 110f, 20f), "GUI:");

            prefs[0] = GUI.Toggle(new Rect(20f, 40f, 210f, 20f), bool.Parse(prefs[0]), " Show Build Settings").ToString();
            showBuildGUI = bool.Parse(prefs[0]);

            prefs[1] = GUI.Toggle(new Rect(20f, 60f, 190f, 20f), bool.Parse(prefs[1]), " Show Advanced Info").ToString();
            showAdvanced = bool.Parse(prefs[1]);

            GUI.Label(new Rect(20f, 90f, 210f, 20f), "Units:");

            prefs[2] = GUI.Toggle(new Rect(20f, 110f, 210f, 20f), bool.Parse(prefs[2]), " Megameters (Mm)").ToString();
            mmUnits = bool.Parse(prefs[2]);

            prefs[3] = GUI.Toggle(new Rect(20f, 130f, 210f, 20f), bool.Parse(prefs[3]), " Kilometers per Second (km/s)").ToString();
            kmsUnits = bool.Parse(prefs[3]);

            prefs[4] = GUI.Toggle(new Rect(20f, 150f, 210f, 20f), bool.Parse(prefs[4]), " Kilotonnes (kt)").ToString();
            ktUnits = bool.Parse(prefs[4]);

            PlayerPrefs.SetString("ASoDVanUpConfig", String.Join("|", prefs));

            if (GUI.Button(new Rect(25f, 180f, 180f, 20f), "Defaults"))
            {
                PlayerPrefs.SetString("ASoDVanUpConfig", "True|True|True|True|True");
                prefs = PlayerPrefs.GetString("ASoDVanUpConfig").Split(char.Parse("|"));
            }

            GUI.DragWindow();

        }

        public void OnGUI()
        {

            if (!showSettings)
            {
                showBuildGUI = bool.Parse(prefs[0]);
                showAdvanced = bool.Parse(prefs[1]);
                mmUnits = bool.Parse(prefs[2]);
                kmsUnits = bool.Parse(prefs[3]);
                ktUnits = bool.Parse(prefs[4]);
                return;
            }

            GUI.Window(GUIUtility.GetControlID(FocusType.Passive), windowRect, windowFunc, "VanillaUpgrades Config");
        }
    }
}
