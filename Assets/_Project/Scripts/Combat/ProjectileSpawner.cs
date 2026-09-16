using Game.Core;
using UnityEngine;

namespace Game.Combat
{
    public class ProjectileSpawner : MonoBehaviour, IProjectileSpawner
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private int _initialPoolSize = 10;

        private ObjectPool<Projectile> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<Projectile>(_projectilePrefab, _initialPoolSize, transform);
        }

        public void SpawnProjectile(Vector2 origin, Vector2 direction, float damage, ICombatTarget target)
        {
            Projectile projectile = _pool.Get();
            projectile.Launch(origin, direction, damage, target, ReleaseProjectile);
        }

        private void ReleaseProjectile(Projectile projectile)
        {
            _pool.Release(projectile);
        }
    }
}
