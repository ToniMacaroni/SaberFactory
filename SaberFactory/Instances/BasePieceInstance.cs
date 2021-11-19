using System;
using System.Collections.Generic;
using SaberFactory.Helpers;
using SaberFactory.Instances.Setters;
using SaberFactory.Models;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
{
    /// <summary>
    ///     Class for managing an instance of a piece <seealso cref="BasePieceModel" />
    /// </summary>
    internal class BasePieceInstance : IDisposable
    {
        public PropertyBlockSetterHandler PropertyBlockSetterHandler { get; protected set; }
        public readonly Transform CachedTransform;
        public readonly GameObject GameObject;

        public readonly BasePieceModel Model;

        private List<Material> _colorableMaterials;

        protected BasePieceInstance(BasePieceModel model)
        {
            Model = model;
            GameObject = Instantiate();
            CachedTransform = GameObject.transform;
            model.ModifyableComponentManager.SetInstance(GameObject);
        }

        public virtual void Dispose()
        {
            foreach (var material in _colorableMaterials)
            {
                material.TryDestroy();
            }
        }

        public void SetParent(Transform parent)
        {
            CachedTransform.SetParent(parent, false);
        }

        protected virtual GameObject Instantiate()
        {
            return new GameObject("BasePiece");
        }

        public virtual PartEvents GetEvents()
        {
            return null;
        }

        public virtual void SetColor(Color color)
        {
            if (_colorableMaterials is null)
            {
                _colorableMaterials = new List<Material>();
                GetColorableMaterials(_colorableMaterials);
            }

            foreach (var material in _colorableMaterials)
            {
                material.SetColor(MaterialProperties.MainColor, color);
            }
        }

        /// <summary>
        ///     Override this to populate the list with all materials
        ///     that should be colored with the current colorscheme
        /// </summary>
        /// <param name="materials"></param>
        protected virtual void GetColorableMaterials(List<Material> materials)
        { }

        internal class Factory : PlaceholderFactory<BasePieceModel, BasePieceInstance>
        { }
    }
}