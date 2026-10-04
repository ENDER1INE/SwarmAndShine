using System;
using Game.Core;
using Game.Run;

namespace Game.Meta
{
    public class RunRewardService : IDisposable
    {
        private readonly RunRewardCalculator _calculator;
        private readonly RunStatsTracker _statsTracker;
        private readonly MetaWallet _wallet;
        private readonly SaveData _saveData;
        private readonly ISaveStorage _storage;

        public RunRewardService(
            RunRewardCalculator calculator,
            RunStatsTracker statsTracker,
            MetaWallet wallet,
            SaveData saveData,
            ISaveStorage storage)
        {
            _calculator = calculator;
            _statsTracker = statsTracker;
            _wallet = wallet;
            _saveData = saveData;
            _storage = storage;
            GameEvents.Subscribe<RunEndedEvent>(OnRunEnded);
        }

        public void Dispose()
        {
            GameEvents.Unsubscribe<RunEndedEvent>(OnRunEnded);
        }

        private void OnRunEnded(RunEndedEvent evt)
        {
            var stats = new RunStats(
                _statsTracker.EnemiesKilled,
                _statsTracker.SecondsSurvived,
                evt.WavesCompleted,
                evt.Survived);

            int reward = _calculator.Calculate(stats);
            _wallet.Add(reward);
            _storage.Save(_saveData);
            GameEvents.Publish(new RunRewardGrantedEvent(reward));
        }
    }
}
