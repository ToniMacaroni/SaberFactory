using System;
using System.Collections.Generic;
using FlowUi.Runtime;
using HMUI;
using SaberFactory.AssetProperties;
using UnityEngine;

namespace SaberFactory.UI.Flow;

public class SliderPropertyBinder
{
    private readonly Dictionary<FlowSlider, Binding> _bindings = new();

    public void Bind(FlowSlider slider, FloatProperty prop)
    {
        Unregister(slider);

        if (prop.HasRange)
        {
            slider.Min = prop.Min!.Value;
            slider.Max = prop.Max!.Value;
        }
        
        if(prop.HasIncrement)
        {
            slider.Increment = prop.Increment!.Value;
            if (prop.Increment == 1)
            {
                slider.Format = "{0:0}";
            }
        }
        
        slider.Value = prop.Value;

        var newBinding = new Binding(slider);
        newBinding.Register(val =>
        {
            prop.Value = val;
        });
        
        _bindings[slider] = newBinding;
    }

    public void Bind(FlowSlider slider, IntProperty prop)
    {
        Unregister(slider);

        if (prop.HasRange)
        {
            slider.Min = prop.Min!.Value;
            slider.Max = prop.Max!.Value;
        }

        if (prop.HasIncrement)
        {
            slider.Increment = prop.Increment!.Value;
        }

        slider.Format = "{0:0}";

        slider.Value = prop.Value;

        var newBinding = new Binding(slider);
        newBinding.Register(val =>
        {
            prop.Value = (int)val;
        });
        
        _bindings[slider] = newBinding;
    }

    public void Unregister(FlowSlider slider)
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
        private readonly FlowSlider _slider;
        private Action<float> _action;

        public Binding(FlowSlider slider)
        {
            _slider = slider;
        }

        public void Register(Action<float> action)
        {
            _action = action;
            _slider.OnSnappedValueChanged += _action;
        }

        public void Unregister()
        {
            _slider.OnSnappedValueChanged -= _action;
        }
    }
}