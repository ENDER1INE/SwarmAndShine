using System;
using System.Collections.Generic;
using Game.Meta;
using UnityEngine;

namespace Game.UI
{
    public class UpgradesTabView : MonoBehaviour
    {
        [SerializeField] private MetaUpgradeRowView _rowPrefab;
        [SerializeField] private Transform _rowContainer;

        private readonly List<MetaUpgradeRowView> _rows = new();
        private IReadOnlyList<MetaUpgradeData> _upgrades;
        private MetaUpgradeService _upgradeService;
        private Action _onChanged;

        public void Init(IReadOnlyList<MetaUpgradeData> upgrades, MetaUpgradeService upgradeService, Action onChanged)
        {
            _upgrades = upgrades;
            _upgradeService = upgradeService;
            _onChanged = onChanged;

            foreach (MetaUpgradeData upgrade in upgrades)
            {
                MetaUpgradeRowView row = Instantiate(_rowPrefab, _rowContainer);
                row.Init(upgrade, OnBuyClicked);
                _rows.Add(row);
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                MetaUpgradeData upgrade = _upgrades[i];
                _rows[i].Render(
                    upgrade,
                    _upgradeService.GetLevel(upgrade),
                    _upgradeService.IsMaxLevel(upgrade),
                    _upgradeService.CanPurchase(upgrade));
            }
        }

        private void OnBuyClicked(MetaUpgradeData upgrade)
        {
            _upgradeService.TryPurchase(upgrade);
            _onChanged();
        }
    }
}
