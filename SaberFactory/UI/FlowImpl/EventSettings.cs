using SaberFactory.AssetProperties;
using SaberFactory.Configuration;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public class EventSettings
{
    [Inject] private readonly PluginConfig _config = null;
    
    public BoolProperty DisableAll = new(true);

    public BoolProperty OnSlice = new(true);
    public BoolProperty OnComboBreak = new(true);
    public BoolProperty MultiplierUp = new(true);
    public BoolProperty SaberStartColliding = new(true);
    public BoolProperty SaberStopColliding = new(true);
    public BoolProperty OnLevelStart = new(true);
    public BoolProperty OnLevelFail = new(true);
    public BoolProperty OnLevelEnded = new(true);
    public BoolProperty OnBlueLightOn = new(true);
    public BoolProperty OnRedLightOn = new(true);
    public BoolProperty OnComboChanged = new(true);
    public BoolProperty OnAccuracyChanged = new(true);
}