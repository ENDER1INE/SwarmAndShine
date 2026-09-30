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

            public void FindTargetsInRange(Vector2 fromPosition, float maxRange, System.Collections.Generic.List<ICombatTarget> results)
            {
                results.Clear();
                if (TargetToReturn != null)
                {
                    results.Add(TargetToReturn);
                }
            }
        }

        private class FakeProjectileSpawner : IProjectileSpawner
        {
            public int SpawnCount;
            public float LastDamage;
            public Vector2 LastOrigin;
            public Vector2 LastDirection;

            public void SpawnProjectile(Vector2 origin, Vector2 direction, float damage, ICombatTarget target)
            {
                SpawnCount++;
                LastDamage = damage;
                LastOrigin = origin;
                LastDirection = direction;
            }
        }

        private class FakeWeaponMount : IWeaponMount
        {
            public Vector2 Position { get; set; } = Vector2.zero;
            public Vector2 MuzzlePosition { get; set; } = Vector2.zero;
            public Vector2 LastAimDirection;
            public int AimCount;
            public int ClearAimCount;

            public void AimAt(Vector2 direction)
            {
                AimCount++;
                LastAimDirection = direction;
            }

            public void ClearAim()
            {
                ClearAimCount++;
            }

            public void PlayThrust(float distance, float duration)
            {
            }

            public void PlaySwing(float startOffsetDegrees, float endOffsetDegrees, float duration)
            {
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
            var weapon = new RangedWeaponBehavior(weaponData, stats, targetProvider, spawner, new FakeWeaponMount());

            weapon.Tick(1f);

            Assert.AreEqual(0, spawner.SpawnCount);
        }

        [Test]
        public void Tick_NoTargetInRange_ClearsAim()
        {
            var weaponData = CreateWeaponData(WeaponCategory.Ranged, 10f, 1f, 5f);
            var mount = new FakeWeaponMount();
            var weapon = new RangedWeaponBehavior(
                weaponData, CreateStats(), new FakeTargetProvider(), new FakeProjectileSpawner(), mount);

            weapon.Tick(0.1f);

            Assert.AreEqual(1, mount.ClearAimCount);
            Assert.AreEqual(0, mount.AimCount);
        }

        [Test]
        public void Tick_TargetInRange_AimsMountTowardTargetEvenDuringCooldown()
        {
            var weaponData = CreateWeaponData(WeaponCategory.Ranged, 10f, 1f, 5f);
            var mount = new FakeWeaponMount();
            var target = new FakeCombatTarget { Position = new Vector2(0f, 3f) };
            var weapon = new RangedWeaponBehavior(
                weaponData,
                CreateStats(),
                new FakeTargetProvider { TargetToReturn = target },
                new FakeProjectileSpawner(),
                mount);

            weapon.Tick(0f);
            weapon.Tick(0.1f);

            Assert.AreEqual(2, mount.AimCount);
            Assert.AreEqual(Vector2.up, mount.LastAimDirection);
        }

        [Test]
        public void Tick_TargetInRange_FiresFromMuzzleTowardTarget()
        {
            var weaponData = CreateWeaponData(WeaponCategory.Ranged, 10f, 1f, 5f);
            var mount = new FakeWeaponMount { Position = Vector2.zero, MuzzlePosition = new Vector2(1f, 0f) };
            var target = new FakeCombatTarget { Position = new Vector2(1f, 4f) };
            var spawner = new FakeProjectileSpawner();
            var weapon = new RangedWeaponBehavior(
                weaponData, CreateStats(), new FakeTargetProvider { TargetToReturn = target }, spawner, mount);

            weapon.Tick(0f);

            Assert.AreEqual(new Vector2(1f, 0f), spawner.LastOrigin);
            Assert.AreEqual(Vector2.up, spawner.LastDirection);
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
            var weapon = new RangedWeaponBehavior(weaponData, stats, targetProvider, spawner, new FakeWeaponMount());

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
            var weapon = new RangedWeaponBehavior(weaponData, stats, targetProvider, spawner, new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(0.1f); // кулдаун при 1 атаке/сек = 1с, ещё не прошёл

            Assert.AreEqual(1, spawner.SpawnCount);
        }
    }
}
