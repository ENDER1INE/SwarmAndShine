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
            if (IsFull)
            {
                return false;
            }

            _activeWeapons.Add(_weaponFactory.Create(weaponData));
            return true;
        }

        public bool TryRemoveWeaponAt(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _activeWeapons.Count)
            {
                return false;
            }

            _activeWeapons[slotIndex].Release();
            _activeWeapons.RemoveAt(slotIndex);
            return true;
        }
    }
}
