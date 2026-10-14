using System.Collections.Generic;
using Game.Combat;

namespace Game.Meta
{
    public class MetaStatApplier
    {
        private readonly SaveData _saveData;
        private readonly IReadOnlyList<MetaUpgradeData> _upgrades;

        public MetaStatApplier(SaveData saveData, IReadOnlyList<MetaUpgradeData> upgrades)
        {
            _saveData = saveData;
            _upgrades = upgrades;
        }

        // One modifier per purchased level keeps every ModifierType correct (e.g. PercentMultiply compounds).
        public void Apply(CharacterStats stats)
        {
            foreach (MetaUpgradeData upgrade in _upgrades)
            {
                int level = _saveData.GetUpgradeLevel(upgrade.Id);
                for (int i = 0; i < level; i++)
                {
                    stats.AddModifier(upgrade.ModifierPerLevel.ToModifier(upgrade));
                }
            }
        }
    }
}
