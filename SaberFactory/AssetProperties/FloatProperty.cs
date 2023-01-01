using SaberFactory.Misc;
using UnityEngine;

namespace SaberFactory.AssetProperties;

public class FloatProperty : AssetProperty<float>
{
    public float? Min;
    public float? Max;
    public float? Increment;

    public bool HasRange => Min.HasValue && Max.HasValue;
    public bool HasIncrement => Increment.HasValue;

    public FloatProperty(float val, float? min = null, float? max = null, float? increment = null) : base(val)
    {
        Min = min;
        Max = max;
        Increment = increment;
    }
    
    public FloatProperty(float val, ValueRange<float> valueRange) : base(val)
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

    public static implicit operator float(FloatProperty prop)
    {
        return prop.Value;
    }
}