using Zenject;

namespace FlowUi.Registration;

public class FlowUiBinding
{
    public DiContainer Container { get; }
    public Type BindingType { get; }

    public FlowUiBinding(DiContainer container, Type bindingType)
    {
        Container = container;
        BindingType = bindingType;
    }
}