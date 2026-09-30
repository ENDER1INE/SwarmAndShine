using Game.Combat;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class WeaponRingLayoutTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void GetOffset_SingleWeapon_SitsAtStartAngle()
        {
            Vector2 offset = WeaponRingLayout.GetOffset(0, 1, 2f, 0f);

            Assert.AreEqual(2f, offset.x, Tolerance);
            Assert.AreEqual(0f, offset.y, Tolerance);
        }

        [Test]
        public void GetOffset_TwoWeapons_AreOppositeEachOther()
        {
            Vector2 first = WeaponRingLayout.GetOffset(0, 2, 1f, 0f);
            Vector2 second = WeaponRingLayout.GetOffset(1, 2, 1f, 0f);

            Assert.AreEqual(-first.x, second.x, Tolerance);
            Assert.AreEqual(-first.y, second.y, Tolerance);
        }

        [Test]
        public void GetOffset_SixWeapons_AreEvenlySpacedOnTheRing()
        {
            const int count = 6;
            const float radius = 1.5f;

            for (int i = 0; i < count; i++)
            {
                Vector2 offset = WeaponRingLayout.GetOffset(i, count, radius, 30f);

                Assert.AreEqual(radius, offset.magnitude, Tolerance);
                float expectedAngle = 30f + 60f * i;
                float actualAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
                Assert.AreEqual(0f, Mathf.DeltaAngle(expectedAngle, actualAngle), 0.001f);
            }
        }

        [Test]
        public void GetOffset_StartAngle_RotatesTheWholeRing()
        {
            Vector2 offset = WeaponRingLayout.GetOffset(0, 4, 1f, 90f);

            Assert.AreEqual(0f, offset.x, Tolerance);
            Assert.AreEqual(1f, offset.y, Tolerance);
        }
    }
}
