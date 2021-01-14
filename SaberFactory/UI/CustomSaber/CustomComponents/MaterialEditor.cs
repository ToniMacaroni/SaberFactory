using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using SaberFactory.Editor;
using SaberFactory.Instances;
using SaberFactory.UI.Lib;
using SiraUtil.Tools;
using Zenject;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class MaterialEditor : SubView
    {
        private BasePieceInstance _currentBasePieceInstance;

        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;

        public override void DidOpen()
        {
        }

        public override void DidClose()
        {
        }

        public void SetMaterials()
        {

        }
    }
}
