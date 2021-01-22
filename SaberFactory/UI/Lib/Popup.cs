using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class Popup : MonoBehaviour
    {
        protected void Show()
        {
            gameObject.SetActive(true);
        }

        protected void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}