using System.Reflection;
using Game.Combat;
using Game.Enemies;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class EnemyChaseBehaviorTests
    {
        private class FakeDamageable : IDamageable
        {
            public int HitCount;
            public float LastDamage;

            public void TakeDamage(float damage)
            {
                HitCount++;
                LastDamage = damage;
            }
        }

        private static EnemyData CreateEnemyData(float moveSpeed, float contactDamage, float contactDamageInterval)
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();
            SetField(data, "_moveSpeed", moveSpeed);
            SetField(data, "_contactDamage", contactDamage);
            SetField(data, "_contactDamageInterval", contactDamageInterval);
            return data;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        [Test]
        public void Tick_TargetOutOfContactRange_MovesTowardTarget()
        {
            var enemyData = CreateEnemyData(moveSpeed: 2f, contactDamage: 5f, contactDamageInterval: 1f);
            var damageable = new FakeDamageable();
            var behavior = new EnemyChaseBehavior(enemyData, damageable, contactRange: 0.5f);

            Vector2 movement = behavior.Tick(1f, Vector2.zero, new Vector2(10f, 0f));

            Assert.AreEqual(new Vector2(2f, 0f), movement);
            Assert.AreEqual(0, damageable.HitCount);
        }

        [Test]
        public void Tick_TargetWithinContactRange_DoesNotMove()
        {
            var enemyData = CreateEnemyData(moveSpeed: 2f, contactDamage: 5f, contactDamageInterval: 1f);
            var damageable = new FakeDamageable();
            var behavior = new EnemyChaseBehavior(enemyData, damageable, contactRange: 0.5f);

            Vector2 movement = behavior.Tick(1f, Vector2.zero, new Vector2(0.2f, 0f));

            Assert.AreEqual(Vector2.zero, movement);
        }

        [Test]
        public void Tick_TargetWithinContactRange_DealsContactDamageOnFirstTick()
        {
            var enemyData = CreateEnemyData(moveSpeed: 2f, contactDamage: 5f, contactDamageInterval: 1f);
            var damageable = new FakeDamageable();
            var behavior = new EnemyChaseBehavior(enemyData, damageable, contactRange: 0.5f);

            behavior.Tick(0.1f, Vector2.zero, new Vector2(0.2f, 0f));

            Assert.AreEqual(1, damageable.HitCount);
            Assert.AreEqual(5f, damageable.LastDamage);
        }

        [Test]
        public void Tick_WithinContactRange_DoesNotDealDamageAgainBeforeIntervalElapsed()
        {
            var enemyData = CreateEnemyData(moveSpeed: 2f, contactDamage: 5f, contactDamageInterval: 1f);
            var damageable = new FakeDamageable();
            var behavior = new EnemyChaseBehavior(enemyData, damageable, contactRange: 0.5f);

            behavior.Tick(0.1f, Vector2.zero, new Vector2(0.2f, 0f));
            behavior.Tick(0.5f, Vector2.zero, new Vector2(0.2f, 0f));

            Assert.AreEqual(1, damageable.HitCount);
        }

        [Test]
        public void Tick_WithinContactRange_DealsDamageAgainAfterIntervalElapsed()
        {
            var enemyData = CreateEnemyData(moveSpeed: 2f, contactDamage: 5f, contactDamageInterval: 1f);
            var damageable = new FakeDamageable();
            var behavior = new EnemyChaseBehavior(enemyData, damageable, contactRange: 0.5f);

            behavior.Tick(0.1f, Vector2.zero, new Vector2(0.2f, 0f));
            behavior.Tick(1f, Vector2.zero, new Vector2(0.2f, 0f));

            Assert.AreEqual(2, damageable.HitCount);
        }

        [Test]
        public void Tick_LeavesContactRangeAfterBeingInIt_ResetsContactTimer()
        {
            var enemyData = CreateEnemyData(moveSpeed: 2f, contactDamage: 5f, contactDamageInterval: 1f);
            var damageable = new FakeDamageable();
            var behavior = new EnemyChaseBehavior(enemyData, damageable, contactRange: 0.5f);

            behavior.Tick(0.1f, Vector2.zero, new Vector2(0.2f, 0f));
            behavior.Tick(1f, Vector2.zero, new Vector2(10f, 0f));
            behavior.Tick(0.1f, Vector2.zero, new Vector2(0.2f, 0f));

            Assert.AreEqual(2, damageable.HitCount);
        }
    }
}
