using UnityEngine;

namespace Game.CameraControl
{
    public static class ViewportFitter
    {
        public static float ComputeOrthographicSize(float aspectRatio, Vector2 minVisibleSize)
        {
            float sizeToFitHeight = minVisibleSize.y * 0.5f;
            float sizeToFitWidth = minVisibleSize.x * 0.5f / aspectRatio;
            return Mathf.Max(sizeToFitHeight, sizeToFitWidth);
        }
    }
}
