using System;
using IPA.Config.Data;
using Newtonsoft.Json;

namespace SaberFactory.AssetProperties;

public abstract class AssetProperty
{
    public abstract object GetSerializableObject();
    public abstract void SetSerializableObject(object obj);

    public abstract void SetSerializableObject(JsonSerializer serializer, JsonReader reader);
    
    public virtual void Revert()
    {
    }

    public virtual void RevertWithoutInvoke()
    {
    }
}

public class AssetProperty<T> : AssetProperty
{
    protected NamedHandlers<T> _handlers = new();

    protected T _defaultValue;
    protected T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (value.Equals(_value))
            {
                return;
            }

            _value = value;
            DidChangeValue();
            _handlers.InvokeAll(_value);
        }
    }

    public T DefaultValue => _defaultValue;

    protected AssetProperty(T val)
    {
        _defaultValue = val;
        _value = val;
    }

    public virtual void SetWithoutInvoke(T val)
    {
        _value = val;
    }

    public void CopyFrom(AssetProperty<T> other, bool withDefault = false)
    {
        _value = other._value;
        if (withDefault)
        {
            _defaultValue = other._defaultValue;
        }
    }

    public void SetDefault(T val)
    {
        _defaultValue = val;
    }

    public override void Revert()
    {
        Value = _defaultValue;
    }

    public override void RevertWithoutInvoke()
    {
        _value = _defaultValue;
    }

    public void InvokeValueChange()
    {
        _handlers.InvokeAll(_value);
    }

    protected virtual void DidChangeValue(){}
    
    public override object GetSerializableObject()
    {
        return _value;
    }

    public override void SetSerializableObject(object obj)
    {
        if (obj is T casted)
        {
            Value = casted;
        }
    }

    public override void SetSerializableObject(JsonSerializer serializer, JsonReader reader)
    {
        Value = serializer.Deserialize<T>(reader);
    }

    public void RegisterHandler(string name, params Action<T>[] handlers)
    {
        _handlers.Register(name, handlers);
    }
    
    public void RegisterHandler(string name, bool shouldUnregister, params Action<T>[] handlers)
    {
        _handlers.Register(name, shouldUnregister, handlers);
    }

    public void UnregisterHandler(string name)
    {
        _handlers.Unregister(name);
    }
}