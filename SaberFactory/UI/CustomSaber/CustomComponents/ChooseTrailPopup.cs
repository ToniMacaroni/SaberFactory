using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.UI.Lib;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class ChooseTrailPopup : Popup
    {
        [UIComponent("saber-list")] private readonly CustomList _saberList = null;

        private Action<TrailModel> _callback;
        private ModelComposition _selectedComposition;

        public void Show(IList<ModelComposition> comps, Action<TrailModel> callback)
        {
            Show();
            _saberList.OnItemSelected += SaberSelected;
            _saberList.SetItems(comps);
            _callback = callback;
        }

        private void SaberSelected(ICustomListItem item)
        {
            _selectedComposition = (ModelComposition) item;
        }

        [UIAction("click-select")]
        private void ClickSelect()
        {
            _saberList.OnItemSelected -= SaberSelected;
            Hide();

            if (_selectedComposition == null)
            {
                _callback?.Invoke(null);
                return;
            }

            var model = (CustomSaberModel) _selectedComposition.GetLeft();
            _callback?.Invoke(model.GetColdTrail());
        }
    }
}
