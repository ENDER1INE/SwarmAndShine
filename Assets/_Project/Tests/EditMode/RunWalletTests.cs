using Game.Core;
using Game.Enemies;
using Game.Shop;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class RunWalletTests
    {
        [Test]
        public void EnemyKilled_AddsRewardToBalance()
        {
            using var wallet = new RunWallet();

            GameEvents.Publish(new EnemyKilledEvent(3, Vector2.zero));

            Assert.AreEqual(3, wallet.Balance);
        }

        [Test]
        public void Dispose_StopsListeningToEnemyKilled()
        {
            var wallet = new RunWallet();
            wallet.Dispose();

            GameEvents.Publish(new EnemyKilledEvent(3, Vector2.zero));

            Assert.AreEqual(0, wallet.Balance);
        }

        [Test]
        public void TrySpend_InsufficientFunds_ReturnsFalseAndKeepsBalance()
        {
            using var wallet = new RunWallet(startingBalance: 5);

            bool success = wallet.TrySpend(10);

            Assert.IsFalse(success);
            Assert.AreEqual(5, wallet.Balance);
        }

        [Test]
        public void TrySpend_SufficientFunds_DeductsBalance()
        {
            using var wallet = new RunWallet(startingBalance: 10);

            bool success = wallet.TrySpend(4);

            Assert.IsTrue(success);
            Assert.AreEqual(6, wallet.Balance);
        }

        [Test]
        public void Add_PublishesCurrencyChangedEvent()
        {
            using var wallet = new RunWallet();
            int lastBalance = -1;
            void OnChanged(CurrencyChangedEvent evt) => lastBalance = evt.Balance;
            GameEvents.Subscribe<CurrencyChangedEvent>(OnChanged);

            try
            {
                wallet.Add(7);

                Assert.AreEqual(7, lastBalance);
            }
            finally
            {
                GameEvents.Unsubscribe<CurrencyChangedEvent>(OnChanged);
            }
        }
    }
}
