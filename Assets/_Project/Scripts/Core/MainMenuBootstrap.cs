using Game.Meta;
using Game.Player;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class MainMenuBootstrap : MonoBehaviour
    {
        [SerializeField] private string _gameSceneName = "Game";
        [SerializeField] private CharacterCatalog _characterCatalog;
        [SerializeField] private MetaUpgradeCatalog _metaUpgradeCatalog;
        [SerializeField] private MainMenuView _mainMenuView;

        [Header("Debug")]
        [Tooltip("Meta currency granted by the debug button. The button exists only in the Editor and development builds.")]
        [SerializeField] private int _debugGrantAmount = 100;

        private MetaServices _metaServices;

        private void Awake()
        {
            _metaServices = MetaServices.CreateDefault(_characterCatalog.Characters);
            _mainMenuView.Init(
                _metaServices,
                _metaUpgradeCatalog.Upgrades,
                _characterCatalog.Characters,
                StartRun,
                GrantDebugCurrency);
        }

        private void OnDestroy()
        {
            _metaServices?.Dispose();
        }

        private void GrantDebugCurrency()
        {
            _metaServices.Wallet.Add(_debugGrantAmount);
            _metaServices.Storage.Save(_metaServices.SaveData);
        }

        private void StartRun()
        {
            SceneManager.LoadScene(_gameSceneName);
        }
    }
}
