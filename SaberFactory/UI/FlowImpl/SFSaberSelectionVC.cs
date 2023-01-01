using System;
using System.Linq;
using System.Threading.Tasks;
using FlowUi.Runtime;
using SaberFactory.AssetProperties;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class SFSaberSelectionVC
{
    [Inject] private readonly MainAssetStore _mainAssetStore = null;
    [Inject] private readonly SaberInstanceManager _saberInstanceManager = null;
    [Inject] private readonly SaberListController _saberListController = null;
    [Inject] private readonly Prefab.Editor _editor = null;
    [Inject] private readonly SaberFactoryMainUi _mainUi = null;
    [Inject] private readonly SaberSet _saberSet = null;
    [Inject] private readonly PluginConfig _pluginConfig = null;
    [Inject] private readonly PresetManager _presetManager = null;
    
    protected override async Task Setup()
    {
        await PresentSabers();

        _buttonBinder.AddBinding(reloadAllButton, ClickedReloadAll);
        _buttonBinder.AddBinding(reloadButton, ClickedReload);
        _buttonBinder.AddBinding(presetsButton, PresetsPressed);
        
        _toggleBinder.AddBinding(holdToggle, ChangeHoldState);
    }

    protected override Task DidOpen()
    {
        saberList.OnAssetSelected += OnAssetSelected;
        saberList.OnAssetFavoriteRequested += OnAssetFavoriteRequested;
        saberList.OnAssetDeleteRequested += OnAssetDeleteRequested;
        saberList.OnFolderSelected += FolderSelected;
        _saberInstanceManager.RegisterOnSaberCreated(OnSaberInstanceCreated);
        UpdateBackground();
        return Task.CompletedTask;
    }

    protected override Task DidClose()
    {
        saberList.OnAssetSelected -= OnAssetSelected;
        saberList.OnAssetFavoriteRequested -= OnAssetFavoriteRequested;
        saberList.OnAssetDeleteRequested -= OnAssetDeleteRequested;
        saberList.OnFolderSelected -= FolderSelected;
        _saberInstanceManager.OnSaberInstanceCreated -= OnSaberInstanceCreated;
        return Task.CompletedTask;
    }

    private async Task PresentSabers()
    {
        await _mainAssetStore.LoadAllMetaAsync(EAssetTypeConfiguration.CustomSaber);
        var fetchResult = _saberListController.GetSabers();
        saberList.SetData((fetchResult.Folders.ToList(), fetchResult.Sabers));

        if (_saberSet.LeftSaber?.GetCustomSaber(out var cs)??false)
        {
            var didSelect = saberList.SelectAsset(x=>x.Name == cs.ModelComposition.Name, callbackTable: false, scroll: true);
            if (!didSelect)
            {
                saberList.ClearSelection();
            }
            
            _currentMetaData = _mainAssetStore.GetMetaDataForComposition(cs.ModelComposition);
            SetBackground(cs.ModelComposition);
        }
    }

    public void UpdateBackground()
    {
        if (_saberSet.LeftSaber?.GetCustomSaber(out var cs)??false)
        {
            SetBackground(cs.ModelComposition);
        }
    }
    
    private async void FolderSelected(IFolderInfo folder)
    {
        _saberListController.ChangeDirectory(folder.Name);
        await PresentSabers();
    }

    private async void OnAssetSelected(IAssetInfo asset)
    {
        if (asset is PreloadMetaData metaData)
        {
            await _editor.SetComposition(metaData);
            _currentMetaData = metaData;
        }
        
        SetBackground(asset);
    }

    private async void OnAssetFavoriteRequested(IAssetInfo asset)
    {
        var metaData = _mainAssetStore.FindByAssetInfo(asset);
        if (metaData == null)
        {
            Debug.LogError("Something went wrong favoriting asset");
            return;
        }

        var comp = await _mainAssetStore.GetCompositionByMeta(metaData);

        var shouldBeOn = !_pluginConfig.IsFavorite(metaData.AssetMetaPath.RelativePath.Path);

        comp.SetFavorite(shouldBeOn);
        metaData.SetFavorite(shouldBeOn);

        if (shouldBeOn)
        {
            _pluginConfig.AddFavorite(metaData.AssetMetaPath.RelativePath);
        }
        else
        {
            _pluginConfig.RemoveFavorite(metaData.AssetMetaPath.RelativePath);
        }

        await PresentSabers();
    }

    private async void OnAssetDeleteRequested(IAssetInfo asset)
    {
        var result = await _mainUi.ShowMessagebox("Do you really want to delete the current saber?", "Confirm");
        if (result == FlowCustomDialog.DialogResult.Ok)
        {
            var metaData = _mainAssetStore.FindByAssetInfo(asset);
            if (metaData == null)
            {
                Debug.LogError("Something went wrong deleting asset");
                return;
            }
            
            _editor.DeleteSaber(metaData.AssetMetaPath.RelativePath);
            await PresentSabers();
        }
    }

    private KawaseBlurRendererSO.KernelSize GetBlurAmount(int strength)
    {
        if (strength > 8)
        {
            strength = 8;
        }

        strength--;
        strength = Mathf.Clamp(strength, 0, 7);
        return (KawaseBlurRendererSO.KernelSize)strength;
    }

    private void SetBackground(IAssetInfo asset)
    {
        if (asset.Cover)
        {
            fadedBGImage.color = Color.white.ColorWithAlpha(_pluginConfig.SaberSelectionBackgroundOpacity);
            
            if (_pluginConfig.SaberSelectionBackgroundBlurAmount == 0)
            {
                fadedBGImage.sprite = asset.Cover;
            }
            else
            {
                imageBlur.SetImage(asset.Cover.texture, GetBlurAmount(_pluginConfig.SaberSelectionBackgroundBlurAmount));
            }
            fadedBGImage.enabled = true;
        }
        else
        {
            fadedBGImage.enabled = false;
        }
    }

    private void OnSaberInstanceCreated(SaberInstance saberInstance)
    {
        RegisterSliderProps(saberInstance);
    }

    private void ChangeHoldState(bool state)
    {
        _editor.SetSaberPresenter(state?Prefab.Editor.ESaberPresenter.Hand:Prefab.Editor.ESaberPresenter.Pedestal);
    }

    private async void ClickedDelete()
    { 
        if (_currentMetaData == null)
        {
            return;
        }
        
        var result = await _mainUi.ShowMessagebox("Do you really want to delete the current saber?", "Confirm");
        if (result == FlowCustomDialog.DialogResult.Ok)
        {
            _editor.DeleteSaber(_currentMetaData.AssetMetaPath.RelativePath);
            await PresentSabers();
        }
    }

    private async void ClickedReload()
    {
        if (_currentMetaData == null)
        {
            return;
        }
        
        await _editor.ReloadSaber(_currentMetaData.AssetMetaPath.RelativePath);
        await PresentSabers();
    }

    private async void PresetsPressed()
    {
        var result = await choosePresetPopup.Show("Presets");
        if (result == FlowCustomDialog.DialogResult.Ok
            && choosePresetPopup.SelectedPreset != null
            && choosePresetPopup.SelectedPreset.Name != _saberSet.LoadedPreset.Name)
        {
            await _saberSet.Save();
            await _presetManager.LoadPreset((PresetInfo)choosePresetPopup.SelectedPreset);
            await PresentSabers();
        }
    }

    private async void ClickedReloadAll()
    {
        await _editor.ReloadAllSabers();
        await PresentSabers();
    }

    private void RegisterSliderProps(SaberInstance saberInstance)
    {
        _sliderPropBinder.Bind(saberWidthSlider, saberInstance.SaberWidth);
    }

    private PreloadMetaData _currentMetaData;
    private readonly SliderPropertyBinder _sliderPropBinder = new();

}