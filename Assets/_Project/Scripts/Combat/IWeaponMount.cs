using UnityEngine;

namespace Game.Combat
{
    public interface IWeaponMount
    {
        Vector2 Position { get; }
        Vector2 MuzzlePosition { get; }

        void AimAt(Vector2 direction);
        void ClearAim();
        void PlayThrust(float distance, float duration);
        void PlaySwing(float startOffsetDegrees, float endOffsetDegrees, float duration);
    }
}
