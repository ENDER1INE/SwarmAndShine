using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public class RegistryTargetProvider : ITargetProvider
    {
        private readonly CombatTargetRegistry _registry;

        public RegistryTargetProvider(CombatTargetRegistry registry)
        {
            _registry = registry;
        }

        public ICombatTarget FindNearestTarget(Vector2 fromPosition, float maxRange)
        {
            ICombatTarget nearest = null;
            float nearestDistanceSquared = maxRange * maxRange;

            foreach (var target in _registry.ActiveTargets)
            {
                if (!target.IsAlive)
                {
                    continue;
                }

                float distanceSquared = (target.Position - fromPosition).sqrMagnitude;
                if (distanceSquared <= nearestDistanceSquared)
                {
                    nearestDistanceSquared = distanceSquared;
                    nearest = target;
                }
            }

            return nearest;
        }

        public void FindTargetsInRange(Vector2 fromPosition, float maxRange, List<ICombatTarget> results)
        {
            results.Clear();
            float maxDistanceSquared = maxRange * maxRange;

            foreach (var target in _registry.ActiveTargets)
            {
                if (target.IsAlive && (target.Position - fromPosition).sqrMagnitude <= maxDistanceSquared)
                {
                    results.Add(target);
                }
            }
        }
    }
}
