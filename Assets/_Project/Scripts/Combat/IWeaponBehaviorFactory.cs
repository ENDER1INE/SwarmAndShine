namespace Game.Combat
{
    public interface IWeaponBehaviorFactory
    {
        IWeaponBehavior Create(WeaponData weaponData);
    }
}
