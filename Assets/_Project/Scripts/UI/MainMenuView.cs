using System;
using System.Collections.Generic;
using Game.Meta;
using Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MainMenuView : MonoBehaviour
    {
        private const string BalanceLabelFormat = "Credits: {0}";

        [SerializeField] private Text _balanceLabel;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _debugGrantButton;
        [SerializeField] private Button _upgradesTabButton;
        [SerializeField] private Button _charactersTabButton;
        [SerializeField] private UpgradesTabView _upgradesTab;
        [SerializeField] private CharactersTabView _charactersTab;

        private MetaWallet _wallet;

        public void Init(
            MetaServices metaServices,
            IReadOnlyList<MetaUpgradeData> upgrades,
            IReadOnlyList<CharacterData> characters,
            Action onPlayClicked,
            Action onDebugGrantClicked)
        {
            _wallet = metaServices.Wallet;

            _upgradesTab.Init(upgrades, metaServices.Upgrades, Refresh);
            _charactersTab.Init(
                characters,
                metaServices.CharacterUnlocks,
                metaServices.CharacterSelection,
                metaServices.Wallet,
                Refresh);

            _playButton.onClick.AddListener(() => onPlayClicked());
            InitDebugGrantButton(onDebugGrantClicked);
            _upgradesTabButton.onClick.AddListener(() => ShowTab(isUpgradesTab: true));
            _charactersTabButton.onClick.AddListener(() => ShowTab(isUpgradesTab: false));

            ShowTab(isUpgradesTab: true);
            Refresh();
        }

        private void InitDebugGrantButton(Action onDebugGrantClicked)
        {
            _debugGrantButton.gameObject.SetActive(Debug.isDebugBuild);
            _debugGrantButton.onClick.AddListener(() =>
            {
                onDebugGrantClicked();
                Refresh();
            });
        }

        private void ShowTab(bool isUpgradesTab)
        {
            _upgradesTab.gameObject.SetActive(isUpgradesTab);
            _charactersTab.gameObject.SetActive(!isUpgradesTab);
        }

        private void Refresh()
        {
            _balanceLabel.text = string.Format(BalanceLabelFormat, _wallet.Balance);
            _upgradesTab.Refresh();
            _charactersTab.Refresh();
        }
    }
}
