using System.Collections.Generic;

namespace Game.Combat
{
    public class CombatTargetRegistry
    {
        private readonly List<ICombatTarget> _activeTargets = new();

        public IReadOnlyList<ICombatTarget> ActiveTargets => _activeTargets;

        public void Register(ICombatTarget target)
        {
            _activeTargets.Add(target);
        }

        public void Unregister(ICombatTarget target)
        {
            _activeTargets.Remove(target);
        }
    }
}
