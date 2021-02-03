using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
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

        private Action<TrailModel> _callback;
        private PreloadMetaData _selectedComposition;

        public void Show(IEnumerable<PreloadMetaData> comps, Action<TrailModel> callback)
        {
            Show();
            _saberList.OnItemSelected += SaberSelected;
            _saberList.SetItems(comps);
            _callback = callback;
        }

        private void SaberSelected(ICustomListItem item)
        {
            _selectedComposition = (PreloadMetaData) item;
        }

        [UIAction("click-select")]
        private async void ClickSelect()
        {
            _saberList.OnItemSelected -= SaberSelected;
            Hide();

            if (_selectedComposition == null)
            {
                _callback?.Invoke(null);
                return;
            }

            var comp = await _mainAssetStore[_selectedComposition.AssetMetaPath.RelativePath];

            if (comp == null)
            {
                _callback?.Invoke(null);
                return;
            }

            var model = (CustomSaberModel) comp.GetLeft();
            _callback?.Invoke(model.GetColdTrail());
        }
    }
}
