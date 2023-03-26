using Heck.SettingsSetter;
using Zenject;

namespace SaberFactory.Configuration;

internal class SaberSettableSettings : IInitializable
{
    private const string GroupName = "_saberFactory";
        
    public SettableSetting<bool> RelativeTrailMode;
        
    public void Initialize()
    {
        RegisterSetting(ref RelativeTrailMode, "_relativeTrailMode");
    }

    private void RegisterSetting<T>(ref SettableSetting<T> setting, string fieldName) where T : struct
    {
        setting = new SettableSetting<T>(GroupName, fieldName);
        SettingSetterSettableSettingsManager.RegisterSettableSetting(setting.GroupName, setting.FieldName, setting);
    }
}