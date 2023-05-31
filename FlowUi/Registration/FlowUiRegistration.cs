using System.Reflection;
using FlowUi.Runtime;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace FlowUi.Registration;

public class FlowUiRegistration : IInitializable, IDisposable
{
    private readonly Assembly _assembly;
    private readonly string _path;
    private readonly BaseGameUiHandler _uiHandler;
    private readonly DiContainer _container;
    private AssetBundle _bundle;
    private GameObject _uiPrefab;
    
    public Type UIType { get; }

    private GameObject _uiInstance;

    public FlowUiRegistration(Assembly assembly, string path, Type uiType, BaseGameUiHandler uiHandler, DiContainer container)
    {
        _assembly = assembly;
        _path = path;
        _uiHandler = uiHandler;
        _container = container;
        UIType = uiType;
    }

    public void Initialize()
    {
        var data = SiraUtil.Extras.Utilities.GetResource(_assembly, _path);
        _bundle = AssetBundle.LoadFromMemory(data);
        _uiPrefab = _bundle.LoadAsset<GameObject>("ui_prefab");
        _uiInstance = Object.Instantiate(_uiPrefab);
        _container.Bind(UIType).FromInstance(_uiInstance.GetComponent(UIType));
        _container.InjectGameObject(_uiInstance);
        
        var clickSignal = InstanceDictionary.ClickSignal.Asset;
        var evnt = clickSignal.GetField<Action, Signal>("_event");

        var root = _uiInstance.GetComponent<UIRoot>();
        root.ClickSignal.Subscribe(evnt);
        _container.BindInstances(root.ViewControllers);

        root.didRequestExit += Close;

        _uiInstance!.transform.SetParent(_uiHandler.GetScreenTransform(BaseGameUiHandler.ScreenType.Main).parent, false);
    }

    public void Close()
    {
        _uiInstance.SetActive(false);
        _uiHandler.PresentGameUI();
    }

    public void Present()
    {
        _uiHandler.DismissGameUI();
        _uiInstance.SetActive(true);
    }
    
    public void Dispose()
    {
        if (_uiInstance)
        {
            Object.DestroyImmediate(_uiInstance);
        }

        if (_bundle)
        {
            _bundle.Unload(true);
        }
    }
}