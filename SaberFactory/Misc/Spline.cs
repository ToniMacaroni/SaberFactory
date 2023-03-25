using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.Misc
{
    public class Spline
    {
        public SplineControlPoint this[int index]
        {
            get
            {
                if (index > -1 && index < mSegments.Count)
                {
                    return mSegments[index];
                }

                return null;
            }
        }

        public List<SplineControlPoint> ControlPoints { get; } = new List<SplineControlPoint>();

        public int Granularity = 20;
        private readonly List<SplineControlPoint> mSegments = new List<SplineControlPoint>();


        public SplineControlPoint NextControlPoint(SplineControlPoint controlpoint)
        {
            if (ControlPoints.Count == 0)
            {
                return null;
            }

            var i = controlpoint.ControlPointIndex + 1;
            if (i >= ControlPoints.Count)
            {
                return null;
            }

            return ControlPoints[i];
        }


        public SplineControlPoint PreviousControlPoint(SplineControlPoint controlpoint)
        {
            if (ControlPoints.Count == 0)
            {
                return null;
            }

            var i = controlpoint.ControlPointIndex - 1;
            if (i < 0)
            {
                return null;
            }

            return ControlPoints[i];
        }

        public Vector3 NextPosition(SplineControlPoint controlpoint)
        {
            var seg = NextControlPoint(controlpoint);
            if (seg != null)
            {
                return seg.Position;
            }

            return controlpoint.Position;
        }


        public Vector3 PreviousPosition(SplineControlPoint controlpoint)
        {
            var seg = PreviousControlPoint(controlpoint);
            if (seg != null)
            {
                return seg.Position;
            }

            return controlpoint.Position;
        }


        public Vector3 PreviousNormal(SplineControlPoint controlpoint)
        {
            var seg = PreviousControlPoint(controlpoint);
            if (seg != null)
            {
                return seg.Normal;
            }

            return controlpoint.Normal;
        }

        public Vector3 NextNormal(SplineControlPoint controlpoint)
        {
            var seg = NextControlPoint(controlpoint);
            if (seg != null)
            {
                return seg.Normal;
            }

            return controlpoint.Normal;
        }

        public SplineControlPoint LenToSegment(float t, out float localF)
        {
            SplineControlPoint seg = null;

            t = Mathf.Clamp01(t);

            var len = t * mSegments[mSegments.Count - 1].Dist;


            int index;
            for (index = 0; index < mSegments.Count; index++)
            {
                if (mSegments[index].Dist >= len)
                {
                    seg = mSegments[index];
                    break;
                }
            }

            if (index == 0)
            {
                //skip the first frame.
                localF = 0f;
                return seg;
            }

            var prevIdx = seg!.SegmentIndex - 1;
            var prevSeg = mSegments[prevIdx];
            var prevLen = seg.Dist - prevSeg.Dist;
            localF = (len - prevSeg.Dist) / prevLen;
            return prevSeg;
        }


        public static Vector3 CatmulRom(Vector3 T0, Vector3 P0, Vector3 P1, Vector3 T1, float f)
        {
            var DT1 = -0.5;
            var DT2 = 1.5;
            var DT3 = -1.5;
            var DT4 = 0.5;

            var DE2 = -2.5;
            double DE3 = 2;
            var DE4 = -0.5;

            var DV1 = -0.5;
            var DV3 = 0.5;

            var FAX = DT1 * T0.x + DT2 * P0.x + DT3 * P1.x + DT4 * T1.x;
            var FBX = T0.x + DE2 * P0.x + DE3 * P1.x + DE4 * T1.x;
            var FCX = DV1 * T0.x + DV3 * P1.x;
            double FDX = P0.x;

            var FAY = DT1 * T0.y + DT2 * P0.y + DT3 * P1.y + DT4 * T1.y;
            var FBY = T0.y + DE2 * P0.y + DE3 * P1.y + DE4 * T1.y;
            var FCY = DV1 * T0.y + DV3 * P1.y;
            double FDY = P0.y;

            var FAZ = DT1 * T0.z + DT2 * P0.z + DT3 * P1.z + DT4 * T1.z;
            var FBZ = T0.z + DE2 * P0.z + DE3 * P1.z + DE4 * T1.z;
            var FCZ = DV1 * T0.z + DV3 * P1.z;
            double FDZ = P0.z;

            var FX = (float)(((FAX * f + FBX) * f + FCX) * f + FDX);
            var FY = (float)(((FAY * f + FBY) * f + FCY) * f + FDY);
            var FZ = (float)(((FAZ * f + FBZ) * f + FCZ) * f + FDZ);

            return new Vector3(FX, FY, FZ);
        }


        public Vector3 InterpolateByLen(float tl)
        {
            var seg = LenToSegment(tl, out var localF);
            return seg.Interpolate(localF);
        }

        public Vector3 InterpolateNormalByLen(float tl)
        {
            var seg = LenToSegment(tl, out var localF);
            return seg.InterpolateNormal(localF);
        }

        public SplineControlPoint AddControlPoint(Vector3 pos, Vector3 up)
        {
            var cp = new SplineControlPoint();

            cp.Init(this);

            cp.Position = pos;

            cp.Normal = up;

            ControlPoints.Add(cp);

            cp.ControlPointIndex = ControlPoints.Count - 1;


            return cp;
        }

        public void Clear()
        {
            ControlPoints.Clear();
        }


        private void RefreshDistance()
        {
            if (mSegments.Count < 1)
            {
                return;
            }

            mSegments[0].Dist = 0f;

            for (var i = 1; i < mSegments.Count; i++)
            {
                var prevLen = (mSegments[i].Position - mSegments[i - 1].Position).magnitude;

                mSegments[i].Dist = mSegments[i - 1].Dist + prevLen;
            }
        }

        public void RefreshSpline()
        {
            mSegments.Clear();

            for (var i = 0; i < ControlPoints.Count; i++)
            {
                if (ControlPoints[i].IsValid)
                {
                    mSegments.Add(ControlPoints[i]);
                    ControlPoints[i].SegmentIndex = mSegments.Count - 1;
                }
            }

            RefreshDistance();
        }
    }
}