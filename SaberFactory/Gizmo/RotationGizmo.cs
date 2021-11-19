using UnityEngine;

namespace SaberFactory.Gizmo
{
    internal class RotationGizmo : FactoryDragGizmoBase
    {
        public static Mesh RotationMesh;
        
        protected override Color GizmoColor => Color.cyan.ColorWithAlpha(0.1f);

        protected override Mesh GizmoMesh => RotationMesh;
    }
}