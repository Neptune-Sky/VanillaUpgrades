using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VanillaUpgrades
{
    public static class UIExtensions
    {
        static RectTransform canvas;

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
            RectTransform rectTransform = holder.gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.position = Vector2.zero;
            return holder;
        }

        static RectTransform GetCanvasRect()
        {
            GameObject temp = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "TEMP");
            RectTransform result = temp.transform.parent as RectTransform;
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
    }
}

