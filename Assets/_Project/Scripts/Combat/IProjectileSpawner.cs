using UnityEngine;

namespace Game.Combat
{
    public interface IProjectileSpawner
    {
        void SpawnProjectile(Vector2 origin, Vector2 direction, float damage, float maxDistance);
    }
}
