using UnityEngine;

namespace Game.Combat
{
    public static class WeaponRingLayout
    {
        public static Vector2 GetOffset(int index, int count, float radius, float startAngleDegrees)
        {
            float angleDegrees = startAngleDegrees + 360f * index / count;
            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians)) * radius;
        }
    }
}
