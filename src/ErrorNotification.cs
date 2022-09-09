﻿using System;
using System.Text;
using SFS.UI;
using UnityEngine;

namespace VanillaUpgrades
{
    public class ErrorNotification : MonoBehaviour
    {
        public static StringBuilder errors = new StringBuilder();

        public static void Error(string error)
        {
            errors.AppendLine($"- {error}");
        }
        
        void Start()
        {
            if (errors.Length != 0)
            {
                errors.Insert(0, "An error occured while loading VanillaUpgrades." + Environment.NewLine + Environment.NewLine);
                Menu.read.ShowReport(errors, () => errors.Clear());
            }
        }
    }
}
