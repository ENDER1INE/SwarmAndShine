using Game.Combat;
using Game.Core;
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
        private bool _isInitialized;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<CharacterAnimator>();
        }

        public void Init(CharacterStats stats, IMovementInputSource inputSource)
        {
            _stats = stats;
            _inputSource = inputSource;
            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            Vector2 movementInput = _inputSource.GetMovementInput();
            _rigidbody.linearVelocity = movementInput * _stats.MoveSpeed;

            _animator?.SetMoving(movementInput.sqrMagnitude > 0.0001f);
            if (movementInput.x != 0f)
            {
                _animator?.SetFacing(movementInput.x);
            }
        }
    }
}
