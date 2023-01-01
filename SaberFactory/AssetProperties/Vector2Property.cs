using UnityEngine;

namespace SaberFactory.AssetProperties;

public class Vector2Property : AssetProperty<Vector2>
{
    public Vector2Property(Vector2 val) : base(val)
    {
    }
    
    public static implicit operator Vector2(Vector2Property prop)
    {
        return prop.Value;
    }
}