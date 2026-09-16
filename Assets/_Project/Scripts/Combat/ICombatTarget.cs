using UnityEngine;

namespace Game.Combat
{
    public interface ICombatTarget
    {
        Vector2 Position { get; }
        bool IsAlive { get; }
        void TakeDamage(float damage);
    }
}
