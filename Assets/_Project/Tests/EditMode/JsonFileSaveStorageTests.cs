using System.IO;
using System.Text.RegularExpressions;
using Game.Meta;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Tests.EditMode
{
    public class JsonFileSaveStorageTests
    {
        private string _directory;
        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_directory);
            _filePath = Path.Combine(_directory, "save.json");
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(_directory, recursive: true);
        }

        [Test]
        public void Load_MissingFile_ReturnsFreshSave()
        {
            var storage = new JsonFileSaveStorage(_filePath);

            SaveData data = storage.Load();

            Assert.AreEqual(0, data.MetaCurrency);
            Assert.AreEqual(SaveData.CurrentVersion, data.Version);
        }

        [Test]
        public void SaveThenLoad_RoundTripsAllFields()
        {
            var storage = new JsonFileSaveStorage(_filePath);
            var data = new SaveData { MetaCurrency = 120 };
            data.SetUpgradeLevel("max_hp", 3);
            data.SetUpgradeLevel("move_speed", 1);
            data.UnlockCharacter("lucy");

            storage.Save(data);
            SaveData loaded = storage.Load();

            Assert.AreEqual(120, loaded.MetaCurrency);
            Assert.AreEqual(3, loaded.GetUpgradeLevel("max_hp"));
            Assert.AreEqual(1, loaded.GetUpgradeLevel("move_speed"));
            Assert.IsTrue(loaded.IsCharacterUnlocked("lucy"));
            Assert.IsFalse(loaded.IsCharacterUnlocked("monter"));
        }

        [Test]
        public void Save_OverExistingFile_ReplacesContentAndLeavesNoTemporaryFile()
        {
            var storage = new JsonFileSaveStorage(_filePath);
            storage.Save(new SaveData { MetaCurrency = 10 });

            storage.Save(new SaveData { MetaCurrency = 25 });

            Assert.AreEqual(25, storage.Load().MetaCurrency);
            Assert.AreEqual(1, Directory.GetFiles(_directory).Length);
        }

        [Test]
        public void Load_CorruptedFile_ReturnsFreshSave()
        {
            File.WriteAllText(_filePath, "{ not valid json");
            var storage = new JsonFileSaveStorage(_filePath);

            LogAssert.Expect(LogType.Warning, new Regex("corrupted"));
            SaveData data = storage.Load();

            Assert.AreEqual(0, data.MetaCurrency);
        }
    }
}
