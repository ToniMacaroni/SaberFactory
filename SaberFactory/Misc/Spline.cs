using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.Misc {
    public class Spline
    {
        readonly List<SplineControlPoint> mControlPoints = new List<SplineControlPoint>();
        readonly List<SplineControlPoint> mSegments = new List<SplineControlPoint>();

        public int Granularity = 20;

        public SplineControlPoint this[int index]
        {
            get
            {
                if (index > -1 && index < mSegments.Count)
                    return mSegments[index];
                return null;

            }
        }

        public List<SplineControlPoint> ControlPoints => mControlPoints;


        public SplineControlPoint NextControlPoint(SplineControlPoint controlpoint)
        {
            if (mControlPoints.Count == 0) return null; 

            int i = controlpoint.ControlPointIndex + 1;
            if (i >= mControlPoints.Count)
                return null;
            return mControlPoints[i];
        }


        public SplineControlPoint PreviousControlPoint(SplineControlPoint controlpoint)
        {
            if (mControlPoints.Count == 0) return null;

            int i = controlpoint.ControlPointIndex - 1;
            if (i < 0)
                return null;
            return mControlPoints[i];
        }

        public Vector3 NextPosition(SplineControlPoint controlpoint)
        {
            SplineControlPoint seg = NextControlPoint(controlpoint);
            if (seg != null)
                return seg.Position;
            return controlpoint.Position;
        }


        public Vector3 PreviousPosition(SplineControlPoint controlpoint)
        {
            SplineControlPoint seg = PreviousControlPoint(controlpoint);
            if (seg != null)
                return seg.Position;
            else
                return controlpoint.Position;
        }


        public Vector3 PreviousNormal(SplineControlPoint controlpoint)
        {
            SplineControlPoint seg = PreviousControlPoint(controlpoint);
            if (seg != null)
                return seg.Normal;
            return controlpoint.Normal;
        }

        public Vector3 NextNormal(SplineControlPoint controlpoint)
        {
            SplineControlPoint seg = NextControlPoint(controlpoint);
            if (seg != null)
                return seg.Normal;
            return controlpoint.Normal;
        }

        public SplineControlPoint LenToSegment(float t, out float localF)
        {
            SplineControlPoint seg = null;

            t = Mathf.Clamp01(t);

            float len = t * mSegments[mSegments.Count - 1].Dist;


            int index = 0;
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

            int prevIdx = seg.SegmentIndex - 1;
            SplineControlPoint prevSeg = mSegments[prevIdx];
            var PrevLen = seg.Dist - prevSeg.Dist;
            localF = (len - prevSeg.Dist) / PrevLen;
            return prevSeg;

        }


        public static Vector3 CatmulRom(Vector3 T0, Vector3 P0, Vector3 P1, Vector3 T1, float f)
        {
            double DT1 = -0.5; 
            double DT2 = 1.5; 
            double DT3 = -1.5; 
            double DT4 = 0.5;

            double DE2 = -2.5; 
            double DE3 = 2; 
            double DE4 = -0.5;

            double DV1 = -0.5;
            double DV3 = 0.5;

            double FAX = DT1 * T0.x + DT2 * P0.x + DT3 * P1.x + DT4 * T1.x;
            double FBX = T0.x + DE2 * P0.x + DE3 * P1.x + DE4 * T1.x;
            double FCX = DV1 * T0.x + DV3 * P1.x;
            double FDX = P0.x;

            double FAY = DT1 * T0.y + DT2 * P0.y + DT3 * P1.y + DT4 * T1.y;
            double FBY = T0.y + DE2 * P0.y + DE3 * P1.y + DE4 * T1.y;
            double FCY = DV1 * T0.y + DV3 * P1.y;
            double FDY = P0.y;

            double FAZ = DT1 * T0.z + DT2 * P0.z + DT3 * P1.z + DT4 * T1.z;
            double FBZ = T0.z + DE2 * P0.z + DE3 * P1.z + DE4 * T1.z;
            double FCZ = DV1 * T0.z + DV3 * P1.z;
            double FDZ = P0.z;

            float FX = (float)(((FAX * f + FBX) * f + FCX) * f + FDX);
            float FY = (float)(((FAY * f + FBY) * f + FCY) * f + FDY);
            float FZ = (float)(((FAZ * f + FBZ) * f + FCZ) * f + FDZ);

            return new Vector3(FX, FY, FZ);
        }


        public Vector3 InterpolateByLen(float tl)
        {
            SplineControlPoint seg = LenToSegment(tl, out var localF);
            return seg.Interpolate(localF);
        }

        public Vector3 InterpolateNormalByLen(float tl)
        {
            SplineControlPoint seg = LenToSegment(tl, out var localF);
            return seg.InterpolateNormal(localF);
        }

        public SplineControlPoint AddControlPoint(Vector3 pos, Vector3 up)
        {
            SplineControlPoint cp = new SplineControlPoint();

            cp.Init(this);

            cp.Position = pos;

            cp.Normal = up;

            mControlPoints.Add(cp);

            cp.ControlPointIndex = mControlPoints.Count - 1;


            return cp;
        }

        public void Clear()
        {
            mControlPoints.Clear();
        }


        void RefreshDistance()
        {
            if (mSegments.Count < 1)
                return;

            mSegments[0].Dist = 0f;

            for (int i = 1; i < mSegments.Count; i++)
            {

                float prevLen = (mSegments[i].Position - mSegments[i - 1].Position).magnitude;

                mSegments[i].Dist = mSegments[i - 1].Dist + prevLen;
            }
        }

        public void RefreshSpline()
        {
            mSegments.Clear();

            for (int i = 0; i < mControlPoints.Count; i++)
            {
                if (mControlPoints[i].IsValid)
                {
                    mSegments.Add(mControlPoints[i]);
                    mControlPoints[i].SegmentIndex = mSegments.Count - 1;
                }
            }

            RefreshDistance();
        }
    }
}


