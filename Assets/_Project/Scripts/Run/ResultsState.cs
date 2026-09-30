using Game.Core;

namespace Game.Run
{
    public class ResultsState : IRunState
    {
        private readonly bool _survived;
        private readonly int _wavesCompleted;

        public ResultsState(bool survived, int wavesCompleted)
        {
            _survived = survived;
            _wavesCompleted = wavesCompleted;
        }

        public void Enter()
        {
            GameEvents.Publish(new RunEndedEvent(_survived, _wavesCompleted));
        }

        public void Tick(float deltaTime)
        {
        }

        public void Exit()
        {
        }
    }
}
