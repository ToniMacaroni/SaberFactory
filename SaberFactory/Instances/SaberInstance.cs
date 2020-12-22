using JetBrains.Annotations;
using SaberFactory.Models;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
{
    internal class SaberInstance
    {
        public SaberSection Blade;
        public SaberSection Emitter;
        public SaberSection Handle;
        public SaberSection Pommel;

        public readonly GameObject GameObject;
        public readonly Transform CachedTransform;

        private SaberInstance(SaberModel model, BasePieceInstance.Factory pieceFactory)
        {
            GameObject = new GameObject("SF Saber");
            CachedTransform = GameObject.transform;

            InstantiateSection(pieceFactory, Blade, model.Blade);
            InstantiateSection(pieceFactory, Emitter, model.Emitter);
            InstantiateSection(pieceFactory, Handle, model.Handle);
            InstantiateSection(pieceFactory, Pommel, model.Pommel);
        }

        public void SetParent(Transform parent)
        {
            CachedTransform.SetParent(parent, false);
        }

        private void InstantiateSection(BasePieceInstance.Factory factory, SaberSection instanceSection, SaberModel.SaberSection modelSection)
        {
            if (modelSection.Model != null)
            {
                instanceSection.Model = factory.Create(modelSection.Model);
                instanceSection.Model.SetParent(CachedTransform);

                if (modelSection.Halo != null)
                {
                    instanceSection.Halo = factory.Create(modelSection.Halo);
                    instanceSection.Halo.SetParent(CachedTransform);
                }
            }
        }

        internal struct SaberSection
        {
            public BasePieceInstance Model;
            public BasePieceInstance Halo;
        }

        internal class Factory : PlaceholderFactory<SaberModel, SaberInstance> {}
    }
}