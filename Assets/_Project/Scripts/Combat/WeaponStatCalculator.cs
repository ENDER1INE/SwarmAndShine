namespace Game.Combat
{
    public static class WeaponStatCalculator
    {
        public static float CalculateDamage(WeaponData weaponData, CharacterStats stats)
        {
            float damagePercent = weaponData.Category == WeaponCategory.Melee
                ? stats.MeleeDamagePercent
                : stats.RangedDamagePercent;

            return weaponData.BaseDamage * (1f + damagePercent);
        }

        public static float CalculateCooldown(WeaponData weaponData, CharacterStats stats)
        {
            float effectiveAttacksPerSecond = weaponData.BaseAttacksPerSecond * (1f + stats.AttackSpeedPercent);

            return 1f / effectiveAttacksPerSecond;
        }
    }
}
