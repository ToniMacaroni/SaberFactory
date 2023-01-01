using System;
using System.Threading.Tasks;
using FlowUi.Runtime;
using ModestTree;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class TrailMainContainer : FlowCategoryContainer
{
    [Inject] private readonly SaberInstanceManager _saberInstanceManager = null;

    protected override Task DidClose()
    {
        _saberInstanceManager.OnSaberInstanceCreated -= Init;
        _buttonBinder.ClearBindings();
        _sliderPropertyBinder.UnregisterAll();
        _togglePropertyBinder.UnregisterAll();
        _instanceTrailData = null;
        _saberInstance = null;
        return base.DidClose();
    }

    protected override Task DidOpen()
    {
        if (!_saberInstanceManager.RegisterOnSaberCreated(Init))
        {
            _flowBlocker.IsBlocked = true;
        }
        
        return base.DidOpen();
    }

    private void Init(SaberInstance saberInstance)
    {
        _saberInstance = saberInstance;
        
        var trailData = saberInstance.GetTrailData(out _);

        if (trailData == null)
        {
            _flowBlocker.IsBlocked = true;
            return;
        }
        
        _flowBlocker.IsBlocked = false;
        _instanceTrailData = trailData;

        _buttonBinder.ClearBindings();
        _buttonBinder.AddBinding(revertButton, RevertTrail);
        _buttonBinder.AddBinding(chooseTrailButton, ChooseTrail);
        
        _sliderPropertyBinder.UnregisterAll();
        _sliderPropertyBinder.Bind(lengthSlider, trailData.Length);
        _sliderPropertyBinder.Bind(widthSlider, trailData.Width);
        
        _sliderPropertyBinder.Bind(whitestepSlider, trailData.Whitestep);
        _sliderPropertyBinder.Bind(offsetSlider, trailData.Offset);
        
        _togglePropertyBinder.Bind(clampToggle, trailData.ClampTexture);
        _togglePropertyBinder.Bind(flipToggle, trailData.Flip);
        
        trailData.Flip.RegisterHandler(nameof(TrailMainContainer), _ =>
        {
            _parentViewController.Cast<SFTrailVC>().CreateTrail(saberInstance);
        });
    }

    private async void ChooseTrail()
    {
        if (_instanceTrailData == null)
        {
            return;
        }

        var result = await chooseTrailPopup.Show(_instanceTrailData.TrailModel, SetExternalTrail);

        if (result == ChooseTrailPopup.Result.Original)
        {
            RevertTrail();
        }
    }

    private void SetExternalTrail(TrailModel otherModel)
    {
        if (_instanceTrailData == null)
        {
            return;
        }

        if (!_saberInstance.Model.GetCustomSaber(out var cs))
        {
            return;
        }

        cs.TrailModel = cs.TrailModel.TransformInto(otherModel);
                
        _saberInstanceManager.Refresh();
    }

    private void RevertTrail()
    {
        if (_saberInstance == null)
        {
            return;
        }
        
        if(_saberInstance.Model.GetCustomSaber(out var cs))
        {
            cs.ResetTrail();
        }
        
        _saberInstanceManager.Refresh();
    }

    private void Awake()
    {
        _flowBlocker = new FlowBlocker(transform.GetCanvasGroup());
    }

    private SaberInstance _saberInstance;
    private InstanceTrailData _instanceTrailData;
    
    private readonly SliderPropertyBinder _sliderPropertyBinder = new();
    private readonly TogglePropertyBinder _togglePropertyBinder = new();
    private readonly FlowButtonBinder _buttonBinder = new();
    private FlowBlocker _flowBlocker;
}