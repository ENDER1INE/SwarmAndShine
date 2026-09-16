using UnityEngine;

namespace Game.Combat
{
    [CreateAssetMenu(fileName = "CharacterBaseStatsData", menuName = "SAS/Combat/Character Base Stats")]
    public class CharacterBaseStatsData : ScriptableObject
    {
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _healthRegen = 0f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _armor = 0f;

        public float MaxHealth => _maxHealth;
        public float HealthRegen => _healthRegen;
        public float MoveSpeed => _moveSpeed;
        public float Armor => _armor;
    }
}
