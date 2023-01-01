using BeatSaberMarkupLanguage.MenuButtons;
using FlowUi.Helpers;
using UnityEngine;
using Zenject;

namespace FlowUi.Registration;

public class MenuButtonRegistration : IInitializable, IDisposable
{
    private readonly InitData _initData;
    private readonly DiContainer _container;

    private MenuButton _button;
    private FlowUiRegistration _uiInstance;

    public MenuButtonRegistration(InitData initData, DiContainer container)
    {
        _initData = initData;
        _container = container;
    }

    public void Initialize()
    {
        if (!MenuButtons.instance)
        {
            return;
        }
            
        _uiInstance = _container.Resolve<List<FlowUiRegistration>>().FirstOrDefault(x=>x.UIType == _initData.VcType);
        _button = new MenuButton(_initData.Text, _initData.Hint, OnClick);
            
        MenuButtons.instance.RegisterButton(_button);
        Debug.Log("Added Button");
    }

    public void Dispose()
    {
        if (_button != null && MenuButtons.instance)
        {
            MenuButtons.instance.UnregisterButton(_button);
        }
    }

    private void OnClick()
    {
        _uiInstance.Present();
    }

    public struct InitData
    {
        public Type VcType;
        public string Text;
        public string Hint;

        public InitData(string text, string hint, Type vcType)
        {
            VcType = vcType;
            Text = text;
            Hint = hint;
        }
    }
}