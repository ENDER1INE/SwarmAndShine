using Game.Core;
using UnityEngine;

namespace Game.Player
{
    public class PlayerDeathPresenter : MonoBehaviour
    {
        private CharacterAnimator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<CharacterAnimator>();
            _animator.DeathAnimationCompleted += OnDeathAnimationCompleted;
            GameEvents.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnDestroy()
        {
            _animator.DeathAnimationCompleted -= OnDeathAnimationCompleted;
            GameEvents.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnPlayerDied(PlayerDiedEvent evt)
        {
            _animator.SetMoving(false);
            _animator.PlayDeath();
        }

        private void OnDeathAnimationCompleted()
        {
            GameEvents.Publish(new PlayerDeathAnimationFinishedEvent());
        }
    }
}
