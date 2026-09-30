using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Shop
{
    [CreateAssetMenu(fileName = "ShopItemData", menuName = "SAS/Shop/Shop Item")]
    public class ShopItemData : ScriptableObject
    {
        [SerializeField] private string _itemName;
        [SerializeField] private ShopItemRarity _rarity;
        [SerializeField] private int _price = 10;
        [SerializeField] private WeaponData _weapon;
        [SerializeField] private List<StatModifierData> _statModifiers = new();

        public string ItemName => _itemName;
        public ShopItemRarity Rarity => _rarity;
        public int Price => _price;
        public WeaponData Weapon => _weapon;
        public IReadOnlyList<StatModifierData> StatModifiers => _statModifiers;
    }
}
