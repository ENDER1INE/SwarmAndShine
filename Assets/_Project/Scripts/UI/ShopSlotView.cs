using System;
using Game.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ShopSlotView : MonoBehaviour
    {
        private const string EmptySlotLabel = "SOLD";
        private const string LockedText = "Unlock";
        private const string UnlockedText = "Lock";

        [SerializeField] private Button _buyButton;
        [SerializeField] private Text _nameLabel;
        [SerializeField] private Text _priceLabel;
        [SerializeField] private Button _lockButton;
        [SerializeField] private Text _lockLabel;

        public void Init(int slotIndex, Action<int> onBuyClicked, Action<int> onLockClicked)
        {
            _buyButton.onClick.AddListener(() => onBuyClicked(slotIndex));
            _lockButton.onClick.AddListener(() => onLockClicked(slotIndex));
        }

        public void Render(ShopSlot slot, bool canBuy)
        {
            if (slot.IsEmpty)
            {
                _nameLabel.text = EmptySlotLabel;
                _priceLabel.text = string.Empty;
                _buyButton.interactable = false;
                _lockButton.interactable = false;
                _lockLabel.text = UnlockedText;
                return;
            }

            _nameLabel.text = slot.Item.ItemName;
            _priceLabel.text = slot.Item.Price.ToString();
            _buyButton.interactable = canBuy;
            _lockButton.interactable = true;
            _lockLabel.text = slot.IsLocked ? LockedText : UnlockedText;
        }
    }
}
