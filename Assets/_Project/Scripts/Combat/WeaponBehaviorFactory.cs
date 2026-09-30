using System;

namespace Game.Combat
{
    public class WeaponBehaviorFactory : IWeaponBehaviorFactory
    {
        private readonly CharacterStats _stats;
        private readonly ITargetProvider _targetProvider;
        private readonly IProjectileSpawner _projectileSpawner;
        private readonly IWeaponMountFactory _mountFactory;

        public WeaponBehaviorFactory(
            CharacterStats stats,
            ITargetProvider targetProvider,
            IProjectileSpawner projectileSpawner,
            IWeaponMountFactory mountFactory)
        {
            _stats = stats;
            _targetProvider = targetProvider;
            _projectileSpawner = projectileSpawner;
            _mountFactory = mountFactory;
        }

        public IWeaponBehavior Create(WeaponData weaponData)
        {
            switch (weaponData.AttackPattern)
            {
                case AttackPatternType.RangedProjectile:
                    return new RangedWeaponBehavior(
                        weaponData,
                        _stats,
                        _targetProvider,
                        _projectileSpawner,
                        _mountFactory.Create(weaponData));
                case AttackPatternType.MeleeSwing:
                    return new MeleeSwingBehavior(
                        weaponData,
                        _stats,
                        _targetProvider,
                        _mountFactory.Create(weaponData));
                case AttackPatternType.MeleeThrust:
                    return new MeleeThrustBehavior(
                        weaponData,
                        _stats,
                        _targetProvider,
                        _mountFactory.Create(weaponData));
                default:
                    throw new NotSupportedException(
                        $"Attack pattern '{weaponData.AttackPattern}' is not implemented yet.");
            }
        }
    }
}
