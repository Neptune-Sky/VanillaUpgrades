using System;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = SFS.UI.ModGUI.Button;
using Object = UnityEngine.Object;

namespace VanillaUpgrades.Utility;

public static class UIExtensions
{
    private static RectTransform canvas;

    public static Vector2 CanvasPixelSize
    {
        get
        {
            canvas ??= GetCanvasRect();
            return canvas.sizeDelta;
        }
    }

    public static Label AlignedLabel(Transform parent, int width, int height, string labelText = "",
        TextAlignmentOptions textAlignment = TextAlignmentOptions.MidlineLeft)
    {
        Label label = Builder.CreateLabel(parent, width, height, text: labelText);
        label.TextAlignment = textAlignment;
        return label;
    }

    public static GameObject ZeroedHolder(Builder.SceneToAttach mode, string name)
    {
        GameObject holder = Builder.CreateHolder(mode, name);
        var rectTransform = holder.gameObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.position = Vector2.zero;
        return holder;
    }

    private static RectTransform GetCanvasRect()
    {
        GameObject temp = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "TEMP");
        var result = temp.transform.parent as RectTransform;
        Object.Destroy(temp);
        return result;
    }

    public static void ClampWindow(this Window window)
    {
        Vector2 pos = window.Position;
        Vector2 size = window.Size;
        Vector2 gameSize = CanvasPixelSize;

        size.x *= Config.settings.persistentVars.windowScale;
        size.y *= Config.settings.persistentVars.windowScale;

        pos.x = Mathf.Clamp(pos.x, size.x / 2, gameSize.x - size.x / 2);
        pos.y = Mathf.Clamp(pos.y, size.y, gameSize.y);
        window.Position = pos;
    }

    public static void ScaleWindow(this Window input)
    {
        input.rectTransform.localScale = new Vector2(Config.settings.persistentVars.windowScale.Value,
            Config.settings.persistentVars.windowScale.Value);
    }

    public static Button ButtonForVanillaUI(out GameObject buttonObject, Transform parent, int width, int height,
        float fontSize, Action onClick = null, string text = "")
    {
        Button button = Builder.CreateButton(parent, width, height, 0, 0, onClick, text);
        buttonObject = button.gameObject;
        var layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.minWidth = width;
        layoutElement.minHeight = height;

        buttonObject.GetComponentInChildren<TextMeshProUGUI>().fontSizeMax = fontSize;
        buttonObject.transform.Find("BackOverTint")?.gameObject.SetActive(false);

        return button;
    }
}