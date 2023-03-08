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
            List,
            Box,
            Simple
        }

        private const string ReuseIdentifier = "BSMLListTableCell";

        private static readonly Color HeartColor = new Color(0.921f, 0.360f, 0.321f);

        private static readonly Sprite FolderSprite =
            Utilities.FindSpriteInAssembly("SaberFactory.Resources.Icons.folder.png");

        public ListStyle Style
        {
            get => _listStyle;
            set
            {
                //Sets the default cell size for certain styles
                switch (value)
                {
                    case ListStyle.List:
                        cellSize = 8.5f;
                        break;
                    case ListStyle.Box:
                        cellSize = TableView.tableType == TableView.TableType.Horizontal ? 30f : 35f;
                        break;
                    case ListStyle.Simple:
                        cellSize = 8f;
                        break;
                }

                _listStyle = value;
            }
        }

        public float cellSize = 8.5f;

        public List<CustomCellInfo> Data = new List<CustomCellInfo>();

        public bool ExpandCell;
        public TableView TableView;

        private ListStyle _listStyle = ListStyle.List;

        //private AnnotatedBeatmapLevelCollectionTableCell _levelPackTableCellInstance;
        private SimpleTextTableCell _simpleTextTableCellInstance;

        private LevelListTableCell _songListTableCellInstance;

        public virtual TableCell CellForIdx(TableView tableView, int idx)
        {
            switch (_listStyle)
            {
                case ListStyle.List:
                    var tableCell = GetTableCell();
                    var cellData = Data[idx];

                    var nameText = _songNameTextAccessor(ref tableCell);
                    var authorText = _songAuthorTextAccessor(ref tableCell);
                    var songDurationText = _songDurationTextAccessor(ref tableCell);
                    var songBpmText = _songBpmTextAccessor(ref tableCell);
                    var coverImage = _coverImageAccessor(ref tableCell);
                    var favoriteImage = _favoriteImageAccessor(ref tableCell);
                    var bg = _backgroundImageAccessor(ref tableCell).Cast<ImageView>();

                    (coverImage as ImageView).SetSkew(0);

                    nameText.color = cellData.IsCategory ? Color.red : Color.white;

                    if (cellData.IsCategory)
                    {
                        nameText.color = HeartColor;
                        nameText.rectTransform.anchoredPosition = nameText.rectTransform.anchoredPosition.With(null, 0);
                    }
                    else
                    {
                        nameText.color = Color.white;
                        nameText.rectTransform.anchoredPosition = nameText.rectTransform.anchoredPosition.With(null, 1.14f);
                    }

                    if (cellData.Icon is null)
                    {
                        if (cellData.IsCategory)
                        {
                            coverImage.gameObject.SetActive(true);
                            coverImage.sprite = FolderSprite;
                        }
                        else
                        {
                            coverImage.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        coverImage.gameObject.SetActive(true);
                        coverImage.sprite = cellData.Icon;
                    }

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
                        favoriteImage.color = HeartColor;
                    }

                    tableCell.transform.Find("BpmIcon").gameObject.SetActive(false);

                    if (ExpandCell)
                    {
                        nameText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                        authorText.rectTransform.anchorMax = new Vector3(2, 0.5f, 0);
                    }

                    nameText.text = Data[idx].Text;
                    authorText.text = Data[idx].Subtext;

                    return tableCell;
                // case ListStyle.Box:
                //     AnnotatedBeatmapLevelCollectionTableCell cell = GetLevelPackTableCell();
                //     cell.showNewRibbon = false;
                //     cell.GetField<TextMeshProUGUI, AnnotatedBeatmapLevelCollectionTableCell>("_infoText").text = $"{Data[idx].Text}\n{Data[idx].Subtext}";
                //     Image packCoverImage = cell.GetField<Image, AnnotatedBeatmapLevelCollectionTableCell>("_coverImage");
                //
                //     packCoverImage.sprite = Data[idx].Icon == null ? Utilities.LoadSpriteFromTexture(Texture2D.blackTexture) : Data[idx].Icon;
                //
                //     return cell;
                case ListStyle.Simple:
                    var simpleCell = GetSimpleTextTableCell();
                    simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").richText = true;
                    simpleCell.GetField<TextMeshProUGUI, SimpleTextTableCell>("_text").enableWordWrapping = true;
                    simpleCell.text = Data[idx].Text;

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
            return Data.Count();
        }

        public LevelListTableCell GetTableCell()
        {
            var tableCell = (LevelListTableCell)TableView.DequeueReusableCellForIdentifier(ReuseIdentifier);
            if (!tableCell)
            {
                if (_songListTableCellInstance == null)
                {
                    _songListTableCellInstance = Resources.FindObjectsOfTypeAll<LevelListTableCell>().First(x => x.name == "LevelListTableCell");
                }

                tableCell = Instantiate(_songListTableCellInstance);

                var t = _favoriteImageAccessor(ref tableCell).gameObject.transform.AsRectTransform();
                t.sizeDelta = new Vector2(5, 5);
                t.anchoredPosition = new Vector2(-8.5f, 0);
                
                tableCell.SetField("_highlightBackgroundColor", Color.white.ColorWithAlpha(0.3f));
                tableCell.SetField("_selectedBackgroundColor", Color.white.ColorWithAlpha(0.7f));
                tableCell.SetField("_selectedAndHighlightedBackgroundColor", Color.white.ColorWithAlpha(0.7f));
                
                // Promo stuff
                tableCell.GetField<GameObject, LevelListTableCell>("_promoBackgroundGo").SetActive(false);
                tableCell.GetField<GameObject, LevelListTableCell>("_promoBadgeGo").SetActive(false);
                tableCell.GetField<GameObject, LevelListTableCell>("_updatedBadgeGo").SetActive(false);
                
                tableCell.GetField<LayoutWidthLimiter, LevelListTableCell>("_layoutWidthLimiter").limitWidth = false;
                
                var bg = _backgroundImageAccessor(ref tableCell).Cast<ImageView>();
                bg.SetSkew(0);
                bg.color1 = new Color(0.875f, 0.086f, 0.435f);
                bg.color0 = new Color(0.047f, 0.471f, 0.949f);
                _songAuthorTextAccessor(ref tableCell).richText = true;
            }

            tableCell.SetField("_notOwned", false);

            tableCell.reuseIdentifier = ReuseIdentifier;
            return tableCell;
        }

        // public AnnotatedBeatmapLevelCollectionTableCell GetLevelPackTableCell()
        // {
        //     var tableCell = (AnnotatedBeatmapLevelCollectionTableCell)TableView.DequeueReusableCellForIdentifier(ReuseIdentifier);
        //     if (!tableCell)
        //     {
        //         if (_levelPackTableCellInstance == null)
        //             _levelPackTableCellInstance = Resources.FindObjectsOfTypeAll<AnnotatedBeatmapLevelCollectionTableCell>().First(x => x.name == "AnnotatedBeatmapLevelCollectionTableCell");
        //
        //         tableCell = Instantiate(_levelPackTableCellInstance);
        //     }
        //
        //     tableCell.reuseIdentifier = ReuseIdentifier;
        //     return tableCell;
        // }

        public SimpleTextTableCell GetSimpleTextTableCell()
        {
            var tableCell = (SimpleTextTableCell)TableView.DequeueReusableCellForIdentifier(ReuseIdentifier);
            if (!tableCell)
            {
                if (_simpleTextTableCellInstance == null)
                {
                    _simpleTextTableCellInstance = Resources.FindObjectsOfTypeAll<SimpleTextTableCell>().First(x => x.name == "SimpleTextTableCell");
                }

                tableCell = Instantiate(_simpleTextTableCellInstance);
            }

            tableCell.reuseIdentifier = ReuseIdentifier;
            return tableCell;
        }

        public class CustomCellInfo
        {
            public Sprite Icon;
            public bool IsCategory;
            public bool IsFavorite;
            public string RightBottomText;
            public string RightText;
            public string Subtext;
            public string Text;
        }

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
    }
}