using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.ProjectComponents
{
    [Serializable]
    public class VisibilityModifier
    {
        public string Name;

        public int Id;

        public bool DefaultValue;

#if !UNITY
    [Newtonsoft.Json.JsonIgnore]
#endif
        public List<GameObject> Objects;

        public List<int> ObjectIndecies;
    }
}