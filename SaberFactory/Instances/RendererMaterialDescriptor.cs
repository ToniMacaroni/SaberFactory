﻿using System;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances
{
    /// <summary>
    ///     Implementation of a <see cref="MaterialDescriptor" /> that is closely tied to a renderer
    /// </summary>
    internal class RendererMaterialDescriptor : MaterialDescriptor, IDisposable
    {
        private readonly int _materialIndex;
        private readonly Renderer _renderer;

        public RendererMaterialDescriptor(Material material, Renderer renderer, int materialIndex) : base(material)
        {
            _renderer = renderer;
            _materialIndex = materialIndex;
        }

        public void Dispose()
        {
            Destroy();
        }

        public override void Revert()
        {
            base.Revert();
            _renderer.SetMaterial(_materialIndex, Material);
        }
    }
}