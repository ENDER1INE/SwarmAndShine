using UnityEngine;

namespace Game.CameraControl
{
    [CreateAssetMenu(fileName = "CameraRigData", menuName = "SAS/World/Camera Rig")]
    public class CameraRigData : ScriptableObject
    {
        [SerializeField] private Vector2 _minVisibleSize = new Vector2(10f, 10f);
        [Range(0f, 1f)]
        [SerializeField] private float _deadZoneFraction = 0.5f;
        [SerializeField] private float _smoothTime = 0.15f;

        public Vector2 MinVisibleSize => _minVisibleSize;
        public float DeadZoneFraction => _deadZoneFraction;
        public float SmoothTime => _smoothTime;
    }
}
