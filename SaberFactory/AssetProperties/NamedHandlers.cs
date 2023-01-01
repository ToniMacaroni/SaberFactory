using System;
using System.Collections.Generic;
using HarmonyLib;

namespace SaberFactory.AssetProperties;

public class NamedHandlers<T>
{
    private readonly Dictionary<string, List<Action<T>>> _registeredHandlers = new();

    private Action<T>[] _flattened;

    public void Register(string name, params Action<T>[] handlers)
    {
        Register(name, true, handlers);
        Flatten();
    }
    
    public void Register(string name, bool shouldUnregister, params Action<T>[] handlers)
    {
        if (shouldUnregister)
        {
            Unregister(name);
        }
        
        if (!_registeredHandlers.ContainsKey(name))
        {
            _registeredHandlers[name] = new List<Action<T>>();
        }
        
        _registeredHandlers[name].AddRange(handlers);
        Flatten();
    }

    public void Unregister(string name)
    {
        _registeredHandlers.Remove(name);
        Flatten();
    }
    
    private void Flatten()
    {
        var list = new List<Action<T>>();
        _registeredHandlers.Do(x => x.Value.Do(y => list.Add(y)));
        _flattened = list.ToArray();
    }

    public void Invoke(string name, T val)
    {
        if (_registeredHandlers.TryGetValue(name, out var handlerList))
        {
            handlerList.Do(x=>x.Invoke(val));
        }
    }

    public void InvokeAll(T val)
    {
        if (_flattened == null)
        {
            return;
        }

        for (int i = 0; i < _flattened.Length; i++)
        {
            _flattened[i].Invoke(val);
        }
    }
}