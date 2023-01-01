using System;
using System.Linq;
using System.Threading.Tasks;
using FlowUi.Runtime;
using HMUI;
using IPA.Utilities;
using ModestTree;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class SFTrailVC : FlowViewController
{
    [Inject] private readonly SaberInstanceManager _saberInstanceManager = null;
    [Inject] private readonly TrailPreviewer _trailPreviewer = null;
    [Inject] private readonly PlayerDataModel _playerDataModel = null;

    protected override async Task Setup()
    {
        navigationSegmentedControl.SetTexts(Enum.GetNames(typeof(ETrailSettingsCategory)));
        await SelectCategory((int)ETrailSettingsCategory.Main);
    }

    protected override Task DidOpen()
    {
        _saberInstanceManager.RegisterOnSaberCreated(CreateTrail);
        navigationSegmentedControl.didSelectCellEvent += CategoryCellSelected;

        return Task.CompletedTask;
    }
    
    protected override Task DidClose()
    {
        _saberInstanceManager.OnSaberInstanceCreated -= CreateTrail;
        navigationSegmentedControl.didSelectCellEvent -= CategoryCellSelected;
        _trailPreviewer.Destroy();

        return Task.CompletedTask;
    }

    public void CreateTrail(SaberInstance saberInstance)
    {
        _trailPreviewer.Destroy();
        
        var trailData = saberInstance?.GetTrailData(out _);

        if (trailData == null)
        {
            return;
        }

        if (saberInstance.TrailHandler != null)
        {
            // Saber is in hand
        }
        else
        {
            _trailPreviewer.Create(saberInstance.GameObject.transform.parent, trailData, true);
            _trailPreviewer.SetColor(_playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme().saberAColor);
        }
    }
}