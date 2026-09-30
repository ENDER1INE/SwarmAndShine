using System;
using System.Collections.Generic;

namespace Game.Combat
{
    public class CharacterStats
    {
        private readonly Dictionary<StatType, float> _baseValues = new();
        private readonly List<StatModifier> _modifiers = new();
        private readonly Dictionary<StatType, float> _finalValues = new();

        public CharacterStats(CharacterBaseStatsData baseStatsData)
        {
            _baseValues[StatType.MaxHealth] = baseStatsData.MaxHealth;
            _baseValues[StatType.HealthRegen] = baseStatsData.HealthRegen;
            _baseValues[StatType.MoveSpeedPercent] = baseStatsData.MoveSpeed;
            _baseValues[StatType.Armor] = baseStatsData.Armor;
            _baseValues[StatType.MeleeDamagePercent] = 0f;
            _baseValues[StatType.RangedDamagePercent] = 0f;
            _baseValues[StatType.AttackSpeedPercent] = 0f;
            _baseValues[StatType.Luck] = 0f;

            Recalculate();
        }

        public float MaxHealth => _finalValues[StatType.MaxHealth];
        public float HealthRegen => _finalValues[StatType.HealthRegen];
        public float MoveSpeed => _finalValues[StatType.MoveSpeedPercent];
        public float Armor => _finalValues[StatType.Armor];
        public float MeleeDamagePercent => _finalValues[StatType.MeleeDamagePercent];
        public float RangedDamagePercent => _finalValues[StatType.RangedDamagePercent];
        public float AttackSpeedPercent => _finalValues[StatType.AttackSpeedPercent];
        public float Luck => _finalValues[StatType.Luck];

        public event Action StatsChanged;

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            Recalculate();
            StatsChanged?.Invoke();
        }

        public void RemoveModifiersFromSource(object source)
        {
            _modifiers.RemoveAll(modifier => ReferenceEquals(modifier.Source, source));
            Recalculate();
            StatsChanged?.Invoke();
        }

        public float GetStat(StatType statType) => _finalValues[statType];

        private void Recalculate()
        {
            foreach (var statType in _baseValues.Keys)
            {
                _finalValues[statType] = CalculateStat(statType);
            }
        }

        private float CalculateStat(StatType statType)
        {
            float flatSum = _baseValues[statType];
            float percentAddSum = 0f;
            float percentMultiplyResult = 1f;

            foreach (var modifier in _modifiers)
            {
                if (modifier.StatType != statType)
                {
                    continue;
                }

                switch (modifier.ModifierType)
                {
                    case ModifierType.Flat:
                        flatSum += modifier.Value;
                        break;
                    case ModifierType.PercentAdd:
                        percentAddSum += modifier.Value;
                        break;
                    case ModifierType.PercentMultiply:
                        percentMultiplyResult *= 1f + modifier.Value;
                        break;
                }
            }

            return flatSum * (1f + percentAddSum) * percentMultiplyResult;
        }
    }
}
