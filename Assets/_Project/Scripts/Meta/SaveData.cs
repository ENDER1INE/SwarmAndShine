using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Meta
{
    [Serializable]
    public class SaveData
    {
        public const int CurrentVersion = 1;

        [SerializeField] private int _version = CurrentVersion;
        [SerializeField] private int _metaCurrency;
        [SerializeField] private List<UpgradeLevelEntry> _upgradeLevels = new();
        [SerializeField] private List<string> _unlockedCharacterIds = new();

        public int Version => _version;

        public int MetaCurrency
        {
            get => _metaCurrency;
            set => _metaCurrency = Mathf.Max(0, value);
        }

        public int GetUpgradeLevel(string upgradeId)
        {
            int index = FindUpgradeIndex(upgradeId);
            return index < 0 ? 0 : _upgradeLevels[index].Level;
        }

        public void SetUpgradeLevel(string upgradeId, int level)
        {
            int index = FindUpgradeIndex(upgradeId);
            if (index >= 0)
            {
                _upgradeLevels[index] = new UpgradeLevelEntry(upgradeId, level);
                return;
            }

            _upgradeLevels.Add(new UpgradeLevelEntry(upgradeId, level));
        }

        public bool IsCharacterUnlocked(string characterId)
        {
            return _unlockedCharacterIds.Contains(characterId);
        }

        public void UnlockCharacter(string characterId)
        {
            if (!IsCharacterUnlocked(characterId))
            {
                _unlockedCharacterIds.Add(characterId);
            }
        }

        private int FindUpgradeIndex(string upgradeId)
        {
            for (int i = 0; i < _upgradeLevels.Count; i++)
            {
                if (_upgradeLevels[i].UpgradeId == upgradeId)
                {
                    return i;
                }
            }

            return -1;
        }

        // JsonUtility cannot serialize Dictionary, so upgrade levels are stored as id/level pairs.
        [Serializable]
        private struct UpgradeLevelEntry
        {
            [SerializeField] private string _upgradeId;
            [SerializeField] private int _level;

            public string UpgradeId => _upgradeId;
            public int Level => _level;

            public UpgradeLevelEntry(string upgradeId, int level)
            {
                _upgradeId = upgradeId;
                _level = level;
            }
        }
    }
}
