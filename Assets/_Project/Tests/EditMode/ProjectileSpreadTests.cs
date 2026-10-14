using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class ProjectileSpreadTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void GetDirection_SingleProjectile_ReturnsAimDirection()
        {
            Vector2 result = ProjectileSpread.GetDirection(Vector2.up, 0, 1, 40f);

            Assert.AreEqual(Vector2.up.x, result.x, Tolerance);
            Assert.AreEqual(Vector2.up.y, result.y, Tolerance);
        }

        [Test]
        public void GetDirection_ZeroSpread_ReturnsAimDirection()
        {
            Vector2 result = ProjectileSpread.GetDirection(Vector2.right, 2, 5, 0f);

            Assert.AreEqual(1f, result.x, Tolerance);
            Assert.AreEqual(0f, result.y, Tolerance);
        }

        [Test]
        public void GetDirection_OddCount_MiddleProjectileKeepsAim()
        {
            Vector2 result = ProjectileSpread.GetDirection(Vector2.right, 2, 5, 40f);

            Assert.AreEqual(1f, result.x, Tolerance);
            Assert.AreEqual(0f, result.y, Tolerance);
        }

        [Test]
        public void GetDirection_EdgeProjectiles_DeviateByHalfSpreadInOppositeDirections()
        {
            Vector2 first = ProjectileSpread.GetDirection(Vector2.right, 0, 5, 40f);
            Vector2 last = ProjectileSpread.GetDirection(Vector2.right, 4, 5, 40f);

            Assert.AreEqual(-20f, Vector2.SignedAngle(Vector2.right, first), 0.001f);
            Assert.AreEqual(20f, Vector2.SignedAngle(Vector2.right, last), 0.001f);
        }
    }
}
