using Game.Meta;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class MetaWalletTests
    {
        [Test]
        public void Add_IncreasesBalanceStoredInSaveData()
        {
            var saveData = new SaveData();
            var wallet = new MetaWallet(saveData);

            wallet.Add(15);

            Assert.AreEqual(15, wallet.Balance);
            Assert.AreEqual(15, saveData.MetaCurrency);
        }

        [Test]
        public void Add_NonPositiveAmount_IsIgnored()
        {
            var wallet = new MetaWallet(new SaveData { MetaCurrency = 5 });

            wallet.Add(0);
            wallet.Add(-3);

            Assert.AreEqual(5, wallet.Balance);
        }

        [Test]
        public void TrySpend_InsufficientFunds_ReturnsFalseAndKeepsBalance()
        {
            var wallet = new MetaWallet(new SaveData { MetaCurrency = 5 });

            bool success = wallet.TrySpend(10);

            Assert.IsFalse(success);
            Assert.AreEqual(5, wallet.Balance);
        }

        [Test]
        public void TrySpend_SufficientFunds_DeductsBalance()
        {
            var wallet = new MetaWallet(new SaveData { MetaCurrency = 10 });

            bool success = wallet.TrySpend(4);

            Assert.IsTrue(success);
            Assert.AreEqual(6, wallet.Balance);
        }
    }
}
