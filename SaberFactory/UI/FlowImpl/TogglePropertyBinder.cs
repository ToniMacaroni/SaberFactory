using System;
using System.Collections.Generic;
using FlowUi.Runtime;
using HMUI;
using SaberFactory.AssetProperties;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow;

public class TogglePropertyBinder
{
    private readonly Dictionary<Toggle, Binding> _bindings = new();

    public void Bind(Toggle toggle, BoolProperty prop)
    {
        Unregister(toggle);

        toggle.isOn = prop.Value;

        var newBinding = new Binding(toggle);
        newBinding.Register(val =>
        {
            prop.Value = val;
        });
        
        _bindings[toggle] = newBinding;
    }

    public void Unregister(Toggle slider)
    {
        if (_bindings.TryGetValue(slider, out var binding))
        {
            binding.Unregister();
        }
    }

    public void UnregisterAll()
    {
        foreach (var binding in _bindings.Values)
        {
            binding.Unregister();
        }
        
        _bindings.Clear();
    }

    internal class Binding
    {
        private readonly Toggle _toggle;
        private UnityAction<bool> _action;

        public Binding(Toggle toggle)
        {
            _toggle = toggle;
        }

        public void Register(UnityAction<bool> action)
        {
            _action = action;
            _toggle.onValueChanged.AddListener(_action);
        }

        public void Unregister()
        {
            _toggle.onValueChanged.RemoveListener(_action);
        }
    }
}