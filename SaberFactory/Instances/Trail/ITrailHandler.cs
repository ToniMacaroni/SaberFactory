using CameraUtils.Core;
using SaberFactory.Configuration;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal interface ITrailHandler
    {
        public void CreateTrail(TrailConfig config, bool editor);

        public void DestroyTrail(bool immediate = false);

        public void SetTrailData(InstanceTrailData instanceTrailData);

        public void SetColor(Color color);

        public void SetVisibilityLayer(VisibilityLayer layer);
    }
}