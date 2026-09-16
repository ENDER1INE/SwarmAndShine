using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        private readonly List<IWeaponBehavior> _weapons = new();

        public void Init(IEnumerable<IWeaponBehavior> weapons)
        {
            _weapons.AddRange(weapons);
        }

        private void Update()
        {
            foreach (var weapon in _weapons)
            {
                weapon.Tick(Time.deltaTime);
            }
        }
    }
}
