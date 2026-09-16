using System.Reflection;
using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CharacterStatsTests
    {
        private static CharacterBaseStatsData CreateBaseStatsData(float maxHealth, float healthRegen, float moveSpeed, float armor)
        {
            var data = ScriptableObject.CreateInstance<CharacterBaseStatsData>();
            SetField(data, "_maxHealth", maxHealth);
            SetField(data, "_healthRegen", healthRegen);
            SetField(data, "_moveSpeed", moveSpeed);
            SetField(data, "_armor", armor);
            return data;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, value);
        }

        [Test]
        public void Recalculate_NoModifiers_ReturnsBaseValues()
        {
            var baseData = CreateBaseStatsData(maxHealth: 100f, healthRegen: 2f, moveSpeed: 5f, armor: 1f);
            var stats = new CharacterStats(baseData);

            Assert.AreEqual(100f, stats.MaxHealth);
            Assert.AreEqual(2f, stats.HealthRegen);
            Assert.AreEqual(5f, stats.MoveSpeed);
            Assert.AreEqual(1f, stats.Armor);
        }

        [Test]
        public void AddModifier_FlatBonus_AddsToBase()
        {
            var baseData = CreateBaseStatsData(100f, 0f, 5f, 0f);
            var stats = new CharacterStats(baseData);

            stats.AddModifier(new StatModifier(StatType.MaxHealth, ModifierType.Flat, 20f));

            Assert.AreEqual(120f, stats.MaxHealth);
        }

        [Test]
        public void AddModifier_MultiplePercentAdd_StackAdditivelyNotMultiplicatively()
        {
            var baseData = CreateBaseStatsData(100f, 0f, 5f, 0f);
            var stats = new CharacterStats(baseData);

            stats.AddModifier(new StatModifier(StatType.MoveSpeedPercent, ModifierType.PercentAdd, 0.1f));
            stats.AddModifier(new StatModifier(StatType.MoveSpeedPercent, ModifierType.PercentAdd, 0.2f));

            // 5 * (1 + 0.1 + 0.2) = 6.5, а не 5 * 1.1 * 1.2 = 6.6
            Assert.AreEqual(6.5f, stats.MoveSpeed, 0.0001f);
        }

        [Test]
        public void AddModifier_PercentMultiplyPenalty_AppliesAfterPercentAdd()
        {
            var baseData = CreateBaseStatsData(100f, 0f, 5f, 0f);
            var stats = new CharacterStats(baseData);

            stats.AddModifier(new StatModifier(StatType.MaxHealth, ModifierType.PercentAdd, 0.3f));
            stats.AddModifier(new StatModifier(StatType.MaxHealth, ModifierType.PercentMultiply, -0.2f));

            // 100 * (1 + 0.3) * (1 - 0.2) = 104
            Assert.AreEqual(104f, stats.MaxHealth, 0.0001f);
        }

        [Test]
        public void RemoveModifiersFromSource_RemovesOnlyModifiersFromThatSource()
        {
            var baseData = CreateBaseStatsData(100f, 0f, 5f, 0f);
            var stats = new CharacterStats(baseData);
            var itemA = new object();
            var itemB = new object();

            stats.AddModifier(new StatModifier(StatType.MaxHealth, ModifierType.Flat, 10f, itemA));
            stats.AddModifier(new StatModifier(StatType.MaxHealth, ModifierType.Flat, 5f, itemB));

            stats.RemoveModifiersFromSource(itemA);

            Assert.AreEqual(105f, stats.MaxHealth);
        }
    }
}
