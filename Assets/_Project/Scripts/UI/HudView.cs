using System;
using Game.Core;
using Game.Player;
using Game.Run;
using Game.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HudView : MonoBehaviour
    {
        private const string HealthFormat = "HP {0}/{1}";
        private const string CoinsFormat = "Coins: {0}";
        private const string WaveFormat = "Wave {0}/{1}";
        private const string TimerFormat = "{0}s";
        private const int NoSecondsShown = -1;

        [SerializeField] private RectTransform _healthFill;
        [SerializeField] private Text _healthLabel;
        [SerializeField] private Text _coinsLabel;
        [SerializeField] private Text _waveLabel;
        [SerializeField] private Text _timerLabel;

        private Func<float> _getRemainingWaveTime;
        private int _totalWaves;
        private int _shownSeconds = NoSecondsShown;

        public void Init(PlayerHealth playerHealth, RunWallet wallet, int totalWaves, Func<float> getRemainingWaveTime)
        {
            _totalWaves = totalWaves;
            _getRemainingWaveTime = getRemainingWaveTime;

            GameEvents.Subscribe<PlayerHealthChangedEvent>(OnHealthChanged);
            GameEvents.Subscribe<CurrencyChangedEvent>(OnCurrencyChanged);
            GameEvents.Subscribe<WaveStartedEvent>(OnWaveStarted);

            RenderHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            RenderCoins(wallet.Balance);
        }

        private void OnDestroy()
        {
            GameEvents.Unsubscribe<PlayerHealthChangedEvent>(OnHealthChanged);
            GameEvents.Unsubscribe<CurrencyChangedEvent>(OnCurrencyChanged);
            GameEvents.Unsubscribe<WaveStartedEvent>(OnWaveStarted);
        }

        private void Update()
        {
            if (_getRemainingWaveTime == null)
            {
                return;
            }

            int seconds = Mathf.CeilToInt(_getRemainingWaveTime());
            if (seconds == _shownSeconds)
            {
                return;
            }

            _shownSeconds = seconds;
            _timerLabel.text = string.Format(TimerFormat, seconds);
        }

        private void OnHealthChanged(PlayerHealthChangedEvent evt)
        {
            RenderHealth(evt.CurrentHealth, evt.MaxHealth);
        }

        private void OnCurrencyChanged(CurrencyChangedEvent evt)
        {
            RenderCoins(evt.Balance);
        }

        private void OnWaveStarted(WaveStartedEvent evt)
        {
            _waveLabel.text = string.Format(WaveFormat, evt.WaveIndex + 1, _totalWaves);
        }

        private void RenderHealth(float currentHealth, float maxHealth)
        {
            float ratio = maxHealth > 0f ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;
            _healthFill.anchorMax = new Vector2(ratio, 1f);
            _healthLabel.text = string.Format(HealthFormat, Mathf.CeilToInt(currentHealth), Mathf.CeilToInt(maxHealth));
        }

        private void RenderCoins(int balance)
        {
            _coinsLabel.text = string.Format(CoinsFormat, balance);
        }
    }
}
