using Game.Core;

namespace Game.Run
{
    public class ShopState : IRunState
    {
        private readonly int _completedWaveIndex;

        public ShopState(int completedWaveIndex)
        {
            _completedWaveIndex = completedWaveIndex;
        }

        public void Enter()
        {
            GameEvents.Publish(new ShopOpenedEvent(_completedWaveIndex));
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
