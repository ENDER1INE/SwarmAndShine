using System.Collections.Generic;
using System.Reflection;
using Game.Core;
using Game.Meta;
using Game.Player;
using Game.Run;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CharacterUnlockTests
    {
        private class InMemorySaveStorage : ISaveStorage
        {
            public int SaveCount;

            public SaveData Load() => new();

            public void Save(SaveData data) => SaveCount++;
        }

        private SaveData _saveData;
        private InMemorySaveStorage _storage;
        private CharacterData _ronin;
        private CharacterData _lucy;
        private CharacterData _monter;
        private CharacterUnlockService _unlockService;

        [SetUp]
        public void SetUp()
        {
            _saveData = new SaveData();
            _storage = new InMemorySaveStorage();
            _ronin = CreateCharacter("ronin", CharacterUnlockType.Default, cost: 0, requiredWaves: 0);
            _lucy = CreateCharacter("lucy", CharacterUnlockType.MetaCurrency, cost: 100, requiredWaves: 0);
            _monter = CreateCharacter("monter", CharacterUnlockType.WavesCompleted, cost: 0, requiredWaves: 5);
            _unlockService = new CharacterUnlockService(
                _saveData,
                new MetaWallet(_saveData),
                _storage,
                new List<CharacterData> { _ronin, _lucy, _monter });
        }

        [TearDown]
        public void TearDown()
        {
            _unlockService.Dispose();
        }

        [Test]
        public void IsUnlocked_DefaultCharacter_IsAlwaysUnlocked()
        {
            Assert.IsTrue(_unlockService.IsUnlocked(_ronin));
            Assert.IsFalse(_unlockService.IsUnlocked(_lucy));
        }

        [Test]
        public void TryPurchase_EnoughCurrency_UnlocksSpendsAndSaves()
        {
            _saveData.MetaCurrency = 150;

            CharacterPurchaseResult result = _unlockService.TryPurchase(_lucy);

            Assert.AreEqual(CharacterPurchaseResult.Success, result);
            Assert.IsTrue(_unlockService.IsUnlocked(_lucy));
            Assert.AreEqual(50, _saveData.MetaCurrency);
            Assert.AreEqual(1, _storage.SaveCount);
        }

        [Test]
        public void TryPurchase_NotEnoughCurrency_ChangesNothing()
        {
            _saveData.MetaCurrency = 99;

            CharacterPurchaseResult result = _unlockService.TryPurchase(_lucy);

            Assert.AreEqual(CharacterPurchaseResult.NotEnoughCurrency, result);
            Assert.IsFalse(_unlockService.IsUnlocked(_lucy));
            Assert.AreEqual(99, _saveData.MetaCurrency);
        }

        [Test]
        public void TryPurchase_AlreadyUnlocked_DoesNotChargeAgain()
        {
            _saveData.MetaCurrency = 300;
            _unlockService.TryPurchase(_lucy);

            CharacterPurchaseResult result = _unlockService.TryPurchase(_lucy);

            Assert.AreEqual(CharacterPurchaseResult.AlreadyUnlocked, result);
            Assert.AreEqual(200, _saveData.MetaCurrency);
        }

        [Test]
        public void TryPurchase_AchievementCharacter_IsNotPurchasable()
        {
            _saveData.MetaCurrency = 1000;

            CharacterPurchaseResult result = _unlockService.TryPurchase(_monter);

            Assert.AreEqual(CharacterPurchaseResult.NotPurchasable, result);
            Assert.AreEqual(1000, _saveData.MetaCurrency);
        }

        [Test]
        public void RunEnded_EnoughWavesCompleted_UnlocksAchievementCharacter()
        {
            GameEvents.Publish(new RunEndedEvent(survived: false, wavesCompleted: 5));

            Assert.IsTrue(_unlockService.IsUnlocked(_monter));
            Assert.AreEqual(1, _storage.SaveCount);
        }

        [Test]
        public void RunEnded_TooFewWavesCompleted_KeepsAchievementCharacterLocked()
        {
            GameEvents.Publish(new RunEndedEvent(survived: false, wavesCompleted: 4));

            Assert.IsFalse(_unlockService.IsUnlocked(_monter));
            Assert.AreEqual(0, _storage.SaveCount);
        }

        [Test]
        public void GetSelected_NothingSaved_FallsBackToDefaultCharacter()
        {
            var selection = CreateSelection();

            Assert.AreSame(_ronin, selection.GetSelected());
        }

        [Test]
        public void TrySelect_UnlockedCharacter_BecomesSelectedAndSaves()
        {
            _saveData.MetaCurrency = 100;
            _unlockService.TryPurchase(_lucy);
            var selection = CreateSelection();

            bool selected = selection.TrySelect(_lucy);

            Assert.IsTrue(selected);
            Assert.AreSame(_lucy, selection.GetSelected());
        }

        [Test]
        public void TrySelect_LockedCharacter_IsRejected()
        {
            var selection = CreateSelection();

            bool selected = selection.TrySelect(_lucy);

            Assert.IsFalse(selected);
            Assert.AreSame(_ronin, selection.GetSelected());
        }

        [Test]
        public void GetSelected_SavedCharacterIsLocked_FallsBackToDefaultCharacter()
        {
            _saveData.SelectedCharacterId = "lucy";
            var selection = CreateSelection();

            Assert.AreSame(_ronin, selection.GetSelected());
        }

        [Test]
        public void GetStatus_CoversEveryMenuState()
        {
            _saveData.MetaCurrency = 100;
            var selection = CreateSelection();

            Assert.AreEqual(CharacterMenuStatus.Selected, selection.GetStatus(_ronin));
            Assert.AreEqual(CharacterMenuStatus.PurchasableWithCurrency, selection.GetStatus(_lucy));
            Assert.AreEqual(CharacterMenuStatus.LockedByAchievement, selection.GetStatus(_monter));

            _unlockService.TryPurchase(_lucy);
            Assert.AreEqual(CharacterMenuStatus.Unlocked, selection.GetStatus(_lucy));

            selection.TrySelect(_lucy);
            Assert.AreEqual(CharacterMenuStatus.Selected, selection.GetStatus(_lucy));
            Assert.AreEqual(CharacterMenuStatus.Unlocked, selection.GetStatus(_ronin));
        }

        private CharacterSelection CreateSelection()
        {
            return new CharacterSelection(
                _saveData,
                _storage,
                _unlockService,
                new List<CharacterData> { _ronin, _lucy, _monter });
        }

        private static CharacterData CreateCharacter(string id, CharacterUnlockType unlockType, int cost, int requiredWaves)
        {
            var character = ScriptableObject.CreateInstance<CharacterData>();
            SetField(character, "_id", id);
            SetField(character, "_unlockType", unlockType);
            SetField(character, "_unlockCost", cost);
            SetField(character, "_requiredWavesCompleted", requiredWaves);
            return character;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }
    }
}
