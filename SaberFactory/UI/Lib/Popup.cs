using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.UI.Lib
{
    internal class Popup : CustomParsable
    {
        private Transform _originalParent;

        protected void Show()
        {
            Parse();
        }

        protected void Hide()
        {
            Unparse();

            if (_originalParent)
            {
                transform.SetParent(_originalParent, false);
            }
        }

        protected void ParentToViewController()
        {
            _originalParent = transform.parent;

            var parent = _originalParent;
            if (parent.GetComponent<CustomViewController>() != null) return;



            while (parent != null)
            {
                var vc = parent.GetComponent<CustomViewController>();
                if (vc != null)
                {
                    break;
                }

                parent = parent.parent;
            }

            transform.SetParent(parent, false);
        }

        internal class Factory : ComponentPlaceholderFactory<Popup>
        {
        }
    }
}