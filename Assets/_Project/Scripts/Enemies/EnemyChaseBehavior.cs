using Game.Combat;
using UnityEngine;

namespace Game.Enemies
{
    public class EnemyChaseBehavior
    {
        private readonly EnemyData _enemyData;
        private readonly IDamageable _damageTarget;
        private readonly float _contactRange;

        private float _contactDamageTimer;

        public EnemyChaseBehavior(EnemyData enemyData, IDamageable damageTarget, float contactRange)
        {
            _enemyData = enemyData;
            _damageTarget = damageTarget;
            _contactRange = contactRange;
        }

        public Vector2 Tick(float deltaTime, Vector2 currentPosition, Vector2 targetPosition)
        {
            Vector2 toTarget = targetPosition - currentPosition;
            float distance = toTarget.magnitude;

            if (distance > _contactRange)
            {
                _contactDamageTimer = 0f;
                Vector2 direction = toTarget / distance;
                return direction * (_enemyData.MoveSpeed * deltaTime);
            }

            _contactDamageTimer -= deltaTime;
            if (_contactDamageTimer <= 0f)
            {
                _contactDamageTimer = _enemyData.ContactDamageInterval;
                _damageTarget.TakeDamage(_enemyData.ContactDamage);
            }

            return Vector2.zero;
        }
    }
}
