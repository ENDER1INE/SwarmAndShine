using UnityEngine;

namespace Game.World
{
    [CreateAssetMenu(fileName = "ArenaData", menuName = "SAS/World/Arena")]
    public class ArenaData : ScriptableObject
    {
        [SerializeField] private Vector2 _center = Vector2.zero;
        [SerializeField] private Vector2 _size = new Vector2(30f, 30f);
        [SerializeField] private float _playerPadding = 0.5f;
        [SerializeField] private float _enemySpawnPadding = 1f;

        public Vector2 Center => _center;
        public Vector2 Size => _size;
        public float PlayerPadding => _playerPadding;
        public float EnemySpawnPadding => _enemySpawnPadding;
    }
}
