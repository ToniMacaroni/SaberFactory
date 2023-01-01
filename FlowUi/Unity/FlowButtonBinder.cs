using System;
using System.Collections.Generic;
using HMUI;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class FlowButtonBinder
    {
        private List<Tuple<Button, UnityAction>> _bindings;

        public FlowButtonBinder()
        {
            Init();
        }

        private void Init()
        {
            _bindings = new List<Tuple<Button, UnityAction>>();
        }

        public void AddBindings(List<Tuple<Button, Action>> bindingData)
        {
            foreach (Tuple<Button, Action> bindingDatum in bindingData)
            {
                AddBinding(bindingDatum.Item1, bindingDatum.Item2);
            }
        }

        public void AddBinding(Button button, Action action)
        {
            if (!button)
            {
                return;
            }
            
            UnityAction unityAction = action.Invoke;
            button.onClick.AddListener(unityAction);
            _bindings.Add(button, unityAction);
        }

        public void ClearBindings()
        {
            if (_bindings == null)
            {
                return;
            }

            foreach (Tuple<Button, UnityAction> binding in _bindings)
            {
                Button item = binding.Item1;
                if (item != null)
                {
                    item.onClick.RemoveListener(binding.Item2);
                }
            }

            _bindings.Clear();
        }
    }
}