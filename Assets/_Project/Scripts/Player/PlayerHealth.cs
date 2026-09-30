using System;
using Game.Combat;
using Game.Core;

namespace Game.Player
{
    public class PlayerHealth : IDamageable
    {
        private readonly CharacterStats _stats;
        private float _currentHealth;
        private float _lastKnownMaxHealth;

        public PlayerHealth(CharacterStats stats)
        {
            _stats = stats;
            _currentHealth = _stats.MaxHealth;
            _lastKnownMaxHealth = _stats.MaxHealth;
            _stats.StatsChanged += OnStatsChanged;
        }

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _stats.MaxHealth;
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
            PublishHealthChanged();

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

            float previousHealth = _currentHealth;
            _currentHealth = Math.Min(_currentHealth + amount, _stats.MaxHealth);
            if (_currentHealth != previousHealth)
            {
                PublishHealthChanged();
            }
        }

        public void Tick(float deltaTime)
        {
            if (IsDead)
            {
                return;
            }

            Heal(_stats.HealthRegen * deltaTime);
        }

        private void OnStatsChanged()
        {
            float maxHealth = _stats.MaxHealth;
            float maxHealthDelta = maxHealth - _lastKnownMaxHealth;
            _lastKnownMaxHealth = maxHealth;

            if (IsDead || maxHealthDelta == 0f)
            {
                return;
            }

            _currentHealth = Math.Min(_currentHealth + Math.Max(0f, maxHealthDelta), maxHealth);
            PublishHealthChanged();
        }

        private void PublishHealthChanged()
        {
            GameEvents.Publish(new PlayerHealthChangedEvent(_currentHealth, _stats.MaxHealth));
        }
    }
}
