using Game.CameraControl;
using Game.World;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class CameraFollowLogicTests
    {
        private static readonly Vector2 HalfViewSize = new Vector2(8f, 5f);

        private static ArenaBounds CreateLargeArena()
        {
            return ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(60f, 60f));
        }

        [Test]
        public void ComputeDesiredPosition_TargetInsideDeadZone_KeepsCameraStill()
        {
            var logic = new CameraFollowLogic(deadZoneFraction: 0.5f);

            Vector2 desired = logic.ComputeDesiredPosition(Vector2.zero, new Vector2(3f, 2f), HalfViewSize, CreateLargeArena());

            Assert.AreEqual(Vector2.zero, desired);
        }

        [Test]
        public void ComputeDesiredPosition_TargetBeyondDeadZone_MovesCameraToKeepTargetOnDeadZoneEdge()
        {
            var logic = new CameraFollowLogic(deadZoneFraction: 0.5f);

            Vector2 desired = logic.ComputeDesiredPosition(Vector2.zero, new Vector2(10f, 0f), HalfViewSize, CreateLargeArena());

            Assert.AreEqual(6f, desired.x, 0.0001f);
            Assert.AreEqual(0f, desired.y, 0.0001f);
        }

        [Test]
        public void ComputeDesiredPosition_TargetNearArenaEdge_CameraStaysInsideArena()
        {
            var logic = new CameraFollowLogic(deadZoneFraction: 0.5f);
            var arena = ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(40f, 40f));

            Vector2 desired = logic.ComputeDesiredPosition(new Vector2(11f, 0f), new Vector2(19.5f, 0f), HalfViewSize, arena);

            Assert.AreEqual(12f, desired.x, 0.0001f);
        }

        [Test]
        public void ClampToArena_ArenaSmallerThanViewOnAxis_CentersCameraOnThatAxis()
        {
            var logic = new CameraFollowLogic(deadZoneFraction: 0.5f);
            var arena = ArenaBounds.FromCenterAndSize(new Vector2(4f, 0f), new Vector2(10f, 40f));

            Vector2 clamped = logic.ClampToArena(new Vector2(-3f, 7f), HalfViewSize, arena);

            Assert.AreEqual(4f, clamped.x, 0.0001f);
            Assert.AreEqual(7f, clamped.y, 0.0001f);
        }
    }
}
