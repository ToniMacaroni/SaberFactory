using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using IPA.Loader;
using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Loaders;
using SaberFactory.Misc;
using SaberFactory.Models;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.CustomSaber.Popups;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

namespace SaberFactory.UI.CustomSaber.Views
{
    internal class SaberSelectorView : SubView, INavigationCategoryView
    {
        private static readonly string MODELSABER_LINK = "https://modelsaber.com/Sabers/?pc";
        [UIComponent("choose-sort-popup")] private readonly ChooseSort _chooseSortPopup = null;
        [UIComponent("loading-popup")] private readonly LoadingPopup _loadingPopup = null;
        [UIComponent("message-popup")] private readonly MessagePopup _messagePopup = null;

        [UIComponent("saber-list")] private readonly CustomList _saberList = null;
        [UIComponent("toggle-favorite")] private readonly IconToggleButton _toggleButtonFavorite = null;

        [UIValue("global-saber-width-max")] private float GlobalSaberWidthMax => _pluginConfig.GlobalSaberWidthMax;

        [UIValue("download-sabers-popup")]
        private bool ShowDownloadSabersPopup
        {
            get => _showDownloadSabersPopup;
            set
            {
                _showDownloadSabersPopup = value;
                OnPropertyChanged();
            }
        }

        [UIValue("saber-width")]
        private float SaberWidth
        {
            set => SetSaberWidth(value);
            get => GetSaberWidth();
        }

        public bool IsReloading { get; private set; }

        [Inject] private readonly Editor.Editor _editor = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;

        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly PluginConfig _pluginConfig = null;
        [Inject] private readonly RemotePartRetriever _remotePartRetriever = null;
        [Inject] private readonly SaberFileWatcher _saberFileWatcher = null;
        [Inject] private readonly SaberSet _saberSet = null;
        private ModelComposition _currentComposition;
        private PreloadMetaData _currentPreloadMetaData;
        private ListItemDirectoryManager _dirManager;
        private string _listTitle;

        private bool _showDownloadSabersPopup;

        private ChooseSort.ESortMode _sortMode = ChooseSort.ESortMode.Name;

        public ENavigationCategory Category => ENavigationCategory.Saber;

        public override void DidOpen()
        {
            _editorInstanceManager.OnModelCompositionSet += CompositionDidChange;

            if (_pluginConfig.ReloadOnSaberUpdate)
            {
                _saberFileWatcher.OnSaberUpdate += OnSaberFileUpdate;
                _saberFileWatcher.Watch();
            }
        }

        public override void DidClose()
        {
            _editorInstanceManager.OnModelCompositionSet -= CompositionDidChange;

            if (_pluginConfig.ReloadOnSaberUpdate)
            {
                _saberFileWatcher.OnSaberUpdate -= OnSaberFileUpdate;
            }

            _saberFileWatcher.StopWatching();
        }

        [UIAction("#post-parse")]
        private async void Setup()
        {
            _dirManager = new ListItemDirectoryManager(_mainAssetStore.AdditionalCustomSaberFolders);
            _saberList.OnItemSelected += SaberSelected;
            _saberList.OnCategorySelected += DirectorySelected;
            _listTitle = "";
            _saberList.SetText(_listTitle);
            await LoadSabers();

            if (CommonHelpers.IsDate(null, 10) &&
                _pluginConfig.SpecialBackground &&
                _saberList.transform.GetChild(0) is {} child && child.GetComponent<ImageView>() is { } imageView)
            {
                imageView.sprite = Utilities.FindSpriteInAssembly("SaberFactory.Resources.UI.halloween_bg.png");
                imageView.overrideSprite = imageView.sprite;
                // ColorUtility.TryParseHtmlString("#c5c5c5", out var newBgColor);
                // imageView.color = newBgColor;
            }
        }

