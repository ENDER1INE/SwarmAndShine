using UnityEngine;

namespace Game.Shop
{
    public class UnityRandomSource : IRandomSource
    {
        public float NextValue() => Random.value;
    }
}
