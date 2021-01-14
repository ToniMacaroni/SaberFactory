using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
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
        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;

        [UIComponent("saber-list")] private readonly CustomList _saberList = null;

        [UIAction("#post-parse")]
        private async void Setup()
        {
            _saberList.OnItemSelected += SaberSelected;
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
            _saberList.SetItems(_mainAssetStore.GetAllModelCompositions().Select(c => c.GetLeft()));
        }

        private void SaberSelected(object item)
        {
            var saber = (CustomSaberModel) item;
            _editorInstanceManager.SetModelComposition(saber.ModelComposition);
        }
    }
}