        private async void DirectorySelected(string dir)
        {
            _dirManager.Navigate(dir);
            _saberList.SetText(_dirManager.IsInRoot ? _listTitle : _dirManager.DirectoryString);
            _saberList.Deselect();

            await ShowSabers(true);
        }

        public async Task LoadSabers()
        {
            _loadingPopup.Show();
            await _remotePartRetriever.WaitForFinish();
            await _mainAssetStore.LoadAllMetaAsync(_pluginConfig.AssetType);
            await ShowSabers(false, 500);
            _loadingPopup.Hide();
        }

        private async Task ShowSabers(bool scrollToTop = false, int delay = 0)
        {
            // Get all metadata and sort by favorite
            var metaEnumerable = from meta in _mainAssetStore.GetAllMetaData()
                orderby meta.IsFavorite descending
                select meta;

            // Sort everything else by the selected sort mode
            switch (_sortMode)
            {
                case ChooseSort.ESortMode.Name:
                    metaEnumerable = metaEnumerable.ThenBy(x => x.ListName);
                    break;
                case ChooseSort.ESortMode.Date:
                    metaEnumerable = metaEnumerable.ThenByDescending(x => x.AssetMetaPath.File.LastWriteTime);
                    break;
                case ChooseSort.ESortMode.Size:
                    metaEnumerable = metaEnumerable.ThenByDescending(x => x.AssetMetaPath.File.Length);
                    break;
                case ChooseSort.ESortMode.Author:
                    metaEnumerable = metaEnumerable.ThenBy(x => x.ListAuthor);
                    break;
            }

            if (delay > 0)
            {
                await Task.Delay(delay);
            }

            var items = new List<ICustomListItem>(metaEnumerable);
            var loadedNames = items.Select(x => x.ListName).ToList();

            var addedDownloadables = 0;

            // Show downloadable sabers
            if (_pluginConfig.ShowDownloadableSabers && _remotePartRetriever.RetrievingStatus == RemotePartRetriever.Status.Success)
            {
                var idx = items.Count(x => x.IsFavorite);

                // if the saber isn't aleady present
                // add the downloadable option
                foreach (var remotePart in _remotePartRetriever.RemoteSabers)
                {
                    if (!loadedNames.Contains(remotePart.ListName))
                    {
                        items.Insert(idx, remotePart);
                        addedDownloadables++;
                    }
                }
            }

            // If there are only downloadable sabers present show the modelsaber hint
            ShowDownloadSabersPopup = items.Count() <= addedDownloadables;

            // Fill the saber list with the currently selected directory
            _saberList.SetItems(_dirManager.Process(items));

            _currentComposition = _editorInstanceManager.CurrentModelComposition;

            if (_currentComposition != null)
            {
                _saberList.Select(_mainAssetStore.GetMetaDataForComposition(_currentComposition)?.ListName, !scrollToTop);
                UpdatePedestalText(_currentComposition);
            }

            if (scrollToTop)
            {
                _saberList.ScrollTo(0);
            }


            UpdateUi();
        }

        public void OnSaberFileUpdate(string filename)
        {
            var currentSaberPath = _currentComposition.GetLeft().StoreAsset.RelativePath;
            if (!filename.Contains(currentSaberPath))
            {
                return;
            }

            Debug.Log("Saber got updated\n" + filename);
            if (File.Exists(filename))
            {
                ClickedReload();
            }
        }

