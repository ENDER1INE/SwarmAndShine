using Game.World;
using UnityEngine;

namespace Game.CameraControl
{
    public class CameraFollowLogic
    {
        private readonly float _deadZoneFraction;

        public CameraFollowLogic(float deadZoneFraction)
        {
            _deadZoneFraction = deadZoneFraction;
        }

        public Vector2 ComputeDesiredPosition(Vector2 cameraPosition, Vector2 targetPosition, Vector2 halfViewSize, ArenaBounds arena)
        {
            Vector2 deadZoneHalfSize = halfViewSize * _deadZoneFraction;
            var desired = new Vector2(
                FollowAxis(cameraPosition.x, targetPosition.x, deadZoneHalfSize.x),
                FollowAxis(cameraPosition.y, targetPosition.y, deadZoneHalfSize.y));

            return ClampToArena(desired, halfViewSize, arena);
        }

        public Vector2 ClampToArena(Vector2 cameraPosition, Vector2 halfViewSize, ArenaBounds arena)
        {
            return new Vector2(
                ClampAxis(cameraPosition.x, arena.Min.x, arena.Max.x, halfViewSize.x),
                ClampAxis(cameraPosition.y, arena.Min.y, arena.Max.y, halfViewSize.y));
        }

        private static float FollowAxis(float cameraValue, float targetValue, float deadZoneHalfSize)
        {
            float offset = targetValue - cameraValue;
            if (offset > deadZoneHalfSize)
            {
                return targetValue - deadZoneHalfSize;
            }

            if (offset < -deadZoneHalfSize)
            {
                return targetValue + deadZoneHalfSize;
            }

            return cameraValue;
        }

        private static float ClampAxis(float value, float arenaMin, float arenaMax, float halfViewSize)
        {
            float lowest = arenaMin + halfViewSize;
            float highest = arenaMax - halfViewSize;
            if (lowest > highest)
            {
                return (arenaMin + arenaMax) * 0.5f;
            }

            return Mathf.Clamp(value, lowest, highest);
        }
    }
}
