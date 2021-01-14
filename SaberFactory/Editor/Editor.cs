using System;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.UI;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Editor
{
    internal class Editor : IInitializable, IDisposable
    {
        public bool IsActive { get; private set; }

        private readonly SiraLog _logger;
        private readonly SaberFactoryUI _saberFactoryUi;
        private readonly EditorInstanceManager _editorInstanceManager;
        private readonly SaberSet _saberSet;

        private readonly Pedestal _pedestal;
        private SaberInstance _spawnedSaber;

        private Editor(
            SiraLog logger,
            SaberFactoryUI saberFactoryUi,
            EditorInstanceManager editorInstanceManager,
            EmbeddedAssetLoader embeddedAssetLoader,
            SaberSet saberSet)
        {
            _logger = logger;
            _saberFactoryUi = saberFactoryUi;
            _editorInstanceManager = editorInstanceManager;
            _saberSet = saberSet;

            _pedestal = new Pedestal(embeddedAssetLoader);
        }

        public async void Initialize()
        {
            _saberFactoryUi.Initialize();

            // Create Pedestal
            var pos = new Vector3(-0.3f, 0, 1.0f);
            await _pedestal.Instantiate(pos, Quaternion.identity);
        }

        public void Dispose()
        {
            _saberFactoryUi.OnClosePressed -= Close;

            _pedestal.Destroy();
        }

        public void Open()
        {
            if (IsActive) return;

            _editorInstanceManager.OnModelCompositionSet += OnModelCompositionSet;

            _pedestal.IsVisible = true;

            if (_editorInstanceManager.CurrentModelComposition != null)
            {
                _editorInstanceManager.SetModelComposition(_editorInstanceManager.CurrentModelComposition);
            }

            _saberFactoryUi.Open();
            _saberFactoryUi.OnClosePressed += Close;

            IsActive = true;
        }

        public void Close()
        {
            if (!IsActive) return;

            _editorInstanceManager.OnModelCompositionSet -= OnModelCompositionSet;
            _editorInstanceManager.DestroySaber();
            _spawnedSaber?.Destroy();

            _pedestal.IsVisible = false;

            _saberFactoryUi.Close();
            _saberFactoryUi.OnClosePressed -= Close;
            
            IsActive = false;
        }

        private void OnModelCompositionSet(ModelComposition composition)
        {
            _spawnedSaber?.Destroy();
            _spawnedSaber = _editorInstanceManager.CreateSaber(_saberSet.LeftSaber, true, true);
            _spawnedSaber.SetParent(_pedestal.SaberContainerTransform);
        }
    }
}