using System.Collections.Generic;
using System.Reflection;
using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class MeleeThrustBehaviorTests
    {
        private const float AttackDuration = 0.2f;

        private class FakeCombatTarget : ICombatTarget
        {
            public Vector2 Position { get; set; }
            public bool IsAlive { get; set; } = true;
            public float DamageTaken;
            public int HitCount;

            public void TakeDamage(float damage)
            {
                DamageTaken += damage;
                HitCount++;
            }
        }

        private class FakeTargetProvider : ITargetProvider
        {
            public List<FakeCombatTarget> Targets = new();

            public ICombatTarget FindNearestTarget(Vector2 fromPosition, float maxRange)
            {
                FakeCombatTarget nearest = null;
                float nearestDistance = maxRange;
                foreach (var target in Targets)
                {
                    float distance = Vector2.Distance(fromPosition, target.Position);
                    if (target.IsAlive && distance <= nearestDistance)
                    {
                        nearestDistance = distance;
                        nearest = target;
                    }
                }

                return nearest;
            }

            public void FindTargetsInRange(Vector2 fromPosition, float maxRange, List<ICombatTarget> results)
            {
                results.Clear();
                foreach (var target in Targets)
                {
                    if (target.IsAlive && Vector2.Distance(fromPosition, target.Position) <= maxRange)
                    {
                        results.Add(target);
                    }
                }
            }
        }

        private class FakeWeaponMount : IWeaponMount
        {
            public Vector2 Position { get; set; } = Vector2.zero;
            public Vector2 MuzzlePosition { get; set; } = Vector2.zero;
            public int ThrustCount;
            public int ClearAimCount;
            public float LastThrustDistance;
            public float LastThrustDuration;

            public void AimAt(Vector2 direction)
            {
            }

            public void ClearAim()
            {
                ClearAimCount++;
            }

            public void Release()
            {
            }

            public void PlayThrust(float distance, float duration)
            {
                ThrustCount++;
                LastThrustDistance = distance;
                LastThrustDuration = duration;
            }

            public void PlaySwing(float startOffsetDegrees, float endOffsetDegrees, float duration)
            {
            }
        }

        private static WeaponData CreateWeaponData(float baseDamage = 10f, float attacksPerSecond = 1f, float range = 2f, float hitWidth = 0.6f)
        {
            var data = ScriptableObject.CreateInstance<WeaponData>();
            SetField(data, "_category", WeaponCategory.Melee);
            SetField(data, "_attackPattern", AttackPatternType.MeleeThrust);
            SetField(data, "_baseDamage", baseDamage);
            SetField(data, "_baseAttacksPerSecond", attacksPerSecond);
            SetField(data, "_range", range);
            SetField(data, "_meleeHitWidth", hitWidth);
            SetField(data, "_meleeAttackDuration", AttackDuration);
            return data;
        }

        private static CharacterStats CreateStats()
        {
            return new CharacterStats(ScriptableObject.CreateInstance<CharacterBaseStatsData>());
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        [Test]
        public void Tick_NoTargetInRange_DoesNotAttackAndClearsAim()
        {
            var mount = new FakeWeaponMount();
            var weapon = new MeleeThrustBehavior(CreateWeaponData(), CreateStats(), new FakeTargetProvider(), mount);

            weapon.Tick(0.1f);

            Assert.AreEqual(0, mount.ThrustCount);
            Assert.AreEqual(1, mount.ClearAimCount);
        }

        [Test]
        public void Tick_TargetInRange_StartsThrustWithDataDrivenDistanceAndDuration()
        {
            var mount = new FakeWeaponMount();
            var provider = new FakeTargetProvider();
            provider.Targets.Add(new FakeCombatTarget { Position = new Vector2(1.5f, 0f) });
            var weapon = new MeleeThrustBehavior(CreateWeaponData(range: 2f), CreateStats(), provider, mount);

            weapon.Tick(0f);

            Assert.AreEqual(1, mount.ThrustCount);
            Assert.AreEqual(2f, mount.LastThrustDistance, 0.0001f);
            Assert.AreEqual(AttackDuration, mount.LastThrustDuration, 0.0001f);
        }

        [Test]
        public void Tick_DamageIsDealtAtThrustPeakNotAtStart()
        {
            var target = new FakeCombatTarget { Position = new Vector2(1.5f, 0f) };
            var provider = new FakeTargetProvider();
            provider.Targets.Add(target);
            var weapon = new MeleeThrustBehavior(CreateWeaponData(), CreateStats(), provider, new FakeWeaponMount());

            weapon.Tick(0f);
            Assert.AreEqual(0, target.HitCount);

            weapon.Tick(AttackDuration * 0.5f);
            Assert.AreEqual(1, target.HitCount);
        }

        [Test]
        public void Tick_SingleThrust_HitsEachTargetOnlyOnce()
        {
            var target = new FakeCombatTarget { Position = new Vector2(1.5f, 0f) };
            var provider = new FakeTargetProvider();
            provider.Targets.Add(target);
            var weapon = new MeleeThrustBehavior(CreateWeaponData(), CreateStats(), provider, new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration * 0.5f);
            weapon.Tick(AttackDuration * 0.5f);

            Assert.AreEqual(1, target.HitCount);
        }

        [Test]
        public void Tick_ThrustHitsEveryEnemyInTheZoneButNotOnesOutsideIt()
        {
            var inFront = new FakeCombatTarget { Position = new Vector2(1.5f, 0f) };
            var behindInFront = new FakeCombatTarget { Position = new Vector2(1.9f, 0.1f) };
            var offToTheSide = new FakeCombatTarget { Position = new Vector2(0.5f, 1.5f) };
            var provider = new FakeTargetProvider();
            provider.Targets.AddRange(new[] { inFront, behindInFront, offToTheSide });
            var weapon = new MeleeThrustBehavior(CreateWeaponData(range: 2f, hitWidth: 0.6f), CreateStats(), provider, new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration * 0.5f);

            Assert.AreEqual(1, inFront.HitCount);
            Assert.AreEqual(1, behindInFront.HitCount);
            Assert.AreEqual(0, offToTheSide.HitCount);
        }

        [Test]
        public void Tick_DamageScalesWithMeleeDamageStat()
        {
            var stats = CreateStats();
            stats.AddModifier(new StatModifier(StatType.MeleeDamagePercent, ModifierType.Flat, 0.5f));
            var target = new FakeCombatTarget { Position = new Vector2(1.5f, 0f) };
            var provider = new FakeTargetProvider();
            provider.Targets.Add(target);
            var weapon = new MeleeThrustBehavior(CreateWeaponData(baseDamage: 10f), stats, provider, new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration * 0.5f);

            Assert.AreEqual(15f, target.DamageTaken, 0.0001f);
        }

        [Test]
        public void Tick_DuringCooldown_DoesNotStartSecondThrust()
        {
            var mount = new FakeWeaponMount();
            var provider = new FakeTargetProvider();
            provider.Targets.Add(new FakeCombatTarget { Position = new Vector2(1.5f, 0f) });
            var weapon = new MeleeThrustBehavior(CreateWeaponData(attacksPerSecond: 1f), CreateStats(), provider, mount);

            weapon.Tick(0f);
            weapon.Tick(AttackDuration);
            weapon.Tick(0.2f);

            Assert.AreEqual(1, mount.ThrustCount);
        }

        [Test]
        public void Tick_AfterCooldown_StartsNextThrust()
        {
            var mount = new FakeWeaponMount();
            var provider = new FakeTargetProvider();
            provider.Targets.Add(new FakeCombatTarget { Position = new Vector2(1.5f, 0f) });
            var weapon = new MeleeThrustBehavior(CreateWeaponData(attacksPerSecond: 1f), CreateStats(), provider, mount);

            weapon.Tick(0f);
            weapon.Tick(AttackDuration);
            weapon.Tick(1f);

            Assert.AreEqual(2, mount.ThrustCount);
        }
    }
}
