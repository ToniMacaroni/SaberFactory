using System;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using UnityEngine;

namespace SaberFactory.UI.Lib.BSML
{
    internal class BSMLCustomUIComponentTag : BSMLTag
    {
        public override string[] Aliases => new[] {"component"};

        public override GameObject CreateObject(Transform parent)
        {
            throw new NotImplementedException();
        }
    }
}