using Game.Combat;
using Game.Enemies;
using Game.Player;
using UnityEngine;

namespace Game.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private CharacterBaseStatsData _characterBaseStatsData;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerWeaponController _playerWeaponController;
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        [SerializeField] private WeaponData _testWeaponData;
        [SerializeField] private EnemyController[] _sceneEnemies;

        private CharacterStats _characterStats;
        private PlayerHealth _playerHealth;
        private CombatTargetRegistry _combatTargetRegistry;
        private WeaponSlotManager _weaponSlotManager;

        private void Awake()
        {
            _characterStats = new CharacterStats(_characterBaseStatsData);
            _playerHealth = new PlayerHealth(_characterStats);
            _combatTargetRegistry = new CombatTargetRegistry();

            IMovementInputSource movementInputSource = new KeyboardMovementInputSource();
            _playerMovement.Init(_characterStats, movementInputSource);

            foreach (var enemy in _sceneEnemies)
            {
                enemy.Init(_combatTargetRegistry);
            }

            ITargetProvider targetProvider = new RegistryTargetProvider(_combatTargetRegistry);
            var weaponFactory = new WeaponBehaviorFactory(
                _characterStats,
                targetProvider,
                _projectileSpawner,
                () => _playerMovement.transform.position);

            _weaponSlotManager = new WeaponSlotManager(weaponFactory);
            _weaponSlotManager.TryAddWeapon(_testWeaponData);

            _playerWeaponController.Init(_weaponSlotManager.ActiveWeapons);
        }
    }
}
