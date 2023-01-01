using System;
using FlowUi.Runtime;
using HMUI;

namespace SaberFactory.UI.Flow;

public partial class PresetCell
{
    public IPresetInfo Preset => _presetInfo;

    public override void SetData(object data)
    {
        _presetInfo = (IPresetInfo)data;
        nameTextmesh.text = _presetInfo.Name;
        monitorToggle.SetIsOnWithoutNotify(_presetInfo.IsMonitorOnly);
        
        _buttonBinder.ClearBindings();
        _toggleBinder.ClearBindings();
        
        _buttonBinder.AddBinding(deleteButton, DeletePressed);
        _toggleBinder.AddBinding(monitorToggle, MonitorModeChanged);
    }

    public void SetMonitorModeToggleState(bool isOn)
    {
        monitorToggle.SetIsOnWithoutNotify(isOn);
    }

    private void MonitorModeChanged(bool isOn)
    {
        OnMonitorModeChanged?.Invoke(isOn);
    }

    private void DeletePressed()
    {
        OnDeletePressed?.Invoke();
    }

    private IPresetInfo _presetInfo;
    
    private readonly FlowButtonBinder _buttonBinder = new();
    private readonly FlowToggleBinder _toggleBinder = new();
}