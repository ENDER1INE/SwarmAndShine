using System;
using Game.Enemies;
using UnityEngine;

namespace Game.Spawning
{
    [Serializable]
    public class WaveSpawnEntry
    {
        [SerializeField] private EnemyData _enemyData;
        [SerializeField] private int _count = 5;
        [SerializeField] private float _spawnInterval = 1f;

        public EnemyData EnemyData => _enemyData;
        public int Count => _count;
        public float SpawnInterval => _spawnInterval;
    }
}
