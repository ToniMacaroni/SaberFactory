using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Macros;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using SaberFactory.UI.Lib.BSML.Tags;
using SiraUtil.Tools;
using Zenject;

namespace SaberFactory.UI.Lib.BSML
{
    internal class CustomComponentHandler : IInitializable
    {
        public static bool Registered { get; private set; }
        private readonly SiraLog _logger;

        private CustomComponentHandler(
            SiraLog logger,
            Popup.Factory popupFactory,
            CustomUiComponent.Factory customUiComponentFactory)
        {
            _logger = logger;
            _popupFactory = popupFactory;
            _customUiComponentFactory = customUiComponentFactory;
            RegisterAll(BSMLParser.instance);
        }

        public void Initialize()
        {
        }

        private void RegisterAll(BSMLParser parser)
        {
            if (Registered) return;

            foreach (var tag in InstantiateOfType<BSMLTag>()) parser.RegisterTag(tag);

            foreach (var macro in InstantiateOfType<BSMLMacro>()) parser.RegisterMacro(macro);

            foreach (var handler in InstantiateOfType<TypeHandler>()) parser.RegisterTypeHandler(handler);

            RegisterCustomComponents(parser);

            _logger.Info("Registered Custom Components");

            Registered = true;
        }

        private void RegisterCustomComponents(BSMLParser parser)
        {
            foreach (var type in GetListOfType<CustomUiComponent>()) parser.RegisterTag(new CustomUiComponentTag(type, _customUiComponentFactory));

            foreach (var type in GetListOfType<Popup>()) parser.RegisterTag(new PopupTag(type, _popupFactory));
        }

        private List<T> InstantiateOfType<T>(params object[] constructorArgs)
        {
            var objects = new List<T>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(myType =>
                myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)) && myType != typeof(CustomUiComponentTag) &&
                myType != typeof(PopupTag)))
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));

            return objects;
        }

        private List<Type> GetListOfType<T>()
        {
            var types = new List<Type>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                types.Add(type);

            return types;
        }

        #region Factories

        private readonly Popup.Factory _popupFactory;
        private readonly CustomUiComponent.Factory _customUiComponentFactory;

        #endregion
    }
}