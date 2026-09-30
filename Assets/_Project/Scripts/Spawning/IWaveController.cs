namespace Game.Spawning
{
    public interface IWaveController
    {
        void StartWave(WaveConfig waveConfig);
        void StopWave();
    }
}
