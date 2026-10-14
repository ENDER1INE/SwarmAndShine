using System.Collections.Generic;
using Game.Combat;
using Game.Core;

namespace Game.Shop
{
    public class WeaponSellService
    {
        private readonly WeaponSlotManager _weaponSlots;
        private readonly RunWallet _wallet;
        private readonly float _sellPriceRatio;

        public WeaponSellService(WeaponSlotManager weaponSlots, RunWallet wallet, float sellPriceRatio)
        {
            _weaponSlots = weaponSlots;
            _wallet = wallet;
            _sellPriceRatio = sellPriceRatio;
        }

        public IReadOnlyList<IWeaponBehavior> Weapons => _weaponSlots.ActiveWeapons;

        public int GetSellPrice(WeaponData weaponData)
        {
            return (int)(weaponData.BasePrice * _sellPriceRatio);
        }

        public bool TrySell(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _weaponSlots.ActiveWeapons.Count)
            {
                return false;
            }

            int sellPrice = GetSellPrice(_weaponSlots.ActiveWeapons[slotIndex].WeaponData);
            _weaponSlots.TryRemoveWeaponAt(slotIndex);
            _wallet.Add(sellPrice);
            GameEvents.Publish(new WeaponInventoryChangedEvent());
            return true;
        }
    }
}
