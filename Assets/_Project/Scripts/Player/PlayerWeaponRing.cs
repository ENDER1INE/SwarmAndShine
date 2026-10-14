using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Player
{
    public class PlayerWeaponRing : MonoBehaviour, IWeaponMountFactory
    {
        [SerializeField] private WeaponMountView _mountPrefab;
        [SerializeField] private WeaponRingData _data;

        private readonly List<WeaponMountView> _mounts = new();

        private void Awake()
        {
            transform.localPosition = _data.CenterOffset;
        }

        public IWeaponMount Create(WeaponData weaponData)
        {
            WeaponMountView mount = Instantiate(_mountPrefab, transform);
            mount.Configure(weaponData);
            mount.Released += OnMountReleased;
            _mounts.Add(mount);
            Relayout();
            return mount;
        }

        private void OnMountReleased(WeaponMountView mount)
        {
            mount.Released -= OnMountReleased;
            _mounts.Remove(mount);
            Relayout();
        }

        private void Relayout()
        {
            for (int i = 0; i < _mounts.Count; i++)
            {
                Vector2 offset = WeaponRingLayout.GetOffset(i, _mounts.Count, _data.Radius, _data.StartAngleDegrees);
                _mounts[i].PlaceOnRing(offset);
            }
        }
    }
}
