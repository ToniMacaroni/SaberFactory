using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class SFTrailRenderer : SaberTrailRenderer
    {
        private float _uvMuliplier = 2.5f;
        private static readonly Color _cutoffColor = new Color(0, 0, 0, 0);

        public MeshRenderer MeshRenderer
        {
            get => _meshRenderer;
            set => _meshRenderer = value;
        }

        public MeshFilter MeshFilter
        {
            get => _meshFilter;
            set => _meshFilter = value;
        }

        public static SFTrailRenderer Create()
        {
            var go = new GameObject("SFTrailRenderer");
            var renderer =  go.AddComponent<SFTrailRenderer>();
            renderer.MeshRenderer = go.AddComponent<MeshRenderer>();
            renderer.MeshFilter = go.AddComponent<MeshFilter>();
            return renderer;
        }

        public void Init(float trailWidth, float trailDuration, int granularity, float whiteSectionMaxDuration, float uvMultiplier)
        {
            _uvMuliplier = uvMultiplier;
            base.Init(trailWidth, trailDuration, granularity, whiteSectionMaxDuration);
        }

        protected override void UpdateVertices(TrailElementCollection trailElementCollection, Color color)
        {
            for (var index1 = 0; index1 < _granularity; ++index1)
            {
                var index2 = index1 * 3;
                var tl = (float) index1 / _granularity;
                var num1 = TimeHelper.time - trailElementCollection.InterpolateTimeByLen(tl);
                var num2 = num1 / _trailDuration;
                var uv = Vector2.zero;
                var vector3_1 = trailElementCollection.InterpolateByLen(tl);
                var vector3_2 = trailElementCollection.InterpolateNormalByLen(tl);
                var vector3_3 = vector3_1 + vector3_2.normalized * (_trailWidth * 0.5f);
                var vector3_4 = vector3_1 - vector3_2.normalized * (_trailWidth * 0.5f);
                Color color1;
                if ((double) num1 < _whiteSectionMaxDuration)
                {
                    var t = num1 / _whiteSectionMaxDuration;
                    color1 = Color.Lerp(Color.white, color, t);
                }
                else
                {
                    color1 = color;
                }

                var yUV = num2 * _uvMuliplier;

                if (yUV > 1)
                {
                    color1 = _cutoffColor;
                }

                _vertices[index2] = vector3_3;
                _colors[index2] = color1;
                uv.x = 0.0f;
                uv.y = yUV;
                _uvs[index2] = uv;
                _vertices[index2 + 1] = vector3_1;
                _colors[index2 + 1] = color1;
                uv.x = 0.5f;
                uv.y = yUV;
                _uvs[index2 + 1] = uv;
                _vertices[index2 + 2] = vector3_4;
                _colors[index2 + 2] = color1;
                uv.x = 1f;
                uv.y = yUV;
                _uvs[index2 + 2] = uv;
            }
        }
    }
}