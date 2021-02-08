using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class SaberSelectorView : SubView, INavigationCategoryView
    {
        public ENavigationCategory Category => ENavigationCategory.Saber;

        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly SaberSet _saberSet = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;
        [Inject] private readonly PluginConfig _pluginConfig = null;

        [UIComponent("saber-list")] private readonly CustomList _saberList = null;
        [UIComponent("toggle-favorite")] private readonly IconToggleButton _toggleButtonFavorite = null;
        [UIComponent("loading-popup")] private readonly LoadingPopup _loadingPopup = null;

        [UIValue("saber-width")]
        private float _saberWidth
        {
            set => SetSaberWidth(value);
            get => GetSaberWidth();
        }

        private ModelComposition _currentComposition;
        private PreloadMetaData _currentPreloadMetaData;

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
            _saberList.OnItemSelected += SaberSelected;
            await LoadSabers();
        }

        public async Task LoadSabers()
        {
            _loadingPopup.Show();
            await _mainAssetStore.LoadAllMetaAsync(_pluginConfig.AssetType);
            await ShowSabers(500);
            _loadingPopup.Hide();
        }

        private async Task ShowSabers(int delay = 0)
        {
            var metas = from meta in _mainAssetStore.GetAllMetaData()
                orderby meta.IsFavorite descending
                select meta;

            if(delay>0) await Task.Delay(delay);

            _saberList.SetItems(metas);

            _currentComposition = _editorInstanceManager.CurrentModelComposition;

            if (_currentComposition != null)
            {
                _saberList.Select(_mainAssetStore.GetMetaDataForComposition(_currentComposition)?.ListName);
            }

            UpdateUi();
        }

        private async void SaberSelected(object item)
        {
            if (item is PreloadMetaData metaData)
            {
                _currentPreloadMetaData = metaData;
                var relativePath = PathTools.ToRelativePath(metaData.AssetMetaPath.Path);
                _currentComposition = await _mainAssetStore[relativePath];
            }
            else
            {
                _currentComposition = (ModelComposition) item;
            }

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

        private void SetSaberWidth(float width)
        {
            _editorInstanceManager.CurrentSaber?.SetSaberWidth(width);
        }

        private float GetSaberWidth()
        {
            return _editorInstanceManager.CurrentSaber?.Model.SaberWidth ?? 1;
        }

        [UIAction("toggled-favorite")]
        private async void ToggledFavorite(bool isOn)
        {
            if (_currentComposition == null) return;
            _currentComposition.SetFavorite(isOn);
            _currentPreloadMetaData?.SetFavorite(isOn);

            if (isOn)
            {
                _pluginConfig.AddFavorite(_currentComposition.GetLeft().StoreAsset.Path);
            }
            else
            {
                _pluginConfig.RemoveFavorite(_currentComposition.GetLeft().StoreAsset.Path);
            }

            await ShowSabers();
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
            await ShowSabers();
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
            await ShowSabers();
            _loadingPopup.Hide();
        }

        [UIAction("clicked-delete")]
        private async void ClickedDelete()
        {
            if (_currentComposition == null) return;
            _editorInstanceManager.DestroySaber();
            _mainAssetStore.Delete(_currentComposition.GetLeft().StoreAsset.Path);
            await ShowSabers();
        }
    }
}
