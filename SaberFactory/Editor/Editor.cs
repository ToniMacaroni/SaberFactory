using System;
using System.Threading;
using System.Threading.Tasks;
using SaberFactory.Configuration;
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
    /// Class for managing the presentation of the saber factory editor
    /// </summary>
    internal class Editor : IInitializable, IDisposable
    {
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

        private readonly SiraLog _logger;
        private readonly PluginConfig _pluginConfig;
        private readonly SaberFactoryUI _saberFactoryUi;
        private readonly EditorInstanceManager _editorInstanceManager;
        private readonly SaberSet _saberSet;
        private readonly PlayerDataModel _playerDataModel;
        private readonly SaberGrabController _saberGrabController;

        private readonly Pedestal _pedestal;
        private readonly SFLogoAnim _sfLogoAnim;
        private SaberInstance _spawnedSaber;
        private bool _isFirstActivation = true;
        private bool _isSaberInHand;

        private Editor(
            SiraLog logger,
            PluginConfig pluginConfig,
            SaberFactoryUI saberFactoryUi,
            EditorInstanceManager editorInstanceManager,
            EmbeddedAssetLoader embeddedAssetLoader,
            SaberSet saberSet,
            PlayerDataModel playerDataModel,
            SaberGrabController saberGrabController)
        {
            _logger = logger;
            _pluginConfig = pluginConfig;
            _saberFactoryUi = saberFactoryUi;
            _editorInstanceManager = editorInstanceManager;
            _saberSet = saberSet;
            _playerDataModel = playerDataModel;
            _saberGrabController = saberGrabController;

            _pedestal = new Pedestal(embeddedAssetLoader);
            _sfLogoAnim = new SFLogoAnim(embeddedAssetLoader);
        }

        public async void Initialize()
        {
            _saberFactoryUi.Initialize();

            // Create Pedestal
            var pos = new Vector3(0.3f, 0, 0.9f);
            await _pedestal.Instantiate(pos, Quaternion.Euler(0, 25, 0));
        }

        public void Dispose()
        {
            _saberFactoryUi.OnClosePressed -= Close;

            _pedestal.Destroy();
        }

        public async void Open()
        {
            if (IsActive) return;

            _editorInstanceManager.OnModelCompositionSet += OnModelCompositionSet;

            _pedestal.IsVisible = true;

            if (_editorInstanceManager.CurrentModelComposition == null)
            {
                // TODO: Use Part or Custom Saber
                var piece = _saberSet.LeftSaber.PieceCollection[AssetTypeDefinition.CustomSaber];
                if (piece != null)
                {
                    _editorInstanceManager.SetModelComposition(piece.ModelComposition);
                }
            }
            else
            {
                _editorInstanceManager.Refresh();
            }

            _saberFactoryUi.Open();

            if (_isFirstActivation && _pluginConfig.RuntimeFirstLaunch)
            {
                await _sfLogoAnim.Instantiate(new Vector3(-1, -0.04f, 2), Quaternion.Euler(0, 45, 0));
                await _sfLogoAnim.PlayAnim();
            }

            _saberFactoryUi.OnClosePressed += Close;

            IsActive = true;
            _isFirstActivation = false;
        }

        public void Close()
        {
            if (!IsActive) return;

            _editorInstanceManager.SyncSabers();
            _editorInstanceManager.OnModelCompositionSet -= OnModelCompositionSet;
            _editorInstanceManager.DestroySaber();
            _spawnedSaber?.Destroy();

            _pedestal.IsVisible = false;

            _saberFactoryUi.Close();
            _saberFactoryUi.OnClosePressed -= Close;

            _saberGrabController.ShowHandle();
            
            IsActive = false;
        }

        private async void OnModelCompositionSet(ModelComposition composition)
        {
            _spawnedSaber?.Destroy();

            var parent = IsSaberInHand ? _saberGrabController.GrabContainer : _pedestal.SaberContainerTransform;

            _spawnedSaber = _editorInstanceManager.CreateSaber(_saberSet.LeftSaber, parent);

            if (IsSaberInHand)
            {
                _spawnedSaber.CreateTrail();
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
                await AnimationHelper.AsyncAnimation(0.3f, CancellationToken.None, t =>
                {
                    parent.localScale = new Vector3(t, t, t);
                });
            }

        }
    }
}