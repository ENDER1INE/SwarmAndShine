using System.Collections.Generic;

namespace Game.Shop
{
    public class RarityUnlockPoolRule : IShopPoolRule
    {
        private readonly IReadOnlyList<RarityPoolEntry> _entries;

        public RarityUnlockPoolRule(IReadOnlyList<RarityPoolEntry> entries)
        {
            _entries = entries;
        }

        public float GetWeight(ShopItemData item, int waveIndex)
        {
            foreach (RarityPoolEntry entry in _entries)
            {
                if (entry.Rarity == item.Rarity)
                {
                    return waveIndex >= entry.FirstWaveIndex ? entry.Weight : 0f;
                }
            }

            return 0f;
        }
    }
}
