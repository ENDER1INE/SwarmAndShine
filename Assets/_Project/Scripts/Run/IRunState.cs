namespace Game.Run
{
    public interface IRunState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
