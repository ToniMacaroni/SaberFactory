using UnityEngine;

namespace SaberFactory.Gizmo
{
    internal class ControllerInteractionHandler
    {
        public Vector3 Position => _controller.position;
        
        private readonly VRController _controller;
        private bool _wasPressed;

        public ControllerInteractionHandler(VRController controller)
        {
            _controller = controller;
        }

        public bool IsPressed()
        {
            return _controller.triggerValue > 0.5f;
        }

        public bool IsNear(Vector3 pos, float thresh)
        {
            return Vector3.Distance(Position, pos) < thresh;
        }

        public bool IsNearAndPressed(Vector3 pos, float thresh)
        {
            return IsNear(pos, thresh) && IsPressed();
        }

        public bool TriggerChangedThisFrame(out bool isPressed)
        {
            isPressed = false;
            var pressed = IsPressed();
            if (!_wasPressed && pressed)
            {
                _wasPressed = true;
                isPressed = true;
                return true;
            }

            if (_wasPressed && !pressed)
            {
                _wasPressed = false;
                isPressed = false;
                return true;
            }

            return false;
        }
    }
}