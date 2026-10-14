using System;
using System.Collections.Generic;
using Game.Meta;
using Game.Player;
using UnityEngine;

namespace Game.UI
{
    public class CharactersTabView : MonoBehaviour
    {
        [SerializeField] private CharacterRowView _rowPrefab;
        [SerializeField] private Transform _rowContainer;

        private readonly List<CharacterRowView> _rows = new();
        private IReadOnlyList<CharacterData> _characters;
        private CharacterUnlockService _unlockService;
        private CharacterSelection _selection;
        private MetaWallet _wallet;
        private Action _onChanged;

        public void Init(
            IReadOnlyList<CharacterData> characters,
            CharacterUnlockService unlockService,
            CharacterSelection selection,
            MetaWallet wallet,
            Action onChanged)
        {
            _characters = characters;
            _unlockService = unlockService;
            _selection = selection;
            _wallet = wallet;
            _onChanged = onChanged;

            foreach (CharacterData character in characters)
            {
                CharacterRowView row = Instantiate(_rowPrefab, _rowContainer);
                row.Init(character, OnActionClicked);
                _rows.Add(row);
            }
        }

        public void Refresh()
        {
            for (int i = 0; i < _rows.Count; i++)
            {
                CharacterData character = _characters[i];
                _rows[i].Render(character, _selection.GetStatus(character), _wallet.Balance);
            }
        }

        private void OnActionClicked(CharacterData character)
        {
            switch (_selection.GetStatus(character))
            {
                case CharacterMenuStatus.Unlocked:
                    _selection.TrySelect(character);
                    break;
                case CharacterMenuStatus.PurchasableWithCurrency:
                    if (_unlockService.TryPurchase(character) == CharacterPurchaseResult.Success)
                    {
                        _selection.TrySelect(character);
                    }

                    break;
            }

            _onChanged();
        }
    }
}
