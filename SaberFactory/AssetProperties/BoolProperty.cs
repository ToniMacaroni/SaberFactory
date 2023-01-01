namespace SaberFactory.AssetProperties;

public class BoolProperty : AssetProperty<bool>
{
    public BoolProperty(bool val) : base(val)
    {
    }
    
    public static implicit operator bool(BoolProperty prop)
    {
        return prop.Value;
    }
}