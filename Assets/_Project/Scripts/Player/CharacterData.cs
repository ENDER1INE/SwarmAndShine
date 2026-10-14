using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Player
{
    public enum CharacterUnlockType
    {
        Default,
        MetaCurrency,
        WavesCompleted
    }

    [CreateAssetMenu(fileName = "CharacterData", menuName = "SAS/Player/Character")]
    public class CharacterData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private CharacterBaseStatsData _baseStats;
        [SerializeField] private WeaponData _startingWeapon;
        [Tooltip("Animation set for the player sprite. Leave empty to keep the controller set in the scene.")]
        [SerializeField] private RuntimeAnimatorController _animatorController;
        [Tooltip("Bonuses and penalties that define the character, applied at the start of every run.")]
        [SerializeField] private List<StatModifierData> _startModifiers = new();

        [Header("Unlock")]
        [SerializeField] private CharacterUnlockType _unlockType = CharacterUnlockType.Default;
        [SerializeField] private int _unlockCost;
        [Tooltip("For WavesCompleted: waves that must be completed within a single run.")]
        [SerializeField] private int _requiredWavesCompleted;

        public string Id => _id;
        public string DisplayName => _displayName;
        public CharacterBaseStatsData BaseStats => _baseStats;
        public WeaponData StartingWeapon => _startingWeapon;
        public RuntimeAnimatorController AnimatorController => _animatorController;
        public IReadOnlyList<StatModifierData> StartModifiers => _startModifiers;
        public CharacterUnlockType UnlockType => _unlockType;
        public int UnlockCost => _unlockCost;
        public int RequiredWavesCompleted => _requiredWavesCompleted;
    }
}
