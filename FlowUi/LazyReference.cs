using UnityEngine;
using Object = UnityEngine.Object;

namespace FlowUi;

public class LazyReference<T> where T : Object
{
    private readonly string _name;
        
    private T _cachedObject;

    public T Asset
    {
        get
        {
            if (!_cachedObject)
            {
                _cachedObject = GetObject();
            }

            return _cachedObject;
        }
    }

    public LazyReference(string name)
    {
        _name = name;
    }

    private T GetObject()
    {
        var obj = Resources.FindObjectsOfTypeAll<T>().Where(x => x.name == _name).FirstOrDefault();
        if (!obj)
        {
            throw new Exception($"Couldn't find reference for {_name} ({typeof(T).Name})");
        }

        return obj;
    }
}