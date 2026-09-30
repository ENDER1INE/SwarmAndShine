using System;
using Game.Core;
using Game.Run;
using Game.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ShopView : MonoBehaviour
    {
        private const string RerollLabelFormat = "Reroll ({0})";
        private const string BalanceLabelFormat = "Coins: {0}";

        [SerializeField] private GameObject _panel;
        [SerializeField] private ShopSlotView[] _slotViews;
        [SerializeField] private Button _rerollButton;
        [SerializeField] private Text _rerollLabel;
        [SerializeField] private Text _balanceLabel;
        [SerializeField] private Button _continueButton;

        private ShopSession _session;
        private RunWallet _wallet;

        public void Init(ShopSession session, RunWallet wallet, Action onContinueClicked)
        {
            _session = session;
            _wallet = wallet;

            for (int i = 0; i < _slotViews.Length; i++)
            {
                _slotViews[i].Init(i, OnBuyClicked, OnLockClicked);
            }

            _rerollButton.onClick.AddListener(OnRerollClicked);
            _continueButton.onClick.AddListener(() => onContinueClicked());

            GameEvents.Subscribe<ShopOpenedEvent>(OnShopOpened);
            GameEvents.Subscribe<WaveStartedEvent>(OnWaveStarted);
            GameEvents.Subscribe<RunEndedEvent>(OnRunEnded);
            GameEvents.Subscribe<ShopContentChangedEvent>(OnShopContentChanged);
            GameEvents.Subscribe<CurrencyChangedEvent>(OnCurrencyChanged);

            _panel.SetActive(false);
        }

        private void OnDestroy()
        {
            GameEvents.Unsubscribe<ShopOpenedEvent>(OnShopOpened);
            GameEvents.Unsubscribe<WaveStartedEvent>(OnWaveStarted);
            GameEvents.Unsubscribe<RunEndedEvent>(OnRunEnded);
            GameEvents.Unsubscribe<ShopContentChangedEvent>(OnShopContentChanged);
            GameEvents.Unsubscribe<CurrencyChangedEvent>(OnCurrencyChanged);
        }

        private void OnShopOpened(ShopOpenedEvent evt)
        {
            _panel.SetActive(true);
            Refresh();
        }

        private void OnWaveStarted(WaveStartedEvent evt)
        {
            _panel.SetActive(false);
        }

        private void OnRunEnded(RunEndedEvent evt)
        {
            _panel.SetActive(false);
        }

        private void OnShopContentChanged(ShopContentChangedEvent evt)
        {
            Refresh();
        }

        private void OnCurrencyChanged(CurrencyChangedEvent evt)
        {
            Refresh();
        }

        private void OnBuyClicked(int slotIndex)
        {
            _session.TryBuy(slotIndex);
        }

        private void OnLockClicked(int slotIndex)
        {
            _session.ToggleLock(slotIndex);
        }

        private void OnRerollClicked()
        {
            _session.TryReroll();
        }

        private void Refresh()
        {
            int balance = _wallet.Balance;
            for (int i = 0; i < _slotViews.Length; i++)
            {
                _slotViews[i].Render(_session.Slots[i], balance);
            }

            _rerollLabel.text = string.Format(RerollLabelFormat, _session.RerollCost);
            _rerollButton.interactable = balance >= _session.RerollCost;
            _balanceLabel.text = string.Format(BalanceLabelFormat, balance);
        }
    }
}
