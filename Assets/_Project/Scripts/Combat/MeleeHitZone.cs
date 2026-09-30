using UnityEngine;

namespace Game.Combat
{
    // Капсула: отрезок от origin длиной reach вдоль direction, расширенный на halfWidth во все стороны.
    public static class MeleeHitZone
    {
        public static bool Contains(Vector2 origin, Vector2 direction, float reach, float halfWidth, Vector2 point)
        {
            Vector2 toPoint = point - origin;
            float distanceAlongDirection = Mathf.Clamp(Vector2.Dot(toPoint, direction), 0f, reach);
            Vector2 closestPointOnSegment = origin + direction * distanceAlongDirection;

            return (point - closestPointOnSegment).sqrMagnitude <= halfWidth * halfWidth;
        }
    }
}
