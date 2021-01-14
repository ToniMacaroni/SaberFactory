using CustomSaber;
using SaberFactory.Helpers;
using SaberFactory.Models;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal struct InstanceTrailData
    {
        public Vector3 StartPos => PointStart.position + _startPosOffset;
        public Vector3 EndPos => PointEnd.position;
        public Material Material;
        public int Length;
        public float WhiteStep;

        public float Width => Mathf.Abs(EndPos.y - StartPos.y);

        public Transform PointStart { get; private set; }
        public Transform PointEnd { get; private set; }
        private int _length;

        private Vector3 _startPosOffset;

        public void ApplyModel(TrailModel? trailModel)
        {
            if (!trailModel.HasValue) return;
            var offsets = trailModel.Value;

            _startPosOffset = new Vector3(0, offsets.TrailWidthOffset, 0);
            Material = offsets.Material;
            Length = _length + offsets.TrailLengthOffset;
            WhiteStep = offsets.Whitestep;
        }

        public (Transform start, Transform end) CopyPoints()
        {
            var start = PointStart.parent.CreateGameObject("InstancePointStart").transform;
            start.position = StartPos;

            var end = PointEnd.parent.CreateGameObject("InstancePointEnd").transform;
            end.position = EndPos;

            return (start, end);
        }

        public static InstanceTrailData FromCustomSaber(GameObject gameobject)
        {
            var saberTrail = gameobject.GetComponent<CustomTrail>();

            if (!saberTrail) return default;

            var data = new InstanceTrailData
            {
                PointStart = saberTrail.PointStart,
                PointEnd = saberTrail.PointEnd,
                Material = saberTrail.TrailMaterial,
                _length = saberTrail.Length,
                Length = saberTrail.Length,
                WhiteStep = 0
            };

            return data;
        }
    }
}