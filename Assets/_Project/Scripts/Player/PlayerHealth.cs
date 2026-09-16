using System;
using Game.Combat;
using Game.Core;

namespace Game.Player
{
    public class PlayerHealth : IDamageable
    {
        private readonly CharacterStats _stats;
        private float _currentHealth;

        public PlayerHealth(CharacterStats stats)
        {
            _stats = stats;
            _currentHealth = _stats.MaxHealth;
        }

        public float CurrentHealth => _currentHealth;
        public bool IsDead => _currentHealth <= 0f;

        public void TakeDamage(float rawDamage)
        {
            if (IsDead)
            {
                return;
            }

            float mitigatedDamage = rawDamage - _stats.Armor;
            if (mitigatedDamage < 0f)
            {
                mitigatedDamage = 0f;
            }

            _currentHealth = Math.Max(0f, _currentHealth - mitigatedDamage);

            GameEvents.Publish(new PlayerDamagedEvent(mitigatedDamage, _currentHealth));

            if (IsDead)
            {
                GameEvents.Publish(new PlayerDiedEvent());
            }
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            _currentHealth = Math.Min(_currentHealth + amount, _stats.MaxHealth);
        }

        public void Tick(float deltaTime)
        {
            if (IsDead)
            {
                return;
            }

            Heal(_stats.HealthRegen * deltaTime);
        }
    }
}
