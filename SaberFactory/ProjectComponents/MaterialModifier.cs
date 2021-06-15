using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace SaberFactory.ProjectComponents
{
    public class MaterialModifierCollection : MonoBehaviour
    {
        [SerializeField]
        public List<MaterialModifier> Modifiers;
    }

    public class MaterialModifier
    {
        [SerializeField] public Material Material;
        [SerializeField] public List<string> EditableProperties;
    }
}
