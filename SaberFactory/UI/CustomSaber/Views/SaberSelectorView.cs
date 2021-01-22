using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Editor;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.Saving;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class SaberSelectorView : SubView
    {
        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly SaberSet _saberSet = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;
        [Inject] private readonly PluginConfig _pluginConfig = null;

        [UIComponent("saber-list")] private readonly CustomList _saberList = null;
        [UIComponent("toggle-favorite")] private readonly IconToggleButton _toggleButtonFavorite = null;
        [UIComponent("loading-popup")] private readonly LoadingPopup _loadingPopup = null;

        [UIValue("mod-enabled")]
        private bool IsModEnabled
        {
            get => _pluginConfig.Enabled;
            set => _pluginConfig.Enabled = value;
        }

        [UIValue("saber-width")]
        private float _saberWidth
        {
            set => SetSaberWidth(value);
            get => GetSaberWidth();
        }

        private ModelComposition _currentComposition;

        public override void DidOpen()
        {
            base.DidOpen();
            _editorInstanceManager.OnModelCompositionSet += CompositionDidChange;
        }

        public override void DidClose()
        {
            base.DidClose();
            _editorInstanceManager.OnModelCompositionSet -= CompositionDidChange;
        }

        [UIAction("#post-parse")]
        private async void Setup()
        {
            //_modToggle.Value = _pluginConfig.Enabled;
            _saberList.OnItemSelected += SaberSelected;
            await LoadSabers();
        }

        public async Task LoadSabers()
        {
            _loadingPopup.Show();
            await _mainAssetStore.LoadAll();
            ShowSabers();
            _loadingPopup.Hide();
        }

        private void ShowSabers()
        {
            var sabers = 
                from comp in _mainAssetStore.GetAllModelCompositions()
                orderby comp.IsFavorite descending 
                select comp;

            _saberList.SetItems(sabers);

            _currentComposition = _editorInstanceManager.CurrentModelComposition;
            _saberList.Select(_currentComposition);
            UpdateUi();
        }

        private void SaberSelected(object item)
        {
            _currentComposition = (ModelComposition) item;
            _editorInstanceManager.SetModelComposition(_currentComposition);
            UpdateUi();
        }

        private void CompositionDidChange(ModelComposition comp)
        {
            _currentComposition = comp;
            _saberList.Select(comp);
        }

        private void UpdateUi()
        {
            if (_currentComposition == null) return;
            _toggleButtonFavorite.SetState(_currentComposition.IsFavorite, false);
        }

        [UIAction("toggled-favorite")]
        private void ToggledFavorite(bool isOn)
        {
            if (_currentComposition == null) return;
            _currentComposition.SetFavorite(isOn);

            if (isOn)
            {
                _pluginConfig.AddFavorite(_currentComposition.GetLeft().StoreAsset.Path);
            }
            else
            {
                _pluginConfig.RemoveFavorite(_currentComposition.GetLeft().StoreAsset.Path);
            }

            ShowSabers();
        }

        private void SetSaberWidth(float width)
        {
            _editorInstanceManager.CurrentSaber?.SetSaberWidth(width);
        }

        private float GetSaberWidth()
        {
            return _editorInstanceManager.CurrentSaber?.Model.SaberWidth ?? 1;
        }

        [UIAction("clicked-reload")]
        private async void ClickedReload()
        {
            if (_currentComposition == null) return;
            _loadingPopup.Show();
            _saberSet.Save();
            _editorInstanceManager.DestroySaber();
            await _mainAssetStore.Reload(_currentComposition.GetLeft().StoreAsset.Path);
            await _saberSet.Load("default");
            ShowSabers();
            _loadingPopup.Hide();
        }

        [UIAction("clicked-reloadall")]
        private async void ClickedReloadAll()
        {
            _loadingPopup.Show();
            _saberSet.Save();
            _editorInstanceManager.DestroySaber();
            await _mainAssetStore.ReloadAll();
            await _saberSet.Load("default");
            ShowSabers();
            _loadingPopup.Hide();
        }

        [UIAction("clicked-delete")]
        private void ClickedDelete()
        {
            if (_currentComposition == null) return;
            _editorInstanceManager.DestroySaber();
            _mainAssetStore.Delete(_currentComposition.GetLeft().StoreAsset.Path);
            ShowSabers();
        }
    }
}
