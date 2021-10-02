using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.ProjectComponents
{
    public class MaterialModifierCollection : MonoBehaviour
    {
        [SerializeField] public List<MaterialModifier> Modifiers;
    }

    public class MaterialModifier
    {
        [SerializeField] public List<string> EditableProperties;
        [SerializeField] public Material Material;
    }
}