using UnityEngine;

namespace Game.World
{
    // Собирает пол арены из отдельных тайлов одного бесшовного спрайта.
    // Ряды укладываются в шахматном/кирпичном порядке (сдвиг на половину тайла
    // через ряд) — это разбивает совпадающие по вертикали повторяющиеся детали
    // рисунка (трещины и т.п.). Поворот тайлов на 90° и тонировка цветом не
    // используются: спрайт бесшовен только в исходной ориентации (поворот даёт
    // видимые швы), а тонировка даёт резкую границу между соседними тайлами
    // даже при небольшой разнице цвета — визуально это хуже, чем сам повтор рисунка.
    public class TiledFloorView : MonoBehaviour
    {
        [SerializeField] private Transform _tileContainer;
        [SerializeField] private Sprite _tileSprite;
        [SerializeField] private int _sortingOrder = -100;

        public void Init(ArenaBounds arena)
        {
            ClearTiles();
            if (_tileSprite == null)
            {
                return;
            }

            Vector2 tileSize = _tileSprite.bounds.size;
            Vector2 size = arena.Size;
            int columns = Mathf.CeilToInt(size.x / tileSize.x);
            int rows = Mathf.CeilToInt(size.y / tileSize.y);
            Vector2 origin = arena.Center - size * 0.5f;

            for (int row = 0; row < rows; row++)
            {
                bool offsetRow = row % 2 == 1;
                float rowOffsetX = offsetRow ? tileSize.x * 0.5f : 0f;
                int columnStart = offsetRow ? -1 : 0;
                int columnEnd = offsetRow ? columns : columns - 1;

                for (int column = columnStart; column <= columnEnd; column++)
                {
                    Vector2 tileCenter = origin + new Vector2(
                        rowOffsetX + (column + 0.5f) * tileSize.x,
                        (row + 0.5f) * tileSize.y);
                    SpawnTile(tileCenter);
                }
            }
        }

        private void SpawnTile(Vector2 position)
        {
            var tile = new GameObject("FloorTile");
            tile.transform.SetParent(_tileContainer, false);
            tile.transform.position = new Vector3(position.x, position.y, 0f);

            var renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = _tileSprite;
            renderer.sortingOrder = _sortingOrder;
        }

        private void ClearTiles()
        {
            for (int i = _tileContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(_tileContainer.GetChild(i).gameObject);
            }
        }
    }
}
