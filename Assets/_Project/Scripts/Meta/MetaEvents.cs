namespace Game.Meta
{
    public readonly struct MetaCurrencyChangedEvent
    {
        public readonly int Balance;

        public MetaCurrencyChangedEvent(int balance)
        {
            Balance = balance;
        }
    }

    public readonly struct MetaUpgradePurchasedEvent
    {
        public readonly string UpgradeId;
        public readonly int NewLevel;

        public MetaUpgradePurchasedEvent(string upgradeId, int newLevel)
        {
            UpgradeId = upgradeId;
            NewLevel = newLevel;
        }
    }

    public readonly struct CharacterUnlockedEvent
    {
        public readonly string CharacterId;

        public CharacterUnlockedEvent(string characterId)
        {
            CharacterId = characterId;
        }
    }

    public readonly struct RunRewardGrantedEvent
    {
        public readonly int Amount;

        public RunRewardGrantedEvent(int amount)
        {
            Amount = amount;
        }
    }
}
