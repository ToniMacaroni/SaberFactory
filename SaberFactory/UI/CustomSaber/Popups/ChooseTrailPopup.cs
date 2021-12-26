using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using CustomSaber;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.UI.CustomSaber.Views;
using SaberFactory.UI.Lib;
using Zenject;

namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class ChooseTrailPopup : Popup
    {
        [UIComponent("saber-list")] private readonly CustomList _saberList = null;
        [Inject] private readonly MainAssetStore _mainAssetStore = null;

        private List<PreloadMetaData> _comps;

        private SaberListDirectoryManager _dirManager;
        private Action<TrailModel, List<CustomTrail>> _onSelectionChanged;

        private (TrailModel, List<CustomTrail>) _selectedTrailModel;

        public async void Show(IEnumerable<PreloadMetaData> comps, Action<TrailModel, List<CustomTrail>> onSelectionChanged)
        {
            _dirManager ??= new SaberListDirectoryManager(_mainAssetStore.AdditionalCustomSaberFolders);

            _onSelectionChanged = onSelectionChanged;

            _ = Create(true);
            _saberList.OnItemSelected += SaberSelected;
            _saberList.OnCategorySelected += OnDirectorySelected;

            _comps = comps.ToList();

            _saberList.SetItems(_dirManager.Process(_comps));

            await AnimateIn();
        }

        private void OnDirectorySelected(string path)
        {
            _dirManager.Navigate(path);
            _saberList.SetText(_dirManager.IsInRoot ? "Saber-Os" : _dirManager.DirectoryString);
            _saberList.Deselect();

            _saberList.SetItems(_dirManager.Process(_comps));
        }

        private async Task<(TrailModel, List<CustomTrail>)> GetTrail(PreloadMetaData metaData)
        {
            if (metaData is null)
            {
                return default;
            }

            var comp = await _mainAssetStore[metaData];

            if (comp?.GetLeft() is CustomSaberModel cs)
            {
                return (cs.GrabTrail(true), SaberHelpers.GetTrails(cs.Prefab));
            }

            return default;
        }

        private async void SaberSelected(ICustomListItem item)
        {
            if (item is PreloadMetaData metaData)
            {
                _selectedTrailModel = await GetTrail(metaData);
                _onSelectionChanged?.Invoke(_selectedTrailModel.Item1, _selectedTrailModel.Item2);
            }
        }

        public async void Exit()
        {
            if (!IsOpen)
            {
                return;
            }

            _onSelectionChanged = null;

            _saberList.OnItemSelected -= SaberSelected;
            _saberList.OnCategorySelected -= OnDirectorySelected;
            await Hide(true);
        }

        [UIAction("click-select")]
        private void ClickSelect()
        {
            Exit();
        }

        [UIAction("click-original")]
        private void ClickOriginal()
        {
            _onSelectionChanged?.Invoke(null, null);
            Exit();
        }

        [UIAction("click-cancel")]
        private void ClickCancel()
        {
            Exit();
        }
    }
}