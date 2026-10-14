using System;
using System.Collections.Generic;
using Game.Core;
using Game.Player;
using Game.Run;

namespace Game.Meta
{
    public class CharacterUnlockService : IDisposable
    {
        private readonly SaveData _saveData;
        private readonly MetaWallet _wallet;
        private readonly ISaveStorage _storage;
        private readonly IReadOnlyList<CharacterData> _characters;

        public CharacterUnlockService(
            SaveData saveData,
            MetaWallet wallet,
            ISaveStorage storage,
            IReadOnlyList<CharacterData> characters)
        {
            _saveData = saveData;
            _wallet = wallet;
            _storage = storage;
            _characters = characters;
            GameEvents.Subscribe<RunEndedEvent>(OnRunEnded);
        }

        public bool IsUnlocked(CharacterData character)
        {
            return character.UnlockType == CharacterUnlockType.Default || _saveData.IsCharacterUnlocked(character.Id);
        }

        public CharacterPurchaseResult TryPurchase(CharacterData character)
        {
            if (IsUnlocked(character))
            {
                return CharacterPurchaseResult.AlreadyUnlocked;
            }

            if (character.UnlockType != CharacterUnlockType.MetaCurrency)
            {
                return CharacterPurchaseResult.NotPurchasable;
            }

            if (!_wallet.TrySpend(character.UnlockCost))
            {
                return CharacterPurchaseResult.NotEnoughCurrency;
            }

            Unlock(character);
            return CharacterPurchaseResult.Success;
        }

        public void Dispose()
        {
            GameEvents.Unsubscribe<RunEndedEvent>(OnRunEnded);
        }

        private void OnRunEnded(RunEndedEvent evt)
        {
            foreach (CharacterData character in _characters)
            {
                bool earnedByAchievement = character.UnlockType == CharacterUnlockType.WavesCompleted
                                           && evt.WavesCompleted >= character.RequiredWavesCompleted;
                if (earnedByAchievement && !IsUnlocked(character))
                {
                    Unlock(character);
                }
            }
        }

        private void Unlock(CharacterData character)
        {
            _saveData.UnlockCharacter(character.Id);
            _storage.Save(_saveData);
            GameEvents.Publish(new CharacterUnlockedEvent(character.Id));
        }
    }
}
