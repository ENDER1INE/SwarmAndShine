using System;
using System.Collections.Generic;
using Game.Combat;
using Game.Core;
using Game.Enemies;
using Game.World;
using UnityEngine;

namespace Game.Spawning
{
    public class EnemySpawner : MonoBehaviour, IWaveController
    {
        [SerializeField] private float _spawnRadius = 10f;
        [SerializeField] private int _initialPoolSizePerType = 10;

        private readonly Dictionary<EnemyData, ObjectPool<EnemyController>> _pools = new();
        private readonly WaveSpawnScheduler _scheduler = new();
        private readonly HashSet<EnemyController> _activeEnemies = new();
        private readonly List<EnemyController> _despawnBuffer = new();

        private CombatTargetRegistry _registry;
        private Func<Vector2> _playerPositionProvider;
        private IDamageable _playerDamageable;
        private ArenaBounds _spawnBounds;
        private bool _isInitialized;

        public bool IsWaveComplete => _scheduler.IsWaveComplete;

        public void Init(
            CombatTargetRegistry registry,
            Func<Vector2> playerPositionProvider,
            IDamageable playerDamageable,
            ArenaBounds spawnBounds)
        {
            _registry = registry;
            _playerPositionProvider = playerPositionProvider;
            _playerDamageable = playerDamageable;
            _spawnBounds = spawnBounds;
            _isInitialized = true;
        }

        public void StartWave(WaveConfig waveConfig)
        {
            _scheduler.StartWave(waveConfig);
        }

        public void StopWave()
        {
            _scheduler.Clear();

            _despawnBuffer.Clear();
            _despawnBuffer.AddRange(_activeEnemies);
            foreach (EnemyController enemy in _despawnBuffer)
            {
                enemy.Despawn();
            }
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            _scheduler.Tick(Time.deltaTime, SpawnEnemy);
        }

        private void SpawnEnemy(EnemyData enemyData)
        {
            ObjectPool<EnemyController> pool = GetOrCreatePool(enemyData);
            EnemyController enemy = pool.Get();
            enemy.transform.position = GetSpawnPosition();
            _activeEnemies.Add(enemy);
            enemy.Init(enemyData, _registry, _playerPositionProvider, _playerDamageable, releasedEnemy => ReleaseEnemy(pool, releasedEnemy));
        }

        private void ReleaseEnemy(ObjectPool<EnemyController> pool, EnemyController enemy)
        {
            _activeEnemies.Remove(enemy);
            pool.Release(enemy);
        }

        private ObjectPool<EnemyController> GetOrCreatePool(EnemyData enemyData)
        {
            if (!_pools.TryGetValue(enemyData, out ObjectPool<EnemyController> pool))
            {
                pool = new ObjectPool<EnemyController>(enemyData.Prefab, _initialPoolSizePerType, transform);
                _pools[enemyData] = pool;
            }

            return pool;
        }

        private Vector2 GetSpawnPosition()
        {
            Vector2 playerPosition = _playerPositionProvider();
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _spawnRadius;
            return _spawnBounds.Clamp(playerPosition + offset);
        }
    }
}
