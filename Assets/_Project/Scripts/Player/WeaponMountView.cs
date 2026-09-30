using Game.Combat;
using UnityEngine;

namespace Game.Player
{
    public class WeaponMountView : MonoBehaviour, IWeaponMount
    {
        private enum AttackAnimation
        {
            None,
            Thrust,
            Swing
        }

        [SerializeField] private Transform _visual;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _muzzle;

        private AttackAnimation _animation;
        private Vector2 _spriteRestOffset;
        private float _restingAngleDegrees;
        private float _animationDuration;
        private float _animationElapsedTime;
        private float _thrustDistance;
        private float _swingStartDegrees;
        private float _swingEndDegrees;
        private bool _isAiming;

        public Vector2 Position => transform.position;
        public Vector2 MuzzlePosition => _muzzle.position;

        private void Awake()
        {
            enabled = false;
        }

        private void Update()
        {
            _animationElapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(_animationElapsedTime / _animationDuration);

            switch (_animation)
            {
                case AttackAnimation.Thrust:
                    float outAndBack = 1f - Mathf.Abs(2f * progress - 1f);
                    SetSpriteOffset(_thrustDistance * outAndBack);
                    break;
                case AttackAnimation.Swing:
                    float swingAngle = Mathf.Lerp(_swingStartDegrees, _swingEndDegrees, progress);
                    _visual.localRotation = Quaternion.Euler(0f, 0f, swingAngle);
                    break;
            }

            if (progress >= 1f)
            {
                FinishAnimation();
            }
        }

        public void Configure(WeaponData weaponData)
        {
            Transform spriteTransform = _spriteRenderer.transform;
            _spriteRenderer.sprite = weaponData.WorldSprite;
            _spriteRenderer.color = weaponData.WorldTint;
            spriteTransform.localScale = new Vector3(weaponData.WorldScale.x, weaponData.WorldScale.y, 1f);
            spriteTransform.localRotation = Quaternion.Euler(0f, 0f, weaponData.WorldSpriteRotationDegrees);
            _spriteRestOffset = weaponData.WorldSpriteOffset;
            _muzzle.localPosition = weaponData.MuzzleOffset;

            SetSpriteOffset(0f);
            ApplyAngle(_restingAngleDegrees);
        }

        public void PlaceOnRing(Vector2 localOffset)
        {
            transform.localPosition = localOffset;
            _restingAngleDegrees = Mathf.Atan2(localOffset.y, localOffset.x) * Mathf.Rad2Deg;
            if (!_isAiming)
            {
                ApplyAngle(_restingAngleDegrees);
            }
        }

        public void AimAt(Vector2 direction)
        {
            _isAiming = true;
            ApplyAngle(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        public void ClearAim()
        {
            if (!_isAiming)
            {
                return;
            }

            _isAiming = false;
            ApplyAngle(_restingAngleDegrees);
        }

        public void PlayThrust(float distance, float duration)
        {
            StartAnimation(AttackAnimation.Thrust, duration);
            _thrustDistance = distance;
        }

        public void PlaySwing(float startOffsetDegrees, float endOffsetDegrees, float duration)
        {
            StartAnimation(AttackAnimation.Swing, duration);
            _swingStartDegrees = startOffsetDegrees;
            _swingEndDegrees = endOffsetDegrees;
            _visual.localRotation = Quaternion.Euler(0f, 0f, startOffsetDegrees);
        }

        private void StartAnimation(AttackAnimation animation, float duration)
        {
            FinishAnimation();
            _animation = animation;
            _animationDuration = duration;
            _animationElapsedTime = 0f;
            enabled = true;
        }

        private void FinishAnimation()
        {
            _animation = AttackAnimation.None;
            SetSpriteOffset(0f);
            _visual.localRotation = Quaternion.identity;
            enabled = false;
        }

        private void ApplyAngle(float angleDegrees)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, angleDegrees);

            bool isFacingLeft = Mathf.Cos(angleDegrees * Mathf.Deg2Rad) < 0f;
            _visual.localScale = new Vector3(1f, isFacingLeft ? -1f : 1f, 1f);
        }

        private void SetSpriteOffset(float thrustDistance)
        {
            _spriteRenderer.transform.localPosition = new Vector3(_spriteRestOffset.x + thrustDistance, _spriteRestOffset.y, 0f);
        }
    }
}
