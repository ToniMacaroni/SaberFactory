using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    /// <summary>
    /// Direct implementation of <see cref="ICustomParsable"/>
    /// </summary>
    internal class CustomParsable : MonoBehaviour, ICustomParsable
    {
        public BSMLParserParams ParserParams { get; private set; }

        protected string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        public virtual void Parse()
        {
            ParserParams = UIHelpers.ParseFromResource(ResourceName, gameObject, this);
        }

        public void Unparse()
        {
            foreach (Transform t in transform)
            {
                t.gameObject.TryDestroy();
            }
        }
    }
}