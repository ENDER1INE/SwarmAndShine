using UnityEngine;

namespace Game.Player
{
    // Временная замена виртуального джойстика (GDD 2.3) для тестирования движения в редакторе.
    public class KeyboardMovementInputSource : IMovementInputSource
    {
        public Vector2 GetMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            return new Vector2(horizontal, vertical).normalized;
        }
    }
}
