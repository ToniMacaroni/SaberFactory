using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Serialization;

namespace SaberFactory.Modifiers
{
    internal abstract class BaseModifierImpl : UpdatableSerialializable, IStringUiProvider
    {
        public abstract string Name { get; }
        public abstract string TypeName { get; }
        
        public BSMLParserParams ParserParams { get; set; }
        
        public abstract string DrawUi();
        
        public int Id { get; }

        protected BaseModifierImpl(int id)
        {
            Id = id;
        }

        public abstract void SetInstance(object instance);

        public abstract void Reset();

        public virtual void WasSelected(params object[] args){}

        public virtual void OnTick(){}
    }
}