using UnityEngine;

namespace Game.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "SAS/Enemies/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private string _enemyName;
        [SerializeField] private EnemyController _prefab;
        [SerializeField] private float _maxHealth = 10f;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _contactDamage = 5f;
        [SerializeField] private float _contactDamageInterval = 1f;

        [SerializeField] private int _currencyReward = 1;

        public int CurrencyReward => _currencyReward;
        public string EnemyName => _enemyName;
        public EnemyController Prefab => _prefab;
        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float ContactDamage => _contactDamage;
        public float ContactDamageInterval => _contactDamageInterval;
    }
}
