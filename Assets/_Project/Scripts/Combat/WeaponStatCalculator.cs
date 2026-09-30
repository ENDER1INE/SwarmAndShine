namespace Game.Combat
{
    public static class WeaponStatCalculator
    {
        public static float CalculateDamage(WeaponData weaponData, CharacterStats stats, int stackCount)
        {
            float damagePercent = weaponData.Category == WeaponCategory.Melee
                ? stats.MeleeDamagePercent
                : stats.RangedDamagePercent;

            float duplicateBonus = weaponData.DuplicateDamageBonusPercent * (stackCount - 1);

            return weaponData.BaseDamage * (1f + damagePercent) * (1f + duplicateBonus);
        }

        public static float CalculateCooldown(WeaponData weaponData, CharacterStats stats, int stackCount)
        {
            float duplicateAttackSpeedBonus = weaponData.DuplicateAttackSpeedBonusPercent * (stackCount - 1);
            float effectiveAttacksPerSecond = weaponData.BaseAttacksPerSecond
                * (1f + stats.AttackSpeedPercent)
                * (1f + duplicateAttackSpeedBonus);

            return 1f / effectiveAttacksPerSecond;
        }
    }
}
