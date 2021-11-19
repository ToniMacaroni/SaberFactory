using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.ProjectComponents
{
    [Serializable]
    public class ComponentModifier
    {
        public string Name;
        public int Id;

        public string ComponentType;
        
        #if !UNITY
        [Newtonsoft.Json.JsonIgnore]
        #endif
        public GameObject Object;

        public int ObjectIndex;
    }
}