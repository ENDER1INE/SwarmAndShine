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
    }
}
