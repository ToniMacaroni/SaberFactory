using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class FlowToggleBinder
    {
        private List<Tuple<Toggle, UnityAction<bool>>> _bindings;

        private bool _enabled = true;

        public FlowToggleBinder()
        {
            Init();
        }

        private void Init()
        {
            _bindings = new List<Tuple<Toggle, UnityAction<bool>>>();
        }

        public void AddBindings(List<Tuple<Toggle, Action<bool>>> bindingData)
        {
            foreach (Tuple<Toggle, Action<bool>> bindingDatum in bindingData)
            {
                AddBinding(bindingDatum.Item1, bindingDatum.Item2);
            }
        }

        public void AddBinding(Toggle toggle, Action<bool> action)
        {
            UnityAction<bool> unityAction = action.Invoke;
            toggle.onValueChanged.AddListener(unityAction);
            _bindings.Add(toggle, unityAction);
        }

        public void AddBinding(Toggle toggle, bool enabled, Action action)
        {
            if (!toggle)
            {
                return;
            }
            
            UnityAction<bool> unityAction = delegate(bool b)
            {
                if (b == enabled)
                {
                    action();
                }
            };
            toggle.onValueChanged.AddListener(unityAction);
            _bindings.Add(toggle, unityAction);
        }

        public void ClearBindings()
        {
            if (_bindings == null)
            {
                return;
            }

            foreach (Tuple<Toggle, UnityAction<bool>> binding in _bindings)
            {
                Toggle item = binding.Item1;
                if (item != null)
                {
                    item.onValueChanged.RemoveListener(binding.Item2);
                }
            }

            _bindings.Clear();
        }

        public void Disable()
        {
            if (!_enabled)
            {
                return;
            }

            _enabled = false;
            if (_bindings == null)
            {
                return;
            }

            foreach (Tuple<Toggle, UnityAction<bool>> binding in _bindings)
            {
                Toggle item = binding.Item1;
                if (item != null)
                {
                    item.onValueChanged.RemoveListener(binding.Item2);
                }
            }
        }

        public void Enable()
        {
            if (_enabled)
            {
                return;
            }

            _enabled = true;
            if (_bindings == null)
            {
                return;
            }

            foreach (Tuple<Toggle, UnityAction<bool>> binding in _bindings)
            {
                Toggle item = binding.Item1;
                if (item != null)
                {
                    item.onValueChanged.AddListener(binding.Item2);
                }
            }
        }
    }
}