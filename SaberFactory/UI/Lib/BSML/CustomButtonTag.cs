using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using SaberFactory.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SaberFactory.UI.Lib.BSML
{
    public class CustomButtonTag : BSMLTag
    {
        private static readonly Color _defaultNormalColor = new Color(0.086f, 0.090f, 0.101f, 0.8f);
        private static readonly Color _defaultHoveredColor = new Color(0.086f, 0.090f, 0.101f);

        public override string[] Aliases => new[] { "this.button" };
        public virtual string PrefabButton => "PracticeButton";

        private Button buttonPrefab;
        public override GameObject CreateObject(Transform parent)
        {
            if (buttonPrefab == null)
                buttonPrefab = Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == PrefabButton));

            Button button = Object.Instantiate(buttonPrefab, parent, false);
            button.name = "BSMLButton";
            button.interactable = true;
            button.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>(true).TryDestroy();

            ExternalComponents externalComponents = button.gameObject.AddComponent<ExternalComponents>();

            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.richText = true;
            externalComponents.components.Add(textMesh);

            Object.Destroy(button.transform.Find("Content").GetComponent<LayoutElement>());

            var bgImage = button.transform.Find("BG").gameObject.GetComponent<ImageView>();

            button.gameObject.GetComponent<ButtonStaticAnimations>().TryDestroy();
            var buttonStateColors = button.gameObject.AddComponent<ButtonStateColors>();
            buttonStateColors.Image = bgImage;
            buttonStateColors.NormalColor = _defaultNormalColor;
            buttonStateColors.HoveredColor = _defaultHoveredColor;
            buttonStateColors.SelectionDidChange(NoTransitionsButton.SelectionState.Normal);

            button.gameObject.GetComponent<NoTransitionsButton>().selectionStateDidChangeEvent +=
                buttonStateColors.SelectionDidChange;

            var buttonImageController = button.gameObject.AddComponent<ButtonImageController>();
            buttonImageController.BackgroundImage = bgImage;
            buttonImageController.LineImage = button.transform.Find("Underline").gameObject.GetComponent<ImageView>();
            buttonImageController.ShowLine(false);

            ContentSizeFitter buttonSizeFitter = button.gameObject.AddComponent<ContentSizeFitter>();
            buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            // miss me with those small ass buttons
            button.gameObject.GetComponent<LayoutElement>().preferredHeight = 10;

            LayoutGroup stackLayoutGroup = button.GetComponentInChildren<LayoutGroup>();
            if (stackLayoutGroup != null)
                externalComponents.components.Add(stackLayoutGroup);

            return button.gameObject;
        }
    }
}