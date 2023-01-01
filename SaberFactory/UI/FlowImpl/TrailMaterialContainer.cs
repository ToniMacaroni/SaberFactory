using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPA.Config.Data;
using SaberFactory.AssetProperties;
using SaberFactory.Editor;
using SaberFactory.Instances;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class TrailMaterialContainer
{
    [Inject] private readonly SaberInstanceManager _saberInstanceManager = null;

    protected override Task DidOpen()
    {
        _saberInstanceManager.RegisterOnSaberCreated(Init);
        return base.DidOpen();
    }

    protected override Task DidClose()
    {
        _saberInstanceManager.OnSaberInstanceCreated -= Init;
        return base.DidClose();
    }

    private void Init(SaberInstance saberInstance)
    {
        _saberInstance = saberInstance;
        
        var trailData = saberInstance.GetTrailData(out _);

        if (trailData == null)
        {
            return;
        }

        var mat = new MaterialAssetProperty(trailData.Material.Material);
        
        var names = new List<string>();
        var props = new List<AssetProperty>();
        
        foreach (var prop in mat.FloatProperties)
        {
            names.Add(prop.Key);
            props.Add(prop.Value);
        }
        
        foreach (var prop in mat.BoolProperties)
        {
            names.Add(prop.Key);
            props.Add(prop.Value);
        }
        
        _propList.SetProps(props, names);
    }
    
    private SaberInstance _saberInstance;
}