using UnityEngine;

namespace SaberFactory.AssetProperties;

public class Vector3Property : AssetProperty<Vector3>
{
    public Vector3Property(Vector3 val) : base(val)
    {
    }
    
    public static implicit operator Vector3(Vector3Property prop)
    {
        return prop.Value;
    }
}