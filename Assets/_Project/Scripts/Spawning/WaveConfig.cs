using System.Collections.Generic;
using UnityEngine;

namespace Game.Spawning
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "SAS/Spawning/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] private float _duration = 30f;
        [SerializeField] private List<WaveSpawnEntry> _spawnEntries = new();

        public float Duration => _duration;
        public IReadOnlyList<WaveSpawnEntry> SpawnEntries => _spawnEntries;
    }
}
