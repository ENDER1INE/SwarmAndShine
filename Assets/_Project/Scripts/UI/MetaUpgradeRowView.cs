using System;
using Game.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MetaUpgradeRowView : MonoBehaviour
    {
        private const string TitleFormat = "{0}  ({1}/{2})";
        private const string BuyLabelFormat = "Buy ({0})";
        private const string MaxLabel = "MAX";

        [SerializeField] private Text _titleLabel;
        [SerializeField] private Text _descriptionLabel;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Text _buyLabel;

        public void Init(MetaUpgradeData upgrade, Action<MetaUpgradeData> onBuyClicked)
        {
            _descriptionLabel.text = upgrade.Description;
            _buyButton.onClick.AddListener(() => onBuyClicked(upgrade));
        }

        public void Render(MetaUpgradeData upgrade, int level, bool isMaxLevel, bool canBuy)
        {
            _titleLabel.text = string.Format(TitleFormat, upgrade.DisplayName, level, upgrade.MaxLevel);
            _buyLabel.text = isMaxLevel ? MaxLabel : string.Format(BuyLabelFormat, upgrade.GetCost(level));
            _buyButton.interactable = canBuy;
        }
    }
}
