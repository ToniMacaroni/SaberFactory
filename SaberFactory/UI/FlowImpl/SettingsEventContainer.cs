using System;
using System.Reflection;
using System.Threading.Tasks;
using FlowUi.Runtime;
using HMUI;
using IPA.Utilities;
using SaberFactory.AssetProperties;
using SaberFactory.Configuration;
using TMPro;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class SettingsEventContainer
{
    [Inject] private readonly PluginConfig _config = null;

    private EventSettings EventSettings => _config.EventSettings;

    protected override Task Setup()
    {
        _blocker = new FlowBlocker(gridContainer.GetComponent<CanvasGroup>());

        _togglePropertyBinder.Bind(globalSwitchToggle, EventSettings.DisableAll);
        
        foreach (var field in typeof(EventSettings).GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if(field.Name == "DisableAll" || field.FieldType != typeof(BoolProperty))
            {
                continue;
            }
            
            var toggle = Instantiate(eventTogglePrefab, gridContainer, false);
            toggle.GetComponentInChildren<TextMeshProUGUI>().text = field.Name;
            var val = (BoolProperty)field.GetValue(EventSettings);
            _togglePropertyBinder.Bind(toggle.GetComponent<ToggleWithCallbacks>(), val);
        }
        
        EventSettings.DisableAll.RegisterHandler(nameof(SettingsEventContainer), IsDisabledChanged);
        IsDisabledChanged(EventSettings.DisableAll.Value);
        
        return base.Setup();
    }

    protected override Task DidOpen()
    {
        return base.DidOpen();
    }

    private void IsDisabledChanged(bool val)
    {
        _blocker.IsBlocked = val;
    }

    private readonly TogglePropertyBinder _togglePropertyBinder = new();
    private FlowBlocker _blocker;
}