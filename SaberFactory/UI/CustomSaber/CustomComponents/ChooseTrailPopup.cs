using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [UIAction("#post-parse")]
        private void Setup()
        {
            gameObject.SetActive(false);
            _saberList.OnItemSelected += SaberSelected;
        }

        public void Show(IList<ModelComposition> comps, Action<TrailModel> callback)
        {
            _saberList.SetItems(comps);
            _callback = callback;
            Show();
        }

        private void SaberSelected(ICustomListItem item)
        {
            _selectedComposition = (ModelComposition) item;
        }

        [UIAction("click-select")]
        private void ClickSelect()
        {
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
