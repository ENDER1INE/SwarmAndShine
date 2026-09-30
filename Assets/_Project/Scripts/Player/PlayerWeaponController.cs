using System;
using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        private IReadOnlyList<IWeaponBehavior> _weapons;
        private Func<bool> _isFiringAllowed;

        public void Init(IReadOnlyList<IWeaponBehavior> weapons, Func<bool> isFiringAllowed)
        {
            _weapons = weapons;
            _isFiringAllowed = isFiringAllowed;
        }

        private void Update()
        {
            if (_weapons == null || _isFiringAllowed == null || !_isFiringAllowed())
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            for (int i = 0; i < _weapons.Count; i++)
            {
                _weapons[i].Tick(deltaTime);
            }
        }
    }
}
