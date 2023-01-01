using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SaberFactory.AssetProperties;
using UnityEngine;

namespace SaberFactory.UI.Flow;

public partial class PropList
{
    public void SetProps(IEnumerable<AssetProperty> props, IEnumerable<string> names)
    {
        ClearCells();
        
        _properties = props.ToList();
        
        foreach (var prop in _properties.Zip(names, (prop, n) => (prop, n)))
        {
            InstantiateCell(prop.n, prop.prop);
        }

        StartCoroutine(UpdateContentSize());
    }

    private IEnumerator UpdateContentSize()
    {
        yield return new WaitForEndOfFrame();
        _scrollView.UpdateContentSize();
    }

    public void InstantiateCell(string propName, AssetProperty prop)
    {
        if (prop is FloatProperty floatProp)
        {
            var cell = Instantiate(_floatCellPrefab, _container, false);
            _instantiatedCells.Add(prop, cell);
            cell.SetProp(propName, floatProp);
            return;
        }
        
        if (prop is BoolProperty boolProp)
        {
            var cell = Instantiate(_boolCellPrefab, _container, false);
            _instantiatedCells.Add(prop, cell);
            cell.SetProp(propName, boolProp);
            return;
        }
    }

    public void ClearCells()
    {
        foreach (var cell in _instantiatedCells.Values)
        {
            DestroyImmediate(cell.gameObject);
        }
        
        _instantiatedCells.Clear();
    }
    private Dictionary<AssetProperty, PropCell> _instantiatedCells = new();
    private List<AssetProperty> _properties;
}