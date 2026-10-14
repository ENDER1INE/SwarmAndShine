using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public class MeleeSwingBehavior : IWeaponBehavior
    {
        private readonly WeaponData _weaponData;
        private readonly CharacterStats _stats;
        private readonly ITargetProvider _targetProvider;
        private readonly IWeaponMount _mount;
        private readonly List<ICombatTarget> _candidateBuffer = new();
        private readonly List<ICombatTarget> _alreadyHitTargets = new();

        private float _cooldownTimer;
        private float _attackElapsedTime;
        private float _startOffsetDegrees;
        private float _endOffsetDegrees;
        private float _previousOffsetDegrees;
        private float _nextSwingSign = 1f;
        private bool _isAttacking;
        private Vector2 _attackDirection;

        public MeleeSwingBehavior(
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

        public void Release()
        {
            _mount.Release();
        }

        public void Tick(float deltaTime)
        {
            _cooldownTimer -= deltaTime;

            if (_isAttacking)
            {
                ContinueSwing(deltaTime);
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

            StartSwing(directionToTarget);
        }

        private void StartSwing(Vector2 direction)
        {
            float halfArcDegrees = _weaponData.MeleeSwingArcDegrees * 0.5f;

            _isAttacking = true;
            _attackElapsedTime = 0f;
            _attackDirection = direction;
            _alreadyHitTargets.Clear();

            _startOffsetDegrees = -halfArcDegrees * _nextSwingSign;
            _endOffsetDegrees = halfArcDegrees * _nextSwingSign;
            _previousOffsetDegrees = _startOffsetDegrees;
            _nextSwingSign = -_nextSwingSign;

            _cooldownTimer = WeaponStatCalculator.CalculateCooldown(_weaponData, _stats);
            _mount.PlaySwing(_startOffsetDegrees, _endOffsetDegrees, _weaponData.MeleeAttackDuration);
        }

        private void ContinueSwing(float deltaTime)
        {
            _attackElapsedTime += deltaTime;

            float progress = Mathf.Clamp01(_attackElapsedTime / _weaponData.MeleeAttackDuration);
            float currentOffsetDegrees = Mathf.Lerp(_startOffsetDegrees, _endOffsetDegrees, progress);

            DamageTargetsSweptBetween(_previousOffsetDegrees, currentOffsetDegrees);
            _previousOffsetDegrees = currentOffsetDegrees;

            if (progress >= 1f)
            {
                _isAttacking = false;
            }
        }

        private void DamageTargetsSweptBetween(float fromOffsetDegrees, float toOffsetDegrees)
        {
            float lowestOffset = Mathf.Min(fromOffsetDegrees, toOffsetDegrees);
            float highestOffset = Mathf.Max(fromOffsetDegrees, toOffsetDegrees);

            Vector2 origin = _mount.Position;
            float reach = _weaponData.Range + _weaponData.MeleeHitWidth * 0.5f;
            float damage = WeaponStatCalculator.CalculateDamage(_weaponData, _stats);

            _targetProvider.FindTargetsInRange(origin, reach, _candidateBuffer);
            for (int i = 0; i < _candidateBuffer.Count; i++)
            {
                ICombatTarget target = _candidateBuffer[i];
                if (_alreadyHitTargets.Contains(target))
                {
                    continue;
                }

                float targetOffsetDegrees = Vector2.SignedAngle(_attackDirection, target.Position - origin);
                if (targetOffsetDegrees < lowestOffset || targetOffsetDegrees > highestOffset)
                {
                    continue;
                }

                _alreadyHitTargets.Add(target);
                target.TakeDamage(damage);
            }
        }
    }
}
