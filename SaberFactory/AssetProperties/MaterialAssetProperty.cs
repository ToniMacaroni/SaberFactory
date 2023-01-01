using System.Collections.Generic;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.AssetProperties;

public class MaterialAssetProperty : AssetProperty<Material>
{
    private readonly ShaderPropertyInfo _shaderInfo;
    
    public readonly Dictionary<string, FloatProperty> FloatProperties = new();
    public readonly Dictionary<string, BoolProperty> BoolProperties = new();

    public MaterialAssetProperty(Material val) : base(val)
    {
        _shaderInfo = new ShaderPropertyInfo(val.shader);

        foreach (var shaderFloat in _shaderInfo.Floats)
        {
            if (shaderFloat.PropId == MaterialProperties.CustomColors || shaderFloat.HasAttribute(MaterialAttributes.SFToggle))
            {
                var p = new BoolProperty(val.GetFloat(shaderFloat.PropId)>0.5f);
                p.RegisterHandler("", v =>
                {
                    Value.SetFloat(shaderFloat.PropId, v ? 1 : 0);
                });
            
                BoolProperties[shaderFloat.Description] = p;
            }
            else
            {
                var p = new FloatProperty(val.GetFloat(shaderFloat.PropId));
                p.RegisterHandler("", v =>
                {
                    Value.SetFloat(shaderFloat.PropId, v);
                });
            
                FloatProperties[shaderFloat.Description] = p;
            }
        }
        
        foreach (var shaderRange in _shaderInfo.Ranges)
        {
            if (shaderRange.PropId == MaterialProperties.CustomColors || shaderRange.HasAttribute(MaterialAttributes.SFToggle))
            {
                var p = new BoolProperty(val.GetFloat(shaderRange.PropId)>0.5f);
                p.RegisterHandler("", v =>
                {
                    Value.SetFloat(shaderRange.PropId, v ? 1 : 0);
                });
            
                BoolProperties[shaderRange.Description] = p;
            }
            else
            {
                var p = new FloatProperty(val.GetFloat(shaderRange.PropId), shaderRange.Min, shaderRange.Max);
                p.RegisterHandler("", v =>
                {
                    Value.SetFloat(shaderRange.PropId, v);
                });
            
                FloatProperties[shaderRange.Description] = p;
            }
        }
    }
    
    public List<AssetProperty> GetAllProperties()
    {
        var props = new List<AssetProperty>();
        props.AddRange(FloatProperties.Values);
        props.AddRange(BoolProperties.Values);
        return props;
    }

    public override void Revert()
    {
        foreach (var assetProperty in GetAllProperties())
        {
            assetProperty.Revert();
        }

        InvokeValueChange();
    }

    public override void RevertWithoutInvoke()
    {
        foreach (var assetProperty in GetAllProperties())
        {
            assetProperty.RevertWithoutInvoke();
        }
    }
    
    public static implicit operator Material(MaterialAssetProperty prop)
    {
        return prop.Value;
    }
}