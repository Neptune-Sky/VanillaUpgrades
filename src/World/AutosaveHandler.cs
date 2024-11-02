using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using SFS;
using SFS.IO;
using SFS.Parsers.Json;
using SFS.World;
using UnityEngine;

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(GameManager), "Update")]
    internal static class AutosaveHandler
    {
        [Serializable]
        private class AutosaveFile
        {
            public List<string> fileNames = new();
        }
        private static float _timeSinceLastAutosave;

        private static void Postfix()
        {
            if (Time.timeScale < 0.01f) return;
            if (Config.settings.persistentVars.allowedAutosaveSlots == 0)
            {
                _timeSinceLastAutosave = 0f;
                return;
            }
            _timeSinceLastAutosave += Time.unscaledDeltaTime;
            if (_timeSinceLastAutosave < Config.settings.persistentVars.minutesUntilAutosave * 60) return;

            var fileList = new AutosaveFile();
            var rootPath = new FolderPath(Base.worldBase.paths.path + "/");
            var quicksavesPath = new FolderPath(Base.worldBase.paths.quicksavesPath + "/");
            
            if (File.Exists(rootPath + "Autosaves.txt"))
            {
                var filePath = rootPath + "Autosaves.txt";
                try
                {
                    var jsonContent = File.ReadAllText(filePath);
                    fileList = JsonUtility.FromJson<AutosaveFile>(jsonContent);
                }
                catch (Exception)
                {
                    File.Delete(filePath);
                }
            }
            
            var toSave = Traverse.Create(GameManager.main).Method("CreateWorldSave").GetValue() as WorldSave;
            var name = "Autosave " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            FolderPath where = new(quicksavesPath + name);
            
            WorldSave.Save(where, true, toSave, Base.worldBase.IsCareer);

            fileList.fileNames.Add(name);
            
            // Check if directories exist and remove non-existent ones
            fileList.fileNames = fileList.fileNames.Where(dir => Directory.Exists(Path.Combine(quicksavesPath, dir))).ToList();

            // If the number of directories exceeds the limit, delete the oldest ones
            if (fileList.fileNames.Count > Config.settings.persistentVars.allowedAutosaveSlots)
            {
                // Get directories with their last modified times
                var directoryInfos = fileList.fileNames
                    .Select(dir => new DirectoryInfo(Path.Combine(quicksavesPath, dir)))
                    .OrderBy(dirInfo => dirInfo.LastWriteTime)
                    .ToList();

                // Calculate how many directories to delete
                var excessCount = fileList.fileNames.Count - Config.settings.persistentVars.allowedAutosaveSlots;
                

                // Delete the oldest directories
                for (var i = 0; i < excessCount; i++)
                {
                    Directory.Delete(directoryInfos[i].FullName, true);
                }

                // Update the directory list to exclude deleted directories
                fileList.fileNames = directoryInfos.Skip(excessCount).Select(dirInfo => dirInfo.Name).ToList();
            }
            JsonWrapper.SaveAsJson(new FilePath(rootPath + "Autosaves.txt"), fileList, false);

            _timeSinceLastAutosave = 0f;
        }
    }
}