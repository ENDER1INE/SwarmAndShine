using System.Collections.Generic;

namespace Game.Shop
{
    public class ShopSlotGenerator
    {
        private readonly IReadOnlyList<ShopItemData> _items;
        private readonly IShopPoolRule _poolRule;
        private readonly IRandomSource _random;

        public ShopSlotGenerator(IReadOnlyList<ShopItemData> items, IShopPoolRule poolRule, IRandomSource random)
        {
            _items = items;
            _poolRule = poolRule;
            _random = random;
        }

        public ShopItemData PickItem(int waveIndex)
        {
            float totalWeight = 0f;
            foreach (ShopItemData item in _items)
            {
                totalWeight += _poolRule.GetWeight(item, waveIndex);
            }

            if (totalWeight <= 0f)
            {
                return null;
            }

            float roll = _random.NextValue() * totalWeight;
            ShopItemData lastEligible = null;
            foreach (ShopItemData item in _items)
            {
                float weight = _poolRule.GetWeight(item, waveIndex);
                if (weight <= 0f)
                {
                    continue;
                }

                lastEligible = item;
                roll -= weight;
                if (roll < 0f)
                {
                    return item;
                }
            }

            return lastEligible;
        }
    }
}
