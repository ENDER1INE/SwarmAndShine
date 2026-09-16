using System;
using UnityEngine;

namespace Game.Combat
{
    public class WeaponBehaviorFactory : IWeaponBehaviorFactory
    {
        private readonly CharacterStats _stats;
        private readonly ITargetProvider _targetProvider;
        private readonly IProjectileSpawner _projectileSpawner;
        private readonly Func<Vector2> _originProvider;

        public WeaponBehaviorFactory(
            CharacterStats stats,
            ITargetProvider targetProvider,
            IProjectileSpawner projectileSpawner,
            Func<Vector2> originProvider)
        {
            _stats = stats;
            _targetProvider = targetProvider;
            _projectileSpawner = projectileSpawner;
            _originProvider = originProvider;
        }

        public IWeaponBehavior Create(WeaponData weaponData)
        {
            switch (weaponData.AttackPattern)
            {
                case AttackPatternType.RangedProjectile:
                    return new RangedWeaponBehavior(weaponData, _stats, _targetProvider, _projectileSpawner, _originProvider);
                default:
                    throw new NotSupportedException(
                        $"Attack pattern '{weaponData.AttackPattern}' is not implemented yet.");
            }
        }
    }
}
