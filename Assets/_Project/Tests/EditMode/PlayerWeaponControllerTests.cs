using System.Collections.Generic;
using System.Reflection;
using Game.Combat;
using Game.Player;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class PlayerWeaponControllerTests
    {
        private class CountingWeaponBehavior : IWeaponBehavior
        {
            public WeaponData WeaponData => null;
            public int StackCount => 1;
            public int TickCount;

            public void Tick(float deltaTime) => TickCount++;

            public void AddStack()
            {
            }
        }

        private GameObject _controllerObject;

        [TearDown]
        public void TearDown()
        {
            if (_controllerObject != null)
            {
                Object.DestroyImmediate(_controllerObject);
            }
        }

        private PlayerWeaponController CreateController(List<IWeaponBehavior> weapons, bool isFiringAllowed)
        {
            _controllerObject = new GameObject("TestWeaponController");
            var controller = _controllerObject.AddComponent<PlayerWeaponController>();
            controller.Init(weapons, () => isFiringAllowed);
            return controller;
        }

        private static void RunUpdate(PlayerWeaponController controller)
        {
            typeof(PlayerWeaponController).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(controller, null);
        }

        [Test]
        public void Update_WeaponAddedAfterInit_IsTicked()
        {
            var weapons = new List<IWeaponBehavior>();
            var controller = CreateController(weapons, isFiringAllowed: true);
            var lateWeapon = new CountingWeaponBehavior();

            weapons.Add(lateWeapon);
            RunUpdate(controller);

            Assert.AreEqual(1, lateWeapon.TickCount);
        }

        [Test]
        public void Update_FiringNotAllowed_DoesNotTickWeapons()
        {
            var weapon = new CountingWeaponBehavior();
            var controller = CreateController(new List<IWeaponBehavior> { weapon }, isFiringAllowed: false);

            RunUpdate(controller);

            Assert.AreEqual(0, weapon.TickCount);
        }
    }
}
