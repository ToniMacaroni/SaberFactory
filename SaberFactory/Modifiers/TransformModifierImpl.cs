using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using SaberFactory.Gizmo;
using SaberFactory.Helpers;
using SaberFactory.ProjectComponents;
using SaberFactory.Serialization;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Modifiers
{
    internal class TransformModifierImpl : BaseModifierImpl
    {
        public enum ETransformMode
        {
            Positioning,
            Scaling,
            Rotating
        }

        public static ETransformMode TransformMode { get; set; }

        public FactoryDragGizmoBase CurrentGizmo { get; set; } = new PositionGizmo();

        public override string Name { get; }

        public override string TypeName => "Transform Modifier";

        [HandledValue]
        public Vector3 PositionOffset
        {
            get => _positionOffset;
            set
            {
                _positionOffset = value;
                SetPositionOffset(value);
                if (_positionText != null)
                {
                    _positionText.text = PositionText;
                }
            }
        }

        [HandledValue]
        public Vector3 ScaleOffset
        {
            get => _scaleOffset;
            set
            {
                _scaleOffset = value;
                SetScaleOffset(value);
                if (_scaleText != null)
                {
                    _scaleText.text = ScaleText;
                }
            }
        }

        [HandledValue]
        public float RotationOffset
        {
            get => _rotationOffset;
            set
            {
                _rotationOffset = value;
                SetRotationOffset(value);
                if (_rotationText != null)
                {
                    _rotationText.text = RotationText;
                }
            }
        }

        [UIComponent("position-text")] private readonly TextMeshProUGUI _positionText = null;
        [UIComponent("rotation-text")] private readonly TextMeshProUGUI _rotationText = null;
        [UIComponent("scale-text")] private readonly TextMeshProUGUI _scaleText = null;

        private static readonly Color _defaultButtonColor = new Color(0.086f, 0.090f, 0.101f, 0.8f);

        public static bool LockX { get; set; }
        public static bool LockY { get; set; }
        public static bool LockZ { get; set; }
        public static bool UniformScaling { get; set; }
        public static float Sensitivity { get; set; } = 1;

        public string PositionText => $"Position: {_positionOffset.x:F} {_positionOffset.y:F} {_positionOffset.z:F}";
        public string RotationText => "Rotation: " + _rotationOffset;
        public string ScaleText => $"Scale: {_scaleOffset.x:F} {_scaleOffset.y:F} {_scaleOffset.z:F}";

        private TransformModifier _transformModifier;
        private List<(Transform transform, Vector3 ogPos, Quaternion ogRotation, Vector3 ogScale)> _transforms;

        private Vector3 _positionOffset;
        private Vector3 _scaleOffset;
        private float _rotationOffset;

        private ControllerInteractionHandler _currentController;

        private string _cachedBsml;

        public TransformModifierImpl(TransformModifier transformModifier) : base(transformModifier.Id)
        {
            Name = transformModifier.Name;
        }

        public override void SetInstance(object instance)
        {
            _transformModifier = (TransformModifier)instance;
            if (_transformModifier == null)
            {
                return;
            }

            _transforms = _transformModifier.Objects
                .Select(x => (x.transform, x.transform.localPosition, x.transform.localRotation, x.transform.localScale))
                .ToList();
            Update();
        }

        public override void Reset()
        {
            PositionOffset = Vector3.zero;
            RotationOffset = 0;
            ScaleOffset = Vector3.zero;
        }

        public override string DrawUi()
        {
            return _cachedBsml ??= Readers.ReadResource("SaberFactory.Modifiers.TransformModifierImpl").BytesToString();
        }

        [UIAction("positioning-mode")]
        private void SetPositioningMode()
        {
            TransformMode = ETransformMode.Positioning;

            var controller = Object.FindObjectsOfType<VRController>().FirstOrDefault();
            if (!controller)
            {
                return;
            }

            _currentController = new ControllerInteractionHandler(controller);

            var gizmo = new PositionGizmo();
            gizmo.SetPollFunction(delta =>
            {
                var t = _transforms[0].transform;
                PositionOffset += TransformVector(t, GetVectorWithLockedValues(delta)) * Sensitivity;
            });
            CurrentGizmo = gizmo;
        }

        [UIAction("rotation-mode")]
        private void SetRotationMode()
        {
            TransformMode = ETransformMode.Rotating;

            var controller = Object.FindObjectsOfType<VRController>().FirstOrDefault();
            if (!controller)
            {
                return;
            }

            _currentController = new ControllerInteractionHandler(controller);

            var gizmo = new RotationGizmo();
            gizmo.SetPollFunction(delta => { RotationOffset += delta.x * 200 * Sensitivity; });
            CurrentGizmo = gizmo;
        }

        [UIAction("scaling-mode")]
        private void SetScalingMode()
        {
            TransformMode = ETransformMode.Scaling;

            var controller = Object.FindObjectsOfType<VRController>().FirstOrDefault();
            if (!controller)
            {
                return;
            }

            _currentController = new ControllerInteractionHandler(controller);

            var gizmo = new ScaleGizmo();
            gizmo.SetPollFunction(delta =>
            {
                var t = _transforms[0].transform;
                if (UniformScaling)
                {
                    ScaleOffset += Vector3.Scale(t.parent.worldToLocalMatrix.lossyScale, GetVectorWithLockedValues(Vector3.one * delta.x)) *
                                   Sensitivity;
                }
                else
                {
                    ScaleOffset += t.localToWorldMatrix.rotation * GetVectorWithLockedValues(delta) * Sensitivity;
                    //ScaleOffset += TransformVector(t, GetVectorWithLockedValues(delta)) * Sensitivity;
                }
            });
            CurrentGizmo = gizmo;
        }

        [UIAction("reset-pos")]
        private void ResetPos()
        {
            PositionOffset = Vector3.zero;
        }

        [UIAction("reset-rot")]
        private void ResetRot()
        {
            RotationOffset = 0;
        }

        [UIAction("reset-scale")]
        private void ResetScale()
        {
            ScaleOffset = Vector3.zero;
        }

        private Vector3 GetVectorWithLockedValues(Vector3 vec)
        {
            return Vector3.Scale(vec, new Vector3(LockX ? 0 : 1, LockY ? 0 : 1, LockZ ? 0 : 1));
        }

        private Vector3 TransformVector(Transform t, Vector3 vec)
        {
            //return Vector3.Scale(t.parent.InverseTransformDirection(vec), t.parent.worldToLocalMatrix.lossyScale);
            return t.parent.InverseTransformDirection(Vector3.Scale(vec, t.parent.worldToLocalMatrix.lossyScale));
        }

        private void SetPositionOffset(Vector3 offset)
        {
            if (_transforms == null)
            {
                return;
            }

            foreach (var t in _transforms)
            {
                if (!t.transform)
                {
                    continue;
                }

                t.transform.localPosition = t.ogPos + offset;
            }
        }

        private void SetScaleOffset(Vector3 offset)
        {
            if (_transforms == null)
            {
                return;
            }

            foreach (var t in _transforms)
            {
                if (!t.transform)
                {
                    continue;
                }

                t.transform.localScale = t.ogScale + offset;
            }
        }

        private void SetRotationOffset(float offset)
        {
            if (_transforms == null)
            {
                return;
            }

            foreach (var t in _transforms)
            {
                if (!t.transform)
                {
                    continue;
                }

                t.transform.localRotation = t.ogRotation * Quaternion.Euler(Vector3.forward * offset);
            }
        }

        public override void WasSelected(object[] args)
        {
            switch (TransformMode)
            {
                case ETransformMode.Positioning:
                    SetPositioningMode();
                    break;
                case ETransformMode.Rotating:
                    SetRotationMode();
                    break;
                case ETransformMode.Scaling:
                    SetScalingMode();
                    break;
            }

            if (_positionText != null)
            {
                _positionText.text = PositionText;
            }

            if (_rotationText != null)
            {
                _rotationText.text = RotationText;
            }

            if (_scaleText != null)
            {
                _scaleText.text = ScaleText;
            }
        }

        public override void OnTick()
        {
            if (CurrentGizmo == null || _transforms == null || _transforms.Count < 1)
            {
                return;
            }

            var transform = _transforms[0].transform;

            if (transform == null)
            {
                return;
            }

            var pos = transform.position;
            var trs = Matrix4x4.TRS(pos, transform.parent.rotation, Vector3.one);
            pos = trs.MultiplyPoint(-new Vector3(0.15f, 0, 0));

            if (_currentController != null)
            {
                var isNear = _currentController.IsNear(pos, 0.1f);
                if (isNear)
                {
                    CurrentGizmo.Hover();
                }
                else
                {
                    CurrentGizmo.Unhover();
                }

                if (_currentController.TriggerChangedThisFrame(out var isPressed))
                {
                    if (isNear && isPressed)
                    {
                        CurrentGizmo.Activate();
                    }

                    if (!isPressed)
                    {
                        CurrentGizmo.Deactivate();
                    }
                }

                if (_currentController.IsPressed())
                {
                    CurrentGizmo.Update(_currentController.Position);
                }
            }

            CurrentGizmo.Draw(pos, transform.parent.rotation * Quaternion.Euler(90, 0, 0));
        }
    }
}