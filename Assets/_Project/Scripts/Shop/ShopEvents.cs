namespace Game.Shop
{
    public readonly struct CurrencyChangedEvent
    {
        public readonly int Balance;

        public CurrencyChangedEvent(int balance)
        {
            Balance = balance;
        }
    }

    public readonly struct ShopContentChangedEvent
    {
    }

    public readonly struct WeaponInventoryChangedEvent
    {
    }
}
