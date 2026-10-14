using System;
using UnityEngine;

namespace Game.Core
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterAnimator : MonoBehaviour
    {
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int DieHash = Animator.StringToHash("Die");

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public event Action DeathAnimationCompleted;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetController(RuntimeAnimatorController controller)
        {
            // Called from a bootstrap that may run before this component's Awake.
            GetComponent<Animator>().runtimeAnimatorController = controller;
        }

        public void SetMoving(bool isMoving)
        {
            _animator.SetBool(IsMovingHash, isMoving);
        }

        public void SetFacing(float directionX)
        {
            if (Mathf.Approximately(directionX, 0f))
            {
                return;
            }

            _spriteRenderer.flipX = directionX < 0f;
        }

        public void PlayDeath()
        {
            _animator.SetTrigger(DieHash);
        }

        public void ResetToIdle()
        {
            _animator.Rebind();
            _animator.ResetTrigger(DieHash);
            _animator.Update(0f);
        }

        public void NotifyDeathAnimationComplete()
        {
            DeathAnimationCompleted?.Invoke();
        }
    }
}
