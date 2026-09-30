using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "WeaponRingData", menuName = "SAS/Player/Weapon Ring")]
    public class WeaponRingData : ScriptableObject
    {
        [SerializeField] private Vector2 _centerOffset = new Vector2(0f, 0.6f);
        [SerializeField] private float _radius = 0.9f;
        [SerializeField] private float _startAngleDegrees;

        public Vector2 CenterOffset => _centerOffset;
        public float Radius => _radius;
        public float StartAngleDegrees => _startAngleDegrees;
    }
}
