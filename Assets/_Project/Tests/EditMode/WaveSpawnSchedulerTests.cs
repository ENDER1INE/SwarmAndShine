using System.Collections.Generic;
using System.Reflection;
using Game.Enemies;
using Game.Spawning;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class WaveSpawnSchedulerTests
    {
        private static WaveSpawnEntry CreateEntry(EnemyData enemyData, int count, float spawnInterval)
        {
            var entry = new WaveSpawnEntry();
            SetField(entry, "_enemyData", enemyData);
            SetField(entry, "_count", count);
            SetField(entry, "_spawnInterval", spawnInterval);
            return entry;
        }

        private static WaveConfig CreateWaveConfig(params WaveSpawnEntry[] entries)
        {
            var waveConfig = ScriptableObject.CreateInstance<WaveConfig>();
            SetField(waveConfig, "_spawnEntries", new List<WaveSpawnEntry>(entries));
            return waveConfig;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        [Test]
        public void Tick_IntervalElapsed_RequestsSpawnOnce()
        {
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            var waveConfig = CreateWaveConfig(CreateEntry(enemyData, count: 3, spawnInterval: 1f));
            var scheduler = new WaveSpawnScheduler();
            scheduler.StartWave(waveConfig);

            int spawnCount = 0;
            scheduler.Tick(1f, data => spawnCount++);

            Assert.AreEqual(1, spawnCount);
        }

        [Test]
        public void Tick_BeforeIntervalElapsed_DoesNotRequestSpawn()
        {
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            var waveConfig = CreateWaveConfig(CreateEntry(enemyData, count: 3, spawnInterval: 1f));
            var scheduler = new WaveSpawnScheduler();
            scheduler.StartWave(waveConfig);

            int spawnCount = 0;
            scheduler.Tick(0.5f, data => spawnCount++);

            Assert.AreEqual(0, spawnCount);
        }

        [Test]
        public void Tick_ReachesEntryCount_StopsSpawningThatEntry()
        {
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            var waveConfig = CreateWaveConfig(CreateEntry(enemyData, count: 2, spawnInterval: 1f));
            var scheduler = new WaveSpawnScheduler();
            scheduler.StartWave(waveConfig);

            int spawnCount = 0;
            for (int i = 0; i < 5; i++)
            {
                scheduler.Tick(1f, data => spawnCount++);
            }

            Assert.AreEqual(2, spawnCount);
        }

        [Test]
        public void IsWaveComplete_AllEntriesSpawned_ReturnsTrue()
        {
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            var waveConfig = CreateWaveConfig(CreateEntry(enemyData, count: 1, spawnInterval: 1f));
            var scheduler = new WaveSpawnScheduler();
            scheduler.StartWave(waveConfig);

            Assert.IsFalse(scheduler.IsWaveComplete);

            scheduler.Tick(1f, _ => { });

            Assert.IsTrue(scheduler.IsWaveComplete);
        }

        [Test]
        public void Tick_MultipleEntries_SpawnsCorrectEnemyDataForEach()
        {
            var enemyDataA = ScriptableObject.CreateInstance<EnemyData>();
            var enemyDataB = ScriptableObject.CreateInstance<EnemyData>();
            var waveConfig = CreateWaveConfig(
                CreateEntry(enemyDataA, count: 1, spawnInterval: 1f),
                CreateEntry(enemyDataB, count: 1, spawnInterval: 1f));
            var scheduler = new WaveSpawnScheduler();
            scheduler.StartWave(waveConfig);

            var spawnedTypes = new List<EnemyData>();
            scheduler.Tick(1f, spawnedTypes.Add);

            Assert.AreEqual(2, spawnedTypes.Count);
            CollectionAssert.Contains(spawnedTypes, enemyDataA);
            CollectionAssert.Contains(spawnedTypes, enemyDataB);
        }
    }
}
