namespace Game.Run
{
    public readonly struct WaveStartedEvent
    {
        public readonly int WaveIndex;
        public readonly float Duration;

        public WaveStartedEvent(int waveIndex, float duration)
        {
            WaveIndex = waveIndex;
            Duration = duration;
        }
    }

    public readonly struct WaveEndedEvent
    {
        public readonly int WaveIndex;

        public WaveEndedEvent(int waveIndex)
        {
            WaveIndex = waveIndex;
        }
    }

    public readonly struct ShopOpenedEvent
    {
        public readonly int CompletedWaveIndex;

        public ShopOpenedEvent(int completedWaveIndex)
        {
            CompletedWaveIndex = completedWaveIndex;
        }
    }

    public readonly struct RunEndedEvent
    {
        public readonly bool Survived;
        public readonly int WavesCompleted;

        public RunEndedEvent(bool survived, int wavesCompleted)
        {
            Survived = survived;
            WavesCompleted = wavesCompleted;
        }
    }
}
