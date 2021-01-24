using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class CustomParsable : MonoBehaviour
    {
        public BSMLParserParams ParserParams { get; private set; }

        private string _resourcePath;

        public virtual void Parse(string resourcePath)
        {
            _resourcePath = resourcePath;
            ParserParams = UIHelpers.ParseFromResource(resourcePath, gameObject, this);
        }

        public void Reparse()
        {
            Parse(_resourcePath);
        }
    }
}