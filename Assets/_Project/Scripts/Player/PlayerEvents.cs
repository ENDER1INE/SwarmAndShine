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

    public readonly struct PlayerDiedEvent
    {
    }
}
