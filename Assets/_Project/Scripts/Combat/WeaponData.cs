using UnityEngine;
using UnityEngine.Serialization;

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
        Boomerang,
        MeleeThrust
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
        [Min(0)]
        [SerializeField] private int _basePrice = 10;

        [Header("Ranged")]
        [Min(1)]
        [SerializeField] private int _projectilesPerShot = 1;
        [SerializeField] private float _spreadAngleDegrees;

        [Header("Melee")]
        [SerializeField] private float _meleeHitWidth = 0.6f;
        [FormerlySerializedAs("_meleeThrustDuration")]
        [SerializeField] private float _meleeAttackDuration = 0.2f;
        [SerializeField] private float _meleeSwingArcDegrees = 110f;

        [Header("World visual")]
        [SerializeField] private Sprite _worldSprite;
        [SerializeField] private Vector2 _worldScale = Vector2.one;
        [SerializeField] private Color _worldTint = Color.white;
        [SerializeField] private float _worldSpriteRotationDegrees;
        [SerializeField] private Vector2 _worldSpriteOffset = Vector2.zero;
        [SerializeField] private Vector2 _muzzleOffset = new Vector2(0.25f, 0f);

        public string WeaponName => _weaponName;
        public WeaponCategory Category => _category;
        public AttackPatternType AttackPattern => _attackPattern;
        public float BaseDamage => _baseDamage;
        public float BaseAttacksPerSecond => _baseAttacksPerSecond;
        public float Range => _range;
        public int BasePrice => _basePrice;
        public int ProjectilesPerShot => _projectilesPerShot;
        public float SpreadAngleDegrees => _spreadAngleDegrees;
        public float MeleeHitWidth => _meleeHitWidth;
        public float MeleeAttackDuration => _meleeAttackDuration;
        public float MeleeSwingArcDegrees => _meleeSwingArcDegrees;
        public Sprite WorldSprite => _worldSprite;
        public Vector2 WorldScale => _worldScale;
        public Color WorldTint => _worldTint;
        public float WorldSpriteRotationDegrees => _worldSpriteRotationDegrees;
        public Vector2 WorldSpriteOffset => _worldSpriteOffset;
        public Vector2 MuzzleOffset => _muzzleOffset;
    }
}
