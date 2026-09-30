using UnityEngine;

namespace Game.World
{
    // Заглушка до финального арта арены: тайл-спрайты 1x1 растягиваются под размер арены.
    public class ArenaBorderView : MonoBehaviour
    {
        [SerializeField] private Transform _leftWall;
        [SerializeField] private Transform _rightWall;
        [SerializeField] private Transform _topWall;
        [SerializeField] private Transform _bottomWall;
        [SerializeField] private float _wallThickness = 0.3f;

        public void Init(ArenaBounds arena)
        {
            Vector2 center = arena.Center;
            Vector2 size = arena.Size;
            float halfThickness = _wallThickness * 0.5f;
            float horizontalWallLength = size.x - _wallThickness * 2f;

            Place(_leftWall, new Vector2(arena.Min.x + halfThickness, center.y), new Vector2(_wallThickness, size.y));
            Place(_rightWall, new Vector2(arena.Max.x - halfThickness, center.y), new Vector2(_wallThickness, size.y));
            Place(_topWall, new Vector2(center.x, arena.Max.y - halfThickness), new Vector2(horizontalWallLength, _wallThickness));
            Place(_bottomWall, new Vector2(center.x, arena.Min.y + halfThickness), new Vector2(horizontalWallLength, _wallThickness));
        }

        private static void Place(Transform target, Vector2 position, Vector2 size)
        {
            target.position = new Vector3(position.x, position.y, target.position.z);
            target.localScale = new Vector3(size.x, size.y, 1f);
        }
    }
}
