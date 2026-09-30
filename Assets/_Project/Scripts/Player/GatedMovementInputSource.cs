using System;
using UnityEngine;

namespace Game.Player
{
    public class GatedMovementInputSource : IMovementInputSource
    {
        private readonly IMovementInputSource _inner;
        private readonly Func<bool> _isInputAllowed;

        public GatedMovementInputSource(IMovementInputSource inner, Func<bool> isInputAllowed)
        {
            _inner = inner;
            _isInputAllowed = isInputAllowed;
        }

        public Vector2 GetMovementInput()
        {
            return _isInputAllowed() ? _inner.GetMovementInput() : Vector2.zero;
        }
    }
}
