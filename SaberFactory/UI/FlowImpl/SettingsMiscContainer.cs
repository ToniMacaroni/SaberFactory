using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SaberFactory.AssetProperties;
using SaberFactory.Configuration;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class SettingsMiscContainer
{
    [Inject] private readonly PluginConfig _pluginConfig = null;

    protected override Task DidOpen()
    {
        if (_assetProperties.Count < 1)
        {
            RegisterProps();
        }
        
        propList.SetProps(_assetProperties.Select(x=>x.prop).ToList(), _assetProperties.Select(x=>x.propName).ToList());
        
        return base.DidOpen();
    }

    private void RegisterProps()
    {
        _assetProperties.Clear();
        
        var bgOpacity = new FloatProperty(_pluginConfig.SaberSelectionBackgroundOpacity, 0, 1, 0.05f);
        bgOpacity.RegisterHandler(nameof(SettingsMiscContainer), val =>
        {
            _pluginConfig.SaberSelectionBackgroundOpacity = val;
        });
        _assetProperties.Add(("Background opacity", bgOpacity));

        var bgBlur = new FloatProperty(_pluginConfig.SaberSelectionBackgroundBlurAmount, 0, 8, 1);
        bgBlur.RegisterHandler(nameof(SettingsMiscContainer), val =>
        {
            _pluginConfig.SaberSelectionBackgroundBlurAmount = (int)val;
        });
        _assetProperties.Add(("Background blur", bgBlur));
    }
    
    private readonly List<(string propName, AssetProperty prop)> _assetProperties = new();
    
}