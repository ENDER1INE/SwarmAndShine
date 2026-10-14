using System.Collections.Generic;
using UnityEngine;

namespace Game.Meta
{
    [CreateAssetMenu(fileName = "MetaUpgradeCatalog", menuName = "SAS/Meta/Meta Upgrade Catalog")]
    public class MetaUpgradeCatalog : ScriptableObject
    {
        [SerializeField] private List<MetaUpgradeData> _upgrades = new();

        public IReadOnlyList<MetaUpgradeData> Upgrades => _upgrades;
    }
}
