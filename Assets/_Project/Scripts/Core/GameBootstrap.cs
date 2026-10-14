using Game.CameraControl;
using Game.Combat;
using Game.Meta;
using Game.Player;
using Game.Run;
using Game.Shop;
using Game.Spawning;
using Game.UI;
using Game.World;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        [SerializeField] private CharacterCatalog _characterCatalog;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private CharacterAnimator _playerAnimator;
        [SerializeField] private PlayerWeaponController _playerWeaponController;
        [SerializeField] private PlayerWeaponRing _playerWeaponRing;
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private RunConfig _runConfig;
        [SerializeField] private ShopConfig _shopConfig;
        [SerializeField] private RunRewardConfig _runRewardConfig;
        [SerializeField] private MetaUpgradeCatalog _metaUpgradeCatalog;
        [SerializeField] private ShopView _shopView;
        [SerializeField] private ResultsView _resultsView;
        [SerializeField] private HudView _hudView;
        [SerializeField] private ArenaData _arenaData;
        [SerializeField] private ArenaBorderView _arenaBorderView;
        [SerializeField] private TiledFloorView _tiledFloorView;
        [SerializeField] private CameraRigController _cameraRig;

        [Header("Debug")]
        [Tooltip("Weapons granted at run start in the Editor and development builds. Leave empty for normal play.")]
        [SerializeField] private List<WeaponData> _debugStartingWeapons = new();

        private MetaServices _metaServices;
        private RunStatsTracker _runStatsTracker;
        private RunRewardService _runRewardService;
        private CharacterStats _characterStats;
        private PlayerHealth _playerHealth;
        private CombatTargetRegistry _combatTargetRegistry;
        private WeaponSlotManager _weaponSlotManager;
        private RunWallet _runWallet;
        private RunFlow _runFlow;
        private ShopSession _shopSession;

        private void OnDestroy()
        {
            _runWallet?.Dispose();
            _runFlow?.Dispose();
            _shopSession?.Dispose();
            _runRewardService?.Dispose();
            _metaServices?.Dispose();
            _runStatsTracker?.Dispose();
        }

        private void Awake()
        {
            _metaServices = MetaServices.CreateDefault(_characterCatalog.Characters);
            _runStatsTracker = new RunStatsTracker();
            _runRewardService = new RunRewardService(
                new RunRewardCalculator(_runRewardConfig.ToRates()),
                _runStatsTracker,
                _metaServices.Wallet,
                _metaServices.SaveData,
                _metaServices.Storage);
            CharacterData selectedCharacter = _metaServices.CharacterSelection.GetSelected();

            if (selectedCharacter.AnimatorController != null)
            {
                _playerAnimator.SetController(selectedCharacter.AnimatorController);
            }

            _runWallet = new RunWallet();
            _characterStats = new CharacterStats(selectedCharacter.BaseStats);
            foreach (StatModifierData modifierData in selectedCharacter.StartModifiers)
            {
                _characterStats.AddModifier(modifierData.ToModifier(selectedCharacter));
            }

            new MetaStatApplier(_metaServices.SaveData, _metaUpgradeCatalog.Upgrades).Apply(_characterStats);
            _playerHealth = new PlayerHealth(_characterStats);
            _combatTargetRegistry = new CombatTargetRegistry();

            IMovementInputSource movementInputSource = new GatedMovementInputSource(
                new KeyboardMovementInputSource(),
                () => _runFlow.IsWaveActive);
            var arenaBounds = ArenaBounds.FromCenterAndSize(_arenaData.Center, _arenaData.Size);
            _playerMovement.Init(_characterStats, movementInputSource, arenaBounds.Shrink(_arenaData.PlayerPadding));

            _enemySpawner.Init(
                _combatTargetRegistry,
                () => _playerMovement.transform.position,
                _playerHealth,
                arenaBounds.Shrink(_arenaData.EnemySpawnPadding));

            _arenaBorderView.Init(arenaBounds);
            _tiledFloorView.Init(arenaBounds);
            _cameraRig.Init(() => _playerMovement.transform.position, arenaBounds);

            ITargetProvider targetProvider = new RegistryTargetProvider(_combatTargetRegistry);
            _projectileSpawner.Init(targetProvider);
            var weaponFactory = new WeaponBehaviorFactory(
                _characterStats,
                targetProvider,
                _projectileSpawner,
                _playerWeaponRing);

            _weaponSlotManager = new WeaponSlotManager(weaponFactory);
            _weaponSlotManager.TryAddWeapon(selectedCharacter.StartingWeapon);
            GrantDebugStartingWeapons();

            _playerWeaponController.Init(_weaponSlotManager.ActiveWeapons, () => _runFlow.IsWaveActive);

            var shopSlotGenerator = new ShopSlotGenerator(
                _shopConfig.Items,
                new RarityUnlockPoolRule(_shopConfig.RarityPool),
                new UnityRandomSource());
            _shopSession = new ShopSession(_shopConfig, shopSlotGenerator, _runWallet, _weaponSlotManager, _characterStats);

            _runFlow = new RunFlow(_runConfig.Waves, _enemySpawner, _runConfig.DefeatFallbackDelay);
            var weaponSellService = new WeaponSellService(_weaponSlotManager, _runWallet, _shopConfig.WeaponSellPriceRatio);
            _shopView.Init(_shopSession, _runWallet, weaponSellService, _runFlow.ContinueFromShop);
            _resultsView.Init(RestartRun, LoadMainMenu);
            _hudView.Init(_playerHealth, _runWallet, _runConfig.Waves.Count, () => _runFlow.RemainingWaveTime);
            _runFlow.Start();
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private void GrantDebugStartingWeapons()
        {
            foreach (WeaponData weaponData in _debugStartingWeapons)
            {
                _weaponSlotManager.TryAddWeapon(weaponData);
            }
        }

        private void RestartRun()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene(_mainMenuSceneName);
        }

        private void Update()
        {
            _runFlow.Tick(Time.deltaTime);

            if (_runFlow.IsWaveActive)
            {
                _runStatsTracker.Tick(Time.deltaTime);
                _playerHealth.Tick(Time.deltaTime);
            }
        }
    }
}
