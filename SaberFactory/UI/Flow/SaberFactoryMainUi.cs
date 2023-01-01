using System;
using System.Collections.Generic;
using FlowUi.Runtime;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class SaberFactoryMainUi : MonoBehaviour , IPreExport
    {
        [SerializeField] private NoTransitionsButton exitButton;
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private TabController navigationController;

        [SerializeField] private FlowMessageDialog messageDialog;

        [Space] [SerializeField] private List<FlowViewController> NavigationViewControllers;

        public void PreExport()
        {
            foreach (var vc in NavigationViewControllers)
            {
                vc.gameObject.SetActive(false);
            }
        }
    }
}