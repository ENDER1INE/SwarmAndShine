using Game.Core;

namespace Game.Meta
{
    public class MetaUpgradeService
    {
        private readonly SaveData _saveData;
        private readonly MetaWallet _wallet;
        private readonly ISaveStorage _storage;

        public MetaUpgradeService(SaveData saveData, MetaWallet wallet, ISaveStorage storage)
        {
            _saveData = saveData;
            _wallet = wallet;
            _storage = storage;
        }

        public int GetLevel(MetaUpgradeData upgrade)
        {
            return _saveData.GetUpgradeLevel(upgrade.Id);
        }

        public bool IsMaxLevel(MetaUpgradeData upgrade)
        {
            return GetLevel(upgrade) >= upgrade.MaxLevel;
        }

        public bool CanPurchase(MetaUpgradeData upgrade)
        {
            int currentLevel = GetLevel(upgrade);
            return currentLevel < upgrade.MaxLevel && _wallet.Balance >= upgrade.GetCost(currentLevel);
        }

        public MetaUpgradePurchaseResult TryPurchase(MetaUpgradeData upgrade)
        {
            int currentLevel = GetLevel(upgrade);
            if (currentLevel >= upgrade.MaxLevel)
            {
                return MetaUpgradePurchaseResult.MaxLevelReached;
            }

            if (!_wallet.TrySpend(upgrade.GetCost(currentLevel)))
            {
                return MetaUpgradePurchaseResult.NotEnoughCurrency;
            }

            int newLevel = currentLevel + 1;
            _saveData.SetUpgradeLevel(upgrade.Id, newLevel);
            _storage.Save(_saveData);
            GameEvents.Publish(new MetaUpgradePurchasedEvent(upgrade.Id, newLevel));
            return MetaUpgradePurchaseResult.Success;
        }
    }
}
