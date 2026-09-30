using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class MeleeHitZoneTests
    {
        [Test]
        public void Contains_PointStraightAheadWithinReach_ReturnsTrue()
        {
            bool result = MeleeHitZone.Contains(Vector2.zero, Vector2.right, 2f, 0.3f, new Vector2(1.5f, 0f));

            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_PointBeyondReachPlusWidth_ReturnsFalse()
        {
            bool result = MeleeHitZone.Contains(Vector2.zero, Vector2.right, 2f, 0.3f, new Vector2(2.5f, 0f));

            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_PointBehindTheWeapon_ReturnsFalse()
        {
            bool result = MeleeHitZone.Contains(Vector2.zero, Vector2.right, 2f, 0.3f, new Vector2(-1f, 0f));

            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_PointToTheSideWithinHalfWidth_ReturnsTrue()
        {
            bool result = MeleeHitZone.Contains(Vector2.zero, Vector2.right, 2f, 0.3f, new Vector2(1f, 0.25f));

            Assert.IsTrue(result);
        }

        [Test]
        public void Contains_PointToTheSideBeyondHalfWidth_ReturnsFalse()
        {
            bool result = MeleeHitZone.Contains(Vector2.zero, Vector2.right, 2f, 0.3f, new Vector2(1f, 0.5f));

            Assert.IsFalse(result);
        }

        [Test]
        public void Contains_DiagonalDirection_UsesRotatedZone()
        {
            Vector2 direction = new Vector2(1f, 1f).normalized;

            Assert.IsTrue(MeleeHitZone.Contains(Vector2.zero, direction, 3f, 0.3f, new Vector2(1f, 1f)));
            Assert.IsFalse(MeleeHitZone.Contains(Vector2.zero, direction, 3f, 0.3f, new Vector2(1f, -1f)));
        }
    }
}
