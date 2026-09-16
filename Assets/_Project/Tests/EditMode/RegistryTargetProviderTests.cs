using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class RegistryTargetProviderTests
    {
        private class FakeCombatTarget : ICombatTarget
        {
            public Vector2 Position { get; set; }
            public bool IsAlive { get; set; } = true;

            public void TakeDamage(float damage)
            {
            }
        }

        [Test]
        public void FindNearestTarget_ReturnsClosestAliveTargetWithinRange()
        {
            var registry = new CombatTargetRegistry();
            var far = new FakeCombatTarget { Position = new Vector2(10f, 0f) };
            var near = new FakeCombatTarget { Position = new Vector2(2f, 0f) };
            registry.Register(far);
            registry.Register(near);

            var provider = new RegistryTargetProvider(registry);
            var result = provider.FindNearestTarget(Vector2.zero, maxRange: 20f);

            Assert.AreSame(near, result);
        }

        [Test]
        public void FindNearestTarget_TargetOutOfRange_ReturnsNull()
        {
            var registry = new CombatTargetRegistry();
            var outOfRange = new FakeCombatTarget { Position = new Vector2(50f, 0f) };
            registry.Register(outOfRange);

            var provider = new RegistryTargetProvider(registry);
            var result = provider.FindNearestTarget(Vector2.zero, maxRange: 5f);

            Assert.IsNull(result);
        }

        [Test]
        public void FindNearestTarget_IgnoresDeadTargets()
        {
            var registry = new CombatTargetRegistry();
            var dead = new FakeCombatTarget { Position = new Vector2(1f, 0f), IsAlive = false };
            var alive = new FakeCombatTarget { Position = new Vector2(5f, 0f), IsAlive = true };
            registry.Register(dead);
            registry.Register(alive);

            var provider = new RegistryTargetProvider(registry);
            var result = provider.FindNearestTarget(Vector2.zero, maxRange: 20f);

            Assert.AreSame(alive, result);
        }
    }
}
