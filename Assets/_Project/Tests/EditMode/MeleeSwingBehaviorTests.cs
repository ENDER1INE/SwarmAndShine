using System.Collections.Generic;
using System.Reflection;
using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class MeleeSwingBehaviorTests
    {
        private const float AttackDuration = 0.2f;
        private const float ArcDegrees = 100f;

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
            public int SwingCount;
            public int ClearAimCount;
            public readonly List<Vector2> SwingArcs = new();
            public float LastSwingDuration;

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
            }

            public void PlaySwing(float startOffsetDegrees, float endOffsetDegrees, float duration)
            {
                SwingCount++;
                SwingArcs.Add(new Vector2(startOffsetDegrees, endOffsetDegrees));
                LastSwingDuration = duration;
            }
        }

        private static WeaponData CreateWeaponData(float baseDamage = 10f, float attacksPerSecond = 1f, float range = 2f)
        {
            var data = ScriptableObject.CreateInstance<WeaponData>();
            SetField(data, "_category", WeaponCategory.Melee);
            SetField(data, "_attackPattern", AttackPatternType.MeleeSwing);
            SetField(data, "_baseDamage", baseDamage);
            SetField(data, "_baseAttacksPerSecond", attacksPerSecond);
            SetField(data, "_range", range);
            SetField(data, "_meleeHitWidth", 0.4f);
            SetField(data, "_meleeAttackDuration", AttackDuration);
            SetField(data, "_meleeSwingArcDegrees", ArcDegrees);
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

        private static Vector2 PolarPosition(float angleDegrees, float distance)
        {
            float radians = angleDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * distance;
        }

        private static FakeTargetProvider ProviderWith(params FakeCombatTarget[] targets)
        {
            var provider = new FakeTargetProvider();
            provider.Targets.AddRange(targets);
            return provider;
        }

        [Test]
        public void Tick_NoTargetInRange_DoesNotSwingAndClearsAim()
        {
            var mount = new FakeWeaponMount();
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), new FakeTargetProvider(), mount);

            weapon.Tick(0.1f);

            Assert.AreEqual(0, mount.SwingCount);
            Assert.AreEqual(1, mount.ClearAimCount);
        }

        [Test]
        public void Tick_TargetInRange_StartsSwingAcrossTheConfiguredArc()
        {
            var mount = new FakeWeaponMount();
            var provider = ProviderWith(new FakeCombatTarget { Position = PolarPosition(0f, 1.5f) });
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), provider, mount);

            weapon.Tick(0f);

            Assert.AreEqual(1, mount.SwingCount);
            Assert.AreEqual(-ArcDegrees * 0.5f, mount.SwingArcs[0].x, 0.0001f);
            Assert.AreEqual(ArcDegrees * 0.5f, mount.SwingArcs[0].y, 0.0001f);
            Assert.AreEqual(AttackDuration, mount.LastSwingDuration, 0.0001f);
        }

        [Test]
        public void Tick_ConsecutiveSwings_AlternateDirection()
        {
            var mount = new FakeWeaponMount();
            var provider = ProviderWith(new FakeCombatTarget { Position = PolarPosition(0f, 1.5f) });
            var weapon = new MeleeSwingBehavior(CreateWeaponData(attacksPerSecond: 1f), CreateStats(), provider, mount);

            weapon.Tick(0f);
            weapon.Tick(AttackDuration);
            weapon.Tick(1f);

            Assert.AreEqual(2, mount.SwingCount);
            Assert.AreEqual(mount.SwingArcs[0].x, mount.SwingArcs[1].y, 0.0001f);
            Assert.AreEqual(mount.SwingArcs[0].y, mount.SwingArcs[1].x, 0.0001f);
        }

        [Test]
        public void Tick_SwingStart_DealsNoDamageYet()
        {
            var target = new FakeCombatTarget { Position = PolarPosition(0f, 1.5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), ProviderWith(target), new FakeWeaponMount());

            weapon.Tick(0f);

            Assert.AreEqual(0, target.HitCount);
        }

        [Test]
        public void Tick_TargetInTheMiddleOfTheArc_IsHitWhenBladePassesIt()
        {
            var target = new FakeCombatTarget { Position = PolarPosition(0f, 1.5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), ProviderWith(target), new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration * 0.25f);
            Assert.AreEqual(0, target.HitCount);

            weapon.Tick(AttackDuration * 0.5f);
            Assert.AreEqual(1, target.HitCount);
        }

        [Test]
        public void Tick_TargetAtTheEndOfTheArc_IsHitOnlyNearTheEndOfTheSwing()
        {
            var nearest = new FakeCombatTarget { Position = PolarPosition(0f, 1.0f) };
            var lateInArc = new FakeCombatTarget { Position = PolarPosition(45f, 1.5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), ProviderWith(nearest, lateInArc), new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration * 0.5f);
            Assert.AreEqual(0, lateInArc.HitCount);

            weapon.Tick(AttackDuration * 0.5f);
            Assert.AreEqual(1, lateInArc.HitCount);
        }

        [Test]
        public void Tick_SingleSwing_HitsEachTargetOnlyOnce()
        {
            var target = new FakeCombatTarget { Position = PolarPosition(0f, 1.5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), ProviderWith(target), new FakeWeaponMount());

            weapon.Tick(0f);
            for (int i = 0; i < 10; i++)
            {
                weapon.Tick(AttackDuration * 0.1f);
            }

            Assert.AreEqual(1, target.HitCount);
        }

        [Test]
        public void Tick_TargetOutsideTheArc_IsNotHit()
        {
            var nearest = new FakeCombatTarget { Position = PolarPosition(0f, 1.0f) };
            var behindTheWielder = new FakeCombatTarget { Position = PolarPosition(90f, 1.5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), ProviderWith(nearest, behindTheWielder), new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration);

            Assert.AreEqual(0, behindTheWielder.HitCount);
        }

        [Test]
        public void Tick_TargetBeyondReach_IsNotHit()
        {
            var nearest = new FakeCombatTarget { Position = PolarPosition(0f, 1.0f) };
            var tooFar = new FakeCombatTarget { Position = PolarPosition(10f, 5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(range: 2f), CreateStats(), ProviderWith(nearest, tooFar), new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration);

            Assert.AreEqual(0, tooFar.HitCount);
        }

        [Test]
        public void Tick_LargeDeltaTime_StillHitsEveryTargetInTheArc()
        {
            var nearest = new FakeCombatTarget { Position = PolarPosition(0f, 1.0f) };
            var early = new FakeCombatTarget { Position = PolarPosition(-40f, 1.5f) };
            var late = new FakeCombatTarget { Position = PolarPosition(40f, 1.5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(), CreateStats(), ProviderWith(nearest, early, late), new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration * 5f);

            Assert.AreEqual(1, nearest.HitCount);
            Assert.AreEqual(1, early.HitCount);
            Assert.AreEqual(1, late.HitCount);
        }

        [Test]
        public void Tick_DamageScalesWithMeleeDamageStat()
        {
            var stats = CreateStats();
            stats.AddModifier(new StatModifier(StatType.MeleeDamagePercent, ModifierType.Flat, 0.5f));
            var target = new FakeCombatTarget { Position = PolarPosition(0f, 1.5f) };
            var weapon = new MeleeSwingBehavior(CreateWeaponData(baseDamage: 10f), stats, ProviderWith(target), new FakeWeaponMount());

            weapon.Tick(0f);
            weapon.Tick(AttackDuration);

            Assert.AreEqual(15f, target.DamageTaken, 0.0001f);
        }

        [Test]
        public void Tick_DuringCooldown_DoesNotStartSecondSwing()
        {
            var mount = new FakeWeaponMount();
            var provider = ProviderWith(new FakeCombatTarget { Position = PolarPosition(0f, 1.5f) });
            var weapon = new MeleeSwingBehavior(CreateWeaponData(attacksPerSecond: 1f), CreateStats(), provider, mount);

            weapon.Tick(0f);
            weapon.Tick(AttackDuration);
            weapon.Tick(0.2f);

            Assert.AreEqual(1, mount.SwingCount);
        }
    }
}
