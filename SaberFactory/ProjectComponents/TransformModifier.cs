using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.ProjectComponents
{
    [Serializable]
    public class TransformModifier
    {
        public string Name;
        
        public int Id;

        #if !UNITY
        [Newtonsoft.Json.JsonIgnore]
        #endif
        public List<GameObject> Objects;

        public List<int> ObjectIndecies;
    }
}