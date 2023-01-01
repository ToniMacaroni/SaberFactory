using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FlowUi.Runtime;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SaberFactory.UI.Flow
{
    public partial class SFSaberSelectionVC : FlowViewController
    {
        [SerializeField] private SaberList saberList;
        [SerializeField] private GameObject downloadMoreSabersHint;

        [Space, Header("Controls")]
        [SerializeField] private Button reloadAllButton;
        [SerializeField] private Button reloadButton;
        [SerializeField] private Button presetsButton;
        [SerializeField] private Toggle holdToggle;
        [SerializeField] private FlowSlider saberWidthSlider;
        
        [Space, Header("Popups")]
        [SerializeField] private ChoosePresetPopup choosePresetPopup;


        [Space, Header("Misc")]
        [SerializeField] private ImageView fadedBGImage;
        [SerializeField] private FlowImageBlur imageBlur;

        public void OpenModelSaber()
        {
            Process.Start("https://modelsaber.com/Sabers/?pc");
        }

        public void OpenSaberFolder()
        {
            var dir = new DirectoryInfo("CustomSabers");
            Process.Start("explorer.exe", dir.FullName);
        }
    }
}