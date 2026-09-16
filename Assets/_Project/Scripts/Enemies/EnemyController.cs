using Game.Combat;
using UnityEngine;

namespace Game.Enemies
{
    public class EnemyController : MonoBehaviour, ICombatTarget
    {
        [SerializeField] private float _maxHealth = 30f;

        private float _currentHealth;
        private CombatTargetRegistry _registry;

        public Vector2 Position => transform.position;
        public bool IsAlive => _currentHealth > 0f;

        public void Init(CombatTargetRegistry registry)
        {
            _registry = registry;
            _currentHealth = _maxHealth;
            _registry.Register(this);
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive)
            {
                return;
            }

            _currentHealth -= damage;
            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                _registry.Unregister(this);
                gameObject.SetActive(false);
            }
        }
    }
}
