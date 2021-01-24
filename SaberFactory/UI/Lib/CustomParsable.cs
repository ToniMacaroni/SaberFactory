using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class CustomParsable : MonoBehaviour
    {
        public BSMLParserParams ParserParams { get; private set; }

        protected string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        public virtual void Parse()
        {
            ParserParams = UIHelpers.ParseFromResource(_resourceName, gameObject, this);
        }

        public void UnParse()
        {
            foreach (Transform t in transform)
            {
                t.gameObject.TryDestroy();
            }
        }
    }
}