using System;
using Game.Meta;
using Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class CharacterRowView : MonoBehaviour
    {
        private const string SelectedLabel = "Selected";
        private const string SelectLabel = "Select";
        private const string BuyLabelFormat = "Buy ({0})";
        private const string LockedLabel = "Locked";
        private const string AchievementHintFormat = "Complete {0} waves in one run to unlock";

        [SerializeField] private Text _nameLabel;
        [SerializeField] private Text _hintLabel;
        [SerializeField] private Button _actionButton;
        [SerializeField] private Text _actionLabel;

        public void Init(CharacterData character, Action<CharacterData> onActionClicked)
        {
            _nameLabel.text = character.DisplayName;
            _actionButton.onClick.AddListener(() => onActionClicked(character));
        }

        public void Render(CharacterData character, CharacterMenuStatus status, int metaBalance)
        {
            _hintLabel.text = status == CharacterMenuStatus.LockedByAchievement
                ? string.Format(AchievementHintFormat, character.RequiredWavesCompleted)
                : string.Empty;

            switch (status)
            {
                case CharacterMenuStatus.Selected:
                    _actionLabel.text = SelectedLabel;
                    _actionButton.interactable = false;
                    break;
                case CharacterMenuStatus.Unlocked:
                    _actionLabel.text = SelectLabel;
                    _actionButton.interactable = true;
                    break;
                case CharacterMenuStatus.PurchasableWithCurrency:
                    _actionLabel.text = string.Format(BuyLabelFormat, character.UnlockCost);
                    _actionButton.interactable = metaBalance >= character.UnlockCost;
                    break;
                default:
                    _actionLabel.text = LockedLabel;
                    _actionButton.interactable = false;
                    break;
            }
        }
    }
}
