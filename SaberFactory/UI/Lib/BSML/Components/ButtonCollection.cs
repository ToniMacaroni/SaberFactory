using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Lib.BSML.Components
{
    public class ButtonCollection : MonoBehaviour
    {
        public TextSegmentedControl textSegmentedControl;
        private BSMLParserParams parserParams;

        private bool shouldRefresh;

        private void OnEnable()
        {
            if (shouldRefresh)
                Refresh();
        }

        public event Action<int> OnButtonClicked;

        public void Setup()
        {
            Refresh();
            textSegmentedControl.didSelectCellEvent -= TabSelected;
            textSegmentedControl.didSelectCellEvent += TabSelected;
            textSegmentedControl.SelectCellWithNumber(0);
            TabSelected(textSegmentedControl, 0);
        }

        public void SetTexts(IReadOnlyList<string> texts)
        {
            textSegmentedControl.SetTexts(texts);
        }

        private void TabSelected(SegmentedControl segmentedControl, int index)
        {
            OnButtonClicked?.Invoke(index);
        }

        public void Refresh()
        {
            if (!isActiveAndEnabled)
            {
                shouldRefresh = true;
                return;
            }

            shouldRefresh = false;

            TabSelected(null, 0);
        }

        [ComponentHandler(typeof(ButtonCollection))]
        public class ButtonCollectionHandler : TypeHandler<ButtonCollection>
        {
            public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
            {
                //{ "tabTag", new[]{"tab-tag"} },
            };

            public override Dictionary<string, Action<ButtonCollection, string>> Setters => new Dictionary<string, Action<ButtonCollection, string>>
            {
                //{"pageCount", new Action<ButtonCollection,string>((component, value) => component.PageCount = Parse.Int(value)) },
            };

            public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
            {
                base.HandleType(componentType, parserParams);
                var buttonCollection = componentType.component as ButtonCollection;
                buttonCollection.SetTexts(new[] { "t1", "t2", "t3" });
                buttonCollection.parserParams = parserParams;
                parserParams.AddEvent("post-parse", buttonCollection.Setup);
            }
        }
    }
}