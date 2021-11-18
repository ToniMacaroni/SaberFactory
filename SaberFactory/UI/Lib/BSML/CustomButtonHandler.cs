using System;
using System.Collections.Generic;
using System.Diagnostics;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;

namespace SaberFactory.UI.Lib.BSML
{
    [ComponentHandler(typeof(NoTransitionsButton))]
    public class CustomButtonHandler : TypeHandler<NoTransitionsButton>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "run", new[] { "run" } }
        };

        public override Dictionary<string, Action<NoTransitionsButton, string>> Setters => new Dictionary<string, Action<NoTransitionsButton, string>>
        {
            { "run", SetRunAction}
        };

        private void SetRunAction(NoTransitionsButton button, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Process.Start(value);
            });
        }
    }
}