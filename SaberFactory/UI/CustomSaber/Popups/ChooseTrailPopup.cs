using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaberFactory.DataStore;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.UI.Lib;
using Zenject;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class ChooseTrailPopup : Popup
    {
        [Inject] private readonly MainAssetStore _mainAssetStore = null;

        [UIComponent("saber-list")] private readonly CustomList _saberList = null;

        private TrailModel _selectedTrailModel;
        private Action<TrailModel> _onSelectionChanged;

        public async void Show(IEnumerable<PreloadMetaData> comps, Action<TrailModel> onSelectionChanged)
        {
            _onSelectionChanged = onSelectionChanged;

            _ = Create(true);
            _saberList.OnItemSelected += SaberSelected;
            _saberList.SetItems(comps);

            await AnimateIn();
        }

        private async Task<TrailModel> GetTrail(PreloadMetaData metaData)
        {
            if (metaData is null) return null;

            var comp = await _mainAssetStore[metaData];

            if (comp?.GetLeft() is CustomSaberModel cs)
            {
                return cs.GrabTrail(true);
            }

            return null;
        }

        private async void SaberSelected(ICustomListItem item)
        {
            if (item is PreloadMetaData metaData)
            {
                _selectedTrailModel = await GetTrail(metaData);
                _onSelectionChanged?.Invoke(_selectedTrailModel);
            }
        }

        private async void Exit()
        {
            _onSelectionChanged = null;

            _saberList.OnItemSelected -= SaberSelected;
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
            _onSelectionChanged?.Invoke(null);
            Exit();
        }
    }
}
