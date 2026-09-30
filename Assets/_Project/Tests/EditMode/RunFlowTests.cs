using System.Collections.Generic;
using System.Reflection;
using Game.Core;
using Game.Player;
using Game.Run;
using Game.Spawning;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class RunFlowTests
    {
        private const float DefeatFallbackDelay = 2f;

        private class FakeWaveController : IWaveController
        {
            public int StartCount;
            public int StopCount;

            public void StartWave(WaveConfig waveConfig) => StartCount++;
            public void StopWave() => StopCount++;
        }

        private static WaveConfig CreateWave(float duration)
        {
            var wave = ScriptableObject.CreateInstance<WaveConfig>();
            FieldInfo field = typeof(WaveConfig).GetField("_duration", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(wave, duration);
            return wave;
        }

        private static RunFlow CreateFlow(FakeWaveController controller, params float[] waveDurations)
        {
            var waves = new List<WaveConfig>();
            foreach (float duration in waveDurations)
            {
                waves.Add(CreateWave(duration));
            }

            return new RunFlow(waves, controller, DefeatFallbackDelay);
        }

        [Test]
        public void Start_EntersFirstWaveAndStartsSpawning()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f, 10f);

            flow.Start();

            Assert.IsInstanceOf<WaveState>(flow.CurrentState);
            Assert.AreEqual(1, controller.StartCount);
        }

        [Test]
        public void WaveTimeElapsed_NotLastWave_OpensShopAndStopsSpawning()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f, 10f);
            flow.Start();

            flow.Tick(10f);

            Assert.IsInstanceOf<ShopState>(flow.CurrentState);
            Assert.AreEqual(1, controller.StopCount);
        }

        [Test]
        public void ContinueFromShop_StartsNextWave()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f, 10f);
            flow.Start();
            flow.Tick(10f);

            flow.ContinueFromShop();

            Assert.IsInstanceOf<WaveState>(flow.CurrentState);
            Assert.AreEqual(2, controller.StartCount);
        }

        [Test]
        public void ContinueFromShop_OutsideShop_DoesNothing()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f, 10f);
            flow.Start();

            flow.ContinueFromShop();

            Assert.AreEqual(1, controller.StartCount);
        }

        [Test]
        public void LastWaveTimeElapsed_EndsRunAsSurvived()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f);
            RunEndedEvent? ended = null;
            void OnEnded(RunEndedEvent evt) => ended = evt;
            GameEvents.Subscribe<RunEndedEvent>(OnEnded);

            try
            {
                flow.Start();
                flow.Tick(10f);

                Assert.IsInstanceOf<ResultsState>(flow.CurrentState);
                Assert.IsTrue(ended.Value.Survived);
                Assert.AreEqual(1, ended.Value.WavesCompleted);
            }
            finally
            {
                GameEvents.Unsubscribe<RunEndedEvent>(OnEnded);
            }
        }

        [Test]
        public void PlayerDied_DuringWave_WaitsForDeathAnimationAndStopsSpawning()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f, 10f);
            flow.Start();
            flow.Tick(10f);
            flow.ContinueFromShop();

            GameEvents.Publish(new PlayerDiedEvent());

            Assert.IsInstanceOf<PlayerDyingState>(flow.CurrentState);
            Assert.AreEqual(2, controller.StopCount);
        }

        [Test]
        public void DeathAnimationFinished_EndsRunAsDefeated()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f, 10f);
            RunEndedEvent? ended = null;
            void OnEnded(RunEndedEvent evt) => ended = evt;
            GameEvents.Subscribe<RunEndedEvent>(OnEnded);

            try
            {
                flow.Start();
                flow.Tick(10f);
                flow.ContinueFromShop();
                GameEvents.Publish(new PlayerDiedEvent());

                GameEvents.Publish(new PlayerDeathAnimationFinishedEvent());

                Assert.IsInstanceOf<ResultsState>(flow.CurrentState);
                Assert.IsFalse(ended.Value.Survived);
                Assert.AreEqual(1, ended.Value.WavesCompleted);
            }
            finally
            {
                GameEvents.Unsubscribe<RunEndedEvent>(OnEnded);
            }
        }

        [Test]
        public void DeathAnimationNeverFinishes_FallbackDelayEndsRun()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f);
            flow.Start();
            GameEvents.Publish(new PlayerDiedEvent());

            flow.Tick(DefeatFallbackDelay - 0.5f);
            Assert.IsInstanceOf<PlayerDyingState>(flow.CurrentState);

            flow.Tick(0.5f);
            Assert.IsInstanceOf<ResultsState>(flow.CurrentState);
        }

        [Test]
        public void PlayerDied_WhileAlreadyDying_IsIgnored()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f);
            flow.Start();
            GameEvents.Publish(new PlayerDiedEvent());

            GameEvents.Publish(new PlayerDiedEvent());

            Assert.IsInstanceOf<PlayerDyingState>(flow.CurrentState);
            Assert.AreEqual(1, controller.StopCount);
        }

        [Test]
        public void DeathAnimationFinished_WhenNotDying_IsIgnored()
        {
            var controller = new FakeWaveController();
            using var flow = CreateFlow(controller, 10f);
            flow.Start();

            GameEvents.Publish(new PlayerDeathAnimationFinishedEvent());

            Assert.IsInstanceOf<WaveState>(flow.CurrentState);
        }

        [Test]
        public void Constructor_EmptyWaveList_Throws()
        {
            Assert.Throws<System.ArgumentException>(() => new RunFlow(new List<WaveConfig>(), new FakeWaveController(), DefeatFallbackDelay));
        }
    }
}
