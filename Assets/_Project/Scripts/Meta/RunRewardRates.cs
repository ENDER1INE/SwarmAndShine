namespace Game.Meta
{
    public readonly struct RunRewardRates
    {
        public readonly int BaseReward;
        public readonly int PerEnemyKilled;
        public readonly int PerWaveCompleted;
        public readonly int PerMinuteSurvived;
        public readonly int VictoryBonus;

        public RunRewardRates(int baseReward, int perEnemyKilled, int perWaveCompleted, int perMinuteSurvived, int victoryBonus)
        {
            BaseReward = baseReward;
            PerEnemyKilled = perEnemyKilled;
            PerWaveCompleted = perWaveCompleted;
            PerMinuteSurvived = perMinuteSurvived;
            VictoryBonus = victoryBonus;
        }
    }
}
