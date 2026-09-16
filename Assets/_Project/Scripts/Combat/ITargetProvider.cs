using UnityEngine;

namespace Game.Combat
{
    public interface ITargetProvider
    {
        ICombatTarget FindNearestTarget(Vector2 fromPosition, float maxRange);
    }
}
