using System.Reflection;
using Game.Combat;
using Game.Core;
using Game.Player;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class PlayerHealthTests
    {
        private static CharacterStats CreateStats(float maxHealth, float armor, float healthRegen = 0f)
        {
            var data = ScriptableObject.CreateInstance<CharacterBaseStatsData>();
            SetField(data, "_maxHealth", maxHealth);
            SetField(data, "_armor", armor);
            SetField(data, "_healthRegen", healthRegen);
            return new CharacterStats(data);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        [Test]
        public void TakeDamage_ReducesHealthByDamageMinusArmor()
        {
            var stats = CreateStats(maxHealth: 100f, armor: 5f);
            var health = new PlayerHealth(stats);

            health.TakeDamage(20f);

            Assert.AreEqual(85f, health.CurrentHealth);
        }

        [Test]
        public void TakeDamage_DamageBelowArmor_DealsZero()
        {
            var stats = CreateStats(maxHealth: 100f, armor: 50f);
            var health = new PlayerHealth(stats);

            health.TakeDamage(10f);

            Assert.AreEqual(100f, health.CurrentHealth);
        }

        [Test]
        public void TakeDamage_LethalDamage_PublishesPlayerDiedEventExactlyOnce()
        {
            var stats = CreateStats(maxHealth: 10f, armor: 0f);
            var health = new PlayerHealth(stats);
            int deathEventCount = 0;
            void OnDied(PlayerDiedEvent evt) => deathEventCount++;
            GameEvents.Subscribe<PlayerDiedEvent>(OnDied);

            try
            {
                health.TakeDamage(15f);
                health.TakeDamage(15f); // уже мёртв, второй урон должен игнорироваться

                Assert.AreEqual(1, deathEventCount);
                Assert.IsTrue(health.IsDead);
                Assert.AreEqual(0f, health.CurrentHealth);
            }
            finally
            {
                GameEvents.Unsubscribe<PlayerDiedEvent>(OnDied);
            }
        }

        [Test]
        public void Heal_CannotExceedMaxHealth()
        {
            var stats = CreateStats(maxHealth: 50f, armor: 0f);
            var health = new PlayerHealth(stats);

            health.TakeDamage(10f);
            health.Heal(100f);

            Assert.AreEqual(50f, health.CurrentHealth);
        }

        [Test]
        public void Tick_AppliesHealthRegenOverTime()
        {
            var stats = CreateStats(maxHealth: 100f, armor: 0f, healthRegen: 10f);
            var health = new PlayerHealth(stats);
            health.TakeDamage(50f);

            health.Tick(1f);

            Assert.AreEqual(60f, health.CurrentHealth, 0.0001f);
        }
    }
}
