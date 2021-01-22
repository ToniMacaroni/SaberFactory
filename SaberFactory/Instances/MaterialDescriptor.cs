using UnityEngine;

namespace SaberFactory.Instances
{
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