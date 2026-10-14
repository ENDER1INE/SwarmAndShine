using System.Collections.Generic;
using System.Reflection;
using Game.Combat;
using Game.Core;
using Game.Run;
using Game.Shop;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class ShopSessionTests
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

        private class SequenceItemPoolRule : IShopPoolRule
        {
            public float GetWeight(ShopItemData item, int waveIndex) => 1f;
        }

        private class FixedRandomSource : IRandomSource
        {
            public float Value;

            public float NextValue() => Value;
        }

        private class Fixture
        {
            public ShopConfig Config;
            public RunWallet Wallet;
            public WeaponSlotManager WeaponSlots;
            public CharacterStats Stats;
            public FixedRandomSource Random;
            public ShopSession Session;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        private static ShopItemData CreateItem(int price, WeaponData weapon = null)
        {
            var item = ScriptableObject.CreateInstance<ShopItemData>();
            SetField(item, "_price", price);
            SetField(item, "_weapon", weapon);
            return item;
        }

        private static Fixture CreateFixture(int startingCurrency, params ShopItemData[] items)
        {
            var fixture = new Fixture();
            fixture.Config = ScriptableObject.CreateInstance<ShopConfig>();
            SetField(fixture.Config, "_slotCount", 2);
            SetField(fixture.Config, "_rerollBaseCost", 5);
            SetField(fixture.Config, "_rerollCostIncrement", 3);

            fixture.Wallet = new RunWallet(startingCurrency);
            fixture.WeaponSlots = new WeaponSlotManager(new FakeWeaponBehaviorFactory());
            fixture.Stats = new CharacterStats(ScriptableObject.CreateInstance<CharacterBaseStatsData>());
            fixture.Random = new FixedRandomSource();

            var generator = new ShopSlotGenerator(new List<ShopItemData>(items), new SequenceItemPoolRule(), fixture.Random);
            fixture.Session = new ShopSession(fixture.Config, generator, fixture.Wallet, fixture.WeaponSlots, fixture.Stats);
            return fixture;
        }

        private static void OpenShop()
        {
            GameEvents.Publish(new ShopOpenedEvent(0));
        }

        [Test]
        public void ShopOpened_FillsAllSlots()
        {
            var fixture = CreateFixture(0, CreateItem(10));
            using var session = fixture.Session;

            OpenShop();

            Assert.AreEqual(2, session.Slots.Count);
            Assert.IsFalse(session.Slots[0].IsEmpty);
            Assert.IsFalse(session.Slots[1].IsEmpty);
        }

        [Test]
        public void TryBuy_EnoughCurrency_SpendsPriceAndEmptiesSlot()
        {
            var fixture = CreateFixture(20, CreateItem(8));
            using var session = fixture.Session;
            OpenShop();

            PurchaseResult result = session.TryBuy(0);

            Assert.AreEqual(PurchaseResult.Success, result);
            Assert.AreEqual(12, fixture.Wallet.Balance);
            Assert.IsTrue(session.Slots[0].IsEmpty);
        }

        [Test]
        public void TryBuy_NotEnoughCurrency_KeepsItemAndBalance()
        {
            var fixture = CreateFixture(5, CreateItem(8));
            using var session = fixture.Session;
            OpenShop();

            PurchaseResult result = session.TryBuy(0);

            Assert.AreEqual(PurchaseResult.NotEnoughCurrency, result);
            Assert.AreEqual(5, fixture.Wallet.Balance);
            Assert.IsFalse(session.Slots[0].IsEmpty);
        }

        [Test]
        public void TryBuy_WeaponItem_AddsWeaponToSlots()
        {
            var weapon = ScriptableObject.CreateInstance<WeaponData>();
            var fixture = CreateFixture(20, CreateItem(8, weapon));
            using var session = fixture.Session;
            OpenShop();

            session.TryBuy(0);

            Assert.AreEqual(1, fixture.WeaponSlots.ActiveWeapons.Count);
        }

        [Test]
        public void TryBuy_WeaponSlotsFull_RejectsWithoutSpending()
        {
            var fixture = CreateFixture(100, CreateItem(8, ScriptableObject.CreateInstance<WeaponData>()));
            using var session = fixture.Session;
            for (int i = 0; i < WeaponSlotManager.MaxSlots; i++)
            {
                fixture.WeaponSlots.TryAddWeapon(ScriptableObject.CreateInstance<WeaponData>());
            }

            OpenShop();

            PurchaseResult result = session.TryBuy(0);

            Assert.AreEqual(PurchaseResult.WeaponSlotsFull, result);
            Assert.AreEqual(100, fixture.Wallet.Balance);
        }

        [Test]
        public void CanBuy_WeaponWithFullSlots_ReturnsFalseUntilSlotFreed()
        {
            var fixture = CreateFixture(100, CreateItem(8, ScriptableObject.CreateInstance<WeaponData>()));
            using var session = fixture.Session;
            for (int i = 0; i < WeaponSlotManager.MaxSlots; i++)
            {
                fixture.WeaponSlots.TryAddWeapon(ScriptableObject.CreateInstance<WeaponData>());
            }

            OpenShop();
            Assert.IsFalse(session.CanBuy(0));

            fixture.WeaponSlots.TryRemoveWeaponAt(0);

            Assert.IsTrue(session.CanBuy(0));
        }

        [Test]
        public void CanBuy_NotEnoughCurrency_ReturnsFalse()
        {
            var fixture = CreateFixture(5, CreateItem(8));
            using var session = fixture.Session;
            OpenShop();

            Assert.IsFalse(session.CanBuy(0));
        }

        [Test]
        public void TryReroll_SpendsGrowingCost()
        {
            var fixture = CreateFixture(100, CreateItem(8));
            using var session = fixture.Session;
            OpenShop();

            session.TryReroll();
            session.TryReroll();

            Assert.AreEqual(100 - 5 - 8, fixture.Wallet.Balance);
            Assert.AreEqual(11, session.RerollCost);
        }

        [Test]
        public void TryReroll_NotEnoughCurrency_ReturnsFalse()
        {
            var fixture = CreateFixture(2, CreateItem(8));
            using var session = fixture.Session;
            OpenShop();

            bool result = session.TryReroll();

            Assert.IsFalse(result);
            Assert.AreEqual(2, fixture.Wallet.Balance);
        }

        [Test]
        public void TryReroll_LockedSlotKeepsItem()
        {
            var first = CreateItem(1);
            var second = CreateItem(2);
            var fixture = CreateFixture(100, first, second);
            using var session = fixture.Session;
            fixture.Random.Value = 0f;
            OpenShop();
            session.ToggleLock(0);

            fixture.Random.Value = 0.99f;
            session.TryReroll();

            Assert.AreSame(first, session.Slots[0].Item);
            Assert.AreSame(second, session.Slots[1].Item);
        }
    }
}
