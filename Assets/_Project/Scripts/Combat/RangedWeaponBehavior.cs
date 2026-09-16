using System;
using UnityEngine;

namespace Game.Combat
{
    public class RangedWeaponBehavior : IWeaponBehavior
    {
        private readonly WeaponData _weaponData;
        private readonly CharacterStats _stats;
        private readonly ITargetProvider _targetProvider;
        private readonly IProjectileSpawner _projectileSpawner;
        private readonly Func<Vector2> _originProvider;

        private float _cooldownTimer;
        private int _stackCount = 1;

        public RangedWeaponBehavior(
            WeaponData weaponData,
            CharacterStats stats,
            ITargetProvider targetProvider,
            IProjectileSpawner projectileSpawner,
            Func<Vector2> originProvider)
        {
            _weaponData = weaponData;
            _stats = stats;
            _targetProvider = targetProvider;
            _projectileSpawner = projectileSpawner;
            _originProvider = originProvider;
        }

        public WeaponData WeaponData => _weaponData;
        public int StackCount => _stackCount;

        public void AddStack()
        {
            _stackCount++;
        }

        public void Tick(float deltaTime)
        {
            _cooldownTimer -= deltaTime;
            if (_cooldownTimer > 0f)
            {
                return;
            }

            Vector2 origin = _originProvider();
            ICombatTarget target = _targetProvider.FindNearestTarget(origin, _weaponData.Range);
            if (target == null || !target.IsAlive)
            {
                return;
            }

            float finalDamage = CalculateDamage();
            Vector2 direction = (target.Position - origin).normalized;
            _projectileSpawner.SpawnProjectile(origin, direction, finalDamage, target);

            ResetCooldown();
        }

        private float CalculateDamage()
        {
            float damagePercent = _weaponData.Category == WeaponCategory.Melee
                ? _stats.MeleeDamagePercent
                : _stats.RangedDamagePercent;

            float duplicateBonus = _weaponData.DuplicateDamageBonusPercent * (_stackCount - 1);

            return _weaponData.BaseDamage * (1f + damagePercent) * (1f + duplicateBonus);
        }

        private void ResetCooldown()
        {
            float duplicateAttackSpeedBonus = _weaponData.DuplicateAttackSpeedBonusPercent * (_stackCount - 1);
            float effectiveAttacksPerSecond = _weaponData.BaseAttacksPerSecond
                * (1f + _stats.AttackSpeedPercent)
                * (1f + duplicateAttackSpeedBonus);

            _cooldownTimer = 1f / effectiveAttacksPerSecond;
        }
    }
}
