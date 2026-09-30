using System.Collections.Generic;
using Game.Spawning;
using UnityEngine;

namespace Game.Run
{
    [CreateAssetMenu(fileName = "RunConfig", menuName = "SAS/Run/Run Config")]
    public class RunConfig : ScriptableObject
    {
        [SerializeField] private List<WaveConfig> _waves = new();
        [SerializeField] private float _defeatFallbackDelay = 2f;

        public IReadOnlyList<WaveConfig> Waves => _waves;
        public float DefeatFallbackDelay => _defeatFallbackDelay;
    }
}
