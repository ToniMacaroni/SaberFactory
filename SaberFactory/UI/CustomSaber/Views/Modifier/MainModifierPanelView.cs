using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using SaberFactory.Editor;
using SaberFactory.Helpers;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.CustomSaber.Views
{
    internal class MainModifierPanelView : SubView, INavigationCategoryView
    {
        [Inject] private readonly EditorInstanceManager _instanceManager = null;
        [Inject] private readonly BsmlDecorator _decorator = null;
        
        [UIObject("container")] private readonly GameObject _container = null;
        [UIComponent("component-list")] private readonly CustomListTableData _componentList = null;

        public ENavigationCategory Category => ENavigationCategory.Modifier;

        private ModifyableComponentManager _modifyableComponentManager;
        private List<BaseComponentModifier> _mods;
        private BaseComponentModifier _selectedMod;

        private void Awake()
        {
        }

        [UIAction("#post-parse")]
        private void Setup()
        {
        }

        public override void DidOpen()
        {
            _modifyableComponentManager = _instanceManager.CurrentPiece?.Model.ModifyableComponentManager;
            if (_modifyableComponentManager is { })
            {
                SetupMod();
            }
        }

        public override void DidClose()
        {
            _modifyableComponentManager = null;
            _mods = null;
        }

        public void SetupMod()
        {
            var list = new List<CustomListTableData.CustomCellInfo>();
            _mods = _modifyableComponentManager.GetAllModifiers().ToList();
            
            foreach (var mod in _mods)
            {
                list.Add(new CustomListTableData.CustomCellInfo(mod.GoName, mod.TypeName));
            }

            _componentList.data = list;
            _componentList.tableView.ReloadData();

            if (_mods.Count > 0)
            {
                _componentList.tableView.SelectCellWithIdx(0, true);
            }
        }

        private void ClearCurrentView()
        {
            for (int i = 0; i < _container.transform.childCount; i++)
            {
                Destroy(_container.transform.GetChild(0).gameObject);
            }
        }

        [UIAction("component-selected")]
        private void ComponentSelected(TableView table, int idx)
        {
            _selectedMod = _mods[idx];
            ClearCurrentView();
            _decorator.ParseFromString(_selectedMod.DrawUi(), _container, _selectedMod);
        }
    }
}