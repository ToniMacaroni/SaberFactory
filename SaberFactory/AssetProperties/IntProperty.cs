using SaberFactory.Misc;
using UnityEngine;

namespace SaberFactory.AssetProperties;

public class IntProperty : AssetProperty<int>
{
    public int? Min;
    public int? Max;
    public int? Increment;
    
    public bool HasRange => Min.HasValue && Max.HasValue;
    public bool HasIncrement => Increment.HasValue;
    
    public IntProperty(int val, int? min = null, int? max = null, int? increment = null) : base(val)
    {
        Min = min;
        Max = max;
        Increment = increment;
    }
    
    public IntProperty(int val, ValueRange<int> valueRange) : base(val)
    {
        Min = valueRange.Min;
        Max = valueRange.Max;
        Increment = valueRange.Increment;
    }

    protected override void DidChangeValue()
    {
        if (HasRange)
        {
            _value = Mathf.Clamp(_value, Min!.Value, Max!.Value);
        }
    }
    
    public static implicit operator int(IntProperty prop)
    {
        return prop.Value;
    }
}