namespace Game.Combat
{
    public interface IWeaponBehavior
    {
        WeaponData WeaponData { get; }

        void Tick(float deltaTime);
        void Release();
    }
}
