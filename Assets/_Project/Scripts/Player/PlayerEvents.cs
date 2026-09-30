namespace Game.Player
{
    public readonly struct PlayerDamagedEvent
    {
        public readonly float DamageTaken;
        public readonly float RemainingHealth;

        public PlayerDamagedEvent(float damageTaken, float remainingHealth)
        {
            DamageTaken = damageTaken;
            RemainingHealth = remainingHealth;
        }
    }

    public readonly struct PlayerHealthChangedEvent
    {
        public readonly float CurrentHealth;
        public readonly float MaxHealth;

        public PlayerHealthChangedEvent(float currentHealth, float maxHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }

    public readonly struct PlayerDiedEvent
    {
    }

    public readonly struct PlayerDeathAnimationFinishedEvent
    {
    }
}
