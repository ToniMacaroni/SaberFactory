using Heck.SettingsSetter;
using UnityEngine;
using Zenject;

namespace SaberFactory.Installers
{
    internal class HeckSettableSettingsInitializer : IInitializable

    {
        public static bool Initialized { get; private set; }
        
        private const string GroupName = "_saberFactory";

        public SettableSetting<bool> RelativeTrailMode;

        public HeckSettableSettingsInitializer(SaberSettableSettings saberSettableSettings)
        {
            _saberSettableSettings = saberSettableSettings;
        }

        public void Initialize()
        {
            if (Initialized)
                return;
            
            RegisterSetting(ref RelativeTrailMode, _saberSettableSettings.RelativeTrailMode, "_relativeTrailMode");
            Initialized = true;
        }

        private void RegisterSetting<T>(ref SettableSetting<T> setting, SaberSettableSettings.SettableSettingAbstraction<T> abstraction,
            string fieldName)
            where T : struct
        {
            var newSetting = new SettableSetting<T>(GroupName, fieldName);
            SettingSetterSettableSettingsManager.RegisterSettableSetting(newSetting.GroupName, newSetting.FieldName, newSetting);
            newSetting.ValueChanged += () => abstraction.Value = newSetting.Value;
            setting = newSetting;
        }

        private readonly SaberSettableSettings _saberSettableSettings;
    }
}