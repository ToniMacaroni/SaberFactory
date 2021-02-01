using UnityEngine;

namespace SaberFactory.Instances
{
    /// <summary>
    /// Extension class for <see cref="Material"/> with more information
    /// and possibly to revert a changed <see cref="Material"/>
    /// </summary>
    internal class MaterialDescriptor
    {
        public Material Material;

        private readonly Material _originalMaterial;

        public MaterialDescriptor(Material material)
        {
            Material = material;
            _originalMaterial = new Material(material);
        }

        public virtual void Revert()
        {
            Material = new Material(_originalMaterial);
        }
    }
}