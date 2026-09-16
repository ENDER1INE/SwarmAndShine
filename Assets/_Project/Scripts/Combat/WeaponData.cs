using UnityEngine;

namespace Game.Combat
{
    public enum WeaponCategory
    {
        Melee,
        Ranged
    }

    public enum AttackPatternType
    {
        MeleeSwing,
        RangedProjectile,
        AreaOfEffect,
        Boomerang
    }

    [CreateAssetMenu(fileName = "WeaponData", menuName = "SAS/Combat/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private string _weaponName;
        [SerializeField] private WeaponCategory _category;
        [SerializeField] private AttackPatternType _attackPattern;
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _baseAttacksPerSecond = 1f;
        [SerializeField] private float _range = 1f;
        [SerializeField] private float _duplicateDamageBonusPercent = 0.2f;
        [SerializeField] private float _duplicateAttackSpeedBonusPercent = 0.1f;

        public string WeaponName => _weaponName;
        public WeaponCategory Category => _category;
        public AttackPatternType AttackPattern => _attackPattern;
        public float BaseDamage => _baseDamage;
        public float BaseAttacksPerSecond => _baseAttacksPerSecond;
        public float Range => _range;
        public float DuplicateDamageBonusPercent => _duplicateDamageBonusPercent;
        public float DuplicateAttackSpeedBonusPercent => _duplicateAttackSpeedBonusPercent;
    }
}
