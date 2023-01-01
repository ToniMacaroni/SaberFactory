using System;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.DataStore;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class ChooseTrailPopup
{
    [Inject] private readonly MainAssetStore _mainAssetStore;
    [Inject] private readonly SaberListController _saberListController;
    
    public async Task<Result> Show(TrailModel originalTrail, Action<TrailModel> trailSelectedCallback)
    {
        _taskCompletionSource = new TaskCompletionSource<Result>();
        _trailSelectedCallback = trailSelectedCallback;
        _originalTrail = originalTrail;
        saberList.OnAssetSelected += OnAssetSelected;
        saberList.OnFolderSelected += FolderSelected;
        Show();
        await PresentSabers();
        return await _taskCompletionSource.Task;
    }
    
    private void Cleanup()
    {
        saberList.OnAssetSelected -= OnAssetSelected;
        saberList.OnFolderSelected -= FolderSelected;
        _trailSelectedCallback = null;
    }

    protected override void Awake()
    {
        base.Awake();
        _buttonBinder.AddBinding(originalButton, PressedOriginal);
    }

    public override void PressedOk()
    {
        Cleanup();
        Hide();
        _taskCompletionSource.SetResult(Result.Ok);
    }

    public override void PressedCancel()
    {
        _trailSelectedCallback?.Invoke(_originalTrail);
        Cleanup();
        Hide();
        _taskCompletionSource.SetResult(Result.Cancel);
    }

    public void PressedOriginal()
    {
        Cleanup();
        Hide();
        _taskCompletionSource.SetResult(Result.Original);
    }

    private async void FolderSelected(IFolderInfo folder)
    {
        _saberListController.ChangeDirectory(folder.Name);
        await PresentSabers();
    }

    private async void OnAssetSelected(IAssetInfo asset)
    {
        ModelComposition comp = await asset.GetModelComposition(_mainAssetStore);

        if (comp != null && comp.GetLeft() is CustomSaberModel cs)
        {
            var trail = cs.GrabTrail(true);
            if (trail != null)
            {
                _trailSelectedCallback?.Invoke(trail);
            }
        }
    }

    private async Task PresentSabers()
    {
        await _mainAssetStore.LoadAllMetaAsync(EAssetTypeConfiguration.CustomSaber);
        var fetchResult = _saberListController.GetSabers();
        saberList.SetData((fetchResult.Folders.ToList(), fetchResult.Sabers));
    }

    private TaskCompletionSource<Result> _taskCompletionSource;
    private Action<TrailModel> _trailSelectedCallback;
    private TrailModel _originalTrail;

    public enum Result
    {
        Ok,
        Cancel,
        Original
    }
}