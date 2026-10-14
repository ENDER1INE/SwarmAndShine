using System;
using UnityEngine;

namespace Game.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _hitDistance = 0.15f;

        private Vector2 _direction;
        private float _damage;
        private float _remainingDistance;
        private ITargetProvider _targetProvider;
        private Action<Projectile> _releaseToPool;

        public void Launch(
            Vector2 origin,
            Vector2 direction,
            float damage,
            float maxDistance,
            ITargetProvider targetProvider,
            Action<Projectile> releaseToPool)
        {
            transform.position = origin;
            _direction = direction;
            _damage = damage;
            _remainingDistance = maxDistance;
            _targetProvider = targetProvider;
            _releaseToPool = releaseToPool;
        }

        private void Update()
        {
            float stepDistance = _speed * Time.deltaTime;
            transform.Translate(_direction * stepDistance, Space.World);
            _remainingDistance -= stepDistance;

            ICombatTarget hitTarget = _targetProvider.FindNearestTarget(transform.position, _hitDistance);
            if (hitTarget != null)
            {
                hitTarget.TakeDamage(_damage);
                Release();
                return;
            }

            if (_remainingDistance <= 0f)
            {
                Release();
            }
        }

        private void Release()
        {
            _releaseToPool?.Invoke(this);
        }
    }
}
