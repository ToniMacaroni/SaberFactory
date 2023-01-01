namespace SaberFactory.AssetProperties;

public class StringProperty : AssetProperty<string>
{
    public StringProperty(string val) : base(val)
    {
    }
    
    public static implicit operator string(StringProperty prop)
    {
        return prop.Value;
    }
}