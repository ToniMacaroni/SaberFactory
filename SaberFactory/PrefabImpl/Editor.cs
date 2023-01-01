using System;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models;
using UnityEngine;
using Zenject;

namespace SaberFactory.Prefab;

public partial class Editor : IInitializable
{
    [Inject] private readonly MainAssetStore _mainAssetStore = null;
    [Inject] private readonly SaberGrabController _saberGrabController = null;
    [Inject] private readonly SaberInstanceManager _saberInstanceManager = null;
    [Inject] private readonly SaberSet _saberSet = null;
    [Inject] private readonly PlayerDataModel _playerDataModel = null;
    [Inject] private readonly PluginConfig _pluginConfig = null;

    public enum ESaberPresenter
    {
        Pedestal,
        Hand
    }

    public ISaberPresenter Presenter { get; set; }
    
    public bool IsUnresponsive { get; private set; }

    public SaberInstance SaberInstance => _spawnedSaber;

    public void Initialize()
    {
        Presenter = pedestal;
    }

    public void Open()
    {
        _saberInstanceManager.OnModelCompositionSet += OnModelCompositionSet;
        _saberInstanceManager.Refresh();
    }

    public async Task Close()
    {
        _saberInstanceManager.SyncSabers();
        _saberInstanceManager.OnModelCompositionSet -= OnModelCompositionSet;
        _saberInstanceManager.DestroySaber();
        _spawnedSaber?.Destroy();
        await _saberSet.Save();
        _pluginConfig.Changed();
    }

    public void SetSaberPresenter(ESaberPresenter saberPresenter)
    {
        switch (saberPresenter)
        {
            case ESaberPresenter.Pedestal:
                Presenter = pedestal;
                break;
            case ESaberPresenter.Hand:
                Presenter = _saberGrabController;
                break;
        }
        
        _saberInstanceManager.Refresh();
    }

    private void OnModelCompositionSet(ModelComposition comp)
    {
        _spawnedSaber?.Destroy();

        _spawnedSaber = _saberInstanceManager.CreateSaber(_saberSet.LeftSaber, null);

        Presenter.Present(_spawnedSaber);

        _spawnedSaber.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme().saberAColor);

        _saberInstanceManager.RaiseSaberCreatedEvent();
        _saberInstanceManager.RaisePieceCreatedEvent();
    }

    public async Task ReloadSaber(RelativePath relativePath)
    {
        if (IsUnresponsive)
        {
            return;
        }

        IsUnresponsive = true;

        try
        {
            await _saberSet.Save();
            _saberInstanceManager.DestroySaber();
            await _mainAssetStore.Reload(relativePath);
            await _saberSet.Reload();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during reload:\n{e}");
        }

        IsUnresponsive = false;
    }

    public async Task ReloadAllSabers()
    {
        if (IsUnresponsive)
        {
            return;
        }

        IsUnresponsive = true;

        try
        {
            await _saberSet.Save();
            _saberInstanceManager.DestroySaber();
            await _mainAssetStore.ReloadAll();
            await _saberSet.Reload();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during reload all:\n{e}");
        }

        IsUnresponsive = false;
    }

    public void DeleteSaber(RelativePath relativePath)
    {
        if (IsUnresponsive)
        {
            return;
        }
        
        _saberInstanceManager.DestroySaber();
        _mainAssetStore.Delete(relativePath);
    }

    public async Task SetComposition(PreloadMetaData metaData)
    {
        var composition = await _mainAssetStore[metaData.AssetMetaPath.RelativePath];
        _saberInstanceManager.SetModelComposition(composition);
    }

    private SaberInstance _spawnedSaber;
}