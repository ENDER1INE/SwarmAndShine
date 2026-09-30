namespace Game.Combat
{
    public interface IWeaponMountFactory
    {
        IWeaponMount Create(WeaponData weaponData);
    }
}
