namespace FlowUi.Registration;

public interface ICanRequestExit
{
    Action DidRequestExit { get; set; }
}