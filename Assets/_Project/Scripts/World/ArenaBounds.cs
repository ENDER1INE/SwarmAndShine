using UnityEngine;

namespace Game.World
{
    public readonly struct ArenaBounds
    {
        public Vector2 Min { get; }
        public Vector2 Max { get; }
        public Vector2 Center => (Min + Max) * 0.5f;
        public Vector2 Size => Max - Min;

        public ArenaBounds(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public static ArenaBounds FromCenterAndSize(Vector2 center, Vector2 size)
        {
            Vector2 halfSize = size * 0.5f;
            return new ArenaBounds(center - halfSize, center + halfSize);
        }

        public Vector2 Clamp(Vector2 point)
        {
            return new Vector2(Mathf.Clamp(point.x, Min.x, Max.x), Mathf.Clamp(point.y, Min.y, Max.y));
        }

        public ArenaBounds Shrink(float margin)
        {
            Vector2 halfSize = Size * 0.5f;
            Vector2 shrunkHalfSize = new Vector2(Mathf.Max(0f, halfSize.x - margin), Mathf.Max(0f, halfSize.y - margin));
            return FromCenterAndSize(Center, shrunkHalfSize * 2f);
        }
    }
}
