using SaberFactory.Helpers;
using SaberFactory.Instances.Trail;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaberFactory.Misc
{
    internal class VertexPool
    {
        public const int BlockSize = 108;

        public Mesh MyMesh => _meshFilter?.sharedMesh;

        public float BoundsScheduleTime = 1f;
        public bool ColorChanged;
        public Color[] Colors;
        public float ElapsedTime;
        public bool FirstUpdate = true;

        public bool IndiceChanged;
        public int[] Indices;
        public bool UV2Changed;
        public bool UVChanged;
        public Vector2[] UVs;
        public bool VertChanged;

        public Vector3[] Vertices;

        protected GameObject _gameObject;
        protected int _indexTotal;
        protected int _indexUsed;
        protected Material _material;
        protected MeshFilter _meshFilter;

        protected AltTrail _owner;

        protected bool _vertCountChanged;

        protected int _vertexTotal;
        protected int _vertexUsed;

        public VertexPool(Material material, AltTrail owner)
        {
            _vertexTotal = _vertexUsed = 0;
            _vertCountChanged = false;
            _owner = owner;
            CreateMeshObj(owner, material);
            _material = material;
            InitArrays();
            IndiceChanged = ColorChanged = UVChanged = UV2Changed = VertChanged = true;
        }

        public void RecalculateBounds()
        {
            MyMesh.RecalculateBounds();
        }


        public void SetMeshObjectActive(bool flag)
        {
            if (_meshFilter == null) return;

            _meshFilter.gameObject.SetActive(flag);
        }

        private void CreateMeshObj(AltTrail owner, Material material)
        {
            _gameObject = new GameObject("SaberTrail");
            _gameObject.layer = owner.gameObject.layer;
            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            var meshrenderer = _gameObject.AddComponent<MeshRenderer>();

            _gameObject.transform.position = Vector3.zero;
            _gameObject.transform.rotation = Quaternion.identity;

            meshrenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshrenderer.receiveShadows = false;
            meshrenderer.sharedMaterial = material;
            meshrenderer.sortingLayerName = _owner.SortingLayerName;
            meshrenderer.sortingOrder = _owner.SortingOrder;
            _meshFilter.sharedMesh = new Mesh();
        }

        public void Destroy()
        {
            _gameObject.TryDestroy();
        }


        public VertexSegment GetVertices(int vcount, int icount)
        {
            var vertNeed = 0;
            var indexNeed = 0;
            if (_vertexUsed + vcount >= _vertexTotal) vertNeed = (vcount / BlockSize + 1) * BlockSize;
            if (_indexUsed + icount >= _indexTotal) indexNeed = (icount / BlockSize + 1) * BlockSize;
            _vertexUsed += vcount;
            _indexUsed += icount;
            if (vertNeed != 0 || indexNeed != 0)
            {
                EnlargeArrays(vertNeed, indexNeed);
                _vertexTotal += vertNeed;
                _indexTotal += indexNeed;
            }

            var ret = new VertexSegment(_vertexUsed - vcount, vcount, _indexUsed - icount, icount, this);

            return ret;
        }


        protected void InitArrays()
        {
            Vertices = new Vector3[4];
            UVs = new Vector2[4];
            Colors = new Color[4];
            Indices = new int[6];
            _vertexTotal = 4;
            _indexTotal = 6;
        }


        public void EnlargeArrays(int count, int icount)
        {
            var tempVerts = Vertices;
            Vertices = new Vector3[Vertices.Length + count];
            tempVerts.CopyTo(Vertices, 0);

            var tempUVs = UVs;
            UVs = new Vector2[UVs.Length + count];
            tempUVs.CopyTo(UVs, 0);

            var tempColors = Colors;
            Colors = new Color[Colors.Length + count];
            tempColors.CopyTo(Colors, 0);

            var tempTris = Indices;
            Indices = new int[Indices.Length + icount];
            tempTris.CopyTo(Indices, 0);

            _vertCountChanged = true;
            IndiceChanged = true;
            ColorChanged = true;
            UVChanged = true;
            VertChanged = true;
            UV2Changed = true;
        }

        public void LateUpdate()
        {
            if (MyMesh == null) return;
            if (_vertCountChanged) MyMesh.Clear();

            MyMesh.vertices = Vertices;
            if (UVChanged) MyMesh.uv = UVs;

            if (ColorChanged) MyMesh.colors = Colors;

            if (IndiceChanged) MyMesh.triangles = Indices;

            ElapsedTime += Time.deltaTime;
            if (ElapsedTime > BoundsScheduleTime || FirstUpdate)
            {
                RecalculateBounds();
                ElapsedTime = 0f;
            }

            if (ElapsedTime > BoundsScheduleTime)
                FirstUpdate = false;

            _vertCountChanged = false;
            IndiceChanged = false;
            ColorChanged = false;
            UVChanged = false;
            UV2Changed = false;
            VertChanged = false;
        }

        public class VertexSegment
        {
            public int IndexCount;
            public int IndexStart;
            public VertexPool Pool;
            public int VertCount;
            public int VertStart;

            public VertexSegment(int start, int count, int istart, int icount, VertexPool pool)
            {
                VertStart = start;
                VertCount = count;
                IndexCount = icount;
                IndexStart = istart;
                Pool = pool;
            }
        }
    }
}