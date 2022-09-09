using SFS.UI.ModGUI;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Type = SFS.UI.ModGUI.Type;

namespace VanillaUpgrades
{
    public class NumberInput
    {
        public TextInput textInput;
        public string oldText;
        public double defaultVal;
        public double currentVal;
        public double min;
        public double max;
    }
    public class CustomUI
    {

        public static NumberInput CreateData(double defaultVal, double min, double max)
        {
            return new NumberInput
            {
                textInput = new TextInput(),
                oldText = defaultVal.ToString(),
                defaultVal = defaultVal,
                currentVal = defaultVal,
                min = min,
                max = max
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
                if (data.textInput.Text == "." || data.textInput.Text == "")
                {
                    return data;
                }

                data.textInput.Text = data.oldText;
                return data;
            }


            if (data.textInput.Text.Length > 3)
            {
                data.textInput.Text = data.oldText;
            }

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

        public static Label LeftAlignedLabel(Transform parent, int width, int height, string labelText = "")
        {
            Label label = Builder.CreateLabel(parent, width, height, text: labelText);
            label.gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
            return label;
        }
    }
}
