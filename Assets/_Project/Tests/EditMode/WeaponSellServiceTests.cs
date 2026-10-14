using System.Reflection;
using Game.Combat;
using Game.Shop;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class WeaponSellServiceTests
    {
        private class FakeWeaponBehavior : IWeaponBehavior
        {
            public FakeWeaponBehavior(WeaponData weaponData)
            {
                WeaponData = weaponData;
            }

            public WeaponData WeaponData { get; }

            public void Tick(float deltaTime)
            {
            }

            public void Release()
            {
            }
        }

        private class FakeWeaponBehaviorFactory : IWeaponBehaviorFactory
        {
            public IWeaponBehavior Create(WeaponData weaponData) => new FakeWeaponBehavior(weaponData);
        }

        private static WeaponData CreateWeapon(int basePrice)
        {
            var weapon = ScriptableObject.CreateInstance<WeaponData>();
            FieldInfo field = typeof(WeaponData).GetField("_basePrice", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(weapon, basePrice);
            return weapon;
        }

        private static (WeaponSellService service, WeaponSlotManager slots, RunWallet wallet) CreateService(float ratio)
        {
            var slots = new WeaponSlotManager(new FakeWeaponBehaviorFactory());
            var wallet = new RunWallet();
            return (new WeaponSellService(slots, wallet, ratio), slots, wallet);
        }

        [Test]
        public void GetSellPrice_AppliesRatioToBasePrice()
        {
            var (service, _, wallet) = CreateService(0.5f);
            using (wallet)
            {
                Assert.AreEqual(10, service.GetSellPrice(CreateWeapon(20)));
            }
        }

        [Test]
        public void GetSellPrice_FractionalResult_RoundsDown()
        {
            var (service, _, wallet) = CreateService(0.5f);
            using (wallet)
            {
                Assert.AreEqual(3, service.GetSellPrice(CreateWeapon(7)));
            }
        }

        [Test]
        public void TrySell_ValidSlot_RemovesWeaponAndAddsCurrency()
        {
            var (service, slots, wallet) = CreateService(0.5f);
            using (wallet)
            {
                slots.TryAddWeapon(CreateWeapon(20));

                bool result = service.TrySell(0);

                Assert.IsTrue(result);
                Assert.AreEqual(0, slots.ActiveWeapons.Count);
                Assert.AreEqual(10, wallet.Balance);
            }
        }

        [Test]
        public void TrySell_InvalidSlot_ChangesNothing()
        {
            var (service, slots, wallet) = CreateService(0.5f);
            using (wallet)
            {
                slots.TryAddWeapon(CreateWeapon(20));

                bool result = service.TrySell(3);

                Assert.IsFalse(result);
                Assert.AreEqual(1, slots.ActiveWeapons.Count);
                Assert.AreEqual(0, wallet.Balance);
            }
        }

        [Test]
        public void TrySell_OneOfTwoDuplicates_KeepsTheOther()
        {
            var (service, slots, wallet) = CreateService(1f);
            using (wallet)
            {
                WeaponData weapon = CreateWeapon(8);
                slots.TryAddWeapon(weapon);
                slots.TryAddWeapon(weapon);

                service.TrySell(0);

                Assert.AreEqual(1, slots.ActiveWeapons.Count);
                Assert.AreEqual(8, wallet.Balance);
            }
        }
    }
}
