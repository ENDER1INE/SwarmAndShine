using System.Reflection;
using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class RangedWeaponBehaviorTests
    {
        private class FakeCombatTarget : ICombatTarget
        {
            public Vector2 Position { get; set; }
            public bool IsAlive { get; set; } = true;

            public void TakeDamage(float damage)
            {
            }
        }

        private class FakeTargetProvider : ITargetProvider
        {
            public ICombatTarget TargetToReturn;

            public ICombatTarget FindNearestTarget(Vector2 fromPosition, float maxRange) => TargetToReturn;
        }

        private class FakeProjectileSpawner : IProjectileSpawner
        {
            public int SpawnCount;
            public float LastDamage;

            public void SpawnProjectile(Vector2 origin, Vector2 direction, float damage, ICombatTarget target)
            {
                SpawnCount++;
                LastDamage = damage;
            }
        }

        private static WeaponData CreateWeaponData(WeaponCategory category, float baseDamage, float attacksPerSecond, float range)
        {
            var data = ScriptableObject.CreateInstance<WeaponData>();
            SetField(data, "_category", category);
            SetField(data, "_baseDamage", baseDamage);
            SetField(data, "_baseAttacksPerSecond", attacksPerSecond);
            SetField(data, "_range", range);
            return data;
        }

        private static CharacterStats CreateStats()
        {
            var baseData = ScriptableObject.CreateInstance<CharacterBaseStatsData>();
            return new CharacterStats(baseData);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        [Test]
        public void Tick_NoTargetInRange_DoesNotSpawnProjectile()
        {
            var weaponData = CreateWeaponData(WeaponCategory.Ranged, 10f, 1f, 5f);
            var stats = CreateStats();
            var targetProvider = new FakeTargetProvider { TargetToReturn = null };
            var spawner = new FakeProjectileSpawner();
            var weapon = new RangedWeaponBehavior(weaponData, stats, targetProvider, spawner, () => Vector2.zero);

            weapon.Tick(1f);

            Assert.AreEqual(0, spawner.SpawnCount);
        }

        [Test]
        public void Tick_TargetInRange_SpawnsProjectileWithDamageScaledByStats()
        {
            var weaponData = CreateWeaponData(WeaponCategory.Ranged, 10f, 1f, 5f);
            var stats = CreateStats();
            stats.AddModifier(new StatModifier(StatType.RangedDamagePercent, ModifierType.Flat, 0.5f));
            var target = new FakeCombatTarget { Position = new Vector2(1f, 0f) };
            var targetProvider = new FakeTargetProvider { TargetToReturn = target };
            var spawner = new FakeProjectileSpawner();
            var weapon = new RangedWeaponBehavior(weaponData, stats, targetProvider, spawner, () => Vector2.zero);

            weapon.Tick(0f);

            Assert.AreEqual(1, spawner.SpawnCount);
            Assert.AreEqual(15f, spawner.LastDamage, 0.0001f); // 10 * (1 + 0.5)
        }

        [Test]
        public void Tick_CalledAgainDuringCooldown_DoesNotSpawnSecondProjectile()
        {
            var weaponData = CreateWeaponData(WeaponCategory.Ranged, 10f, 1f, 5f);
            var stats = CreateStats();
            var target = new FakeCombatTarget { Position = new Vector2(1f, 0f) };
            var targetProvider = new FakeTargetProvider { TargetToReturn = target };
            var spawner = new FakeProjectileSpawner();
            var weapon = new RangedWeaponBehavior(weaponData, stats, targetProvider, spawner, () => Vector2.zero);

            weapon.Tick(0f);
            weapon.Tick(0.1f); // кулдаун при 1 атаке/сек = 1с, ещё не прошёл

            Assert.AreEqual(1, spawner.SpawnCount);
        }
    }
}
