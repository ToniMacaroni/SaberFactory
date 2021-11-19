using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Helpers;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    /// <summary>
    ///     Direct implementation of <see cref="ICustomParsable" />
    /// </summary>
    internal class CustomParsable : MonoBehaviour, ICustomParsable
    {
        public BSMLParserParams ParserParams { get; private set; }

        protected string ResourceName => string.Join(".", GetType().Namespace, GetType().Name);

        [Inject] protected readonly BsmlDecorator BsmlDecorator = null;

        public virtual void Parse()
        {
            ParserParams = BsmlDecorator.ParseFromResource(ResourceName, gameObject, this);
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