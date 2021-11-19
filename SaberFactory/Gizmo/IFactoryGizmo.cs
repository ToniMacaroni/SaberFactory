using UnityEngine;

namespace SaberFactory.Gizmo
{
    internal interface IFactoryGizmo
    {
        /// <summary>
        /// The color of the gizmo
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Get the delta position between <paramref name="newPos"/> and the last polled position
        /// </summary>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public Vector3 GetDelta(Vector3 newPos);

        /// <summary>
        /// Draw the gizmo at a certain position
        /// </summary>
        /// <param name="pos"></param>
        public void Draw(Vector3 pos, Quaternion? rotation);

        /// <summary>
        /// Notify that the gizmo has been hovered
        /// </summary>
        public void Hover();
        
        /// <summary>
        /// Notify that the gizmo is no longer hovered
        /// </summary>
        public void Unhover();
    }
}