using System;
using Game.Combat;
using Game.Core;
using UnityEngine;

namespace Game.Enemies
{
    public class EnemyController : MonoBehaviour, ICombatTarget
    {
        [SerializeField] private float _contactRange = 0.4f;

        private EnemyData _enemyData;
        private CombatTargetRegistry _registry;
        private Func<Vector2> _targetPositionProvider;
        private EnemyChaseBehavior _chaseBehavior;
        private Action<EnemyController> _releaseToPool;
        private CharacterAnimator _animator;
        private float _currentHealth;
        private bool _isDying;

        public Vector2 Position => transform.position;
        public bool IsAlive => _currentHealth > 0f;

        private void Awake()
        {
            _animator = GetComponentInChildren<CharacterAnimator>();
            if (_animator != null)
            {
                _animator.DeathAnimationCompleted += OnDeathAnimationCompleted;
            }
        }

        public void Init(
            EnemyData enemyData,
            CombatTargetRegistry registry,
            Func<Vector2> targetPositionProvider,
            IDamageable damageTarget,
            Action<EnemyController> releaseToPool)
        {
            _enemyData = enemyData;
            _registry = registry;
            _targetPositionProvider = targetPositionProvider;
            _chaseBehavior = new EnemyChaseBehavior(enemyData, damageTarget, _contactRange);
            _releaseToPool = releaseToPool;
            _currentHealth = _enemyData.MaxHealth;
            _isDying = false;
            _animator?.ResetToIdle();
            _registry.Register(this);
        }

        private void Update()
        {
            if (_isDying)
            {
                return;
            }

            Vector2 movement = _chaseBehavior.Tick(Time.deltaTime, Position, _targetPositionProvider());
            transform.Translate(movement, Space.World);

            _animator?.SetMoving(movement.sqrMagnitude > 0.0001f);
            if (movement.x != 0f)
            {
                _animator?.SetFacing(movement.x);
            }
        }

        public void TakeDamage(float damage)
        {
            if (!IsAlive || _isDying)
            {
                return;
            }

            _currentHealth -= damage;
            if (_currentHealth <= 0f)
            {
                _currentHealth = 0f;
                _registry.Unregister(this);
                BeginDeath();
            }
        }

        private void BeginDeath()
        {
            _isDying = true;

            if (_animator != null)
            {
                _animator.SetMoving(false);
                _animator.PlayDeath();
                return;
            }

            _releaseToPool?.Invoke(this);
        }

        private void OnDeathAnimationCompleted()
        {
            if (!_isDying)
            {
                return;
            }

            _releaseToPool?.Invoke(this);
        }
    }
}
