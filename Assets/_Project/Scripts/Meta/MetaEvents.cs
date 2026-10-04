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

    public readonly struct RunRewardGrantedEvent
    {
        public readonly int Amount;

        public RunRewardGrantedEvent(int amount)
        {
            Amount = amount;
        }
    }
}
