using System;
using System.Collections.Generic;
using System.IO;
using Game.Player;
using UnityEngine;

namespace Game.Meta
{
    // Composition root for everything that lives in the save file. Each scene's bootstrap
    // creates its own instance, so scenes share state only through the save file.
    public class MetaServices : IDisposable
    {
        private const string SaveFileName = "save.json";

        public MetaServices(ISaveStorage storage, IReadOnlyList<CharacterData> characters)
        {
            Storage = storage;
            SaveData = storage.Load();
            Wallet = new MetaWallet(SaveData);
            Upgrades = new MetaUpgradeService(SaveData, Wallet, storage);
            CharacterUnlocks = new CharacterUnlockService(SaveData, Wallet, storage, characters);
            CharacterSelection = new CharacterSelection(SaveData, storage, CharacterUnlocks, characters);
        }

        public ISaveStorage Storage { get; }
        public SaveData SaveData { get; }
        public MetaWallet Wallet { get; }
        public MetaUpgradeService Upgrades { get; }
        public CharacterUnlockService CharacterUnlocks { get; }
        public CharacterSelection CharacterSelection { get; }

        public static MetaServices CreateDefault(IReadOnlyList<CharacterData> characters)
        {
            var storage = new JsonFileSaveStorage(Path.Combine(Application.persistentDataPath, SaveFileName));
            return new MetaServices(storage, characters);
        }

        public void Dispose()
        {
            CharacterUnlocks.Dispose();
        }
    }
}
