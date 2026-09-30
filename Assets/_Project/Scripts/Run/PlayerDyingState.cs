using System;

namespace Game.Run
{
    public class PlayerDyingState : IRunState
    {
        private readonly float _fallbackDelay;
        private readonly Action _onFallbackElapsed;
        private float _elapsedTime;

        public PlayerDyingState(float fallbackDelay, Action onFallbackElapsed)
        {
            _fallbackDelay = fallbackDelay;
            _onFallbackElapsed = onFallbackElapsed;
        }

        public void Enter()
        {
            _elapsedTime = 0f;
        }

        public void Tick(float deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= _fallbackDelay)
            {
                _onFallbackElapsed();
            }
        }

        public void Exit()
        {
        }
    }
}
