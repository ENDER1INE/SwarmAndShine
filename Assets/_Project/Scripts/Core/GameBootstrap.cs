using Game.CameraControl;
using System.IO;
using Game.Combat;
using Game.Meta;
using Game.Player;
using Game.Run;
using Game.Shop;
using Game.Spawning;
using Game.UI;
using Game.World;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        private const string SaveFileName = "save.json";

        [SerializeField] private CharacterBaseStatsData _characterBaseStatsData;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerWeaponController _playerWeaponController;
        [SerializeField] private PlayerWeaponRing _playerWeaponRing;
        [SerializeField] private ProjectileSpawner _projectileSpawner;
        [FormerlySerializedAs("_testWeaponData")]
        [SerializeField] private WeaponData _startingWeaponData;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private RunConfig _runConfig;
        [SerializeField] private ShopConfig _shopConfig;
        [SerializeField] private RunRewardConfig _runRewardConfig;
        [SerializeField] private ShopView _shopView;
        [SerializeField] private ResultsView _resultsView;
        [SerializeField] private HudView _hudView;
        [SerializeField] private ArenaData _arenaData;
        [SerializeField] private ArenaBorderView _arenaBorderView;
        [SerializeField] private TiledFloorView _tiledFloorView;
        [SerializeField] private CameraRigController _cameraRig;

        private SaveData _saveData;
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
            _runStatsTracker?.Dispose();
        }

        private void Awake()
        {
            ISaveStorage saveStorage = new JsonFileSaveStorage(Path.Combine(Application.persistentDataPath, SaveFileName));
            _saveData = saveStorage.Load();
            _runStatsTracker = new RunStatsTracker();
            _runRewardService = new RunRewardService(
                new RunRewardCalculator(_runRewardConfig.ToRates()),
                _runStatsTracker,
                new MetaWallet(_saveData),
                _saveData,
                saveStorage);

            _runWallet = new RunWallet();
            _characterStats = new CharacterStats(_characterBaseStatsData);
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
            var weaponFactory = new WeaponBehaviorFactory(
                _characterStats,
                targetProvider,
                _projectileSpawner,
                _playerWeaponRing);

            _weaponSlotManager = new WeaponSlotManager(weaponFactory);
            _weaponSlotManager.TryAddWeapon(_startingWeaponData);

            _playerWeaponController.Init(_weaponSlotManager.ActiveWeapons, () => _runFlow.IsWaveActive);

            var shopSlotGenerator = new ShopSlotGenerator(
                _shopConfig.Items,
                new RarityUnlockPoolRule(_shopConfig.RarityPool),
                new UnityRandomSource());
            _shopSession = new ShopSession(_shopConfig, shopSlotGenerator, _runWallet, _weaponSlotManager, _characterStats);

            _runFlow = new RunFlow(_runConfig.Waves, _enemySpawner, _runConfig.DefeatFallbackDelay);
            _shopView.Init(_shopSession, _runWallet, _runFlow.ContinueFromShop);
            _resultsView.Init(RestartRun);
            _hudView.Init(_playerHealth, _runWallet, _runConfig.Waves.Count, () => _runFlow.RemainingWaveTime);
            _runFlow.Start();
        }

        private void RestartRun()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
