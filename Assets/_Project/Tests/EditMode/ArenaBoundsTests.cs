using Game.World;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class ArenaBoundsTests
    {
        [Test]
        public void FromCenterAndSize_ComputesMinAndMax()
        {
            var bounds = ArenaBounds.FromCenterAndSize(new Vector2(2f, 3f), new Vector2(10f, 6f));

            Assert.AreEqual(new Vector2(-3f, 0f), bounds.Min);
            Assert.AreEqual(new Vector2(7f, 6f), bounds.Max);
            Assert.AreEqual(new Vector2(2f, 3f), bounds.Center);
        }

        [Test]
        public void Clamp_PointOutside_ReturnsNearestPointInside()
        {
            var bounds = ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(10f, 10f));

            Vector2 clamped = bounds.Clamp(new Vector2(20f, -20f));

            Assert.AreEqual(new Vector2(5f, -5f), clamped);
        }

        [Test]
        public void Clamp_PointInside_ReturnsSamePoint()
        {
            var bounds = ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(10f, 10f));

            Vector2 clamped = bounds.Clamp(new Vector2(1f, 2f));

            Assert.AreEqual(new Vector2(1f, 2f), clamped);
        }

        [Test]
        public void Shrink_ReducesEachSideByMargin()
        {
            var bounds = ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(10f, 10f));

            ArenaBounds shrunk = bounds.Shrink(1f);

            Assert.AreEqual(new Vector2(-4f, -4f), shrunk.Min);
            Assert.AreEqual(new Vector2(4f, 4f), shrunk.Max);
        }

        [Test]
        public void Shrink_MarginLargerThanArena_CollapsesToCenter()
        {
            var bounds = ArenaBounds.FromCenterAndSize(new Vector2(1f, 1f), new Vector2(2f, 2f));

            ArenaBounds shrunk = bounds.Shrink(50f);

            Assert.AreEqual(new Vector2(1f, 1f), shrunk.Min);
            Assert.AreEqual(new Vector2(1f, 1f), shrunk.Max);
        }
    }
}
