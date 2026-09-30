using System;
using Game.World;
using UnityEngine;

namespace Game.CameraControl
{
    [RequireComponent(typeof(Camera))]
    public class CameraRigController : MonoBehaviour
    {
        [SerializeField] private CameraRigData _data;

        private Camera _camera;
        private CameraFollowLogic _followLogic;
        private Func<Vector2> _getTargetPosition;
        private ArenaBounds _arenaBounds;
        private Vector2 _smoothingVelocity;
        private int _lastScreenWidth;
        private int _lastScreenHeight;
        private bool _isInitialized;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        public void Init(Func<Vector2> getTargetPosition, ArenaBounds arenaBounds)
        {
            if (!enabled)
            {
                return;
            }

            _getTargetPosition = getTargetPosition;
            _arenaBounds = arenaBounds;
            _followLogic = new CameraFollowLogic(_data.DeadZoneFraction);

            FitViewport();
            SetPosition(_followLogic.ClampToArena(_getTargetPosition(), GetHalfViewSize(), _arenaBounds));
            _isInitialized = true;
        }

        private void LateUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                FitViewport();
            }

            Vector2 halfViewSize = GetHalfViewSize();
            Vector2 currentPosition = transform.position;
            Vector2 desiredPosition = _followLogic.ComputeDesiredPosition(currentPosition, _getTargetPosition(), halfViewSize, _arenaBounds);
            Vector2 smoothedPosition = Vector2.SmoothDamp(currentPosition, desiredPosition, ref _smoothingVelocity, _data.SmoothTime);

            SetPosition(_followLogic.ClampToArena(smoothedPosition, halfViewSize, _arenaBounds));
        }

        private void FitViewport()
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            _camera.orthographicSize = ViewportFitter.ComputeOrthographicSize(_camera.aspect, _data.MinVisibleSize);
        }

        private Vector2 GetHalfViewSize()
        {
            return new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
        }

        private void SetPosition(Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }
    }
}
