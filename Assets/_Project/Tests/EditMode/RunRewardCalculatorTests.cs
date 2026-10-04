using Game.Meta;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public class RunRewardCalculatorTests
    {
        private static readonly RunRewardRates Rates = new(
            baseReward: 10,
            perEnemyKilled: 2,
            perWaveCompleted: 5,
            perMinuteSurvived: 6,
            victoryBonus: 50);

        [Test]
        public void Calculate_EmptyDefeat_ReturnsBaseReward()
        {
            var calculator = new RunRewardCalculator(Rates);

            int reward = calculator.Calculate(new RunStats(0, 0f, 0, survived: false));

            Assert.AreEqual(10, reward);
        }

        [Test]
        public void Calculate_SumsKillsWavesAndTime()
        {
            var calculator = new RunRewardCalculator(Rates);

            int reward = calculator.Calculate(new RunStats(enemiesKilled: 20, secondsSurvived: 120f, wavesCompleted: 3, survived: false));

            // 10 base + 20*2 kills + 3*5 waves + 2 min*6 = 77
            Assert.AreEqual(77, reward);
        }

        [Test]
        public void Calculate_Victory_AddsVictoryBonus()
        {
            var calculator = new RunRewardCalculator(Rates);

            int defeat = calculator.Calculate(new RunStats(5, 60f, 2, survived: false));
            int victory = calculator.Calculate(new RunStats(5, 60f, 2, survived: true));

            Assert.AreEqual(50, victory - defeat);
        }

        [Test]
        public void Calculate_FractionalTimeReward_IsFlooredToWholeCurrency()
        {
            var calculator = new RunRewardCalculator(new RunRewardRates(0, 0, 0, perMinuteSurvived: 6, victoryBonus: 0));

            int reward = calculator.Calculate(new RunStats(0, secondsSurvived: 25f, 0, survived: false));

            // 25s = 0.4166 min * 6 = 2.5 -> 2
            Assert.AreEqual(2, reward);
        }
    }
}
