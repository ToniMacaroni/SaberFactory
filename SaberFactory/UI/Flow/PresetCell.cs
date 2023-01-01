using System;
using FlowUi.Runtime;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow
{
    public partial class PresetCell : FlowTableCell
    {
        [SerializeField] private TextMeshProUGUI nameTextmesh;
        [SerializeField] private Button deleteButton;
        [SerializeField] private ToggleWithCallbacks monitorToggle;
        
        public Action OnDeletePressed;
        public Action<bool> OnMonitorModeChanged;
    }
}