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

        private Material _originalMaterial;

        public MaterialDescriptor(Material material)
        {
            Material = material;

            if (material != null)
            {
                _originalMaterial = new Material(material);
            }
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

        public void UpdateBackupMaterial(bool deleteOld)
        {
            if (deleteOld && _originalMaterial != null)
            {
                DestroyBackupMaterial();
            }

            _originalMaterial = new Material(Material);
        }

        public MaterialDescriptor CreateCopy()
        {
            return new MaterialDescriptor(new Material(Material));
        }
    }
}