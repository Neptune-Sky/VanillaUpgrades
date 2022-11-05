using System.Globalization;
using TMPro;

namespace VanillaUpgrades;

public class NumberInput
{
    public double currentVal;
    public double defaultVal;
    public double max;
    public int maxChars;
    public double min;
    public string oldText;
    public TextInput textInput;
}

public class CustomUI
{
    public static NumberInput CreateData(double defaultVal, double min, double max, int maxChars)
    {
        return new NumberInput
        {
            textInput = new TextInput(),
            oldText = defaultVal.ToString(),
            defaultVal = defaultVal,
            currentVal = defaultVal,
            min = min,
            max = max,
            maxChars = maxChars
        };
    }

    public static NumberInput Numberify(NumberInput data)
    {
        try
        {
            double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);
        }
        catch
        {
            if (data.textInput.Text == "." || data.textInput.Text == "") return data;

            data.textInput.Text = data.oldText;
            return data;
        }


        if (data.textInput.Text.Length > data.maxChars) data.textInput.Text = data.oldText;

        double numCheck = double.Parse(data.textInput.Text, CultureInfo.InvariantCulture);

        if (numCheck == 0)
        {
            data.currentVal = data.defaultVal;
        }
        else if (numCheck < data.min)
        {
            data.currentVal = data.min;
            data.textInput.Text = data.min.ToString();
        }
        else if (numCheck > data.max)
        {
            data.currentVal = data.max;
            data.textInput.Text = data.max.ToString();
        }
        else
        {
            data.currentVal = numCheck.Round(0.001);
        }

        data.oldText = data.textInput.Text;
        return data;
    }

    public static Label AlignedLabel(Transform parent, int width, int height, string labelText = "",
        TextAlignmentOptions textAlignment = TextAlignmentOptions.MidlineLeft)
    {
        Label label = Builder.CreateLabel(parent, width, height, text: labelText);
        label.gameObject.GetComponent<TextMeshProUGUI>().alignment = textAlignment;
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
}