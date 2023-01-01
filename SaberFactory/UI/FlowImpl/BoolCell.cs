using SaberFactory.AssetProperties;

namespace SaberFactory.UI.Flow;

public partial class BoolCell
{
    public BoolProperty Prop { get; private set; }
    
    private const string ResetTextFormat = "Reset to {0}";
    
    public void SetProp(string propName, BoolProperty prop)
    {
        Prop = prop;
        _resetTextMesh.text = string.Format(ResetTextFormat, prop.DefaultValue? "On" : "Off");
        _propNameTextMesh.text = propName;
        _toggleBinder.Bind(_toggle, prop);
    }
    
    public override void ResetClicked()
    {
        Prop.Revert();
        _toggle.isOn = Prop.Value;
    }
    
    private readonly TogglePropertyBinder _toggleBinder = new();
}