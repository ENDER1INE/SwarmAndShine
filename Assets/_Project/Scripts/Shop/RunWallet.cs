using System;
using Game.Core;
using Game.Enemies;

namespace Game.Shop
{
    public class RunWallet : IDisposable
    {
        public int Balance { get; private set; }

        public RunWallet(int startingBalance = 0)
        {
            Balance = startingBalance;
            GameEvents.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Balance += amount;
            GameEvents.Publish(new CurrencyChangedEvent(Balance));
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0 || amount > Balance)
            {
                return false;
            }

            Balance -= amount;
            GameEvents.Publish(new CurrencyChangedEvent(Balance));
            return true;
        }

        public void Dispose()
        {
            GameEvents.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent evt)
        {
            Add(evt.CurrencyReward);
        }
    }
}
