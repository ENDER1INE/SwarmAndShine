namespace Game.Meta
{
    public readonly struct RunStats
    {
        public readonly int EnemiesKilled;
        public readonly float SecondsSurvived;
        public readonly int WavesCompleted;
        public readonly bool Survived;

        public RunStats(int enemiesKilled, float secondsSurvived, int wavesCompleted, bool survived)
        {
            EnemiesKilled = enemiesKilled;
            SecondsSurvived = secondsSurvived;
            WavesCompleted = wavesCompleted;
            Survived = survived;
        }
    }
}
