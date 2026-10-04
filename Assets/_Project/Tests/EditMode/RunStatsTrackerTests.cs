using Game.Core;
using Game.Enemies;
using Game.Meta;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class RunStatsTrackerTests
    {
        [Test]
        public void EnemyKilled_IncrementsKillCount()
        {
            using var tracker = new RunStatsTracker();

            GameEvents.Publish(new EnemyKilledEvent(1, Vector2.zero));
            GameEvents.Publish(new EnemyKilledEvent(1, Vector2.zero));

            Assert.AreEqual(2, tracker.EnemiesKilled);
        }

        [Test]
        public void Tick_AccumulatesTimeAndIgnoresNonPositiveDelta()
        {
            using var tracker = new RunStatsTracker();

            tracker.Tick(1.5f);
            tracker.Tick(-3f);
            tracker.Tick(0.5f);

            Assert.AreEqual(2f, tracker.SecondsSurvived, 0.0001f);
        }

        [Test]
        public void Dispose_StopsCountingKills()
        {
            var tracker = new RunStatsTracker();
            tracker.Dispose();

            GameEvents.Publish(new EnemyKilledEvent(1, Vector2.zero));

            Assert.AreEqual(0, tracker.EnemiesKilled);
        }
    }
}
