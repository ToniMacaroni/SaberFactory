using SaberFactory.Instances;

namespace SaberFactory;

public interface ISaberPresenter
{
    void Present(SaberInstance saberInstance);
    void StopPresenting();
}