using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Macros;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using SiraUtil.Tools;
using Zenject;

namespace SaberFactory.UI.Lib.BSML
{
    class CustomComponentHandler : IInitializable
    {
        private readonly SiraLog _logger;
        public static bool Registered { get; private set; }

        private CustomComponentHandler(SiraLog logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            RegisterAll(BSMLParser.instance);
        }

        private void RegisterAll(BSMLParser parser)
        {
            if(Registered) return;

            foreach (BSMLTag tag in InstantiateOfType<BSMLTag>())
            {
                parser.RegisterTag(tag);
            }

            foreach (BSMLMacro macro in InstantiateOfType<BSMLMacro>())
            {
                parser.RegisterMacro(macro);
            }

            foreach (TypeHandler handler in InstantiateOfType<TypeHandler>())
            {
                parser.RegisterTypeHandler(handler);
            }

            RegisterCustomComponents(parser);

            Registered = true;
        }

        private void RegisterCustomComponents(BSMLParser parser)
        {
            foreach (var type in GetListOfType<CustomUiComponent>())
            {
                parser.RegisterTag(new CustomUiComponentTag(type));
            }

            foreach (var type in GetListOfType<Popup>())
            {
                parser.RegisterTag(new PopupTag(type));
            }
        }

        private List<T> InstantiateOfType<T>(params object[] constructorArgs)
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)) && myType!=typeof(CustomUiComponentTag) && myType!=typeof(PopupTag)))
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));

            return objects;
        }

        private List<Type> GetListOfType<T>()
        {
            var types = new List<Type>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                types.Add(type);

            return types;
        }
    }
}
