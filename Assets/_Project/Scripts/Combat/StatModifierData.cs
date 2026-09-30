using System;
using UnityEngine;

namespace Game.Combat
{
    [Serializable]
    public struct StatModifierData
    {
        [SerializeField] private StatType _statType;
        [SerializeField] private ModifierType _modifierType;
        [SerializeField] private float _value;

        public StatModifier ToModifier(object source)
        {
            return new StatModifier(_statType, _modifierType, _value, source);
        }
    }
}
