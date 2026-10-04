using System;
using Game.Core;
using Game.Enemies;

namespace Game.Meta
{
    public class RunStatsTracker : IDisposable
    {
        public int EnemiesKilled { get; private set; }
        public float SecondsSurvived { get; private set; }

        public RunStatsTracker()
        {
            GameEvents.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        public void Tick(float deltaTime)
        {
            if (deltaTime > 0f)
            {
                SecondsSurvived += deltaTime;
            }
        }

        public void Dispose()
        {
            GameEvents.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent evt)
        {
            EnemiesKilled++;
        }
    }
}
