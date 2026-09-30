using System;
using Game.Core;
using Game.Spawning;

namespace Game.Run
{
    public class WaveState : IRunState
    {
        private readonly int _waveIndex;
        private readonly WaveConfig _waveConfig;
        private readonly IWaveController _waveController;
        private readonly Action _onTimeElapsed;
        private float _elapsedTime;

        public float RemainingTime => Math.Max(0f, _waveConfig.Duration - _elapsedTime);

        public WaveState(int waveIndex, WaveConfig waveConfig, IWaveController waveController, Action onTimeElapsed)
        {
            _waveIndex = waveIndex;
            _waveConfig = waveConfig;
            _waveController = waveController;
            _onTimeElapsed = onTimeElapsed;
        }

        public void Enter()
        {
            _elapsedTime = 0f;
            _waveController.StartWave(_waveConfig);
            GameEvents.Publish(new WaveStartedEvent(_waveIndex, _waveConfig.Duration));
        }

        public void Tick(float deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= _waveConfig.Duration)
            {
                _onTimeElapsed();
            }
        }

        public void Exit()
        {
            _waveController.StopWave();
        }
    }
}
