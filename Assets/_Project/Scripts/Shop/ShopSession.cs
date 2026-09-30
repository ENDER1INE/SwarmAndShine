using System;
using System.Collections.Generic;
using Game.Combat;
using Game.Core;
using Game.Run;

namespace Game.Shop
{
    public class ShopSession : IDisposable
    {
        private readonly ShopConfig _config;
        private readonly ShopSlotGenerator _generator;
        private readonly RunWallet _wallet;
        private readonly WeaponSlotManager _weaponSlots;
        private readonly CharacterStats _characterStats;
        private readonly List<ShopSlot> _slots = new();
        private int _rerollCount;
        private int _currentWaveIndex;

        public IReadOnlyList<ShopSlot> Slots => _slots;
        public int RerollCost => _config.RerollBaseCost + _config.RerollCostIncrement * _rerollCount;

        public ShopSession(
            ShopConfig config,
            ShopSlotGenerator generator,
            RunWallet wallet,
            WeaponSlotManager weaponSlots,
            CharacterStats characterStats)
        {
            _config = config;
            _generator = generator;
            _wallet = wallet;
            _weaponSlots = weaponSlots;
            _characterStats = characterStats;

            for (int i = 0; i < config.SlotCount; i++)
            {
                _slots.Add(new ShopSlot());
            }

            GameEvents.Subscribe<ShopOpenedEvent>(OnShopOpened);
        }

        public PurchaseResult TryBuy(int slotIndex)
        {
            ShopSlot slot = _slots[slotIndex];
            if (slot.IsEmpty)
            {
                return PurchaseResult.EmptySlot;
            }

            ShopItemData item = slot.Item;
            if (item.Weapon != null && !_weaponSlots.CanAddWeapon(item.Weapon))
            {
                return PurchaseResult.WeaponSlotsFull;
            }

            if (!_wallet.TrySpend(item.Price))
            {
                return PurchaseResult.NotEnoughCurrency;
            }

            ApplyItem(item);
            slot.SetItem(null);
            slot.SetLocked(false);
            GameEvents.Publish(new ShopContentChangedEvent());
            return PurchaseResult.Success;
        }

        public bool TryReroll()
        {
            if (!_wallet.TrySpend(RerollCost))
            {
                return false;
            }

            _rerollCount++;
            FillUnlockedSlots();
            return true;
        }

        public void ToggleLock(int slotIndex)
        {
            ShopSlot slot = _slots[slotIndex];
            if (slot.IsEmpty)
            {
                return;
            }

            slot.SetLocked(!slot.IsLocked);
            GameEvents.Publish(new ShopContentChangedEvent());
        }

        public void Dispose()
        {
            GameEvents.Unsubscribe<ShopOpenedEvent>(OnShopOpened);
        }

        private void OnShopOpened(ShopOpenedEvent evt)
        {
            _currentWaveIndex = evt.CompletedWaveIndex;
            FillUnlockedSlots();
        }

        private void FillUnlockedSlots()
        {
            foreach (ShopSlot slot in _slots)
            {
                if (!slot.IsLocked)
                {
                    slot.SetItem(_generator.PickItem(_currentWaveIndex));
                }
            }

            GameEvents.Publish(new ShopContentChangedEvent());
        }

        private void ApplyItem(ShopItemData item)
        {
            if (item.Weapon != null)
            {
                _weaponSlots.TryAddWeapon(item.Weapon);
            }

            foreach (StatModifierData modifierData in item.StatModifiers)
            {
                _characterStats.AddModifier(modifierData.ToModifier(item));
            }
        }
    }
}
