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
        private ICombatTarget _target;
        private Action<Projectile> _releaseToPool;

        public void Launch(Vector2 origin, Vector2 direction, float damage, ICombatTarget target, Action<Projectile> releaseToPool)
        {
            transform.position = origin;
            _direction = direction;
            _damage = damage;
            _target = target;
            _releaseToPool = releaseToPool;
        }

        private void Update()
        {
            if (_target == null || !_target.IsAlive)
            {
                Release();
                return;
            }

            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);

            float distance = Vector2.Distance(transform.position, _target.Position);
            if (distance <= _hitDistance)
            {
                _target.TakeDamage(_damage);
                Release();
            }
        }

        private void Release()
        {
            _releaseToPool?.Invoke(this);
        }
    }
}
