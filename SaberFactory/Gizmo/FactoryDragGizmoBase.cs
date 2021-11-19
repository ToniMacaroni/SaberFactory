using System;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Gizmo
{
    internal abstract class FactoryDragGizmoBase : IFactoryGizmo
    {
        public Color Color => IsHovered ? (CustomColor??GizmoColor).ColorWithAlpha(0.7f) : CustomColor??GizmoColor;

        protected abstract Color GizmoColor { get; }
        
        public Color? CustomColor { get; set; }
        
        protected abstract Mesh GizmoMesh { get; }

        protected bool IsHovered;
        protected bool IsActive;
        protected Action<Vector3> _pollFunction;

        private Vector3? _lastPos;

        /// <summary>
        /// Reset position so the next poll will return a delta of zero
        /// </summary>
        public virtual void Init()
        {
            _lastPos = null;
        }

        public virtual Vector3 GetDelta(Vector3 newPos)
        {
            _lastPos ??= newPos;
            var lastPos = _lastPos.Value;
            _lastPos = newPos;
            return newPos - lastPos;
        }

        public void Draw(Vector3 pos, Quaternion? rotation = null)
        {
            rotation ??= Quaternion.identity;
            
            if (GizmoMesh)
            {
                GizmoDrawer.Draw(GizmoMesh, Color, pos, rotation.Value, Vector3.one*0.1f);
                return;
            }
            
            GizmoDrawer.DrawSphere(pos, 0.05f, Color);
        }

        public void Hover()
        {
            IsHovered = true;
        }

        public void Unhover()
        {
            IsHovered = false;
        }

        /// <summary>
        /// Init the gizmo and activate for interaction
        /// </summary>
        public void Activate()
        {
            Init();
            IsActive = true;
        }

        /// <summary>
        /// Deactivate gizmo for interaction
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
        }

        public void SetPollFunction(Action<Vector3> pollFunction)
        {
            _pollFunction = pollFunction;
        }

        public void Update(Vector3 newPos)
        {
            if (_pollFunction == null || !IsActive)
            {
                return;
            }
            
            _pollFunction.Invoke(GetDelta(newPos));
        }
    }
}