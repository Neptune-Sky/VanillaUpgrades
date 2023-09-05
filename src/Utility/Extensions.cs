using System.Collections.Generic;
using SFS.UI;
using Button = SFS.UI.ModGUI.Button;

// ReSharper disable once CheckNamespace
namespace VanillaUpgrades
{
    public static class Extensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static void SetSelected(this Button button, bool selected = true)
        {
            button.gameObject.GetComponentInChildren<ButtonPC>().SetSelected(selected);
        }
    }
}

