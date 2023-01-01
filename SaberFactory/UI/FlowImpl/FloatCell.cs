using SaberFactory.AssetProperties;
using UnityEngine;

namespace SaberFactory.UI.Flow;

public partial class FloatCell
{
    public FloatProperty Prop { get; private set; }
    
    private const string ResetTextFormat = "Reset to {0:0.00}";
    
    public void SetProp(string propName, FloatProperty prop)
    {
        Prop = prop;
        _resetTextMesh.text = string.Format(ResetTextFormat, prop.DefaultValue);
        _textMesh.text = propName;
        _sliderBinder.Bind(_slider, prop);
    }

    public override void ResetClicked()
    {
        Prop.Revert();
        _slider.Value = Prop.Value;
    }

    private readonly SliderPropertyBinder _sliderBinder = new();
}