using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "CharacterCatalog", menuName = "SAS/Player/Character Catalog")]
    public class CharacterCatalog : ScriptableObject
    {
        [SerializeField] private List<CharacterData> _characters = new();

        public IReadOnlyList<CharacterData> Characters => _characters;
    }
}
