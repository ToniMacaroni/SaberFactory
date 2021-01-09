using System;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.DataStore;
using SaberFactory.Editor;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class SaberSelectorView : SubView
    {
        #region Custom Components

        private SaberSelector _saberSelector;

        #endregion

        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;

        [UIComponent("list-container")] private readonly Transform _listContainer = null;

        [UIAction("#post-parse")]
        private async void Setup()
        {
            _saberSelector =
                CreateComponent<SaberSelector>(_listContainer, new SaberSelector.SaberSelectorParams("Sabers"));
            _saberSelector.OnItemSelected += SaberSelected;
            await LoadSabers();
        }

        public async Task LoadSabers()
        {
            if (_mainAssetStore.IsLoading)
            {
                _mainAssetStore.RegisterOneShotCallback(ShowSabers);
            }
            else
            {
                await _mainAssetStore.LoadAllCustomSabersAsync(true);
                ShowSabers();
            }
        }

        private void ShowSabers()
        {
            _saberSelector.SetItems(_mainAssetStore.GetAllModelCompositions().Select(c => c.GetLeft()));
        }

        private void SaberSelected(object item)
        {
            var saber = (CustomSaberModel) item;
            _editorInstanceManager.SetModelComposition(saber.ModelComposition);
        }
    }
}
