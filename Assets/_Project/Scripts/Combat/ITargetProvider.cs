using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public interface ITargetProvider
    {
        ICombatTarget FindNearestTarget(Vector2 fromPosition, float maxRange);
        void FindTargetsInRange(Vector2 fromPosition, float maxRange, List<ICombatTarget> results);
    }
}
