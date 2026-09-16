namespace Game.Combat
{
    public enum StatType
    {
        MeleeDamagePercent,
        RangedDamagePercent,
        MaxHealth,
        HealthRegen,
        AttackSpeedPercent,
        MoveSpeedPercent,
        Armor,
        Luck
    }

    public enum ModifierType
    {
        Flat,
        PercentAdd,
        PercentMultiply
    }

    public readonly struct StatModifier
    {
        public readonly StatType StatType;
        public readonly ModifierType ModifierType;
        public readonly float Value;
        public readonly object Source;

        public StatModifier(StatType statType, ModifierType modifierType, float value, object source = null)
        {
            StatType = statType;
            ModifierType = modifierType;
            Value = value;
            Source = source;
        }
    }
}
