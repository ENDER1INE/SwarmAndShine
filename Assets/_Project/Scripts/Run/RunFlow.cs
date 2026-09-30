using System;
using System.Collections.Generic;
using Game.Core;
using Game.Player;
using Game.Spawning;

namespace Game.Run
{
    public class RunFlow : IDisposable
    {
        private readonly IReadOnlyList<WaveConfig> _waves;
        private readonly IWaveController _waveController;
        private readonly float _defeatFallbackDelay;
        private readonly RunStateMachine _stateMachine = new();
        private int _currentWaveIndex;

        public IRunState CurrentState => _stateMachine.CurrentState;
        public bool IsWaveActive => CurrentState is WaveState;
        public float RemainingWaveTime => CurrentState is WaveState waveState ? waveState.RemainingTime : 0f;

        public RunFlow(IReadOnlyList<WaveConfig> waves, IWaveController waveController, float defeatFallbackDelay)
        {
            if (waves == null || waves.Count == 0)
            {
                throw new ArgumentException("Run requires at least one wave.", nameof(waves));
            }

            _waves = waves;
            _waveController = waveController;
            _defeatFallbackDelay = defeatFallbackDelay;
            GameEvents.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            GameEvents.Subscribe<PlayerDeathAnimationFinishedEvent>(OnPlayerDeathAnimationFinished);
        }

        public void Start()
        {
            EnterWave(0);
        }

        public void Tick(float deltaTime)
        {
            _stateMachine.Tick(deltaTime);
        }

        public void ContinueFromShop()
        {
            if (CurrentState is not ShopState)
            {
                return;
            }

            EnterWave(_currentWaveIndex + 1);
        }

        public void Dispose()
        {
            GameEvents.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
            GameEvents.Unsubscribe<PlayerDeathAnimationFinishedEvent>(OnPlayerDeathAnimationFinished);
        }

        private void EnterWave(int waveIndex)
        {
            _currentWaveIndex = waveIndex;
            _stateMachine.ChangeState(new WaveState(waveIndex, _waves[waveIndex], _waveController, OnWaveTimeElapsed));
        }

        private void OnWaveTimeElapsed()
        {
            int completedWaves = _currentWaveIndex + 1;
            GameEvents.Publish(new WaveEndedEvent(_currentWaveIndex));

            if (completedWaves >= _waves.Count)
            {
                _stateMachine.ChangeState(new ResultsState(survived: true, completedWaves));
                return;
            }

            _stateMachine.ChangeState(new ShopState(_currentWaveIndex));
        }

        private void OnPlayerDied(PlayerDiedEvent evt)
        {
            if (CurrentState is ResultsState or PlayerDyingState)
            {
                return;
            }

            _stateMachine.ChangeState(new PlayerDyingState(_defeatFallbackDelay, FinishDefeat));
        }

        private void OnPlayerDeathAnimationFinished(PlayerDeathAnimationFinishedEvent evt)
        {
            FinishDefeat();
        }

        private void FinishDefeat()
        {
            if (CurrentState is not PlayerDyingState)
            {
                return;
            }

            _stateMachine.ChangeState(new ResultsState(survived: false, _currentWaveIndex));
        }
    }
}
