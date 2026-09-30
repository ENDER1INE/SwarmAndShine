using Game.CameraControl;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class ViewportFitterTests
    {
        private static readonly Vector2 MinVisibleSize = new Vector2(10f, 10f);

        [Test]
        public void ComputeOrthographicSize_Landscape_FitsMinimumHeight()
        {
            float size = ViewportFitter.ComputeOrthographicSize(16f / 9f, MinVisibleSize);

            Assert.AreEqual(5f, size, 0.0001f);
        }

        [Test]
        public void ComputeOrthographicSize_Portrait_FitsMinimumWidth()
        {
            float aspect = 9f / 16f;

            float size = ViewportFitter.ComputeOrthographicSize(aspect, MinVisibleSize);

            Assert.AreEqual(5f / aspect, size, 0.0001f);
            Assert.GreaterOrEqual(size * aspect * 2f, MinVisibleSize.x - 0.0001f);
        }

        [Test]
        public void ComputeOrthographicSize_AnyAspect_NeverShowsLessThanMinimumOnEitherAxis()
        {
            foreach (float aspect in new[] { 0.45f, 0.56f, 1f, 1.33f, 2.16f })
            {
                float size = ViewportFitter.ComputeOrthographicSize(aspect, MinVisibleSize);

                Assert.GreaterOrEqual(size * 2f, MinVisibleSize.y - 0.0001f);
                Assert.GreaterOrEqual(size * aspect * 2f, MinVisibleSize.x - 0.0001f);
            }
        }
    }
}
