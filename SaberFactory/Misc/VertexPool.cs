using SaberFactory.Instances.Trail;
using UnityEngine;

namespace SaberFactory.Misc {

    internal class VertexPool
    {
        public class VertexSegment
        {
            public int VertStart;
            public int IndexStart;
            public int VertCount;
            public int IndexCount;
            public VertexPool Pool;

            public VertexSegment(int start, int count, int istart, int icount, VertexPool pool)
            {
                VertStart = start;
                VertCount = count;
                IndexCount = icount;
                IndexStart = istart;
                Pool = pool;
            }
        }

        public Vector3[] Vertices;
        public int[] Indices;
        public Vector2[] UVs;
        public Color[] Colors;

        public bool IndiceChanged;
        public bool ColorChanged;
        public bool UVChanged;
        public bool VertChanged;
        public bool UV2Changed;

        protected int _vertexTotal;
        protected int _vertexUsed;
        protected int _indexTotal;
        protected int _indexUsed;
        public bool FirstUpdate = true;

        protected bool _vertCountChanged;

        public const int BlockSize = 108;

        public float BoundsScheduleTime = 1f;
        public float ElapsedTime;

        protected AltTrail _owner;
        protected MeshFilter _meshFilter;
        protected Material _material;

        public Mesh MyMesh => _meshFilter?.sharedMesh;

        public void RecalculateBounds()
        {
            MyMesh.RecalculateBounds();
        }


        public void SetMeshObjectActive(bool flag) {
            if (_meshFilter == null) {
                return;
            }

            _meshFilter.gameObject.SetActive(flag);
        }

        void CreateMeshObj(AltTrail owner, Material material) {
            GameObject obj = new GameObject("SaberTrail");
            obj.layer = owner.gameObject.layer;
            obj.AddComponent<MeshFilter>();
            var meshrenderer = obj.AddComponent<MeshRenderer>();

            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;

            _meshFilter = (MeshFilter)obj.GetComponent(typeof(MeshFilter));
            meshrenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshrenderer.receiveShadows = false;
            meshrenderer.GetComponent<Renderer>().sharedMaterial = material;
            meshrenderer.sortingLayerName = _owner.SortingLayerName;
            meshrenderer.sortingOrder = _owner.SortingOrder;
            _meshFilter.sharedMesh = new Mesh();
        }

        public void Destroy() {
            if (_meshFilter != null)
            {
                Object.Destroy(_meshFilter.gameObject);
            }
        }

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


        public VertexSegment GetVertices(int vcount, int icount)
        {
            int vertNeed = 0;
            int indexNeed = 0;
            if (_vertexUsed + vcount >= _vertexTotal)
            {
                vertNeed = (vcount / BlockSize + 1) * BlockSize;
            }
            if (_indexUsed + icount >= _indexTotal)
            {
                indexNeed = (icount / BlockSize + 1) * BlockSize;
            }
            _vertexUsed += vcount;
            _indexUsed += icount;
            if (vertNeed != 0 || indexNeed != 0)
            {
                EnlargeArrays(vertNeed, indexNeed);
                _vertexTotal += vertNeed;
                _indexTotal += indexNeed;
            }

            VertexSegment ret = new VertexSegment(_vertexUsed - vcount, vcount, _indexUsed - icount, icount, this);

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
            Vector3[] tempVerts = Vertices;
            Vertices = new Vector3[Vertices.Length + count];
            tempVerts.CopyTo(Vertices, 0);

            Vector2[] tempUVs = UVs;
            UVs = new Vector2[UVs.Length + count];
            tempUVs.CopyTo(UVs, 0);

            Color[] tempColors = Colors;
            Colors = new Color[Colors.Length + count];
            tempColors.CopyTo(Colors, 0);

            int[] tempTris = Indices;
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
            if (MyMesh == null){
                return;
            }
            if (_vertCountChanged)
            {
                MyMesh.Clear();
            }

            MyMesh.vertices = Vertices;
            if (UVChanged)
            {
                MyMesh.uv = UVs;
            }

            if (ColorChanged)
            {
                MyMesh.colors = Colors;
            }

            if (IndiceChanged)
            {
                MyMesh.triangles = Indices;
            }

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
    }
}