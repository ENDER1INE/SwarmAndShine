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
            public int StackCount { get; private set; } = 1;

            public void Tick(float deltaTime)
            {
            }

            public void AddStack()
            {
                StackCount++;
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
        public void TryAddWeapon_DuplicateWeaponData_AddsStackInsteadOfNewSlot()
        {
            var factory = new FakeWeaponBehaviorFactory();
            var manager = new WeaponSlotManager(factory);
            var weaponData = ScriptableObject.CreateInstance<WeaponData>();

            manager.TryAddWeapon(weaponData);
            bool secondResult = manager.TryAddWeapon(weaponData);

            Assert.IsTrue(secondResult);
            Assert.AreEqual(1, manager.ActiveWeapons.Count);
            Assert.AreEqual(1, factory.CreateCallCount);
            Assert.AreEqual(2, manager.ActiveWeapons[0].StackCount);
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
        public void IsFull_SlotsBelowLimit_ReturnsFalse()
        {
            var factory = new FakeWeaponBehaviorFactory();
            var manager = new WeaponSlotManager(factory);

            manager.TryAddWeapon(ScriptableObject.CreateInstance<WeaponData>());

            Assert.IsFalse(manager.IsFull);
        }
    }
}
