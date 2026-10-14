using System.Collections.Generic;
using System.Reflection;
using Game.Combat;
using Game.Meta;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class MetaUpgradeTests
    {
        private class InMemorySaveStorage : ISaveStorage
        {
            public int SaveCount;

            public SaveData Load() => new();

            public void Save(SaveData data) => SaveCount++;
        }

        [Test]
        public void GetCost_GrowsGeometricallyWithLevel()
        {
            MetaUpgradeData upgrade = CreateUpgrade("max_hp", maxLevel: 5, baseCost: 100, costMultiplier: 1.5f);

            Assert.AreEqual(100, upgrade.GetCost(0));
            Assert.AreEqual(150, upgrade.GetCost(1));
            Assert.AreEqual(225, upgrade.GetCost(2));
        }

        [Test]
        public void TryPurchase_EnoughCurrency_RaisesLevelSpendsCostAndSaves()
        {
            MetaUpgradeData upgrade = CreateUpgrade("max_hp", maxLevel: 5, baseCost: 100, costMultiplier: 1.5f);
            var saveData = new SaveData { MetaCurrency = 300 };
            var storage = new InMemorySaveStorage();
            var service = new MetaUpgradeService(saveData, new MetaWallet(saveData), storage);

            MetaUpgradePurchaseResult first = service.TryPurchase(upgrade);
            MetaUpgradePurchaseResult second = service.TryPurchase(upgrade);

            Assert.AreEqual(MetaUpgradePurchaseResult.Success, first);
            Assert.AreEqual(MetaUpgradePurchaseResult.Success, second);
            Assert.AreEqual(2, service.GetLevel(upgrade));
            Assert.AreEqual(50, saveData.MetaCurrency); // 300 - 100 - 150
            Assert.AreEqual(2, storage.SaveCount);
        }

        [Test]
        public void TryPurchase_NotEnoughCurrency_ChangesNothing()
        {
            MetaUpgradeData upgrade = CreateUpgrade("max_hp", maxLevel: 5, baseCost: 100, costMultiplier: 1.5f);
            var saveData = new SaveData { MetaCurrency = 99 };
            var storage = new InMemorySaveStorage();
            var service = new MetaUpgradeService(saveData, new MetaWallet(saveData), storage);

            MetaUpgradePurchaseResult result = service.TryPurchase(upgrade);

            Assert.AreEqual(MetaUpgradePurchaseResult.NotEnoughCurrency, result);
            Assert.AreEqual(0, service.GetLevel(upgrade));
            Assert.AreEqual(99, saveData.MetaCurrency);
            Assert.AreEqual(0, storage.SaveCount);
        }

        [Test]
        public void TryPurchase_MaxLevelReached_IsRejectedWithoutSpending()
        {
            MetaUpgradeData upgrade = CreateUpgrade("max_hp", maxLevel: 1, baseCost: 10, costMultiplier: 1.5f);
            var saveData = new SaveData { MetaCurrency = 1000 };
            var service = new MetaUpgradeService(saveData, new MetaWallet(saveData), new InMemorySaveStorage());
            service.TryPurchase(upgrade);
            int balanceAfterFirst = saveData.MetaCurrency;

            MetaUpgradePurchaseResult result = service.TryPurchase(upgrade);

            Assert.AreEqual(MetaUpgradePurchaseResult.MaxLevelReached, result);
            Assert.AreEqual(balanceAfterFirst, saveData.MetaCurrency);
        }

        [Test]
        public void CanPurchase_ReflectsBalanceAndMaxLevel()
        {
            MetaUpgradeData upgrade = CreateUpgrade("max_hp", maxLevel: 1, baseCost: 100, costMultiplier: 1.5f);
            var saveData = new SaveData { MetaCurrency = 99 };
            var service = new MetaUpgradeService(saveData, new MetaWallet(saveData), new InMemorySaveStorage());

            Assert.IsFalse(service.CanPurchase(upgrade));

            saveData.MetaCurrency = 500;
            Assert.IsTrue(service.CanPurchase(upgrade));

            service.TryPurchase(upgrade);
            Assert.IsTrue(service.IsMaxLevel(upgrade));
            Assert.IsFalse(service.CanPurchase(upgrade));
        }

        [Test]
        public void Apply_AddsOneModifierPerPurchasedLevel()
        {
            MetaUpgradeData upgrade = CreateUpgrade("max_hp", maxLevel: 5, baseCost: 10, costMultiplier: 1.5f, StatType.MaxHealth, ModifierType.Flat, 10f);
            var saveData = new SaveData();
            saveData.SetUpgradeLevel("max_hp", 3);
            var stats = new CharacterStats(CreateBaseStats(maxHealth: 100f));
            var applier = new MetaStatApplier(saveData, new List<MetaUpgradeData> { upgrade });

            applier.Apply(stats);

            Assert.AreEqual(130f, stats.MaxHealth, 0.0001f);
        }

        [Test]
        public void Apply_NoPurchasedLevels_LeavesStatsUntouched()
        {
            MetaUpgradeData upgrade = CreateUpgrade("max_hp", maxLevel: 5, baseCost: 10, costMultiplier: 1.5f, StatType.MaxHealth, ModifierType.Flat, 10f);
            var stats = new CharacterStats(CreateBaseStats(maxHealth: 100f));
            var applier = new MetaStatApplier(new SaveData(), new List<MetaUpgradeData> { upgrade });

            applier.Apply(stats);

            Assert.AreEqual(100f, stats.MaxHealth, 0.0001f);
        }

        private static MetaUpgradeData CreateUpgrade(
            string id,
            int maxLevel,
            int baseCost,
            float costMultiplier,
            StatType statType = StatType.MaxHealth,
            ModifierType modifierType = ModifierType.Flat,
            float modifierValue = 0f)
        {
            var upgrade = ScriptableObject.CreateInstance<MetaUpgradeData>();
            SetField(upgrade, "_id", id);
            SetField(upgrade, "_maxLevel", maxLevel);
            SetField(upgrade, "_baseCost", baseCost);
            SetField(upgrade, "_costMultiplier", costMultiplier);

            object modifierData = new StatModifierData();
            SetField(modifierData, "_statType", statType);
            SetField(modifierData, "_modifierType", modifierType);
            SetField(modifierData, "_value", modifierValue);
            SetField(upgrade, "_modifierPerLevel", modifierData);
            return upgrade;
        }

        private static CharacterBaseStatsData CreateBaseStats(float maxHealth)
        {
            var data = ScriptableObject.CreateInstance<CharacterBaseStatsData>();
            SetField(data, "_maxHealth", maxHealth);
            return data;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }
    }
}