        private async void SaberSelected(object item)
        {
            var reloadList = false;

            if (item is PreloadMetaData metaData)
            {
                _currentPreloadMetaData = metaData;
                var relativePath = PathTools.ToRelativePath(metaData.AssetMetaPath.Path);
                _currentComposition = await _mainAssetStore[relativePath];
            }
            else if (item is RemoteLocationPart remotePart)
            {
                _loadingPopup.Show($"Downloading {remotePart.ListName}...");
                var result = await remotePart.Download(CancellationToken.None);
                if (result == null || !result.Item1)
                {
                    _loadingPopup.Hide();
                    Logger.Error("Couldn't download remote saber: " + remotePart.RemoteLocation);
                    return;
                }

                reloadList = true;
                var relPath = result.Item2;
                _currentComposition =
                    await _mainAssetStore.CreateMetaData(
                        new AssetMetaPath(new FileInfo(Path.Combine(UnityGame.InstallPath, relPath))));
                _remotePartRetriever.RemoveSaber(remotePart);
                _loadingPopup.Hide();
            }
            else if (item is ModelComposition comp)
            {
                _currentComposition = comp;
            }
            else
            {
                return;
            }

            _editorInstanceManager.SetModelComposition(_currentComposition);
            UpdateUi();
            if (reloadList)
            {
                await ShowSabers();
            }

            _editor.FlashPedestal(new Color(0.24f, 0.77f, 1f));
        }

        private void CompositionDidChange(ModelComposition comp)
        {
            _currentComposition = comp;
            UpdatePedestalText(comp);
            _saberList.Select(comp);
        }

        private void UpdatePedestalText(ICustomListItem item)
        {
            var saberName = item.ListName;
            if (saberName.Length > 12)
            {
                saberName = saberName.Substring(0, 9) + "...";
            }
            _editor.SetPedestalText(0, saberName);
        }

        private void UpdateUi()
        {
            if (_currentComposition == null)
            {
                return;
            }

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
            if (_currentComposition == null)
            {
                return;
            }

            _currentComposition.SetFavorite(isOn);
            _currentPreloadMetaData?.SetFavorite(isOn);

            if (isOn)
            {
                _pluginConfig.AddFavorite(_currentComposition.GetLeft().StoreAsset.RelativePath);
            }
            else
            {
                _pluginConfig.RemoveFavorite(_currentComposition.GetLeft().StoreAsset.RelativePath);
            }

            await ShowSabers();
        }

        [UIAction("select-sort")]
        private void SelectSort()
        {
            _chooseSortPopup.Show(async sortMode =>
            {
                _sortMode = sortMode;
                await ShowSabers(_chooseSortPopup.ShouldScrollToTop);
            });
        }

        [UIAction("toggled-grab-saber")]
        private void ToggledGrabSaber(bool isOn)
        {
            _editor.IsSaberInHand = isOn;
        }

        [UIAction("clicked-reload")]
        private async void ClickedReload()
        {
            if (IsReloading)
            {
                return;
            }

            IsReloading = true;

            if (_currentComposition == null)
            {
                return;
            }

            _loadingPopup.Show();

            try
            {
                await _saberSet.Save();
                _editorInstanceManager.DestroySaber();
                await _mainAssetStore.Reload(_currentComposition.GetLeft().StoreAsset.RelativePath);
                await _saberSet.Load();
                await ShowSabers();
            }
            catch (Exception)
            { }

            _loadingPopup.Hide();
            IsReloading = false;
        }

        [UIAction("clicked-reloadall")]
        private async void ClickedReloadAll()
        {
            if (IsReloading)
            {
                return;
            }

            IsReloading = true;

            _loadingPopup.Show();

            try
            {
                await _saberSet.Save();
                _editorInstanceManager.DestroySaber();
                await _mainAssetStore.ReloadAll();
                await _saberSet.Load();
                await ShowSabers();
            }
            catch (Exception)
            { }

            _loadingPopup.Hide();

            IsReloading = false;
        }

        [UIAction("clicked-delete")]
        private async void ClickedDelete()
        {
            if (_currentComposition == null)
            {
                return;
            }

            var result = await _messagePopup.Show("Do you really want to delete this saber?", true);
            if (!result)
            {
                return;
            }

            _editorInstanceManager.DestroySaber();
            _mainAssetStore.Delete(_currentComposition.GetLeft().StoreAsset.RelativePath);
            await ShowSabers();
        }

        [UIAction("open-modelsaber")]
        private void OpenModelsaber()
        {
            Process.Start(MODELSABER_LINK);
        }
    }
}