using UnityEngine;

namespace Game.Combat
{
    public static class ProjectileSpread
    {
        public static Vector2 GetDirection(Vector2 aimDirection, int projectileIndex, int projectileCount, float spreadAngleDegrees)
        {
            if (projectileCount <= 1 || Mathf.Approximately(spreadAngleDegrees, 0f))
            {
                return aimDirection;
            }

            float step = spreadAngleDegrees / (projectileCount - 1);
            float offsetDegrees = -spreadAngleDegrees * 0.5f + step * projectileIndex;
            float radians = offsetDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            return new Vector2(
                aimDirection.x * cos - aimDirection.y * sin,
                aimDirection.x * sin + aimDirection.y * cos);
        }
    }
}
