using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances
{
    /// <summary>
    ///     Extension class for <see cref="Material" /> with more information
    ///     and possibly to revert a changed <see cref="Material" />
    /// </summary>
    internal class MaterialDescriptor
    {
        public bool IsValid => Material != null;
        public Material Material;

        private readonly Material _originalMaterial;

        public MaterialDescriptor(Material material)
        {
            Material = material;
            _originalMaterial = new Material(material);
        }

        public virtual void Revert()
        {
            if (_originalMaterial is null)
            {
                return;
            }

            DestroyCurrentMaterial();
            Material = new Material(_originalMaterial);
        }

        public void DestroyCurrentMaterial()
        {
            Material.TryDestoryImmediate();
        }

        public void DestroyBackupMaterial()
        {
            _originalMaterial.TryDestoryImmediate();
        }

        public void Destroy()
        {
            DestroyCurrentMaterial();
            DestroyBackupMaterial();
        }

        public MaterialDescriptor CreateCopy()
        {
            return new MaterialDescriptor(new Material(Material));
        }
    }
}