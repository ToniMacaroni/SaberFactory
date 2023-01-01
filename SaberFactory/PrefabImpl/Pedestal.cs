using SaberFactory.Instances;
using Zenject;

namespace SaberFactory.Prefab;

public partial class Pedestal : ISaberPresenter
{
    private SaberInstance _presentingSaber;
    
    public void Present(SaberInstance saberInstance)
    {
        _presentingSaber = saberInstance;
        saberInstance.SetParent(saberContainer);
    }

    public void StopPresenting()
    {
    }
}