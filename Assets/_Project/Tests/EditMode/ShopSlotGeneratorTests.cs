using System.Collections.Generic;
using System.Reflection;
using Game.Shop;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class ShopSlotGeneratorTests
    {
        private class FixedRandomSource : IRandomSource
        {
            private readonly float _value;

            public FixedRandomSource(float value)
            {
                _value = value;
            }

            public float NextValue() => _value;
        }

        private static ShopItemData CreateItem(ShopItemRarity rarity)
        {
            var item = ScriptableObject.CreateInstance<ShopItemData>();
            typeof(ShopItemData).GetField("_rarity", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(item, rarity);
            return item;
        }

        private static RarityPoolEntry CreateEntry(ShopItemRarity rarity, int firstWaveIndex, float weight)
        {
            object entry = new RarityPoolEntry();
            typeof(RarityPoolEntry).GetField("_rarity", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(entry, rarity);
            typeof(RarityPoolEntry).GetField("_firstWaveIndex", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(entry, firstWaveIndex);
            typeof(RarityPoolEntry).GetField("_weight", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(entry, weight);
            return (RarityPoolEntry)entry;
        }

        private static ShopSlotGenerator CreateGenerator(IReadOnlyList<ShopItemData> items, float randomValue)
        {
            var rule = new RarityUnlockPoolRule(new List<RarityPoolEntry>
            {
                CreateEntry(ShopItemRarity.Common, firstWaveIndex: 0, weight: 3f),
                CreateEntry(ShopItemRarity.Rare, firstWaveIndex: 2, weight: 1f)
            });

            return new ShopSlotGenerator(items, rule, new FixedRandomSource(randomValue));
        }

        [Test]
        public void PickItem_RarityNotYetUnlocked_IsNeverPicked()
        {
            var common = CreateItem(ShopItemRarity.Common);
            var rare = CreateItem(ShopItemRarity.Rare);
            var generator = CreateGenerator(new List<ShopItemData> { common, rare }, randomValue: 0.99f);

            ShopItemData picked = generator.PickItem(waveIndex: 1);

            Assert.AreSame(common, picked);
        }

        [Test]
        public void PickItem_RarityUnlocked_CanBePicked()
        {
            var common = CreateItem(ShopItemRarity.Common);
            var rare = CreateItem(ShopItemRarity.Rare);
            var generator = CreateGenerator(new List<ShopItemData> { common, rare }, randomValue: 0.99f);

            ShopItemData picked = generator.PickItem(waveIndex: 2);

            Assert.AreSame(rare, picked);
        }

        [Test]
        public void PickItem_LowRoll_PicksFirstWeightedItem()
        {
            var common = CreateItem(ShopItemRarity.Common);
            var rare = CreateItem(ShopItemRarity.Rare);
            var generator = CreateGenerator(new List<ShopItemData> { common, rare }, randomValue: 0f);

            ShopItemData picked = generator.PickItem(waveIndex: 2);

            Assert.AreSame(common, picked);
        }

        [Test]
        public void PickItem_NoEligibleItems_ReturnsNull()
        {
            var rare = CreateItem(ShopItemRarity.Rare);
            var generator = CreateGenerator(new List<ShopItemData> { rare }, randomValue: 0.5f);

            ShopItemData picked = generator.PickItem(waveIndex: 0);

            Assert.IsNull(picked);
        }
    }
}
