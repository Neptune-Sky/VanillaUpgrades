using System;
using System.Text;
using SFS.UI;
using UnityEngine;

namespace VanillaUpgrades;

public class ErrorNotification : MonoBehaviour
{
    private static readonly StringBuilder errors = new();

    private void Start()
    {
        if (errors.Length == 0) return;
        errors.Insert(0,
            "An error occured while loading VanillaUpgrades." + Environment.NewLine + Environment.NewLine);
        Menu.read.ShowReport(errors, () => errors.Clear());
    }

    public static void Error(string error)
    {
        errors.AppendLine($"- {error}");
    }
}