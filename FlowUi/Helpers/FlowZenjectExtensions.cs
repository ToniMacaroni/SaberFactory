using FlowUi.Registration;
using Zenject;

namespace FlowUi.Helpers;

public static class FlowZenjectExtensions
{
    public static FlowUiBinding BindFlowUi<T>(this DiContainer container, string path = "")
    {
        if(!container.HasBinding<BaseGameUiHandler>())
        {
            container.BindInterfacesAndSelfTo<BaseGameUiHandler>().AsSingle();
        }
        
        var type = typeof(T);
        var assembly = type.Assembly;
        
        if (string.IsNullOrEmpty(path))
        {
            path = $"{assembly.GetName().Name}.Resources.Flow.{type.Name}";
        }

        container.BindInterfacesAndSelfTo<FlowUiRegistration>().AsSingle().WithArguments(assembly, path, type);

        return new FlowUiBinding(container, type);
    }

    public static FlowUiBinding CreateMenuButton(this FlowUiBinding binding, string name, string hint)
    {
        binding.Container.BindInterfacesAndSelfTo<MenuButtonRegistration>().AsCached()
            .WithArguments(new MenuButtonRegistration.InitData(name, hint, binding.BindingType));
        return binding;
    }
}