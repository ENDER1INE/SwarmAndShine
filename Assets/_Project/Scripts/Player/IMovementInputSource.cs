using UnityEngine;

namespace Game.Player
{
    public interface IMovementInputSource
    {
        Vector2 GetMovementInput();
    }
}
