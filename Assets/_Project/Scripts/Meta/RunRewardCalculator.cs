using UnityEngine;

namespace Game.Meta
{
    public class RunRewardCalculator
    {
        private const float SecondsPerMinute = 60f;

        private readonly RunRewardRates _rates;

        public RunRewardCalculator(RunRewardRates rates)
        {
            _rates = rates;
        }

        public int Calculate(RunStats stats)
        {
            float minutesSurvived = stats.SecondsSurvived / SecondsPerMinute;
            float reward = _rates.BaseReward
                           + stats.EnemiesKilled * _rates.PerEnemyKilled
                           + stats.WavesCompleted * _rates.PerWaveCompleted
                           + minutesSurvived * _rates.PerMinuteSurvived
                           + (stats.Survived ? _rates.VictoryBonus : 0);

            return Mathf.Max(0, Mathf.FloorToInt(reward));
        }
    }
}
