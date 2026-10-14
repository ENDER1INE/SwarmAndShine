using System;
using Game.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class WeaponSlotView : MonoBehaviour
    {
        private const string EmptySlotLabel = "-";
        private const string SellLabelFormat = "Sell ({0})";

        [SerializeField] private Text _nameLabel;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Text _sellLabel;

        public void Init(int slotIndex, Action<int> onSellClicked)
        {
            _sellButton.onClick.AddListener(() => onSellClicked(slotIndex));
        }

        public void Render(WeaponData weaponData, int sellPrice)
        {
            bool isEmpty = weaponData == null;
            _nameLabel.text = isEmpty ? EmptySlotLabel : weaponData.WeaponName;
            _sellLabel.text = isEmpty ? string.Empty : string.Format(SellLabelFormat, sellPrice);
            _sellButton.interactable = !isEmpty;
        }
    }
}
