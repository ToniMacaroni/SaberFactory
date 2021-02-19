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
        /// <summary>
        /// Called when the selection is final and the
        /// popup is being closed
        /// </summary>
        public Action<TrailModel> OnSelected;

        /// <summary>
        /// Called when the selected item in the list changes
        /// </summary>
        public Action<TrailModel> OnSelectionChanged;

        [Inject] private readonly MainAssetStore _mainAssetStore = null;

        [UIComponent("saber-list")] private readonly CustomList _saberList = null;

        private TrailModel _selectedTrailModel;

        public void Show(IEnumerable<PreloadMetaData> comps)
        {
            Show();
            _saberList.OnItemSelected += SaberSelected;
            _saberList.SetItems(comps);
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
                OnSelectionChanged?.Invoke(_selectedTrailModel);
            }
        }

        [UIAction("click-select")]
        private void ClickSelect()
        {
            _saberList.OnItemSelected -= SaberSelected;
            Hide();

            OnSelected?.Invoke(_selectedTrailModel);
        }

        [UIAction("click-original")]
        private void ClickOriginal()
        {
            _saberList.OnItemSelected -= SaberSelected;
            Hide();

            OnSelected?.Invoke(null);
        }
    }
}
