using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using SaberFactory.Helpers;
using SaberFactory.UI.Lib.PropCells;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    public class PropListTableData : MonoBehaviour, TableView.IDataSource
    {
        public List<PropertyDescriptor> data = new List<PropertyDescriptor>();
        public float cellSize = 16f;
        public string reuseIdentifier = "BSMLListTableCell";
        public TableView tableView;

        private readonly List<GameObject> _spawnedCells = new List<GameObject>();

        private static readonly FieldAccessor<TableView, List<TableCell>>.Accessor _visibleCellsAccessor =
            FieldAccessor<TableView, List<TableCell>>.GetAccessor("_visibleCells");

        public BasePropCell GetTableCell(PropertyDescriptor data)
        {
            var cellGameObject = new GameObject("test");

            var cellType = data.Type switch
            {
                EPropertyType.Float => typeof(FloatPropCell),
                EPropertyType.Bool => typeof(BoolPropCell),
                EPropertyType.Color => typeof(ColorPropCell),
                EPropertyType.Texture => typeof(TexturePropCell),
                _ => throw new ArgumentOutOfRangeException()
            };

            var tableCell = (BasePropCell)cellGameObject.AddComponent(cellType);

            BSMLParser.instance.Parse(
                Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), tableCell.ContentLocation),
                cellGameObject, tableCell);

            _spawnedCells.Add(cellGameObject);

            return tableCell;
        }

        public void Clear()
        {
            foreach (var cellGameObject in _spawnedCells)
            {
                cellGameObject.TryDestroy();
            }
            _spawnedCells.Clear();
            var visibleCells = _visibleCellsAccessor(ref tableView);
            visibleCells.Clear();
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            var cellData = data[idx];
            var tableCell = GetTableCell(cellData);
            return null;
        }

        public float CellSize()
        {
            return cellSize;
        }

        public int NumberOfCells()
        {
            return data.Count();
        }
    }

    public enum EPropertyType
    {
        Unhandled,
        Text,
        Float,
        Bool,
        Texture,
        Color
    }
}