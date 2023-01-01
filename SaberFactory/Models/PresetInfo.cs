using System;
using System.IO;
using SaberFactory.UI.Flow;

namespace SaberFactory.Models;

public class PresetInfo : IPresetInfo
{
    public string Name { get; }
    
    public FileInfo File { get; }

    public bool IsMonitorOnly
    {
        get => _isMonitorOnly;
        set
        {
            _isMonitorOnly = value;
            MonitorOnlyChanged(value);
        }
    }
    
    public event Action<bool> OnMonitorOnlyChanged;

    public PresetInfo(FileInfo file, bool isMonitorOnly = false)
    {
        File = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
        _isMonitorOnly = isMonitorOnly;
    }

    public void Delete()
    {
        File.Delete();
    }

    private void MonitorOnlyChanged(bool isOn)
    {
        OnMonitorOnlyChanged?.Invoke(isOn);
    }

    private bool _isMonitorOnly;
}