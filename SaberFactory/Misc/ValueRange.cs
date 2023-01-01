using System;

namespace SaberFactory.Misc;

public struct ValueRange<T> where T : struct
{
    public T? Min;
    public T? Max;
    public T? Increment;

    public ValueRange(T? min, T? max, T? increment = null)
    {
        Min = min;
        Max = max;
        Increment = increment;
    }
}