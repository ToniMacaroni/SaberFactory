using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Models;
using UnityEngine;
using Zenject;

namespace SaberFactory;

public class PresetManager : IInitializable
{
    public const string PresetExtension = "sfps";

    public PresetInfo LoadedPreset => _saberSet.LoadedPreset;
    
    public List<PresetInfo> Presets { get; } = new();

    private PresetManager(
        PluginDirectories pluginDirectories,
        PluginConfig pluginConfig,
        SaberSet saberSet)
    {
        _pluginConfig = pluginConfig;
        _saberSet = saberSet;
        _presetDirectory = pluginDirectories.PresetDir;
    }

    public void Initialize()
    {
        DiscoverAllPresets();
        var preset = GetPresetByFilename(_pluginConfig.LoadedPreset) ?? GetDefaultPreset();

        if (preset == null)
        {
            return;
        }
        
        _ = _saberSet.Load(preset);
    }

    public void DiscoverAllPresets()
    {
        Presets.Clear();

        var files = _presetDirectory.GetFiles("*."+PresetExtension);
        foreach (var file in files)
        {
            var isMonitor = file.Name == _pluginConfig.MonitorPreset;
            var preset = new PresetInfo(file, isMonitor);
            
            preset.OnMonitorOnlyChanged += isOn =>
            {
                if (isOn)
                {
                    _pluginConfig.MonitorPreset = preset.File.Name;
                }
                else
                {
                    if(_pluginConfig.MonitorPreset == preset.File.Name)
                    {
                        _pluginConfig.MonitorPreset = string.Empty;
                    }
                }
            };
            
            Presets.Add(preset);
        }
    }

    public PresetInfo GetPreset(string presetName)
    {
        return Presets.FirstOrDefault(x => x.Name == presetName);
    }
    
    public PresetInfo GetPresetByFilename(string filename)
    {
        return Presets.FirstOrDefault(x => x.File.Name == filename);
    }

    public PresetInfo GetDefaultPreset()
    {
        return GetPreset("default");
    }

    public bool TryGetMonitorPreset(out PresetInfo monitorPreset)
    {
        monitorPreset = Presets.FirstOrDefault(x => x.IsMonitorOnly = true);
        return monitorPreset != null;
    }

    public bool CreateMonitorSaberSet(out SaberSet monitorSaberSet)
    {
        monitorSaberSet = null;
        if (!TryGetMonitorPreset(out var monitorPreset))
        {
            return false;
        }

        //monitorSaberSet = new SaberSet();
        return true;
    }

    public async Task<bool> AddPreset(string presetName)
    {
        if (GetPreset(presetName) != null)
        {
            return false;
        }
        
        var file = _presetDirectory.GetFile(presetName + "." + PresetExtension);
        var preset = new PresetInfo(file);
        await _saberSet.Save(preset);
        _pluginConfig.LoadedPreset = preset.File.Name;
        DiscoverAllPresets();
        return true;
    }

    public async Task LoadPreset(PresetInfo preset)
    {
        await _saberSet.Load(preset);
        _pluginConfig.LoadedPreset = preset.File.Name;
    }

    private readonly PluginConfig _pluginConfig;
    private readonly SaberSet _saberSet;
    private readonly DirectoryInfo _presetDirectory;
}