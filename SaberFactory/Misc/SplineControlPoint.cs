using UnityEngine;

namespace SaberFactory.Misc
{
    public class SplineControlPoint
    {
        public SplineControlPoint NextControlPoint => mSpline.NextControlPoint(this);

        public SplineControlPoint PreviousControlPoint => mSpline.PreviousControlPoint(this);

        public Vector3 NextPosition => mSpline.NextPosition(this);


        public Vector3 PreviousPosition => mSpline.PreviousPosition(this);


        public Vector3 NextNormal => mSpline.NextNormal(this);


        public Vector3 PreviousNormal => mSpline.PreviousNormal(this);

        public bool IsValid => NextControlPoint != null;
        public int ControlPointIndex = -1;

        public float Dist;
        public Vector3 Normal;
        public Vector3 Position;
        public int SegmentIndex = -1;

        protected Spline mSpline;


        private Vector3 GetNext2Position()
        {
            var cp = NextControlPoint;
            if (cp != null)
            {
                return cp.NextPosition;
            }

            return NextPosition;
        }


        private Vector3 GetNext2Normal()
        {
            var cp = NextControlPoint;
            if (cp != null)
            {
                return cp.NextNormal;
            }


            return Normal;
        }


        public Vector3 Interpolate(float localF)
        {
            localF = Mathf.Clamp01(localF);

            return Spline.CatmulRom(PreviousPosition, Position, NextPosition, GetNext2Position(), localF);
        }


        public Vector3 InterpolateNormal(float localF)
        {
            localF = Mathf.Clamp01(localF);

            return Spline.CatmulRom(PreviousNormal, Normal, NextNormal, GetNext2Normal(), localF);
        }


        public void Init(Spline owner)
        {
            mSpline = owner;
            SegmentIndex = -1;
        }
    }
}