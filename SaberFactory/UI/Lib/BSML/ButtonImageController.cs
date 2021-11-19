using BeatSaberMarkupLanguage;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Lib.BSML
{
    public class ButtonImageController : MonoBehaviour
    {
        public ImageView BackgroundImage;
        public ImageView ForegroundImage;
        public ImageView LineImage;

        public void SetIcon(string path)
        {
            if (ForegroundImage == null)
            {
                return;
            }

            ForegroundImage.SetImage(path);
        }

        public void SetIconColor(string colorString)
        {
            if (ForegroundImage is null)
            {
                return;
            }

            if (ColorUtility.TryParseHtmlString(colorString, out var color))
            {
                ForegroundImage.color = color;
            }
        }

        public void SetBackgroundIcon(string path)
        {
            if (BackgroundImage == null)
            {
                return;
            }

            BackgroundImage.SetImage(path);
        }

        public void ShowLine(bool show)
        {
            if (LineImage == null)
            {
                return;
            }

            LineImage.gameObject.SetActive(show);
        }

        public void SetLineColor(string colorString)
        {
            if (LineImage is null)
            {
                return;
            }

            if (!ColorUtility.TryParseHtmlString(colorString, out var color))
            {
                return;
            }

            LineImage.color = color;
        }

        public void SetIconPad(string sizeStr)
        {
            if (ForegroundImage is null)
            {
                return;
            }

            var size = int.Parse(sizeStr);
            ForegroundImage.transform.parent.GetComponent<StackLayoutGroup>().padding = new RectOffset(size, size, size, size);
        }
    }
}