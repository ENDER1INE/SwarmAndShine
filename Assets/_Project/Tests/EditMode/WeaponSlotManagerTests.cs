using System.Collections.Generic;
using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class WeaponSlotManagerTests
    {
        private class FakeWeaponBehavior : IWeaponBehavior
        {
            public FakeWeaponBehavior(WeaponData weaponData)
            {
                WeaponData = weaponData;
            }

            public WeaponData WeaponData { get; }

            public bool IsReleased { get; private set; }

            public void Tick(float deltaTime)
            {
            }

            public void Release()
            {
                IsReleased = true;
            }
        }

        private class FakeWeaponBehaviorFactory : IWeaponBehaviorFactory
        {
            public int CreateCallCount;

            public IWeaponBehavior Create(WeaponData weaponData)
            {
                CreateCallCount++;
                return new FakeWeaponBehavior(weaponData);
            }
        }

        [Test]
        public void TryAddWeapon_NewWeaponData_CreatesNewSlot()
        {
            var factory = new FakeWeaponBehaviorFactory();
            var manager = new WeaponSlotManager(factory);
            var weaponData = ScriptableObject.CreateInstance<WeaponData>();

            bool result = manager.TryAddWeapon(weaponData);

            Assert.IsTrue(result);
            Assert.AreEqual(1, manager.ActiveWeapons.Count);
            Assert.AreEqual(1, factory.CreateCallCount);
        }

        [Test]
        public void TryAddWeapon_DuplicateWeaponData_CreatesSeparateSlot()
        {
            var factory = new FakeWeaponBehaviorFactory();
            var manager = new WeaponSlotManager(factory);
            var weaponData = ScriptableObject.CreateInstance<WeaponData>();

            manager.TryAddWeapon(weaponData);
            bool secondResult = manager.TryAddWeapon(weaponData);

            Assert.IsTrue(secondResult);
            Assert.AreEqual(2, manager.ActiveWeapons.Count);
            Assert.AreEqual(2, factory.CreateCallCount);
            Assert.AreNotSame(manager.ActiveWeapons[0], manager.ActiveWeapons[1]);
        }

        [Test]
        public void TryAddWeapon_SlotsFull_RejectsDuplicateWeapon()
        {
            var factory = new FakeWeaponBehaviorFactory();
            var manager = new WeaponSlotManager(factory);
            var weaponData = ScriptableObject.CreateInstance<WeaponData>();
            for (int i = 0; i < WeaponSlotManager.MaxSlots; i++)
            {
                manager.TryAddWeapon(weaponData);
            }

            bool result = manager.TryAddWeapon(weaponData);

            Assert.IsFalse(result);
            Assert.AreEqual(WeaponSlotManager.MaxSlots, manager.ActiveWeapons.Count);
        }

        [Test]
        public void TryAddWeapon_SlotsFull_RejectsNewWeaponType()
        {
            var factory = new FakeWeaponBehaviorFactory();
            var manager = new WeaponSlotManager(factory);
            var distinctWeapons = new List<WeaponData>();
            for (int i = 0; i < WeaponSlotManager.MaxSlots; i++)
            {
                var weaponData = ScriptableObject.CreateInstance<WeaponData>();
                distinctWeapons.Add(weaponData);
                manager.TryAddWeapon(weaponData);
            }

            var overflowWeaponData = ScriptableObject.CreateInstance<WeaponData>();
            bool result = manager.TryAddWeapon(overflowWeaponData);

            Assert.IsFalse(result);
            Assert.AreEqual(WeaponSlotManager.MaxSlots, manager.ActiveWeapons.Count);
        }

        [Test]
        public void TryRemoveWeaponAt_ValidIndex_ReleasesAndRemovesOnlyThatWeapon()
        {
            var manager = new WeaponSlotManager(new FakeWeaponBehaviorFactory());
            var weaponData = ScriptableObject.CreateInstance<WeaponData>();
            manager.TryAddWeapon(weaponData);
            manager.TryAddWeapon(weaponData);
            var removed = (FakeWeaponBehavior)manager.ActiveWeapons[0];
            IWeaponBehavior kept = manager.ActiveWeapons[1];

            bool result = manager.TryRemoveWeaponAt(0);

            Assert.IsTrue(result);
            Assert.IsTrue(removed.IsReleased);
            Assert.AreEqual(1, manager.ActiveWeapons.Count);
            Assert.AreSame(kept, manager.ActiveWeapons[0]);
        }

        [Test]
        public void TryRemoveWeaponAt_InvalidIndex_ReturnsFalse()
        {
            var manager = new WeaponSlotManager(new FakeWeaponBehaviorFactory());
            manager.TryAddWeapon(ScriptableObject.CreateInstance<WeaponData>());

            Assert.IsFalse(manager.TryRemoveWeaponAt(-1));
            Assert.IsFalse(manager.TryRemoveWeaponAt(1));
            Assert.AreEqual(1, manager.ActiveWeapons.Count);
        }

        [Test]
        public void TryAddWeapon_AfterRemoval_FreesSlotForNewWeapon()
        {
            var manager = new WeaponSlotManager(new FakeWeaponBehaviorFactory());
            var weaponData = ScriptableObject.CreateInstance<WeaponData>();
            for (int i = 0; i < WeaponSlotManager.MaxSlots; i++)
            {
                manager.TryAddWeapon(weaponData);
            }

            manager.TryRemoveWeaponAt(2);

            Assert.IsFalse(manager.IsFull);
            Assert.IsTrue(manager.TryAddWeapon(weaponData));
        }

        [Test]
        public void IsFull_SlotsBelowLimit_ReturnsFalse()
        {
            var factory = new FakeWeaponBehaviorFactory();
            var manager = new WeaponSlotManager(factory);

            manager.TryAddWeapon(ScriptableObject.CreateInstance<WeaponData>());

            Assert.IsFalse(manager.IsFull);
        }
    }
}
