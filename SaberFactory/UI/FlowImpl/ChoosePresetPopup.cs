using System;
using System.Linq;
using System.Threading.Tasks;
using FlowUi.Runtime;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class ChoosePresetPopup
{
    [Inject] private readonly PresetManager _presetManager = null;

    public IPresetInfo SelectedPreset { get; private set; }

    protected override void Awake()
    {
        _buttonBinder.AddBinding(newButton, NewPresetPressed);
        base.Awake();
    }

    public override Task<DialogResult> Show(string title)
    {
        _presetManager.DiscoverAllPresets();
        ReloadData();
        presetList.OnPresetSelected += PresetSelected;
        presetList.OnPresetDeleted += PresetDeleted;
        presetList.OnPresetMonitorToggled += PresetMonitorToggled;
        return base.Show(title);
    }

    public override void Hide()
    {
        presetList.OnPresetSelected -= PresetSelected;
        presetList.OnPresetDeleted -= PresetDeleted;
        presetList.OnPresetMonitorToggled -= PresetMonitorToggled;
        base.Hide();
    }

    public void ReloadData()
    {
        presetList.SetData(_presetManager.Presets.Cast<IPresetInfo>().ToList());
        presetList.SelectAsset(x=>x.Name==_presetManager.LoadedPreset.Name);
    }

    private void PresetSelected(IPresetInfo preset)
    {
        SelectedPreset = preset;
    }

    private void PresetDeleted(IPresetInfo preset)
    {
        preset.Delete();
        _presetManager.DiscoverAllPresets();
        ReloadData();
    }

    private void PresetMonitorToggled(IPresetInfo preset, bool isOn)
    {
        foreach (var p in _presetManager.Presets)
        {
            p.IsMonitorOnly = false;
        }
        
        preset.IsMonitorOnly = isOn;

        ReloadData();
    }

    private async void NewPresetPressed()
    {
        var result = await _newPresetModal.Show();

        if (result == NewPresetModal.Result.Ok)
        {
            await _presetManager.AddPreset(_newPresetModal.PresetName);
            ReloadData();
        }
    }
}