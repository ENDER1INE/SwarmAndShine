using System.Collections.Generic;
using Game.Player;

namespace Game.Meta
{
    public class CharacterSelection
    {
        private readonly SaveData _saveData;
        private readonly ISaveStorage _storage;
        private readonly CharacterUnlockService _unlockService;
        private readonly IReadOnlyList<CharacterData> _characters;

        public CharacterSelection(
            SaveData saveData,
            ISaveStorage storage,
            CharacterUnlockService unlockService,
            IReadOnlyList<CharacterData> characters)
        {
            _saveData = saveData;
            _storage = storage;
            _unlockService = unlockService;
            _characters = characters;
        }

        // Falls back to the first default character when nothing valid is saved.
        public CharacterData GetSelected()
        {
            CharacterData fallback = null;
            foreach (CharacterData character in _characters)
            {
                if (character.Id == _saveData.SelectedCharacterId && _unlockService.IsUnlocked(character))
                {
                    return character;
                }

                if (fallback == null && character.UnlockType == CharacterUnlockType.Default)
                {
                    fallback = character;
                }
            }

            return fallback;
        }

        public CharacterMenuStatus GetStatus(CharacterData character)
        {
            if (!_unlockService.IsUnlocked(character))
            {
                return character.UnlockType == CharacterUnlockType.MetaCurrency
                    ? CharacterMenuStatus.PurchasableWithCurrency
                    : CharacterMenuStatus.LockedByAchievement;
            }

            return GetSelected() == character ? CharacterMenuStatus.Selected : CharacterMenuStatus.Unlocked;
        }

        public bool TrySelect(CharacterData character)
        {
            if (!_unlockService.IsUnlocked(character))
            {
                return false;
            }

            _saveData.SelectedCharacterId = character.Id;
            _storage.Save(_saveData);
            return true;
        }
    }
}
