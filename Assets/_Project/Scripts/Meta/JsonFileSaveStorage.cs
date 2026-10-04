using System;
using System.IO;
using UnityEngine;

namespace Game.Meta
{
    public class JsonFileSaveStorage : ISaveStorage
    {
        private const string TemporaryFileExtension = ".tmp";

        private readonly string _filePath;

        public JsonFileSaveStorage(string filePath)
        {
            _filePath = filePath;
        }

        public SaveData Load()
        {
            if (!File.Exists(_filePath))
            {
                return new SaveData();
            }

            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(_filePath));
                return data ?? new SaveData();
            }
            catch (ArgumentException exception)
            {
                Debug.LogWarning($"Save file is corrupted, starting with a fresh save: {exception.Message}");
                return new SaveData();
            }
        }

        public void Save(SaveData data)
        {
            // Write to a temporary file first so a crash mid-write cannot destroy the existing save.
            string temporaryPath = _filePath + TemporaryFileExtension;
            File.WriteAllText(temporaryPath, JsonUtility.ToJson(data));

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            File.Move(temporaryPath, _filePath);
        }
    }
}
