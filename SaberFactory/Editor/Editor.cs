using System;
using System.Threading;
using System.Threading.Tasks;
using IPA.Loader;
using SaberFactory.Configuration;
using SaberFactory.HarmonyPatches;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.UI;
using SaberFactory.UI.Lib;
using SiraUtil.Logging;
using SiraUtil.Tools;
using Tweening;
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

        private readonly BaseUiComposition _baseUiComposition;

        private readonly EditorInstanceManager _editorInstanceManager;

        private readonly SiraLog _logger;
        private readonly MenuSaberProvider _menuSaberProvider;

        private readonly Pedestal _pedestal;
        private readonly PlayerDataModel _playerDataModel;
        private readonly PluginConfig _pluginConfig;
        private readonly SaberGrabController _saberGrabController;
        private readonly SaberSet _saberSet;
        private readonly SFLogoAnim _sfLogoAnim;
        private bool _isFirstActivation = true;
        private bool _isSaberInHand;
        private SaberInstance _spawnedSaber;
        private readonly PluginMetadata _metaData;
        private readonly TimeTweeningManager _tweeningManager;

        private Editor(
            SiraLog logger,
            PluginConfig pluginConfig,
            BaseUiComposition baseUiComposition,
            EditorInstanceManager editorInstanceManager,
            EmbeddedAssetLoader embeddedAssetLoader,
            SaberSet saberSet,
            PlayerDataModel playerDataModel,
            SaberGrabController saberGrabController,
            MenuSaberProvider menuSaberProvider,
            PluginDirectories pluginDirs,
            [Inject(Id = nameof(SaberFactory))]PluginMetadata metadata,
            TimeTweeningManager tweeningManager)
        {
            _logger = logger;
            _metaData = metadata;
            _tweeningManager = tweeningManager;
            _pluginConfig = pluginConfig;
            _baseUiComposition = baseUiComposition;
            _editorInstanceManager = editorInstanceManager;
            _saberSet = saberSet;
            _playerDataModel = playerDataModel;
            _saberGrabController = saberGrabController;
            _menuSaberProvider = menuSaberProvider;

            _pedestal = new Pedestal(pluginDirs.SaberFactoryDir.GetFile("pedestal"));
            _sfLogoAnim = new SFLogoAnim(embeddedAssetLoader);

            Instance = this;
            GameplaySetupViewPatch.EntryEnabled = _pluginConfig.ShowGameplaySettingsButton;
        }

        public void Dispose()
        {
            Instance = null;
            _baseUiComposition.OnClosePressed -= Close;

            _pedestal.Destroy();
        }

        public async void Initialize()
        {
            _baseUiComposition.Initialize();

            // Create Pedestal
            var pos = new Vector3(0.3f, 0, 0.9f);
            await _pedestal.Instantiate(pos, Quaternion.Euler(0, 25, 0));
            SetPedestalText(1, "<color=#ffffff70>SF v"+_metaData.HVersion+"</color>");
#if PAT
            SetPedestalText(2, "<color=#ffffff80>Patreon ♥</color>");
#endif
            SetupGlobalShaderVars();
        }

        public async void Open()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            _editorInstanceManager.OnModelCompositionSet += OnModelCompositionSet;

            _pedestal.IsVisible = true;

            _editorInstanceManager.Refresh();

            _baseUiComposition.Open();

            if (_isFirstActivation && _pluginConfig.RuntimeFirstLaunch)
            {
                await _sfLogoAnim.Instantiate(new Vector3(-1, -0.04f, 2), Quaternion.Euler(0, 45, 0));
                await _sfLogoAnim.PlayAnim();
            }

            _baseUiComposition.OnClosePressed += Close;

            _menuSaberProvider.RequestSaberVisiblity(false);

            _isFirstActivation = false;
        }

        public void Close()
        {
            Close(false);
        }

        public void Close(bool instant)
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;

            _editorInstanceManager.SyncSabers();
            _editorInstanceManager.OnModelCompositionSet -= OnModelCompositionSet;
            _editorInstanceManager.DestroySaber();
            _spawnedSaber?.Destroy();

            _pedestal.IsVisible = false;

            _baseUiComposition.Close(instant);
            _baseUiComposition.OnClosePressed -= Close;

            _saberGrabController.ShowHandle();

            _menuSaberProvider.RequestSaberVisiblity(true);
        }

        public void SetPedestalText(int line, string text)
        {
            _pedestal.SetText(line, text);
        }

        public void FlashPedestal(Color color)
        {
            _tweeningManager.KillAllTweens(_pedestal.SaberContainerTransform);
            _tweeningManager.AddTween(new FloatTween(1, 0, f =>
            {
                _pedestal.SetLedColor(color.ColorWithAlpha(f));
            }, 1, EaseType.InCubic), _pedestal.SaberContainerTransform);
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
            {
                await AnimationHelper.AsyncAnimation(0.3f, CancellationToken.None, t => { parent.localScale = new Vector3(t, t, t); });
            }
        }

        private void SetupGlobalShaderVars()
        {
            var scheme = _playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme();
            Shader.SetGlobalColor(MaterialProperties.UserColorLeft, scheme.saberAColor);
            Shader.SetGlobalColor(MaterialProperties.UserColorRight, scheme.saberBColor);
        }
    }
}