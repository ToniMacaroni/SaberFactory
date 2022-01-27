using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.ProjectComponents
{
    public class SaberModifierCollection : MonoBehaviour
    {
        public VisibilityModifier[] VisibilityModifiers;
        public ComponentModifier[] ComponentModifiers;
        public TransformModifier[] TransformModifiers;

#if !UNITY
    [Newtonsoft.Json.JsonIgnore]
#endif
        public string ObjectJson;

        public List<GameObject> Objects;

#if !UNITY
    private bool _inited;

    public bool Init()
    {
        if (string.IsNullOrEmpty(ObjectJson) || _inited)
        {
            return _inited;
        }

        Newtonsoft.Json.JsonConvert.PopulateObject(ObjectJson, this);
        
        foreach (var mod in VisibilityModifiers)
        {
            mod.Objects = new List<GameObject>();
            foreach (var idx in mod.ObjectIndecies)
            {
                if (idx >= Objects.Count)
                {
                    continue;
                }
                mod.Objects.Add(Objects[idx]);
            }
        }

        foreach (var mod in ComponentModifiers)
        {
            if (mod.ObjectIndex >= Objects.Count)
            {
                continue;
            }
            mod.Object = Objects[mod.ObjectIndex];
        }
        
        foreach (var mod in TransformModifiers)
        {
            mod.Objects = new List<GameObject>();
            foreach (var idx in mod.ObjectIndecies)
            {
                if (idx >= Objects.Count)
                {
                    continue;
                }
                mod.Objects.Add(Objects[idx]);
            }
        }

        _inited = true;
        return true;
    }
#endif
    }
}