using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public class MeleeThrustBehavior : IWeaponBehavior
    {
        private const float HitTimeFraction = 0.5f;

        private readonly WeaponData _weaponData;
        private readonly CharacterStats _stats;
        private readonly ITargetProvider _targetProvider;
        private readonly IWeaponMount _mount;
        private readonly List<ICombatTarget> _hitBuffer = new();

        private float _cooldownTimer;
        private float _attackElapsedTime;
        private bool _isAttacking;
        private bool _hasDealtDamage;
        private Vector2 _attackDirection;
        private int _stackCount = 1;

        public MeleeThrustBehavior(
            WeaponData weaponData,
            CharacterStats stats,
            ITargetProvider targetProvider,
            IWeaponMount mount)
        {
            _weaponData = weaponData;
            _stats = stats;
            _targetProvider = targetProvider;
            _mount = mount;
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

            if (_isAttacking)
            {
                ContinueAttack(deltaTime);
                return;
            }

            ICombatTarget target = _targetProvider.FindNearestTarget(_mount.Position, _weaponData.Range);
            if (target == null || !target.IsAlive)
            {
                _mount.ClearAim();
                return;
            }

            Vector2 directionToTarget = (target.Position - _mount.Position).normalized;
            _mount.AimAt(directionToTarget);
            if (_cooldownTimer > 0f)
            {
                return;
            }

            StartAttack(directionToTarget);
        }

        private void StartAttack(Vector2 direction)
        {
            _isAttacking = true;
            _hasDealtDamage = false;
            _attackElapsedTime = 0f;
            _attackDirection = direction;
            _cooldownTimer = WeaponStatCalculator.CalculateCooldown(_weaponData, _stats, _stackCount);

            _mount.PlayThrust(_weaponData.Range, _weaponData.MeleeAttackDuration);
        }

        private void ContinueAttack(float deltaTime)
        {
            _attackElapsedTime += deltaTime;

            if (!_hasDealtDamage && _attackElapsedTime >= _weaponData.MeleeAttackDuration * HitTimeFraction)
            {
                DealDamage();
            }

            if (_attackElapsedTime >= _weaponData.MeleeAttackDuration)
            {
                _isAttacking = false;
            }
        }

        private void DealDamage()
        {
            _hasDealtDamage = true;

            Vector2 origin = _mount.Position;
            float halfWidth = _weaponData.MeleeHitWidth * 0.5f;
            float damage = WeaponStatCalculator.CalculateDamage(_weaponData, _stats, _stackCount);

            _targetProvider.FindTargetsInRange(origin, _weaponData.Range + halfWidth, _hitBuffer);
            for (int i = 0; i < _hitBuffer.Count; i++)
            {
                ICombatTarget target = _hitBuffer[i];
                if (MeleeHitZone.Contains(origin, _attackDirection, _weaponData.Range, halfWidth, target.Position))
                {
                    target.TakeDamage(damage);
                }
            }
        }
    }
}
