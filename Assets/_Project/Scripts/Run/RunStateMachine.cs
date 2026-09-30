namespace Game.Run
{
    public class RunStateMachine
    {
        public IRunState CurrentState { get; private set; }

        public void ChangeState(IRunState nextState)
        {
            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }

        public void Tick(float deltaTime)
        {
            CurrentState?.Tick(deltaTime);
        }
    }
}
