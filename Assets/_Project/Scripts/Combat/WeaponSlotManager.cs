using System.Collections.Generic;

namespace Game.Combat
{
    public class WeaponSlotManager
    {
        public const int MaxSlots = 6;

        private readonly List<IWeaponBehavior> _activeWeapons = new();
        private readonly IWeaponBehaviorFactory _weaponFactory;

        public WeaponSlotManager(IWeaponBehaviorFactory weaponFactory)
        {
            _weaponFactory = weaponFactory;
        }

        public IReadOnlyList<IWeaponBehavior> ActiveWeapons => _activeWeapons;
        public bool IsFull => _activeWeapons.Count >= MaxSlots;

        public bool TryAddWeapon(WeaponData weaponData)
        {
            IWeaponBehavior existing = FindByWeaponData(weaponData);
            if (existing != null)
            {
                existing.AddStack();
                return true;
            }

            if (IsFull)
            {
                return false;
            }

            IWeaponBehavior weapon = _weaponFactory.Create(weaponData);
            _activeWeapons.Add(weapon);
            return true;
        }

        private IWeaponBehavior FindByWeaponData(WeaponData weaponData)
        {
            foreach (var weapon in _activeWeapons)
            {
                if (weapon.WeaponData == weaponData)
                {
                    return weapon;
                }
            }

            return null;
        }
    }
}
