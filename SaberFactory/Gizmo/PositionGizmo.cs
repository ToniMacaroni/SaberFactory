using UnityEngine;

namespace SaberFactory.Gizmo
{
    internal class PositionGizmo : FactoryDragGizmoBase
    {
        public static Mesh PositionMesh;
        
        protected override Color GizmoColor => Color.green.ColorWithAlpha(0.1f);

        protected override Mesh GizmoMesh => PositionMesh;
    }
}