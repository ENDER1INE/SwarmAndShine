using UnityEngine;

namespace Game.Combat
{
    public class RangedWeaponBehavior : IWeaponBehavior
    {
        private readonly WeaponData _weaponData;
        private readonly CharacterStats _stats;
        private readonly ITargetProvider _targetProvider;
        private readonly IProjectileSpawner _projectileSpawner;
        private readonly IWeaponMount _mount;

        private float _cooldownTimer;

        public RangedWeaponBehavior(
            WeaponData weaponData,
            CharacterStats stats,
            ITargetProvider targetProvider,
            IProjectileSpawner projectileSpawner,
            IWeaponMount mount)
        {
            _weaponData = weaponData;
            _stats = stats;
            _targetProvider = targetProvider;
            _projectileSpawner = projectileSpawner;
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

            ICombatTarget target = _targetProvider.FindNearestTarget(_mount.Position, _weaponData.Range);
            if (target == null || !target.IsAlive)
            {
                _mount.ClearAim();
                return;
            }

            _mount.AimAt((target.Position - _mount.Position).normalized);
            if (_cooldownTimer > 0f)
            {
                return;
            }

            Vector2 muzzlePosition = _mount.MuzzlePosition;
            Vector2 aimDirection = (target.Position - muzzlePosition).normalized;
            float damage = WeaponStatCalculator.CalculateDamage(_weaponData, _stats);
            int projectileCount = _weaponData.ProjectilesPerShot;
            for (int projectileIndex = 0; projectileIndex < projectileCount; projectileIndex++)
            {
                Vector2 shotDirection = ProjectileSpread.GetDirection(
                    aimDirection, projectileIndex, projectileCount, _weaponData.SpreadAngleDegrees);
                _projectileSpawner.SpawnProjectile(muzzlePosition, shotDirection, damage, _weaponData.Range);
            }

            _cooldownTimer = WeaponStatCalculator.CalculateCooldown(_weaponData, _stats);
        }
    }
}
