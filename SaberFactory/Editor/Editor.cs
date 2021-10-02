using System;
using System.Threading;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.HarmonyPatches;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.UI;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Editor
{
    /// <summary>
    ///     Class for managing the presentation of the saber factory editor
    /// </summary>
    internal class Editor : IInitializable, IDisposable
    {
        public static Editor Instance;
        public bool IsActive { get; private set; }

        public bool IsSaberInHand
        {
            get => _isSaberInHand;
            set
            {
                _isSaberInHand = value;
                _editorInstanceManager.Refresh();
            }
        }

        private readonly EditorInstanceManager _editorInstanceManager;

        private readonly SiraLog _logger;
        private readonly MenuSaberProvider _menuSaberProvider;

        private readonly Pedestal _pedestal;
        private readonly PlayerDataModel _playerDataModel;
        private readonly PluginConfig _pluginConfig;
        private readonly SaberFactoryUI _saberFactoryUi;
        private readonly SaberGrabController _saberGrabController;
        private readonly SaberSet _saberSet;
        private readonly SFLogoAnim _sfLogoAnim;
        private bool _isFirstActivation = true;
        private bool _isSaberInHand;
        private SaberInstance _spawnedSaber;

        private Editor(
            SiraLog logger,
            PluginConfig pluginConfig,
            SaberFactoryUI saberFactoryUi,
            EditorInstanceManager editorInstanceManager,
            EmbeddedAssetLoader embeddedAssetLoader,
            SaberSet saberSet,
            PlayerDataModel playerDataModel,
            SaberGrabController saberGrabController,
            MenuSaberProvider menuSaberProvider,
            PluginDirectories pluginDirs)
        {
            _logger = logger;
            _pluginConfig = pluginConfig;
            _saberFactoryUi = saberFactoryUi;
            _editorInstanceManager = editorInstanceManager;
            _saberSet = saberSet;
            _playerDataModel = playerDataModel;
            _saberGrabController = saberGrabController;
            _menuSaberProvider = menuSaberProvider;

            _pedestal = new Pedestal(embeddedAssetLoader, pluginDirs.SaberFactoryDir.GetFile("pedestal"));
            _sfLogoAnim = new SFLogoAnim(embeddedAssetLoader);

            Instance = this;
            GameplaySetupViewPatch.EntryEnabled = _pluginConfig.ShowGameplaySettingsButton;
        }

        public void Dispose()
        {
            Instance = null;
            _saberFactoryUi.OnClosePressed -= Close;

            _pedestal.Destroy();
        }

        public async void Initialize()
        {
            _saberFactoryUi.Initialize();

            // Create Pedestal
            var pos = new Vector3(0.3f, 0, 0.9f);
            await _pedestal.Instantiate(pos, Quaternion.Euler(0, 25, 0));
        }

        public async void Open()
        {
            if (IsActive) return;
            IsActive = true;

            _editorInstanceManager.OnModelCompositionSet += OnModelCompositionSet;

            _pedestal.IsVisible = true;

            _editorInstanceManager.Refresh();

            _saberFactoryUi.Open();

            if (_isFirstActivation && _pluginConfig.RuntimeFirstLaunch)
            {
                await _sfLogoAnim.Instantiate(new Vector3(-1, -0.04f, 2), Quaternion.Euler(0, 45, 0));
                await _sfLogoAnim.PlayAnim();
            }

            _saberFactoryUi.OnClosePressed += Close;

            _menuSaberProvider.RequestSaberVisiblity(false);

            _isFirstActivation = false;
        }

        public void Close()
        {
            Close(false);
        }

        public void Close(bool instant)
        {
            if (!IsActive) return;
            IsActive = false;

            _editorInstanceManager.SyncSabers();
            _editorInstanceManager.OnModelCompositionSet -= OnModelCompositionSet;
            _editorInstanceManager.DestroySaber();
            _spawnedSaber?.Destroy();

            _pedestal.IsVisible = false;

            _saberFactoryUi.Close(instant);
            _saberFactoryUi.OnClosePressed -= Close;

            _saberGrabController.ShowHandle();

            _menuSaberProvider.RequestSaberVisiblity(true);
        }

        private async void OnModelCompositionSet(ModelComposition composition)
        {
            _spawnedSaber?.Destroy();

            var parent = IsSaberInHand ? _saberGrabController.GrabContainer : _pedestal.SaberContainerTransform;

            _spawnedSaber = _editorInstanceManager.CreateSaber(_saberSet.LeftSaber, parent);

            if (IsSaberInHand)
            {
                _spawnedSaber.CreateTrail(true);
                _saberGrabController.HideHandle();
            }
            else
            {
                _saberGrabController.ShowHandle();
            }

            _spawnedSaber.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme().saberAColor);

            _editorInstanceManager.RaiseSaberCreatedEvent();
            _editorInstanceManager.RaisePieceCreatedEvent();

            await Task.Yield();

            if (_pluginConfig.AnimateSaberSelection)
                await AnimationHelper.AsyncAnimation(0.3f, CancellationToken.None, t => { parent.localScale = new Vector3(t, t, t); });
        }
    }
}