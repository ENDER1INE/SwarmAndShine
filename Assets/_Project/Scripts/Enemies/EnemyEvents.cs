using UnityEngine;

namespace Game.Enemies
{
    public readonly struct EnemyKilledEvent
    {
        public readonly int CurrencyReward;
        public readonly Vector2 Position;

        public EnemyKilledEvent(int currencyReward, Vector2 position)
        {
            CurrencyReward = currencyReward;
            Position = position;
        }
    }
}
