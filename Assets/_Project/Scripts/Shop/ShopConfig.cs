using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shop
{
    [Serializable]
    public struct RarityPoolEntry
    {
        [SerializeField] private ShopItemRarity _rarity;
        [SerializeField] private int _firstWaveIndex;
        [SerializeField] private float _weight;

        public ShopItemRarity Rarity => _rarity;
        public int FirstWaveIndex => _firstWaveIndex;
        public float Weight => _weight;
    }

    [CreateAssetMenu(fileName = "ShopConfig", menuName = "SAS/Shop/Shop Config")]
    public class ShopConfig : ScriptableObject
    {
        [SerializeField] private int _slotCount = 4;
        [SerializeField] private int _rerollBaseCost = 5;
        [SerializeField] private int _rerollCostIncrement = 3;
        [SerializeField] private List<ShopItemData> _items = new();
        [SerializeField] private List<RarityPoolEntry> _rarityPool = new();

        public int SlotCount => _slotCount;
        public int RerollBaseCost => _rerollBaseCost;
        public int RerollCostIncrement => _rerollCostIncrement;
        public IReadOnlyList<ShopItemData> Items => _items;
        public IReadOnlyList<RarityPoolEntry> RarityPool => _rarityPool;
    }
}
