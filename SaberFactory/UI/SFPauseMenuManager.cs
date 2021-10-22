using System;
using BeatSaberMarkupLanguage.Tags;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SaberFactory.UI
{
    /// <summary>
    ///     Currently not used but kept just in case
    /// </summary>
    internal class SFPauseMenuManager : IInitializable
    {
        [Inject] private readonly PauseMenuManager _pauseMenuManager = null;

        public void Initialize()
        {
            try
            {
                CreateCheckbox();
            }
            catch (Exception)
            {
            }
        }

        public void CreateCheckbox()
        {
            var canvas = _pauseMenuManager.GetField<LevelBar, PauseMenuManager>("_levelBar")
                .transform
                .parent
                .parent
                .GetComponent<Canvas>();
            if (!canvas)
            {
                return;
            }

            var buttonObj = new ButtonTag().CreateObject(canvas.transform);

            (buttonObj.transform as RectTransform).anchoredPosition = new Vector2(26, -15);
            (buttonObj.transform as RectTransform).sizeDelta = new Vector2(-130, 7);

            buttonObj.GetComponent<Button>().onClick.AddListener(ButtonClick);
        }

        private void ButtonClick()
        {
            Editor.Editor.Instance?.Open();
        }
    }
}