using System;
using Game.Core;
using Game.Run;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ResultsView : MonoBehaviour
    {
        private const string VictoryTitle = "Victory";
        private const string DefeatTitle = "Defeat";
        private const string WavesCompletedFormat = "Waves completed: {0}";

        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _titleLabel;
        [SerializeField] private Text _wavesLabel;
        [SerializeField] private Button _restartButton;

        public void Init(Action onRestartClicked)
        {
            _restartButton.onClick.AddListener(() => onRestartClicked());
            GameEvents.Subscribe<RunEndedEvent>(OnRunEnded);
            _panel.SetActive(false);
        }

        private void OnDestroy()
        {
            GameEvents.Unsubscribe<RunEndedEvent>(OnRunEnded);
        }

        private void OnRunEnded(RunEndedEvent evt)
        {
            _titleLabel.text = evt.Survived ? VictoryTitle : DefeatTitle;
            _wavesLabel.text = string.Format(WavesCompletedFormat, evt.WavesCompleted);
            _panel.SetActive(true);
        }
    }
}
