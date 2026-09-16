namespace Game.Combat
{
    public interface IWeaponBehavior
    {
        WeaponData WeaponData { get; }
        int StackCount { get; }

        void Tick(float deltaTime);
        void AddStack();
    }
}
