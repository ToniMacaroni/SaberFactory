using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using SaberFactory.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.BSML
{
    public class CustomListTableData : MonoBehaviour, TableView.IDataSource
    {
        public enum ListStyle
        {
            List, Box, Simple
        }

        private ListStyle listStyle = ListStyle.List;

        private LevelListTableCell songListTableCellInstance;
        private AnnotatedBeatmapLevelCollectionTableCell levelPackTableCellInstance;
        private SimpleTextTableCell simpleTextTableCellInstance;

        public List<CustomCellInfo> data = new List<CustomCellInfo>();
        public float cellSize = 8.5f;
        public string reuseIdentifier = "BSMLListTableCell";
        public TableView tableView;

        public bool expandCell = false;

        public ListStyle Style
        {
            get => listStyle;
            set
            {
                //Sets the default cell size for certain styles
                switch (value)
                {
                    case ListStyle.List:
                        cellSize = 8.5f;
                        break;
                    case ListStyle.Box:
                        cellSize = tableView.tableType == TableView.TableType.Horizontal ? 30f : 35f;
                        break;
                    case ListStyle.Simple:
                        cellSize = 8f;
                        break;
                }

                listStyle = value;
            }
        }

        private static readonly Color _heartColor = new Color(0.921f, 0.360f, 0.321f);

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

        public LevelListTableCell GetTableCell()
        {
            LevelListTableCell tableCell = (LevelListTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (songListTableCellInstance == null)
                    songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => (x.name == "LevelListTableCell"));

                tableCell = Instantiate(songListTableCellInstance);

                var t = _favoriteImageAccessor(ref tableCell).gameObject.transform.AsRectTransform();
                t.sizeDelta = new Vector2(5, 5);
                t.anchoredPosition = new Vector2(-8.5f, 0);

                tableCell.SetField("_highlightBackgroundColor", new Color(0.360f, 0.647f, 1, 0.7f));
                tableCell.SetField("_selectedBackgroundColor", new Color(0.360f, 0.647f, 1, 0.9f));
                tableCell.SetField("_selectedAndHighlightedBackgroundColor", new Color(0.360f, 0.647f, 1, 0.9f));
            }

            tableCell.SetField("_notOwned", false);

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public AnnotatedBeatmapLevelCollectionTableCell GetLevelPackTableCell()
        {
            AnnotatedBeatmapLevelCollectionTableCell tableCell = (AnnotatedBeatmapLevelCollectionTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (levelPackTableCellInstance == null)
                    levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<AnnotatedBeatmapLevelCollectionTableCell>().First(x => x.name == "AnnotatedBeatmapLevelCollectionTableCell");

                tableCell = Instantiate(levelPackTableCellInstance);
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public SimpleTextTableCell GetSimpleTextTableCell()
        {
            SimpleTextTableCell tableCell = (SimpleTextTableCell)tableView.DequeueReusableCellForIdentifier(reuseIdentifier);
            if (!tableCell)
            {
                if (simpleTextTableCellInstance == null)
                    simpleTextTableCellInstance = Resources.FindObjectsOfTypeAll<SimpleTextTableCell>().First(x => x.name == "SimpleTextTableCell");

                tableCell = Instantiate(simpleTextTableCellInstance);
            }

            tableCell.reuseIdentifier = reuseIdentifier;
            return tableCell;
        }

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            switch (listStyle)
            {
                case ListStyle.List:
                    LevelListTableCell tableCell = GetTableCell();
                    var cellData = data[idx];

                    var nameText = _songNameTextAccessor(ref tableCell);
                    var authorText = _songAuthorTextAccessor(ref tableCell);
                    var songDurationText = _songDurationTextAccessor(ref tableCell);
                    var songBpmText = _songBpmTextAccessor(ref tableCell);
                    var coverImage = _coverImageAccessor(ref tableCell);
                    var favoriteImage = _favoriteImageAccessor(ref tableCell);

                    (coverImage as ImageView).SetSkew(0);

                    coverImage.sprite = data[idx].Icon == null ? Utilities.LoadSpriteFromTexture(Texture2D.blackTexture) : data[idx].Icon;

                    if (!string.IsNullOrEmpty(cellData.RightText))
                    {
                        songDurationText.gameObject.SetActive(false);
                    }
                    else
                    {
                        songDurationText.text = cellData.RightText;
                    }

                    if (!string.IsNullOrEmpty(cellData.RightBottomText))
                    {
                        songBpmText.gameObject.SetActive(false);
                    }
                    else
                    {
                        songBpmText.text = cellData.RightBottomText;
                    }

                    favoriteImage.enabled = cellData.IsFavorite;

                    if (cellData.IsFavorite)
                    {
                        favoriteImage.color = _heartColor;
                    }

                    tableCell.transform.Find("BpmIcon").gameObject.SetActive(false);

                    if (expandCell)
                    {
                        nameText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                        authorText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                    }

                    nameText.text = data[idx].Text;
                    authorText.text = data[idx].Subtext;

                    return tableCell;
                case ListStyle.Box:
                    AnnotatedBeatmapLevelCollectionTableCell cell = GetLevelPackTableCell();
                    cell.showNewRibbon = false;
                    cell.GetField<TextMeshProUGUI, AnnotatedBeatmapLevelCollectionTableCell>("_infoText").text = $"{data[idx].Text}\n{data[idx].Subtext}";
                    Image packCoverImage = cell.GetField<Image, AnnotatedBeatmapLevelCollectionTableCell>("_coverImage");

                    packCoverImage.sprite = data[idx].Icon == null ? Utilities.LoadSpriteFromTexture(Texture2D.blackTexture) : data[idx].Icon;

                    return cell;
                case ListStyle.Simple:
                    SimpleTextTableCell simpleCell = GetSimpleTextTableCell();
                    simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").richText = true;
                    simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").enableWordWrapping = true;
                    simpleCell.text = data[idx].Text;

                    return simpleCell;
            }

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

        public class CustomCellInfo
        {
            public string Text;
            public string Subtext;
            public Sprite Icon;
            public bool IsFavorite;
            public string RightText;
            public string RightBottomText;
        };
    }
}