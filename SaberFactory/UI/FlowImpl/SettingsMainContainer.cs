using System.Threading.Tasks;
using SaberFactory.AssetProperties;
using SaberFactory.Configuration;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class SettingsMainContainer
{
    [Inject] private readonly PluginConfig _config = null;
    
    protected override Task Setup()
    {
        _enableSaberFactory = new BoolProperty(_config.Enabled);
        _enableSaberFactory.RegisterHandler(nameof(SettingsMainContainer), val =>
        {
            _config.Enabled = val;
        });
        
        return base.Setup();
    }

    protected override Task DidOpen()
    {
        _togglePropertyBinder.Bind(globalSwitchToggle, _enableSaberFactory);
        return base.DidOpen();
    }

    protected override Task DidClose()
    {
        _togglePropertyBinder.UnregisterAll();
        return base.DidClose();
    }

    private BoolProperty _enableSaberFactory;

    private readonly TogglePropertyBinder _togglePropertyBinder = new();
}