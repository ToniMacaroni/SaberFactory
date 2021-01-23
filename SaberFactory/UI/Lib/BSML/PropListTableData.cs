using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.BSML
{
    public class PropListTableData : MonoBehaviour, TableView.IDataSource
    {
        private PropCell _propCellPrefab;

        public List<PropertyDescriptor> data = new List<PropertyDescriptor>();
        public float cellSize = 8.5f;
        public string reuseIdentifier = "BSMLListTableCell";
        public TableView tableView;

        #region Accessors

        private static readonly FieldAccessor<LevelListTableCell, Image>.Accessor _favoriteImageAccessor =
            FieldAccessor<LevelListTableCell, Image>.GetAccessor("_favoritesBadgeImage");

        private static readonly FieldAccessor<LevelListTableCell, TextMeshProUGUI>.Accessor _songNameTextAccessor =
            FieldAccessor<LevelListTableCell, TextMeshProUGUI>.GetAccessor("_songNameText");

        private static readonly FieldAccessor<LevelListTableCell, TextMeshProUGUI>.Accessor _songAuthorTextAccessor =
            FieldAccessor<LevelListTableCell, TextMeshProUGUI>.GetAccessor("_songAuthorText");

        private static readonly FieldAccessor<LevelListTableCell, TextMeshProUGUI>.Accessor _songDurationTextAccessor =
            FieldAccessor<LevelListTableCell, TextMeshProUGUI>.GetAccessor("_songDurationText");

        private static readonly FieldAccessor<LevelListTableCell, TextMeshProUGUI>.Accessor _songBpmTextAccessor =
            FieldAccessor<LevelListTableCell, TextMeshProUGUI>.GetAccessor("_songBpmText");

        private static readonly FieldAccessor<LevelListTableCell, Image>.Accessor _coverImageAccessor =
            FieldAccessor<LevelListTableCell, Image>.GetAccessor("_coverImage");

        private static readonly FieldAccessor<LevelListTableCell, Image>.Accessor _backgroundImageAccessor =
            FieldAccessor<LevelListTableCell, Image>.GetAccessor("_backgroundImage");

        #endregion

        public PropCell GetTableCell()
        {
            var tableCell = (PropCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (_propCellPrefab == null)
                {
                    var cellGameObject = new GameObject("teset");
                    tableCell = cellGameObject.AddComponent<PropCell>();
                    BSMLParser.instance.Parse(
                        Utilities.GetResourceContent(Assembly.GetExecutingAssembly(),
                            "SaberFactory.UI.Lib.PropCell.bsml"),
                        cellGameObject, tableCell);
                    _propCellPrefab = tableCell;
                }
                else
                {
                    tableCell = Instantiate(_propCellPrefab);
                }
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            var cellData = data[idx];
            var tableCell = GetTableCell();
            return tableCell;
        }

        public float CellSize()
        {
            return cellSize;
        }

        public int NumberOfCells()
        {
            return data.Count();
        }

        public class PropertyDescriptor
        {
            public string Text;
            public EPropertyType Type;
            public Action<object> Callback;

        };

        public enum EPropertyType
        {
            Text,
            Float,
            Texture,
            Color
        }
    }

    public class PropCell : TableCell
    {

    }
}