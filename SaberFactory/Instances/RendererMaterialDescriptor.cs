using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances
{
    /// <summary>
    /// Implementation of a <see cref="MaterialDescriptor"/> that is closely tied to a renderer
    /// </summary>
    internal class RendererMaterialDescriptor : MaterialDescriptor
    {
        private readonly Renderer _renderer;
        private readonly int _materialIndex;

        public RendererMaterialDescriptor(Material material, Renderer renderer, int materialIndex) : base(material)
        {
            _renderer = renderer;
            _materialIndex = materialIndex;
        }

        public override void Revert()
        {
            base.Revert();
            _renderer.SetMaterial(_materialIndex, Material);
        }
    }
}