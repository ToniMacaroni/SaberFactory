using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMUI;
using UnityEngine;
using Screen = HMUI.Screen;

namespace FlowUi.Runtime
{
    public class FlowViewController : ViewController
    {
        public string ViewControllerId;
        public bool RegisterGlobally = true;
        
        [SerializeField]
        protected List<FlowCategoryContainer> categoryContainers;

        protected bool _init;
        protected FlowButtonBinder _buttonBinder = new FlowButtonBinder();
        protected FlowToggleBinder _toggleBinder = new FlowToggleBinder();

        protected bool _isShowing;
        
        protected FlowCategoryContainer _currentCategoryContainer;

        public event Action OnFlowDidDeactivate; 

        protected override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            gameObject.SetActive(true);

            if (!_init)
            {
                await Setup();
                _init = true;
            }

            if (_currentCategoryContainer)
            {
                await _currentCategoryContainer.Open(this);
            }
            
            await DidOpen();
        }

        protected override async void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            if (_currentCategoryContainer)
            {
                await _currentCategoryContainer.Close();
            }
            
            await DidClose();
            OnFlowDidDeactivate?.Invoke();
            gameObject.SetActive(false);
        }

        protected virtual Task Setup()
        {
            return Task.CompletedTask;
        }
        
        protected virtual Task DidOpen()
        {
            return Task.CompletedTask;
        }

        protected virtual Task DidClose()
        {
            return Task.CompletedTask;
        }

        public void Show()
        {
            if (_isShowing)
            {
                return;
            }
            
            DidActivate(true, true, false);
            _isShowing = true;
        }

        public void Hide()
        {
            if (!_isShowing)
            {
                return;
            }
            DidDeactivate(true, false);
            _isShowing = false;
        }
        
        protected async void CategoryCellSelected(SegmentedControl cell, int idx)
        {
            await SelectCategory(idx);
        }

        protected async Task SelectCategory(int id)
        {
            var vc = categoryContainers.FirstOrDefault(x => x.ContainerId == id);
            if (!vc)
            {
                return;
            }

            if (_currentCategoryContainer)
            {
                await _currentCategoryContainer.Close();
            }

            _currentCategoryContainer = vc;
            await vc.Open(this);
        }
    }
}