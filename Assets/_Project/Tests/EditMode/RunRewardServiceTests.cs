using Game.Core;
using Game.Enemies;
using Game.Meta;
using Game.Run;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class RunRewardServiceTests
    {
        private class InMemorySaveStorage : ISaveStorage
        {
            public int SaveCount;

            public SaveData Load() => new();

            public void Save(SaveData data) => SaveCount++;
        }

        [Test]
        public void RunEnded_GrantsRewardAndPersistsSave()
        {
            var rates = new RunRewardRates(baseReward: 10, perEnemyKilled: 2, perWaveCompleted: 5, perMinuteSurvived: 0, victoryBonus: 0);
            var saveData = new SaveData();
            var storage = new InMemorySaveStorage();
            using var tracker = new RunStatsTracker();
            using var service = new RunRewardService(new RunRewardCalculator(rates), tracker, new MetaWallet(saveData), saveData, storage);
            GameEvents.Publish(new EnemyKilledEvent(1, Vector2.zero));
            GameEvents.Publish(new EnemyKilledEvent(1, Vector2.zero));

            GameEvents.Publish(new RunEndedEvent(survived: false, wavesCompleted: 2));

            // 10 base + 2 kills*2 + 2 waves*5 = 24
            Assert.AreEqual(24, saveData.MetaCurrency);
            Assert.AreEqual(1, storage.SaveCount);
        }

        [Test]
        public void RunEnded_PublishesRunRewardGrantedEvent()
        {
            var rates = new RunRewardRates(baseReward: 7, 0, 0, 0, 0);
            var saveData = new SaveData();
            using var tracker = new RunStatsTracker();
            using var service = new RunRewardService(new RunRewardCalculator(rates), tracker, new MetaWallet(saveData), saveData, new InMemorySaveStorage());
            int granted = -1;
            void OnGranted(RunRewardGrantedEvent evt) => granted = evt.Amount;
            GameEvents.Subscribe<RunRewardGrantedEvent>(OnGranted);

            try
            {
                GameEvents.Publish(new RunEndedEvent(survived: false, wavesCompleted: 0));

                Assert.AreEqual(7, granted);
            }
            finally
            {
                GameEvents.Unsubscribe<RunRewardGrantedEvent>(OnGranted);
            }
        }
    }
}
