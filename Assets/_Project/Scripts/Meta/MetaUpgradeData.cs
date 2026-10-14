using Game.Combat;
using UnityEngine;

namespace Game.Meta
{
    [CreateAssetMenu(fileName = "MetaUpgrade", menuName = "SAS/Meta/Meta Upgrade")]
    public class MetaUpgradeData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField, TextArea] private string _description;
        [SerializeField] private int _maxLevel = 5;
        [SerializeField] private int _baseCost = 20;
        [Tooltip("Cost of level N+1 is BaseCost * Multiplier^N. GDD range: 1.4-1.6.")]
        [SerializeField] private float _costMultiplier = 1.5f;
        [Tooltip("Applied once per purchased level.")]
        [SerializeField] private StatModifierData _modifierPerLevel;

        public string Id => _id;
        public string DisplayName => _displayName;
        public string Description => _description;
        public int MaxLevel => _maxLevel;
        public StatModifierData ModifierPerLevel => _modifierPerLevel;

        public int GetCost(int currentLevel)
        {
            return Mathf.RoundToInt(_baseCost * Mathf.Pow(_costMultiplier, currentLevel));
        }
    }
}
