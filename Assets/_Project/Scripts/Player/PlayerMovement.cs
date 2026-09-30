using Game.Combat;
using Game.Core;
using Game.World;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private CharacterAnimator _animator;
        private CharacterStats _stats;
        private IMovementInputSource _inputSource;
        private ArenaBounds _movementBounds;
        private bool _isInitialized;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<CharacterAnimator>();
        }

        public void Init(CharacterStats stats, IMovementInputSource inputSource, ArenaBounds movementBounds)
        {
            _stats = stats;
            _inputSource = inputSource;
            _movementBounds = movementBounds;
            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            Vector2 movementInput = _inputSource.GetMovementInput();
            Vector2 currentPosition = _rigidbody.position;
            Vector2 desiredPosition = currentPosition + movementInput * (_stats.MoveSpeed * Time.fixedDeltaTime);
            Vector2 allowedPosition = _movementBounds.Clamp(desiredPosition);
            _rigidbody.linearVelocity = (allowedPosition - currentPosition) / Time.fixedDeltaTime;

            _animator?.SetMoving(movementInput.sqrMagnitude > 0.0001f);
            if (movementInput.x != 0f)
            {
                _animator?.SetFacing(movementInput.x);
            }
        }
    }
}
