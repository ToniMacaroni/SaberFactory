using System;
using System.Linq;
using System.Threading.Tasks;
using FlowUi.Runtime;
using HMUI;
using ModestTree;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Flow;

public partial class SaberFactoryMainUi : MonoBehaviour
{
    [Inject] private readonly Prefab.Editor _editor = null;
    
    private bool _init;

    private FlowViewController _currentViewController;

    private void Awake()
    {
        if (!_init)
        {
            exitButton.onClick.AddListener(ExitButtonClicked);
            navigationController.OnTabSelected += SwitchTab;
            _init = true;
        }

        SwitchTab("SaberSelection");
    }

    private void OnEnable()
    {
        _editor.Open();
        if (_currentViewController)
        {
            _currentViewController.Show();
        }
    }

    private void SwitchTab(TabData tab)
    {
        SwitchTab(tab.Key);
    }

    private void SwitchTab(string key)
    {
        var vc = NavigationViewControllers.FirstOrDefault(x => x.ViewControllerId == key);
        
        if (!vc)
        {
            return;
        }

        if (_currentViewController)
        {
            _currentViewController.Hide();
        }

        _currentViewController = vc;
        vc.Show();
    }

    public async Task<FlowCustomDialog.DialogResult> ShowMessagebox(string message, string title, bool centered = true)
    {
        return await messageDialog.Show(message, title, centered);
    }

    private async void ExitButtonClicked()
    {
        if (_currentViewController)
        {
            _currentViewController.Hide();
        }
        await _editor.Close();
        GetComponent<UIRoot>().RequestExit();
    }
}