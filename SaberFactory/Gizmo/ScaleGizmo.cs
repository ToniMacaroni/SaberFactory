using UnityEngine;

namespace SaberFactory.Gizmo
{
    internal class ScaleGizmo : FactoryDragGizmoBase
    {
        public static Mesh ScalingMesh;
        
        protected override Color GizmoColor => Color.red.ColorWithAlpha(0.1f);

        protected override Mesh GizmoMesh => ScalingMesh;
    }
}